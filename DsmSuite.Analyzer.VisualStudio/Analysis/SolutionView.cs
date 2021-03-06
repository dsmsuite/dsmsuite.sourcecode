﻿using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Utils;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    class SolutionView : ViewBase
    {
        private readonly Dictionary<string, ProjectFileBase> _projectForSourceFilePath = new Dictionary<string, ProjectFileBase>();
        private readonly Dictionary<string, FileInfo> _sourcesFilesByChecksum = new Dictionary<string, FileInfo>();
        private readonly Dictionary<string, string> _interfaceFileChecksumsByFilePath = new Dictionary<string, string>();

        public SolutionView(SolutionFile solutionFile, AnalyzerSettings analyzerSettings) :
            base(solutionFile, analyzerSettings)
        {
        }

        public override void RegisterInterfaceFile(string interfaceFile)
        {
            _interfaceFileChecksumsByFilePath[interfaceFile.ToLower()] = new SourceFile(interfaceFile).Checksum;
        }

        public override void RegisterSourceFile(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            if (_interfaceFileChecksumsByFilePath.Count > 0)
            {
                _sourcesFilesByChecksum[sourceFile.Checksum] = sourceFile.SourceFileInfo;
            }
            _projectForSourceFilePath[sourceFile.SourceFileInfo.FullName.ToLower()] = visualStudioProject;
        }

        public override string GetSourceFileElementName(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            string name = "";

            if (!string.IsNullOrEmpty(SolutionFile?.Name))
            {
                name += SolutionFile.Name;
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

        public override string ResolveIncludeFileProviderName(ProjectFileBase visualStudioProject, string includedFile)
        {
            string providerName = null;
            if (IsProjectInclude(includedFile))
            {
                // Register as normal visual studio project include
                providerName = ResolveProjectInclude(includedFile);
            }
            else if (IsInterfaceInclude(includedFile))
            {
                // Interface includes must be clones of includes files in other visual studio projects
                string resolvedIncludedFile = ResolveInterfaceInclude(includedFile);
                if (resolvedIncludedFile != null)
                {
                    providerName = ResolveProjectInclude(resolvedIncludedFile);
                }
            }
            else if (IsExternalInclude(includedFile))
            {
                // Register external include
                providerName = ResolveExternalInclude(includedFile);
            }
            else if (IsSystemInclude(includedFile))
            {
                // Ignore system include
                providerName = "";
            }
            else
            {
                // Skip not resolved interface
                AnalyzerLogger.LogIncludeFileNotFoundInVisualStudioProject(includedFile, visualStudioProject.ProjectName);
            }

            return providerName;
        }

        private string ResolveProjectInclude(string includedFile)
        {
            string providerName = null;
            string caseInsensitiveFilename = includedFile.ToLower();

            if (_projectForSourceFilePath.ContainsKey(caseInsensitiveFilename))
            {
                ProjectFileBase projectFile = _projectForSourceFilePath[caseInsensitiveFilename];
                SourceFile includeFile = projectFile.GetSourceFile(caseInsensitiveFilename);
                providerName = GetSourceFileElementName(projectFile, includeFile);
            }

            return providerName;
        }

        private string ResolveInterfaceInclude(string includedFile)
        {
            // Interface files can be clones of source files found in visual studio projects 
            string resolvedIncludedFile = null;
            if (_interfaceFileChecksumsByFilePath.ContainsKey(includedFile.ToLower()))
            {
                string checksum = _interfaceFileChecksumsByFilePath[includedFile.ToLower()];
                if (_sourcesFilesByChecksum.ContainsKey(checksum))
                {
                    resolvedIncludedFile = _sourcesFilesByChecksum[checksum].FullName;
                    Logger.LogInfo("Included interface resolved: " + includedFile + " -> " + resolvedIncludedFile);
                }
            }
            return resolvedIncludedFile;
        }

        public string ResolveExternalInclude(string includedFile)
        {
            string usedExternalIncludeDirectory = null;
            string resolveAs = null;
            foreach (ExternalIncludeDirectory externalIncludeDirectory in AnalyzerSettings.Input.ExternalIncludeDirectories)
            {
                if (includedFile.StartsWith(externalIncludeDirectory.Path))
                {
                    usedExternalIncludeDirectory = externalIncludeDirectory.Path;
                    resolveAs = externalIncludeDirectory.ResolveAs;
                }
            }

            string providerName = null;

            if ((usedExternalIncludeDirectory != null) &&
                (resolveAs != null))
            {
                providerName = includedFile.Replace(usedExternalIncludeDirectory, resolveAs).Replace("\\", ".");
            }

            return providerName;
        }

        private bool IsProjectInclude(string includedFile)
        {
            return _projectForSourceFilePath.ContainsKey(includedFile.ToLower());
        }

    }
}
