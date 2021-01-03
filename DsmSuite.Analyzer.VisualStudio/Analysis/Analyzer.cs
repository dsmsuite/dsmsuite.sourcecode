using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.VisualStudio.Utils;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly SolutionFile _solutionFile;
        private readonly HashSet<string> _registeredSources = new HashSet<string>();
        private readonly Dictionary<string, FileInfo> _projectSourcesFilesByChecksum = new Dictionary<string, FileInfo>();
        private readonly Dictionary<string, string> _interfaceFileChecksumsByFilePath = new Dictionary<string, string>();

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _solutionFile = new SolutionFile(analyzerSettings.Input.Filename, _analyzerSettings, progress);
        }

        public void Analyze()
        {
            RegisterInterfaceFiles();
            AnalyzeSolution();
            RegisterDotNetTypes();
            RegisterDotNetRelations();
            RegisterSourceFiles();
            RegisterDirectIncludeRelations();
            RegisterGeneratedFileRelations();
            WriteFoundProjects();
            AnalyzerLogger.Flush();
        }

        private void AnalyzeSolution()
        {
            _solutionFile.Analyze();
        }

        private void RegisterDotNetTypes()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (DotNetType type in visualStudioProject.DotNetTypes)
                {
                    RegisterDotNetType(_solutionFile, visualStudioProject, type);
                }
            }
        }

        private void RegisterInterfaceFiles()
        {
            foreach (string interfaceDirectory in _analyzerSettings.Input.InterfaceIncludeDirectories)
            {
                DirectoryInfo interfaceDirectoryInfo = new DirectoryInfo(interfaceDirectory);
                RegisterInterfaceFiles(interfaceDirectoryInfo);
            }
        }

        private void RegisterInterfaceFiles(DirectoryInfo interfaceDirectoryInfo)
        {
            if (interfaceDirectoryInfo.Exists)
            {
                foreach (FileInfo interfaceFile in interfaceDirectoryInfo.EnumerateFiles())
                {
                    if (interfaceFile.Exists)
                    {
                        SourceFile sourceFile = new SourceFile(interfaceFile.FullName);
                        _interfaceFileChecksumsByFilePath[interfaceFile.FullName] = sourceFile.Checksum;
                    }
                    else
                    {
                        Logger.LogError($"Interface file {interfaceFile.FullName} does not exist");
                    }
                }

                foreach (DirectoryInfo subDirectoryInfo in interfaceDirectoryInfo.EnumerateDirectories())
                {
                    RegisterInterfaceFiles(subDirectoryInfo);
                }
            }
            else
            {
                Logger.LogError($"Interface directory {interfaceDirectoryInfo.FullName} does not exist");
            }
        }

        private void RegisterDotNetRelations()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (DotNetRelation relation in visualStudioProject.DotNetRelations)
                {
                    RegisterDotNetRelation(_solutionFile, visualStudioProject, relation);
                }
            }
        }

        private void RegisterSourceFiles()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    RegisterSourceFile(_solutionFile, visualStudioProject, sourceFile);
                    _registeredSources.Add(sourceFile.SourceFileInfo.FullName);
                }
            }
        }

        private void RegisterDirectIncludeRelations()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    foreach (string includedFile in sourceFile.Includes)
                    {
                        Logger.LogInfo("Include relation registered: " + sourceFile.Name + " -> " + includedFile);

                        switch (_analyzerSettings.Analysis.ViewMode)
                        {
                            case ViewMode.SolutionView:
                                RegisterDirectIncludeRelationSolutionView(visualStudioProject, sourceFile, includedFile);
                                break;
                            case ViewMode.DirectoryView:
                                RegisterDirectIncludeRelationDirectoryView(sourceFile, includedFile);
                                break;
                            default:
                                Logger.LogError("Unknown view mode");
                                break;
                        }
                    }
                }
            }
        }
        
        private void RegisterDirectIncludeRelationSolutionView(ProjectFileBase visualStudioProject, SourceFile sourceFile, string includedFile)
        {
            string consumerName = GetSolutionViewName(_solutionFile, visualStudioProject, sourceFile);
            if (consumerName != null)
            {
                if (IsProjectInclude(includedFile))
                {
                    // Register as normal visual studio project include
                    RegisterIncludeRelationSolutionView(consumerName, includedFile);
                }
                else if (IsInterfaceInclude(includedFile))
                {
                    // Interface includes must be clones of includes files in other visual studio projects
                    string resolvedIncludedFile = ResolveInterfaceFile(includedFile, sourceFile);
                    if (resolvedIncludedFile != null)
                    {
                        // Register resolved interface as normal visual studio project include
                        // if source of clone in visual studio project found
                        RegisterIncludeRelationSolutionView(consumerName, resolvedIncludedFile);
                    }
                    else
                    {
                        // Skip not resolved interface
                        _model.SkipRelation(consumerName, includedFile, "include");
                    }
                }
                else if (IsExternalInclude(includedFile))
                {
                    // Register external include
                    RegisterExternalIncludeRelationSolutionView(includedFile, consumerName);
                }
                else if (IsSystemInclude(includedFile))
                {
                    // Ignore system include
                }
                else
                {
                    // Skip not resolved interface
                    AnalyzerLogger.LogIncludeFileNotFoundInVisualStudioProject(includedFile, visualStudioProject.ProjectName);
                    _model.SkipRelation(consumerName, includedFile, "include");
                }
            }
        }

        private void RegisterIncludeRelationSolutionView(string consumerName, string includedFile)
        {
            // First check if the included file can be found in a visual studio project
            SolutionFile solutionFile;
            ProjectFileBase projectFile;
            SourceFile includeFile;
            if (FindIncludeFileInVisualStudioProject(includedFile, out solutionFile, out projectFile, out includeFile))
            {
                string providerName = GetSolutionViewName(solutionFile, projectFile, includeFile);
                _model.AddRelation(consumerName, providerName, "include", 1, null);
            }
            else
            {
                _model.SkipRelation(consumerName, includedFile, "include");
            }
        }

        private void RegisterExternalIncludeRelationSolutionView(string includedFile, string consumerName)
        {
            // Add include element to model
            SourceFile includedSourceFile = new SourceFile(includedFile);
            string providerName = GetExternalName(includedSourceFile);
            string type = includedSourceFile.FileType;
            _model.AddElement(providerName, type, includedFile);

            // Add relation to model
            _model.AddRelation(consumerName, providerName, "include", 1, "include file is an external include");
        }

        private void RegisterDirectIncludeRelationDirectoryView(SourceFile sourceFile, string includedFile)
        {
            SourceFile includedSourceFile = new SourceFile(includedFile);
            string consumerName = GetDirectoryViewName(sourceFile);
            string providerName = GetDirectoryViewName(includedSourceFile);
            string type = includedSourceFile.FileType;
            _model.AddElement(providerName, type, includedFile);
            _model.AddRelation(consumerName, providerName, "include", 1, null);
        }

        private string ResolveInterfaceFile(string includedFile, SourceFile sourceFile)
        {
            // Interface files can be clones of source files found in visual studio projects 
            string resolvedIncludedFile = null;
            if (_interfaceFileChecksumsByFilePath.ContainsKey(includedFile))
            {
                string checksum = _interfaceFileChecksumsByFilePath[includedFile];
                if (_projectSourcesFilesByChecksum.ContainsKey(checksum))
                {
                    resolvedIncludedFile = _projectSourcesFilesByChecksum[checksum].FullName;
                    Logger.LogInfo("Included interface resolved: " + sourceFile.Name + " -> " + includedFile + " -> " + resolvedIncludedFile);
                }
            }
            return resolvedIncludedFile;
        }

        private void RegisterGeneratedFileRelations()
        {
            foreach (ProjectFileBase visualStudioProject in _solutionFile.Projects)
            {
                foreach (GeneratedFileRelation relation in visualStudioProject.GeneratedFileRelations)
                {
                    Logger.LogInfo("Generated file relation registered: " + relation.Consumer.Name + " -> " + relation.Provider.Name);

                    switch (_analyzerSettings.Analysis.ViewMode)
                    {
                        case ViewMode.SolutionView:
                            {
                                string consumerName = GetSolutionViewName(_solutionFile, visualStudioProject, relation.Consumer);
                                string providerName = GetSolutionViewName(_solutionFile, visualStudioProject, relation.Provider);
                                _model.AddRelation(consumerName, providerName, "generated", 1, null);
                                break;
                            }
                        case ViewMode.DirectoryView:
                            {
                                string consumerName = GetDirectoryViewName(relation.Consumer);
                                string providerName = GetDirectoryViewName(relation.Provider);
                                _model.AddRelation(consumerName, providerName, "generated", 1, null);
                                break;
                            }
                        default:
                            Logger.LogError("Unknown view mode");
                            break;
                    }
                }
            }
        }

        private void RegisterSourceFile(SolutionFile solutionFile, ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            Logger.LogInfo("Source file registered: " + sourceFile.Name);

            string type = sourceFile.FileType;

            if (_interfaceFileChecksumsByFilePath.Count > 0)
            {
                _projectSourcesFilesByChecksum[sourceFile.Checksum] = sourceFile.SourceFileInfo;
            }

            if (sourceFile.SourceFileInfo.Exists)
            {
                AnalyzerLogger.LogFileFoundInVisualStudioProject(sourceFile.Name, visualStudioProject.ProjectName);

                switch (_analyzerSettings.Analysis.ViewMode)
                {
                    case ViewMode.SolutionView:
                        {
                            string name = GetSolutionViewName(solutionFile, visualStudioProject, sourceFile);
                            _model.AddElement(name, type, sourceFile.SourceFileInfo.FullName);
                            break;
                        }
                    case ViewMode.DirectoryView:
                        {
                            string name = GetDirectoryViewName(sourceFile);
                            _model.AddElement(name, type, sourceFile.SourceFileInfo.FullName);
                            break;
                        }
                    default:
                        Logger.LogError("Unknown view mode");
                        break;
                }
            }
            else
            {
                AnalyzerLogger.LogErrorFileNotFound(sourceFile.Name, visualStudioProject.ProjectName);
            }
        }

        private void RegisterDotNetType(SolutionFile solutionFile, ProjectFileBase visualStudioProject, DotNetType type)
        {
            string name = GetDotNetTypeName(solutionFile, visualStudioProject, type.Name);
            _model.AddElement(name, type.Type, "");
        }

        private void RegisterDotNetRelation(SolutionFile solutionFile, ProjectFileBase visualStudioProject, DotNetRelation relation)
        {
            string consumerName = GetDotNetTypeName(solutionFile, visualStudioProject, relation.ConsumerName);
            string providerName = GetDotNetTypeName(solutionFile, visualStudioProject, relation.ProviderName);
            _model.AddRelation(consumerName, providerName, relation.Type, 1, null);
        }

        private bool IsProjectInclude(string includedFile)
        {
            return _registeredSources.Contains(includedFile);
        }

        private bool IsSystemInclude(string includedFile)
        {
            bool isSystemInclude = false;

            foreach (string systemIncludeDirectory in _analyzerSettings.Input.SystemIncludeDirectories)
            {
                if (includedFile.StartsWith(systemIncludeDirectory))
                {
                    isSystemInclude = true;
                }
            }
            return isSystemInclude;
        }

        private bool IsExternalInclude(string includedFile)
        {
            bool isExternalInclude = false;

            foreach (ExternalIncludeDirectory externalIncludeDirectory in _analyzerSettings.Input.ExternalIncludeDirectories)
            {
                if (includedFile.StartsWith(externalIncludeDirectory.Path))
                {
                    isExternalInclude = true;
                }
            }

            return isExternalInclude;
        }

        private bool IsInterfaceInclude(string includedFile)
        {
            bool isInterfaceInclude = false;

            foreach (string interfaceIncludeDirectory in _analyzerSettings.Input.InterfaceIncludeDirectories)
            {
                if (includedFile.StartsWith(interfaceIncludeDirectory))
                {
                    isInterfaceInclude = true;
                }
            }
            return isInterfaceInclude;
        }
        private bool FindIncludeFileInVisualStudioProject(string includedFile, out SolutionFile solutionFile, out ProjectFileBase projectFile, out SourceFile sourceFile)
        {
            bool found = false;
            solutionFile = null;
            projectFile = null;
            sourceFile = null;

            foreach (ProjectFileBase project in _solutionFile.Projects)
            {
                foreach (SourceFile source in project.SourceFiles)
                {
                    if (includedFile.ToLower() == source.SourceFileInfo.FullName.ToLower())
                    {
                        solutionFile = _solutionFile;
                        projectFile = project;
                        sourceFile = source;
                        found = true;
                    }
                }
            }

            return found;
        }

        private string GetDotNetTypeName(SolutionFile solutionFile, ProjectFileBase visualStudioProject, string typeName)
        {
            string name = "";

            if (!string.IsNullOrEmpty(solutionFile?.Name))
            {
                name += solutionFile.Name;
                name += ".";
            }

            if (visualStudioProject != null)
            {
                if (!string.IsNullOrEmpty(visualStudioProject.SolutionFolder))
                {
                    name += visualStudioProject.SolutionFolder;
                    name += ".";
                }

                if (!string.IsNullOrEmpty(visualStudioProject.ProjectName))
                {
                    name += visualStudioProject.ProjectName;
                }

                if (!string.IsNullOrEmpty(visualStudioProject.TargetExtension))
                {
                    name += " (";
                    name += visualStudioProject.TargetExtension;
                    name += ")";
                }

                if (!string.IsNullOrEmpty(visualStudioProject.ProjectName))
                {
                    name += ".";
                }
            }

            name += typeName;

            return name.Replace("\\", ".");
        }

        private void WriteFoundProjects()
        {
            foreach (ProjectFileBase project in _solutionFile.Projects)
            {
                string projectName = GetSolutionViewName(_solutionFile, project, null);
                string status = project.Success ? "ok" : "failed";
                AnalyzerLogger.LogProjectStatus(projectName, status);
            }
        }

        private string GetSolutionViewName(SolutionFile solutionFile, ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            string name = "";

            if (!string.IsNullOrEmpty(solutionFile?.Name))
            {
                name += solutionFile.Name;
                name += ".";
            }

            if (visualStudioProject != null)
            {
                if (!string.IsNullOrEmpty(visualStudioProject.SolutionFolder))
                {
                    name += visualStudioProject.SolutionFolder;
                    name += ".";
                }

                if (!string.IsNullOrEmpty(visualStudioProject.ProjectName))
                {
                    name += visualStudioProject.ProjectName;
                }

                if (!string.IsNullOrEmpty(visualStudioProject.TargetExtension))
                {
                    name += " (";
                    name += visualStudioProject.TargetExtension;
                    name += ")";
                }

                if (!string.IsNullOrEmpty(visualStudioProject.ProjectName))
                {
                    name += ".";
                }
            }

            if (sourceFile != null)
            {
                if (!string.IsNullOrEmpty(sourceFile.ProjectFolder))
                {
                    name += sourceFile.ProjectFolder;
                    name += ".";
                }

                if (!string.IsNullOrEmpty(sourceFile.SourceFileInfo.Name))
                {
                    name += sourceFile.SourceFileInfo.Name;
                }
            }

            return name.Replace("\\", ".");
        }

        private string GetExternalName(SourceFile sourceFile)
        {
            string usedExternalIncludeDirectory = null;
            string resolveAs = null;
            foreach (ExternalIncludeDirectory externalIncludeDirectory in _analyzerSettings.Input.ExternalIncludeDirectories)
            {
                if (sourceFile.SourceFileInfo.FullName.StartsWith(externalIncludeDirectory.Path))
                {
                    usedExternalIncludeDirectory = externalIncludeDirectory.Path;
                    resolveAs = externalIncludeDirectory.ResolveAs;
                }
            }

            string name = null;

            if ((usedExternalIncludeDirectory != null) &&
                (resolveAs != null))
            {
                name = sourceFile.SourceFileInfo.FullName.Replace(usedExternalIncludeDirectory, resolveAs).Replace("\\", ".");
            }

            return name;
        }

        private string GetDirectoryViewName(SourceFile sourceFile)
        {
            string name = "";

            string rootDirectory = _analyzerSettings.Input.RootDirectory.Trim('\\'); // Ensure without trailing \
            if (sourceFile.SourceFileInfo.FullName.StartsWith(rootDirectory))
            {
                int start = rootDirectory.Length + 1;
                name = sourceFile.SourceFileInfo.FullName.Substring(start).Replace("\\", ".");
            }

            return name;
        }
    }
}
