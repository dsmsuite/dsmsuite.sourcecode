using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.DotNet.Lib;
using Microsoft.Build.Evaluation;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public abstract class ProjectFileBase
    {
        protected ProjectFileBase(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver)
        {
            SolutionFolder = solutionFolder;
            SolutionDir = solutionDir;
            ProjectFileInfo = new FileInfo(projectPath);
            ProjectName = ProjectFileInfo.Name;
            SourceFiles = new HashSet<SourceFile>();
            AnalyzerSettings = analyzerSettings;
            TargetExtension = "";
            GeneratedFileRelations = new List<GeneratedFileRelation>();
            Resolver = resolver;
        }

        public string SolutionFolder { get; }

        public string SolutionDir { get; }

        public string ProjectName { get; }

        public abstract BinaryFile BuildAssembly { get; }

        public abstract void Analyze();

        public ICollection<GeneratedFileRelation> GeneratedFileRelations { get; protected set; }

        public string TargetExtension { get; protected set; }

        public HashSet<SourceFile> SourceFiles { get; }

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
