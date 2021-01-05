using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Utils;
using DsmSuite.Common.Util;
using Microsoft.Build.Construction;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class SolutionFile
    {
        private readonly FileInfo _solutionFileInfo;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly Dictionary<string, ProjectFileBase> _projects = new Dictionary<string, ProjectFileBase>();
        private readonly Dictionary<string, SolutionNode> _solutionNodes = new Dictionary<string, SolutionNode>();
        private readonly DotNetResolver _resolver = new DotNetResolver();

        public SolutionFile(string solutionPath, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _solutionFileInfo = new FileInfo(solutionPath);
            Name = _solutionFileInfo.Name;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            FindProjects();

            foreach (ProjectFileBase visualStudioProject in _projects.Values)
            {
                if (visualStudioProject.BuildAssembly != null)
                {
                    _resolver.AddSearchPath(visualStudioProject.BuildAssembly);
                }
            }

            int analyzedProjects = 0;
            Parallel.ForEach(_projects.Values, visualStudioProject =>
            {
                visualStudioProject.Analyze();
                analyzedProjects++;
                UpdateProjectFileProgress(analyzedProjects, _projects.Count);
            });

            int totalSourceFiles = 0;
            foreach (ProjectFileBase visualStudioProject in _projects.Values)
            {
                totalSourceFiles += visualStudioProject.SourceFiles.Count();
            }

            int analyzedSourceFiles = 0;
            Parallel.ForEach(_projects.Values, visualStudioProject =>
            {
                foreach (SourceFile sourceFile in visualStudioProject.SourceFiles)
                {
                    sourceFile.Analyze();
                    analyzedSourceFiles++;
                    UpdateSourceFileProgress(analyzedSourceFiles, totalSourceFiles);
                }
            });

            TotalSourceFiles = totalSourceFiles;
        }

        public string Name { get; }
        public int TotalSourceFiles { get; private set; }

        public IReadOnlyCollection<ProjectFileBase> Projects => _projects.Values;

        private void FindProjects()
        {
            FindSolutionNodes();
            BuildSolutionNodeHierarchy();
            AddFoundSolutionNodesAsProjects();
        }

        private void FindSolutionNodes()
        {
            Microsoft.Build.Construction.SolutionFile solutionFile =
                Microsoft.Build.Construction.SolutionFile.Parse(_solutionFileInfo.FullName);

            foreach (ProjectInSolution project in solutionFile.ProjectsInOrder)
            {
                if ((project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat) ||
                    (project.ProjectType == SolutionProjectType.SolutionFolder))
                {
                    _solutionNodes[project.ProjectGuid] = new SolutionNode(project);
                }
            }
        }

        private void BuildSolutionNodeHierarchy()
        {
            foreach (SolutionNode solutionNode in _solutionNodes.Values)
            {
                SolutionNode solutionNodeParent = null;

                if ((solutionNode.ParentGuid != null) &&
                    _solutionNodes.ContainsKey(solutionNode.ParentGuid))
                {
                    solutionNodeParent = _solutionNodes[solutionNode.ParentGuid];
                }

                solutionNodeParent?.AddChild(solutionNode);
            }
        }

        private void AddFoundSolutionNodesAsProjects()
        {
            foreach (SolutionNode solutionNode in _solutionNodes.Values)
            {
                if (solutionNode.Type == SolutionProjectType.KnownToBeMSBuildFormat)
                {
                    AddProjectFile(solutionNode.Guid, solutionNode.SolutionFolder, solutionNode.AbsolutePath);
                }
            }
        }

        private void AddProjectFile(string guid, string solutionFolder, string absoluteProjectFilename)
        {
            FileInfo projectFileInfo = new FileInfo(absoluteProjectFilename);
            if (projectFileInfo.Exists)
            {
                string solutionName = _solutionFileInfo.Name.Replace(".sln", "");
                if (absoluteProjectFilename.EndsWith("vcxproj"))
                {
                    VcxProjectFile projectFile = new VcxProjectFile(solutionFolder, _solutionFileInfo.DirectoryName, solutionName, absoluteProjectFilename, _analyzerSettings, _resolver);
                    _projects[guid] = projectFile;
                }
                else if (absoluteProjectFilename.EndsWith("csproj"))
                {
                    //CsProjectFile projectFile = new CsProjectFile(solutionFolder, _solutionFileInfo.DirectoryName, solutionName, absoluteProjectFilename, _analyzerSettings, _resolver);
                    //_projects[guid] = projectFile;
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

        private void UpdateProjectFileProgress(int currentItemCount, int totalItemCount)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Analyzing project files";
            progressInfo.CurrentItemCount = currentItemCount;
            progressInfo.TotalItemCount = totalItemCount;
            progressInfo.ItemType = "files";
            progressInfo.Percentage = currentItemCount * 100 / totalItemCount;
            progressInfo.Done = currentItemCount == totalItemCount;
            _progress?.Report(progressInfo);
        }

        private void UpdateSourceFileProgress(int currentItemCount, int totalItemCount)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Analyzing source files";
            progressInfo.CurrentItemCount = currentItemCount;
            progressInfo.TotalItemCount = totalItemCount;
            progressInfo.ItemType = "files";
            progressInfo.Percentage = currentItemCount * 100 / totalItemCount;
            progressInfo.Done = currentItemCount == totalItemCount;
            _progress?.Report(progressInfo);
        }
    }
}
