using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Util;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    /// <summary>
    /// Class to analyze visual studio solution file contents
    /// </summary>
    public class SolutionFile
    {
        private readonly FileInfo _solutionFileInfo;
        private readonly string _name;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private bool _parsingProjectNesting;
        private readonly Dictionary<string, string> _solutionFolderNames = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _solutionFolderParents = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _projectRelativeFilenames = new Dictionary<string, string>();
        private readonly Dictionary<string, ProjectFile> _projects = new Dictionary<string, ProjectFile>();

        private const string BeginProject = "Project(";
        private const string BeginGlobalSection = "GlobalSection";
        private const string EndGlobalSection = "EndGlobalSection";
        private const string NestedProjects = "(NestedProjects)";
        private const string VcxprojType = "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942";
        private const string CsprojType = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
        private const string SolutionFolderType = "2150E333-8FDC-42A3-9474-1A3956D46DE8";

        private int _progressPercentage;

        public SolutionFile(string solutionPath, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _solutionFileInfo = new FileInfo(solutionPath);
            _name = _solutionFileInfo.Name;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            ParseFile();
            FindProjectsInSolution();

            int analyzedProjects = 0;
            foreach (ProjectFile visualStudioProject in _projects.Values)
            {
                visualStudioProject.Analyze();
                analyzedProjects++;
                UpdateProgress("Analyze projects", _projects.Count, analyzedProjects, "projects");
            }

            int totalSourceFiles = 0;
            foreach (ProjectFile visualStudioProject in _projects.Values)
            {
                totalSourceFiles += visualStudioProject.SourceFiles.Count;
            }

            int analyzedSourceFiles = 0;
            foreach (ProjectFile visualStudioProject in _projects.Values)
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    sourceFile.Analyze();
                    analyzedSourceFiles++;
                    UpdateProgress("Analyze source files", totalSourceFiles, analyzedSourceFiles, "files");
                }
            }

        }

        public string Name => _name;

        public IReadOnlyCollection<ProjectFile> Projects => _projects.Values;

        private void ParseFile()
        {
            foreach (string line in File.ReadLines(_solutionFileInfo.FullName))
            {
                if (line.Contains(BeginProject))
                {
                    // Example: Project("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}") = "SystemConfiguration", "Libs\SystemConfiguration\SystemConfiguration.vcxproj", "{03043243-4E5A-45ED-8B82-58713415323C}"	
                    char[] separators = { '"', '{', '}' };
                    string[] words = line.Split(separators);
                    if (words.Length == 13)
                    {
                        string projectTypeGuid = words[2];
                        string projectName = words[5];
                        string relativeProjectFilename = words[7];
                        string projectGuid = words[10];

                        switch (projectTypeGuid)
                        {
                            case VcxprojType:
                                _projectRelativeFilenames[projectGuid] = relativeProjectFilename;
                                break;
                            case CsprojType:
                                _projectRelativeFilenames[projectGuid] = relativeProjectFilename;
                                break;
                            case SolutionFolderType:
                                _solutionFolderNames[projectGuid] = projectName;
                                break;
                        }
                    }
                }

                if (line.Contains(EndGlobalSection))
                {
                    _parsingProjectNesting = false;
                }

                if (_parsingProjectNesting)
                {
                    // Example: 		{507425FC-300C-4279-9E16-66B5DB0BA39B} = {D7790839-69FC-4B96-A152-2D8263F1F566}
                    char[] separators = { '=', '{', '}' };
                    string[] words = line.Split(separators);
                    if (words.Length == 6)
                    {
                        string childGuid = words[1];
                        string parentGuid = words[4];
                        _solutionFolderParents[childGuid] = parentGuid;
                    }
                }

                if (line.Contains(BeginGlobalSection))
                {
                    if (line.Contains(NestedProjects))
                    {
                        _parsingProjectNesting = true;
                    }
                }
            }
        }

        private void FindProjectsInSolution()
        {
            foreach (KeyValuePair<string, string> kv in _projectRelativeFilenames)
            {
                string solutionFolder = GetSolutionFolder(kv.Key);
                AddProjectFile(kv.Key, solutionFolder, kv.Value);
            }
        }

        private string GetSolutionFolder(string guid)
        {
            string solutionFolder = "";
            BuildSolutionFolder(guid, ref solutionFolder);

            if (solutionFolder.Length > 1)
            {
                return solutionFolder.Substring(0, solutionFolder.Length - 1);
            }
            else
            {
                return solutionFolder;
            }
        }

        private void BuildSolutionFolder(string guid, ref string solutionFolder)
        {
            if (_solutionFolderParents.ContainsKey(guid))
            {
                string parentGuid = _solutionFolderParents[guid];

                solutionFolder = _solutionFolderNames[parentGuid] + "." + solutionFolder;

                BuildSolutionFolder(parentGuid, ref solutionFolder);
            }
        }

        private void AddProjectFile(string guid, string solutionFolder, string relativeProjectFilename)
        {
            string solutionDir = _solutionFileInfo.DirectoryName;
            string absoluteProjectFilename = ResolvePath(solutionDir, relativeProjectFilename);
            if (absoluteProjectFilename != null)
            {
                FileInfo projectFileInfo = new FileInfo(absoluteProjectFilename);
                if (projectFileInfo.Exists)
                {
                    if (absoluteProjectFilename.EndsWith("vcxproj"))
                    {
                        ProjectFile projectFile = new ProjectFile(solutionFolder, solutionDir, absoluteProjectFilename, _analyzerSettings);
                        _projects[guid] = projectFile;
                    }
                    else
                    {
                        Logger.LogInfo("File ignored " + absoluteProjectFilename);
                    }
                }
                else
                {
                    AnalyzerLogger.LogErrorFileNotFound(absoluteProjectFilename, _solutionFileInfo.FullName);
                }
            }
            else
            {
                AnalyzerLogger.LogErrorPathNotResolved(relativeProjectFilename, _solutionFileInfo.FullName);
            }
        }

        private string ResolvePath(string root, string path)
        {
            string strippedRoot = StripTrailingPathSeparator(root);
            string strippedPath = StripTrailingPathSeparator(StripLeadingPathSeparator(path));
            string combinedPath = Path.Combine(strippedRoot, strippedPath);

            try
            {
                return Path.GetFullPath(combinedPath);
            }
            catch (Exception e)
            {
                Logger.LogException($"Resolve path failed solution={_solutionFileInfo.FullName}", e);
                return null;
            }
        }

        private static string StripLeadingPathSeparator(string path)
        {
            if (path.StartsWith(@"\"))
            {
                return path.Substring(1);
            }
            else
            {
                return path;
            }
        }

        private static string StripTrailingPathSeparator(string path)
        {
            if (path.EndsWith(@"\"))
            {
                return path.Substring(0, path.Length - 1);
            }
            else
            {
                return path;
            }
        }

        private void UpdateProgress(string actionText, int totalItemCount, int itemCount, string itemType)
        {
            if (_progress != null)
            {
                int currentProgressPercentage = 0;
                if (itemCount > 0)
                {
                    currentProgressPercentage = itemCount * 100 / totalItemCount;
                }

                if (_progressPercentage != currentProgressPercentage)
                {
                    _progressPercentage = currentProgressPercentage;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ActionText = actionText,
                        TotalItemCount = totalItemCount,
                        CurrentItemCount = itemCount,
                        ItemType = itemType,
                        Percentage = currentProgressPercentage,
                        Done = totalItemCount == itemCount
                    };

                    _progress.Report(progressInfoInfo);
                }
            }
        }
    }
}
