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

        public override string GetSourceFileElementName(ProjectFileBase visualStudioProject, SourceFile sourceFile)
        {
            return GetElementName(sourceFile.SourceFileInfo.FullName);
        }

        public override string ResolveIncludeFileProviderName(ProjectFileBase visualStudioProject, string includedFile)
        {
            if (!IsSystemInclude(includedFile))
            {
                return GetElementName(includedFile);
            }
            else
            {
                // Ignore system include
                return "";
            }
        }

        private string GetElementName(string filename)
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
