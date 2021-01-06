using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    class DirectoryView : ViewBase
    {
        public DirectoryView(SolutionFile solutionFile, AnalyzerSettings analyzerSettings) : 
            base(solutionFile, analyzerSettings)
        {
        }

        public override void RegisterInterfaceFile(string interfaceFile)
        {

        }

        public override void RegisterSourceFile(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {

        }

        public override string GetName(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            return GetName2(sourceFile.SourceFileInfo.FullName);
        }

        public override string ResolveProvider(ProjectFileBase visualStudioProject, string includedFile)
        {
            return GetName2(includedFile);
        }

        private string GetName2(string filename)
        {
            string name = "";

            string rootDirectory = AnalyzerSettings.Input.RootDirectory.Trim('\\'); // Ensure without trailing \
            if (filename.StartsWith(rootDirectory))
            {
                int start = rootDirectory.Length + 1;
                name = filename.Substring(start).Replace("\\", ".");
            }

            return name;
        }
    }
}
