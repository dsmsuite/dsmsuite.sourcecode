using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.Analyzer.VisualStudio.Settings;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public abstract class ProjectFileBase
    {
        public ProjectFileBase(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings)
        {
            SolutionFolder = solutionFolder;
            SolutionDir = solutionDir;
            ProjectFileInfo = new FileInfo(projectPath);
            ProjectName = ProjectFileInfo.Name;
            SourceFiles = new HashSet<SourceFile>();
            AnalyzerSettings = analyzerSettings;
            TargetExtension = "";
            GeneratedFileRelations = new List<GeneratedFileRelation>();
        }

        public string SolutionFolder { get; }

        public string SolutionDir { get; }

        public string ProjectName { get; }

        public abstract void Analyze();

        public ICollection<GeneratedFileRelation> GeneratedFileRelations { get; protected set; }

        public string TargetExtension { get; protected set; }

        public HashSet<SourceFile> SourceFiles { get; private set; }

        protected FileInfo ProjectFileInfo { get; private set; }

        protected AnalyzerSettings AnalyzerSettings { get; private set; }
    }
}
