using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Util;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;
using DsmSuite.Analyzer.DotNet.Lib;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class VcxProjectFile : ProjectFileBase
    {
        private readonly List<string> _includeDirectories = new List<string>();
        private readonly FilterFile _filterFile;
        private IncludeResolveStrategy _includeResolveStrategy;
        private BinaryFile _assembly;

        public VcxProjectFile(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver) :
            base(solutionFolder, solutionDir, projectPath, analyzerSettings, resolver)
        {
            _filterFile = new FilterFile(ProjectFileInfo.FullName + ".filters");
            GeneratedFileRelations = new List<GeneratedFileRelation>();
        }

        public override BinaryFile BuildAssembly => _assembly;

        public IReadOnlyCollection<string> ProjectIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.ProjectIncludeDirectories : new List<string>();

        public IReadOnlyCollection<string> ExternalIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.ExternalIncludeDirectories : new List<string>();

        public IReadOnlyCollection<string> SystemIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.SystemIncludeDirectories : new List<string>();

        public override IEnumerable<DotNetType> DotNetTypes => (_assembly != null) ? _assembly.Types : new List<DotNetType>();
        public override IEnumerable<DotNetRelation> DotNetRelations => (_assembly != null) ? _assembly.Relations : new List<DotNetRelation>();

        public override void Analyze()
        {
            Project project = OpenProject();

            if (project != null)
            {
                DefineProjectIncludeDirectories(project);

                foreach (var property in project.AllEvaluatedProperties)
                {
                    if (property.Name == "ConfigurationType")
                    {
                        switch (property.EvaluatedValue)
                        {
                            case "Application":
                                TargetExtension = "exe";
                                break;
                            case "StaticLibrary":
                                TargetExtension = "lib";
                                break;
                            case "DynamicLibrary":
                                TargetExtension = "dll";
                                break;
                            default:
                                TargetExtension = "?";
                                break;
                        }
                    }
                }

                foreach (ProjectItem projectItem in project.AllEvaluatedItems)
                {
                    try
                    {
                        switch (projectItem.ItemType)
                        {
                            case "None":
                                {
                                    if (projectItem.EvaluatedInclude.EndsWith(".inl"))
                                    {
                                        AddHeaderFile(projectItem);
                                    }
                                    break;
                                }
                            case "ClInclude":
                                {
                                    AddHeaderFile(projectItem);
                                    break;
                                }
                            case "ClCompile":
                                {
                                    AddSourceFile(projectItem);
                                    break;
                                }
                            case "Midl":
                                {
                                    AddIdlFile(projectItem);
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogException($"Analysis failed project={ProjectFileInfo.FullName} file={projectItem.EvaluatedInclude}", e);
                    }
                }

                CloseProject(project);
            }
        }

        protected override Project OpenProject()
        {
            Project project = null;
            try
            {
                string solutionDir = SolutionDir;
                if (!solutionDir.EndsWith(@"\"))
                {
                    solutionDir += @"\";
                }
                Dictionary<string, string> globalProperties = new Dictionary<string, string>();
                globalProperties["SolutionDir"] = solutionDir;
                project = new Project(ProjectFileInfo.FullName, globalProperties, AnalyzerSettings.ToolsVersion);
                foreach (var item in project.AllEvaluatedItems)
                {
                    if (item.ItemType == "ProjectConfiguration")
                    {
                        string[] projectConfiguration = item.EvaluatedInclude.Split('|'); // eg. "Release|x64"
                        globalProperties["Configuration"] = projectConfiguration[0];
                        globalProperties["Platform"] = projectConfiguration[1];
                        break;
                    }
                }
                UpdateConfiguration(project);
            }
            catch (Exception e)
            {
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }
            return project;
        }

        private void AddIdlFile(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    SourceFile sourceFile = AddSourceFile(fileInfo, projectFolder);
                    if (sourceFile != null)
                    {
                        AnalyzeIdlGeneratedFiles(projectItem, projectFolder, sourceFile);
                    }
                }
                else
                {
                    AnalyzerLogger.LogErrorFileNotFound(fileInfo.FullName, ProjectFileInfo.Name);
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Add IDL failed project={ProjectFileInfo.FullName} file={projectItem.EvaluatedInclude}", e);
            }
        }

        private void AddSourceFile(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    AddSourceFile(fileInfo, projectFolder);
                }
                else
                {
                    AnalyzerLogger.LogErrorFileNotFound(fileInfo.FullName, ProjectFileInfo.Name);
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Add source file failed project={ProjectFileInfo.FullName} file={projectItem.EvaluatedInclude}" , e);
            }
        }

        private void AddHeaderFile(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    AddSourceFile(fileInfo, projectFolder);
                }
                else
                {
                    AnalyzerLogger.LogErrorFileNotFound(fileInfo.FullName, ProjectFileInfo.Name);
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Add  header file failed project={ProjectFileInfo.FullName} file={projectItem.EvaluatedInclude}", e);
            }
        }

        private void UpdateConfiguration(Project project)
        {
            foreach (ProjectItem item in project.Items)
            {
                if (item.ItemType == "ProjectConfiguration")
                {
                    string configuration = null;
                    string platform = null;
                    foreach (ProjectMetadata meta in item.Metadata)
                    {
                        if (meta.Name == "Configuration")
                        {
                            configuration = meta.UnevaluatedValue;
                        }

                        if (meta.Name == "Platform")
                        {
                            platform = meta.UnevaluatedValue;
                        }
                    }

                    if ((configuration != null) && (platform != null))
                    {
                        project.SetGlobalProperty("Configuration", configuration);
                        project.SetGlobalProperty("Platform", platform);
                    }

                }
            }
            project.ReevaluateIfNecessary();
        }

        private void AnalyzeIdlGeneratedFiles(ProjectItem projectItem, string projectFolder, SourceFile idlSourceFile)
        {
            if (!IsExcludedFromBuild(projectItem) && (idlSourceFile != null))
            {
                string outputDirectory = GetOutputDirectoryGeneratedFiles(projectItem);

                string headerFileName = GetFilenameGeneratedHeader(projectItem);
                AddGeneratedFile(projectFolder, idlSourceFile, outputDirectory, headerFileName);

                string interfaceIdentifierFileName = GetFilenameGeneratedInterfaceIdentifier(projectItem);
                AddGeneratedFile(projectFolder, idlSourceFile, outputDirectory, interfaceIdentifierFileName);

                string typeLibraryName = GetFilenameGeneratedTypeLibrary(projectItem);
                AddGeneratedFile(projectFolder, idlSourceFile, outputDirectory, typeLibraryName);
            }
        }

        private void AddGeneratedFile(string projectFolder, SourceFile idlSourceFile, string outputDirectory, string generatedFileName)
        {
            FileInfo generatedFileInfo = new FileInfo(GetAbsolutePath(outputDirectory, generatedFileName));
            if (generatedFileInfo.Exists)
            {
                SourceFile generatedFile = AddSourceFile(generatedFileInfo, projectFolder);
                if (generatedFile != null)
                {
                    GeneratedFileRelations.Add(new GeneratedFileRelation(generatedFile, idlSourceFile));
                }
            }
        }

        private SourceFile AddSourceFile(FileInfo fileInfo, string projectFolder)
        {
            SourceFile sourceFile = new SourceFile(fileInfo, projectFolder, _includeResolveStrategy);
            SourceFiles.Add(sourceFile);
            return sourceFile;
        }

        private bool IsExcludedFromBuild(ProjectItem projectItem)
        {
            bool isExcludedFromBuild = false;

            foreach (ProjectMetadata metaData in projectItem.Metadata)
            {
                if ((metaData.Name == "ExcludedFromBuild") &&
                    (string.Compare(metaData.EvaluatedValue, "True", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    isExcludedFromBuild = true;
                }
            }
            return isExcludedFromBuild;
        }

        private string GetOutputDirectoryGeneratedFiles(ProjectItem projectItem)
        {
            string outputDirectory = ProjectFileInfo.DirectoryName;
            foreach (ProjectMetadata metaData in projectItem.Metadata)
            {
                if (metaData.Name == "OutputDirectory")
                {
                    if (metaData.EvaluatedValue.Trim().Length > 0)
                    {
                        string outputDir = metaData.EvaluatedValue;
                        if (Directory.Exists(outputDir))
                        {
                            outputDirectory = outputDir;
                        }
                        else
                        {
                            if (ProjectFileInfo?.DirectoryName != null)
                            {
                                outputDir = Path.GetFullPath(Path.Combine(ProjectFileInfo.DirectoryName, metaData.EvaluatedValue));
                                if (Directory.Exists(outputDir))
                                {
                                    outputDirectory = outputDir;
                                }
                            }
                        }

                    }
                }
            }
            return outputDirectory;
        }

        private string GetFilenameGeneratedHeader(ProjectItem projectItem)
        {
            string filename = GetIdlFilename(projectItem);
            string headerFileName = filename + ".h";
            foreach (ProjectMetadata metaData in projectItem.Metadata)
            {
                if (metaData.Name == "HeaderFileName")
                {
                    if (metaData.EvaluatedValue.Trim().Length > 0)
                    {
                        headerFileName = metaData.EvaluatedValue.Replace("%(Filename)", filename);
                    }
                }
            }
            return headerFileName;
        }

        private string GetFilenameGeneratedInterfaceIdentifier(ProjectItem projectItem)
        {
            string filename = GetIdlFilename(projectItem);
            string interfaceIdentifierFileName = filename + "_i.c";
            foreach (ProjectMetadata metaData in projectItem.Metadata)
            {
                if (metaData.Name == "InterfaceIdentifierFileName")
                {
                    if (metaData.EvaluatedValue.Trim().Length > 0)
                    {
                        interfaceIdentifierFileName = metaData.EvaluatedValue.Replace("%(Filename)", filename);
                    }
                }
            }
            return interfaceIdentifierFileName;
        }

        private string GetFilenameGeneratedTypeLibrary(ProjectItem projectItem)
        {
            string filename = GetIdlFilename(projectItem);
            string typeLibraryFilename = filename + ".tlb";
            foreach (ProjectMetadata metaData in projectItem.Metadata)
            {
                if (metaData.Name == "TypeLibraryName")
                {
                    if (metaData.EvaluatedValue.Trim().Length > 0)
                    {
                        typeLibraryFilename = metaData.EvaluatedValue.Replace("%(Filename)", filename);
                    }
                }
            }
            return typeLibraryFilename;
        }

        private string GetIdlFilename(ProjectItem projectItem)
        {
            char[] separators = { '.', '\\', '/' };
            string[] parts = projectItem.EvaluatedInclude.Split(separators);
            return parts[parts.Length - 2];
        }

        private void DefineProjectIncludeDirectories(Project evaluatedProject)
        {
            AddIncludeDirectory(ProjectFileInfo.DirectoryName, ProjectFileInfo.DirectoryName);

            foreach (ProjectItemDefinition projectItem in evaluatedProject.ItemDefinitions.Values)
            {
                foreach (ProjectMetadata metaData in projectItem.Metadata)
                {
                    if (metaData.Name == "AdditionalIncludeDirectories")
                    {
                        string[] includeDirectories = metaData.EvaluatedValue.Trim(';').Split(';');

                        foreach (string includeDirectory in includeDirectories)
                        {
                            string trimmedIncludeDirectory = includeDirectory.Trim().Replace(@"\r\n", ""); // To fix occasional prefixes

                            if (trimmedIncludeDirectory.Length > 0)
                            {
                                try
                                {
                                    string resolvedIncludeDirectory = Path.GetFullPath(trimmedIncludeDirectory);

                                    if (Directory.Exists(resolvedIncludeDirectory)) // Is existing absolute include path
                                    {
                                        AddIncludeDirectory(resolvedIncludeDirectory, includeDirectory);
                                    }
                                    else
                                    {
                                        resolvedIncludeDirectory = GetAbsolutePath(ProjectFileInfo.DirectoryName, trimmedIncludeDirectory);

                                        if (Directory.Exists(resolvedIncludeDirectory)) // Is existing resolved relative include path
                                        {
                                            AddIncludeDirectory(resolvedIncludeDirectory, includeDirectory);
                                        }
                                        else
                                        {
                                            AnalyzerLogger.LogErrorIncludePathNotFound(resolvedIncludeDirectory, evaluatedProject.FullPath);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    AnalyzerLogger.LogErrorPathNotResolved(includeDirectory, evaluatedProject.FullPath);
                                }
                            }
                        }
                    }
                }
            }

            List<string> externalIncludeDirectories = new List<string>();
            foreach(ExternalIncludeDirectory externalIncludeDirectory in AnalyzerSettings.ExternalIncludeDirectories)
            {
                if (Directory.Exists(externalIncludeDirectory.Path)) 
                {
                    externalIncludeDirectories.Add(externalIncludeDirectory.Path);
                }
                else
                {
                    AnalyzerLogger.LogErrorIncludePathNotFound(externalIncludeDirectory.Path, evaluatedProject.FullPath);
                }
            }

            _includeResolveStrategy = new IncludeResolveStrategy(_includeDirectories, externalIncludeDirectories, AnalyzerSettings.SystemIncludeDirectories);
        }

        private void AddIncludeDirectory(string resolvedIncludeDirectory, string includeDirectory)
        {
            Logger.LogInfo("Added include path " + resolvedIncludeDirectory + " from " + includeDirectory + " in " + ProjectFileInfo.FullName);
            _includeDirectories.Add(resolvedIncludeDirectory);
        }

        private string GetAbsolutePath(string dir, string file)
        {
            return Path.GetFullPath(Path.Combine(dir, file));
        }
    }
}
