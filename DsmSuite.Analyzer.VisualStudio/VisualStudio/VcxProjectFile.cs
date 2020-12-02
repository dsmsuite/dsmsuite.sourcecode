using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Util;
using DsmSuite.Analyzer.VisualStudio.Settings;
using Microsoft.Build.Evaluation;
using DsmSuite.Analyzer.DotNet.Lib;
using Microsoft.Build.Construction;
using Logger = DsmSuite.Common.Util.Logger;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class VcxProjectFile : ProjectFileBase
    {
        private const string ItemBegin = "@(";
        private const string ItemEnd = ")";
        private const string ItemSeparator = "->";
        private const string ItemFullPath = "->'%(FullPath)'";

        private readonly List<string> _includeDirectories = new List<string>();
        private readonly FilterFile _filterFile;
        private IncludeResolveStrategy _includeResolveStrategy;
        private BinaryFile _assembly;
        private Dictionary<string, string> _globalProperties = new Dictionary<string, string>();
        private string _toolsVersion;

        public VcxProjectFile(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver) :
            base(solutionFolder, solutionDir, projectPath, analyzerSettings, resolver)
        {
            _filterFile = new FilterFile(ProjectFileInfo.FullName + ".filters");
            GeneratedFileRelations = new List<GeneratedFileRelation>();
        }

        public override BinaryFile BuildAssembly => _assembly;

        public IReadOnlyCollection<string> ProjectIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.ProjectIncludeDirectories : new List<string>();

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

                Success = true;

                CloseProject(project);
            }
        }

        public override bool Success { get; protected set; }

        protected override Project OpenProject()
        {
            Project project = null;

            try
            {
                DetermineGlobalProperties();
                project = new Project(ProjectFileInfo.FullName, _globalProperties, _toolsVersion);
            }
            catch (Exception e)
            {
                string props = "";
                foreach (var globalProperty in _globalProperties)
                {
                    props += $" {globalProperty.Key} {globalProperty.Value}";
                }

                Logger.LogToFileAlways("failedToLoadProjects.log", $"{ProjectFileInfo.FullName} {e.Message} {props}");
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }

            return project;
        }

        private void DetermineGlobalProperties()
        {
            try
            {
                ProjectRootElement project = ProjectRootElement.Open(ProjectFileInfo.FullName);
                _toolsVersion = AnalyzerSettings.ToolsVersion;

                foreach (ProjectItemElement item in project.Items)
                {
                    if (item.ElementName == "ProjectConfiguration")
                    {
                        string[] projectConfiguration = item.Include.Split('|'); // eg. "Release|x64"
                        _globalProperties["Configuration"] = projectConfiguration[0];
                        _globalProperties["Platform"] = projectConfiguration[1];
                        _globalProperties["SolutionDir"] = NormalizeDirectory(SolutionDir);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                string props = "";
                foreach (var globalProperty in _globalProperties)
                {
                    props += $" {globalProperty.Key} {globalProperty.Value}";
                }
                Logger.LogToFileAlways("failedToLoadProjects.log", $"{ProjectFileInfo.FullName} {e.Message} {props}");
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }
        }

        private string NormalizeDirectory(string directory)
        {
            string normalizedDirectory = directory;
            if (!normalizedDirectory.EndsWith(@"\"))
            {
                normalizedDirectory += @"\";
            }
            return normalizedDirectory;
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
                Logger.LogException($"Add source file failed project={ProjectFileInfo.FullName} file={projectItem.EvaluatedInclude}", e);
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
                            string expandIncludeDirectory = ExpandIncludeDirectory(evaluatedProject, includeDirectory);

                            string trimmedIncludeDirectory = expandIncludeDirectory.Trim().Replace(@"\r\n", ""); // To fix occasional prefixes

                            if (trimmedIncludeDirectory.Length > 0)
                            {
                                try
                                {
                                    string resolvedIncludeDirectory = Path.GetFullPath(trimmedIncludeDirectory);

                                    if (Directory.Exists(resolvedIncludeDirectory)) // Is existing absolute include path
                                    {
                                        AddIncludeDirectory(resolvedIncludeDirectory, expandIncludeDirectory);
                                    }
                                    else
                                    {
                                        resolvedIncludeDirectory = GetAbsolutePath(ProjectFileInfo.DirectoryName, trimmedIncludeDirectory);

                                        if (Directory.Exists(resolvedIncludeDirectory)) // Is existing resolved relative include path
                                        {
                                            AddIncludeDirectory(resolvedIncludeDirectory, expandIncludeDirectory);
                                        }
                                        else
                                        {
                                            AnalyzerLogger.LogErrorIncludePathNotFound(resolvedIncludeDirectory, evaluatedProject.FullPath);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    AnalyzerLogger.LogErrorPathNotResolved(expandIncludeDirectory, evaluatedProject.FullPath);
                                }
                            }
                        }
                    }
                }
            }

            _includeResolveStrategy = new IncludeResolveStrategy(_includeDirectories, AnalyzerSettings.SystemIncludeDirectories);
        }

        private string ExpandIncludeDirectory(Project evaluatedProject, string includeDirectory)
        {
            // See https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-well-known-item-metadata?view=vs-2019 and
            // https://stackoverflow.com/questions/2814424/different-ways-to-pass-variables-in-msbuild

            string expandedIncludeDirectory = includeDirectory;
            if (includeDirectory.StartsWith(ItemBegin) && includeDirectory.EndsWith(ItemEnd))
            {
                if (!includeDirectory.Contains(ItemSeparator))
                {
                    string itemName = includeDirectory
                        .Replace(ItemBegin, string.Empty)
                        .Replace(ItemEnd, "");

                    string itemValue;
                    if (GetEvaluatedItemValue(evaluatedProject, itemName, out itemValue))
                    {
                        expandedIncludeDirectory = itemValue;
                    }
                    else
                    {
                        Logger.LogError($"Expand include directory failed because it could not find item with name {itemName} in include directory {includeDirectory}");
                    }
                }
                else
                {
                    if (includeDirectory.Contains(ItemFullPath))
                    {
                        string itemName = includeDirectory
                            .Replace(ItemBegin, string.Empty)
                            .Replace(ItemFullPath, string.Empty)
                            .Replace(ItemEnd, "");

                        string itemValue;
                        if (GetEvaluatedItemValue(evaluatedProject, itemName, out itemValue))
                        {
                            try
                            {
                                expandedIncludeDirectory = new DirectoryInfo(itemValue).FullName;
                            }
                            catch (Exception e)
                            {
                                Logger.LogError($"Expand include directory failed because value {itemValue} of item with name {itemName} is not a valid directory in include directory {includeDirectory}");
                            }
                        }
                        else
                        {
                            Logger.LogError($"Expand include directory failed because it could not find item with name {itemName} in include directory {includeDirectory}");
                        }
                    }
                    else
                    {
                        Logger.LogError($"Unsupported item meta data include={includeDirectory}");
                    }
                }
            }

            return expandedIncludeDirectory;
        }

        private bool GetEvaluatedItemValue(Project evaluatedProject, string name, out string value)
        {
            bool succes = false;
            value = "";
            foreach (ProjectItem item in evaluatedProject.AllEvaluatedItems)
            {
                if (item.ItemType == name)
                {
                    value = item.EvaluatedInclude;
                    succes = true;
                }
            }
            return succes;
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
