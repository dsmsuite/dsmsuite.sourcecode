using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly SolutionFile _solutionFile;
        private readonly Dictionary<string, FileInfo> _sourcesFilesById = new Dictionary<string, FileInfo>();
        private readonly Dictionary<string, string> _interfaceFilesByPath = new Dictionary<string, string>();

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
            _solutionFile = new SolutionFile(analyzerSettings.InputFilename, _analyzerSettings, progress);
        }

        public void Analyze()
        {
            RegisterInterfaceFiles();
            AnalyzeSolution();
            RegisterSourceFiles();
            RegisterDirectIncludeRelations();
            RegisterGeneratedFileRelations();
            AnalyzerLogger.Flush();
        }

        private void AnalyzeSolution()
        {
            _solutionFile.Analyze();
        }

        private void RegisterSourceFiles()
        {
            foreach (ProjectFile visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    RegisterSourceFile(_solutionFile, visualStudioProject, sourceFile);
                }
            }
        }

        private void RegisterInterfaceFiles()
        {
            foreach (string interfaceDirectory in _analyzerSettings.InterfaceIncludeDirectories)
            {
                DirectoryInfo interfaceDirectoryInfo = new DirectoryInfo(interfaceDirectory);
                RegisterInterfaceFiles(interfaceDirectoryInfo);
            }

            foreach (ExternalIncludeDirectory externalDirectory in _analyzerSettings.ExternalIncludeDirectories)
            {
                DirectoryInfo interfaceDirectoryInfo = new DirectoryInfo(externalDirectory.Path);
                RegisterInterfaceFiles(interfaceDirectoryInfo);
            }
            UpdateProgress("Register interface files", _interfaceFilesByPath.Count, "interfaces", true);
        }

        private void RegisterInterfaceFiles(DirectoryInfo interfaceDirectoryInfo)
        {
            if (interfaceDirectoryInfo.Exists)
            {
                foreach (FileInfo interfacefileInfo in interfaceDirectoryInfo.EnumerateFiles())
                {
                    if (interfacefileInfo.Exists)
                    {
                        SourceFile sourceFile = new SourceFile(interfacefileInfo.FullName);
                        _interfaceFilesByPath[interfacefileInfo.FullName] = sourceFile.Id;
                        UpdateProgress("Register interface files", _interfaceFilesByPath.Count, "interfaces", false);
                    }
                    else
                    {
                        Logger.LogError($"Interface file {interfacefileInfo.FullName} does not exist");
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

        private void RegisterDirectIncludeRelations()
        {
            foreach (ProjectFile visualStudioProject in _solutionFile.Projects)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    foreach (string includedFile in sourceFile.Includes)
                    {
                        Logger.LogInfo("Include relation registered: " + sourceFile.Name + " -> " + includedFile);

                        string consumerName = null;
                        switch (_analyzerSettings.ViewMode)
                        {
                            case ViewMode.LogicalView:
                                consumerName = GetLogicalName(_solutionFile, visualStudioProject, sourceFile);
                                break;
                            case ViewMode.PhysicalView:
                                consumerName = GetPhysicalName(sourceFile);
                                break;
                            default:
                                Logger.LogError("Unknown view mode");
                                break;
                        }

                        if (consumerName != null)
                        {
                            if (IsSystemInclude(includedFile))
                            {
                                // System includes are ignored
                            }
                            else if (IsInterfaceInclude(includedFile))
                            {
                                // Interface includes must be clones of includes files in other visual studio projects
                                string resolvedIncludedFile = ResolveClonedFile(includedFile, sourceFile);
                                if (resolvedIncludedFile != null)
                                {
                                    RegisterIncludeRelation(consumerName, resolvedIncludedFile);
                                }
                                else
                                {
                                    _model.SkipRelation(consumerName, includedFile, "include", "interface include file not be resolved");
                                }
                            }
                            else if (IsExternalInclude(includedFile))
                            {
                                // External includes can be clones of includes files in other visual studio projects or 
                                // reference external software
                                string resolvedIncludedFile = ResolveClonedFile(includedFile, sourceFile);
                                if (resolvedIncludedFile != null)
                                {
                                    RegisterIncludeRelation(consumerName, resolvedIncludedFile);
                                }
                                else
                                {
                                    SourceFile includedSourceFile = new SourceFile(includedFile);
                                    string providerName = GetExternalName(includedSourceFile.SourceFileInfo);
                                    string type = includedSourceFile.FileType;
                                    _model.AddElement(providerName, type, includedFile);
                                    _model.AddRelation(consumerName, providerName, "include", 1, "include file is an external include");
                                }
                            }
                            else
                            {
                                RegisterIncludeRelation(consumerName, includedFile);
                            }
                        }
                    }
                }
            }
        }

        private void RegisterIncludeRelation(string consumerName, string resolvedIncludedFile)
        {
            switch (_analyzerSettings.ViewMode)
            {
                case ViewMode.LogicalView:
                    RegisterLogicalIncludeRelations(consumerName, resolvedIncludedFile);
                    break;
                case ViewMode.PhysicalView:
                    RegisterPhysicalIncludeRelations(consumerName, resolvedIncludedFile);
                    break;
                default:
                    Logger.LogError("Unknown view mode");
                    break;
            }
        }

        private string ResolveClonedFile(string includedFile, SourceFile sourceFile)
        {
            string resolvedIncludedFile = null;
            if (_interfaceFilesByPath.ContainsKey(includedFile))
            {
                string id = _interfaceFilesByPath[includedFile];
                if (_sourcesFilesById.ContainsKey(id))
                {
                    resolvedIncludedFile = _sourcesFilesById[id].FullName;
                    Logger.LogInfo("Include file clone resolved: " + sourceFile.Name + " -> " + includedFile + " -> " + resolvedIncludedFile);
                }
            }
            return resolvedIncludedFile;
        }

        private void RegisterLogicalIncludeRelations(string consumerName, string includedFile)
        {
            // First check if the included file can be found in a visual studio project
            SolutionFile solutionFile;
            ProjectFile projectFile;
            SourceFile includeFile;
            if (FindIncludeFileInVisualStudioProject(includedFile, out solutionFile, out projectFile, out includeFile))
            {
                string providerName = GetLogicalName(solutionFile, projectFile, includeFile);
                _model.AddRelation(consumerName, providerName, "include", 1, "logical include file is resolved");
            }
            else
            {
                _model.SkipRelation(consumerName, includedFile, "include", "logical include file not be resolved");
            }
        }

        private void RegisterPhysicalIncludeRelations(string consumerName, string includedFile)
        {
            SourceFile includedSourceFile = new SourceFile(includedFile);
            string providerName = GetPhysicalName(includedSourceFile);
            _model.AddRelation(consumerName, providerName, "include", 1, "physical include file is resolved");
        }

        private void RegisterGeneratedFileRelations()
        {
            foreach (ProjectFile visualStudioProject in _solutionFile.Projects)
            {
                foreach (GeneratedFileRelation relation in visualStudioProject.GeneratedFileRelations)
                {
                    Logger.LogInfo("Generated file relation registered: " + relation.Consumer.Name + " -> " + relation.Provider.Name);

                    switch (_analyzerSettings.ViewMode)
                    {
                        case ViewMode.LogicalView:
                            {
                                string consumerName = GetLogicalName(_solutionFile, visualStudioProject, relation.Consumer);
                                string providerName = GetLogicalName(_solutionFile, visualStudioProject, relation.Provider);
                                _model.AddRelation(consumerName, providerName, "generated", 1, "generated file relations");
                                break;
                            }
                        case ViewMode.PhysicalView:
                            {
                                string consumerName = GetPhysicalName(relation.Consumer);
                                string providerName = GetPhysicalName(relation.Provider);
                                _model.AddRelation(consumerName, providerName, "generated", 1, "generated file relations");
                                break;
                            }
                        default:
                            Logger.LogError("Unknown view mode");
                            break;
                    }

                }
            }
        }

        private void RegisterSourceFile(SolutionFile solutionFile, ProjectFile visualStudioProject, SourceFile sourceFile)
        {
            Logger.LogInfo("Source file registered: " + sourceFile.Name);

            string type = sourceFile.FileType;
            _sourcesFilesById[sourceFile.Id] = sourceFile.SourceFileInfo;

            if (sourceFile.SourceFileInfo.Exists)
            {
                switch (_analyzerSettings.ViewMode)
                {
                    case ViewMode.LogicalView:
                        {
                            string name = GetLogicalName(solutionFile, visualStudioProject, sourceFile);
                            _model.AddElement(name, type, sourceFile.SourceFileInfo.FullName);
                            break;
                        }
                    case ViewMode.PhysicalView:
                        {
                            string name = GetPhysicalName(sourceFile);
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

        private bool IsSystemInclude(string includedFile)
        {
            bool isSystemInclude = false;

            foreach (string systemIncludeDirectory in _analyzerSettings.SystemIncludeDirectories)
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

            foreach (ExternalIncludeDirectory externalIncludeDirectory in _analyzerSettings.ExternalIncludeDirectories)
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

            foreach (string interfaceIncludeDirectory in _analyzerSettings.InterfaceIncludeDirectories)
            {
                if (includedFile.StartsWith(interfaceIncludeDirectory))
                {
                    isInterfaceInclude = true;
                }
            }
            return isInterfaceInclude;
        }

        private bool FindIncludeFileInVisualStudioProject(string includedFile, out SolutionFile solutionFile, out ProjectFile projectFile, out SourceFile sourceFile)
        {
            bool found = false;
            solutionFile = null;
            projectFile = null;
            sourceFile = null;

            foreach (ProjectFile project in _solutionFile.Projects)
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

        private string GetLogicalName(SolutionFile solutionFile, ProjectFile visualStudioProject, SourceFile sourceFile)
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

        private string GetExternalName(FileInfo includedFileInfo)
        {
            string usedExternalIncludeDirectory = null;
            string resolveAs = null;
            foreach (ExternalIncludeDirectory externalIncludeDirectory in _analyzerSettings.ExternalIncludeDirectories)
            {
                if (includedFileInfo.FullName.StartsWith(externalIncludeDirectory.Path))
                {
                    usedExternalIncludeDirectory = externalIncludeDirectory.Path;
                    resolveAs = externalIncludeDirectory.ResolveAs;
                }
            }

            string name = null;

            if ((usedExternalIncludeDirectory != null) &&
                (resolveAs != null))
            {
                name = includedFileInfo.FullName.Replace(usedExternalIncludeDirectory, resolveAs).Replace("\\", ".").Replace("\\", ".");
            }

            return name;
        }

        private string GetPhysicalName(SourceFile sourceFile)
        {
            string name = "";

            string rootDirectory = _analyzerSettings.RootDirectory.Trim('\\'); // Ensure without trailing \
            if (sourceFile.SourceFileInfo.FullName.StartsWith(rootDirectory))
            {
                int start = rootDirectory.Length + 1;
                name = sourceFile.SourceFileInfo.FullName.Substring(start).Replace("\\", ".");
            }

            return name;
        }

        private void UpdateProgress(string actionText, int itemCount, string itemType, bool done)
        {
            if (_progress != null)
            {
                ProgressInfo progressInfoInfo = new ProgressInfo
                {
                    ActionText = actionText,
                    TotalItemCount = itemCount,
                    CurrentItemCount = itemCount,
                    ItemType = itemType,
                    Percentage = null,
                    Done = done
                };

                _progress.Report(progressInfoInfo);
            }
        }
    }
}
