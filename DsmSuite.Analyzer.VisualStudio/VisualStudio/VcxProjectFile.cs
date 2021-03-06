﻿using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using Microsoft.Build.Evaluation;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.VisualStudio.Utils;
using Microsoft.Build.Construction;
using Logger = DsmSuite.Common.Util.Logger;
using DsmSuite.Common.Util;

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
        private readonly Dictionary<string, string> _globalProperties = new Dictionary<string, string>();
        private string _toolsVersion;
        private List<string> _forcedIncludes = new List<string>();

        public VcxProjectFile(string solutionFolder, string solutionDir, string solutionName, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver) :
            base(solutionFolder, solutionDir, solutionName, projectPath, analyzerSettings, resolver)
        {
            _filterFile = new FilterFile(ProjectFileInfo.FullName + ".filters");
            GeneratedFileRelations = new List<GeneratedFileRelation>();
        }

        public override BinaryFile BuildAssembly => null;

        public IReadOnlyCollection<string> ProjectIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.ProjectIncludeDirectories : new List<string>();

        public IReadOnlyCollection<string> SystemIncludeDirectories => _includeResolveStrategy != null ? _includeResolveStrategy.SystemIncludeDirectories : new List<string>();

        public override IEnumerable<DotNetType> DotNetTypes => new List<DotNetType>();
        public override IEnumerable<DotNetRelation> DotNetRelations => new List<DotNetRelation>();

        public override void Analyze()
        {
            Project project = OpenProject();

            if (project != null)
            {
                if (AnalyzerSettings.LogLevel == LogLevel.All)
                {
                    string filename = $"project{ProjectFileInfo.Name}.log";
                    AnalyzerLogger.LogProjectProperties(filename, project);
                }

                DefineProjectIncludeDirectories(project);

                foreach (var itemDefinition in project.AllEvaluatedItemDefinitionMetadata)
                {
                    if (itemDefinition.Name == "ForcedIncludeFiles")
                    {
                        char[] separators = {';'};
                        foreach (string forcedInclude in itemDefinition.EvaluatedValue.Split(separators))
                        {
                            if (forcedInclude.Length > 0)
                            {
                                AddForcedInclude(forcedInclude);
                            }
                        }
                    }
                }

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
                                        AddIncludeItem(projectItem);
                                    }
                                    break;
                                }
                            case "ClInclude":
                                {
                                    AddIncludeItem(projectItem);
                                    break;
                                }
                            case "ClCompile":
                                {
                                    AddCompileItem(projectItem);
                                    break;
                                }
                            case "Midl":
                                {
                                    AddMidlItem(projectItem);
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
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }

            return project;
        }
        
        private void DetermineGlobalProperties()
        {
            try
            {
                ProjectRootElement project = ProjectRootElement.Open(ProjectFileInfo.FullName);
                _toolsVersion = AnalyzerSettings.Analysis.ToolsVersion;

                if (project != null)
                {
                    foreach (ProjectItemElement item in project.Items)
                    {
                        if (item.ElementName == "ProjectConfiguration")
                        {
                            string[] projectConfiguration = item.Include.Split('|'); // eg. "Release|x64"
                            _globalProperties["Configuration"] = projectConfiguration[0];
                            _globalProperties["Platform"] = projectConfiguration[1];
                            _globalProperties["SolutionDir"] = NormalizeDirectory(SolutionDir);
                            _globalProperties["SolutionName"] = SolutionName;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
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

        private void AddMidlItem(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    SourceFile sourceFile = AddSourceFile(fileInfo, projectFolder, null);
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

        private void AddCompileItem(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    AddSourceFile(fileInfo, projectFolder, _forcedIncludes);
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

        private void AddIncludeItem(ProjectItem projectItem)
        {
            try
            {
                string projectFolder = _filterFile.GetSourceFileProjectFolder(projectItem.EvaluatedInclude);
                FileInfo fileInfo = new FileInfo(GetAbsolutePath(ProjectFileInfo.DirectoryName, projectItem.EvaluatedInclude));
                if (fileInfo.Exists)
                {
                    AddSourceFile(fileInfo, projectFolder, null);
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
                SourceFile generatedFile = AddSourceFile(generatedFileInfo, projectFolder, null);
                if (generatedFile != null)
                {
                    GeneratedFileRelations.Add(new GeneratedFileRelation(generatedFile, idlSourceFile));
                }
            }
        }

        private void AddForcedInclude(string forcedInclude)
        {
            FileInfo fileInfo = new FileInfo(Path.GetFullPath(forcedInclude));
            if (fileInfo.Exists)
            { 
                _forcedIncludes.Add(fileInfo.FullName);
            }
        }

        private SourceFile AddSourceFile(FileInfo fileInfo, string projectFolder, IEnumerable<string> forcedIncludes)
        {
            string caseInsenstiveFilename = fileInfo.FullName.ToLower();
            SourceFile sourceFile = new SourceFile(fileInfo, projectFolder, forcedIncludes, _includeResolveStrategy);
            RegisterSourceFile(caseInsenstiveFilename, sourceFile);
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
                            string processedIncludeDirectory = includeDirectory.Trim().Replace(@"\r\n", ""); // To fix occasional prefixes

                            if (processedIncludeDirectory.Length > 0)
                            {
                                string errorText = "";
                                if (ResolveReferencedProjectItems(evaluatedProject, ref processedIncludeDirectory, ref errorText))
                                {
                                    try
                                    {
                                        string resolvedIncludeDirectory = Path.GetFullPath(processedIncludeDirectory);

                                        if (Directory.Exists(resolvedIncludeDirectory)) // Is existing absolute include path
                                        {
                                            AddIncludeDirectory(resolvedIncludeDirectory, processedIncludeDirectory);
                                        }
                                        else
                                        {
                                            resolvedIncludeDirectory = GetAbsolutePath(ProjectFileInfo.DirectoryName, processedIncludeDirectory);

                                            if (Directory.Exists(resolvedIncludeDirectory)) // Is existing resolved relative include path
                                            {
                                                AddIncludeDirectory(resolvedIncludeDirectory, processedIncludeDirectory);
                                            }
                                            else
                                            {
                                                AnalyzerLogger.LogErrorIncludePathNotFound(resolvedIncludeDirectory, evaluatedProject.FullPath);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        AnalyzerLogger.LogErrorPathNotResolved(processedIncludeDirectory, evaluatedProject.FullPath);
                                    }
                                }
                                else
                                {
                                    AnalyzerLogger.LogErrorIncludePathNotFound($"{processedIncludeDirectory} ({errorText})", evaluatedProject.FullPath);
                                }
                            }
                        }
                    }
                }
            }

            _includeResolveStrategy = new IncludeResolveStrategy(_includeDirectories, AnalyzerSettings.Input.SystemIncludeDirectories);
        }

        private bool ResolveReferencedProjectItems(Project evaluatedProject, ref string includeDirectory, ref string errorText)
        {
            // See https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-well-known-item-metadata?view=vs-2019 and
            // https://stackoverflow.com/questions/2814424/different-ways-to-pass-variables-in-msbuild

            bool success = false;
            if (includeDirectory.StartsWith(ItemBegin) && includeDirectory.EndsWith(ItemEnd))
            {
                if (!includeDirectory.Contains(ItemSeparator))
                {
                    // Resolve @(ItemName)
                    string itemName = includeDirectory
                        .Replace(ItemBegin, string.Empty)
                        .Replace(ItemEnd, "");

                    string itemValue;
                    if (GetEvaluatedItemValue(evaluatedProject, itemName, out itemValue))
                    {
                        includeDirectory = itemValue;
                        success = true;
                    }
                    else
                    {
                        errorText = $"{itemName} item not found";
                    }
                }
                else
                {
                    if (includeDirectory.Contains(ItemFullPath))
                    {
                        // @(ItemName->'%(FullPath)')
                        string itemName = includeDirectory
                            .Replace(ItemBegin, string.Empty)
                            .Replace(ItemFullPath, string.Empty)
                            .Replace(ItemEnd, "");

                        string itemValue;
                        if (GetEvaluatedItemValue(evaluatedProject, itemName, out itemValue))
                        {
                            try
                            {
                                includeDirectory = new DirectoryInfo(itemValue).FullName;
                                success = true;
                            }
                            catch (Exception)
                            {
                                errorText = $"{itemName} item has value {itemValue} which is not a valid path";
                            }
                        }
                        else
                        {
                            errorText = $"{itemName} item not found";
                        }
                    }
                    else
                    {
                        // e.g. @(ItemName->'%(%(RootDir))')
                        errorText = "Format unsupported";
                    }
                }
            }
            else
            {
                // No item reference used
                success = true;
            }

            return success;
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
