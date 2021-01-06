using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.DotNet.Lib;
using Microsoft.Build.Evaluation;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public abstract class ProjectFileBase
    {
        private readonly Dictionary<string, SourceFile> _sourceFiles = new Dictionary<string, SourceFile>();

        protected ProjectFileBase(string solutionFolder, string solutionDir, string solutionName, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver)
        {
            SolutionFolder = solutionFolder;
            SolutionDir = solutionDir;
            SolutionName = solutionName;
            ProjectFileInfo = new FileInfo(projectPath);
            ProjectName = ProjectFileInfo.Name;
            AnalyzerSettings = analyzerSettings;
            TargetExtension = "";
            GeneratedFileRelations = new List<GeneratedFileRelation>();
            Resolver = resolver;
        }

        protected void AddSourceFile(string caseInsensitiveFilename, SourceFile sourceFile)
        {
            _sourceFiles[caseInsensitiveFilename] = sourceFile;
        }

        public SourceFile GetSourceFile(string caseInsensitiveFilename)
        {
            if (_sourceFiles.ContainsKey(caseInsensitiveFilename))
            {
                return _sourceFiles[caseInsensitiveFilename];
            }
            else
            {
                return null;
            }
        }

        public string SolutionFolder { get; }

        public string SolutionDir { get; }

        public string SolutionName { get; }

        public string ProjectName { get; }

        public abstract BinaryFile BuildAssembly { get; }

        public abstract void Analyze();

        public abstract bool Success { get; protected set; }

        public ICollection<GeneratedFileRelation> GeneratedFileRelations { get; protected set; }

        public string TargetExtension { get; protected set; }

        public IEnumerable<SourceFile> SourceFiles => _sourceFiles.Values;

        public abstract IEnumerable<DotNetType> DotNetTypes { get; }

        public abstract IEnumerable<DotNetRelation> DotNetRelations { get; }

        public FileInfo ProjectFileInfo { get; }

        protected AnalyzerSettings AnalyzerSettings { get; }

        protected DotNetResolver Resolver { get; }

        protected abstract Project OpenProject();

        protected void CloseProject(Project project)
        {
            try
            {
                project.ProjectCollection.UnloadProject(project);
            }
            catch (Exception e)
            {
                Logger.LogException($"Exception while closing project={ProjectFileInfo.FullName}", e);
            }
        }
    }
}
