using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;

namespace DsmSuite.Analyzer.VisualStudio.Analysis
{
    abstract class ViewBase
    {
        protected AnalyzerSettings AnalyzerSettings { get; }
        protected SolutionFile SolutionFile { get; }

        protected ViewBase(SolutionFile solutionFile, AnalyzerSettings analyzerSettings)
        {
            SolutionFile = solutionFile;
            AnalyzerSettings = analyzerSettings;
        }

        public string GetDotNetTypeElementName(ProjectFileBase visualStudioProject, string typeName)
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

            name += typeName;

            return name.Replace("\\", ".");
        }

        public abstract void RegisterInterfaceFile(string interfaceFile);
        public abstract void RegisterSourceFile(ProjectFileBase visualStudioProject, SourceFile sourceFile);
        public abstract string GetSourceFileElementName(ProjectFileBase visualStudioProject, SourceFile sourceFile);
        public abstract string ResolveIncludeFileProviderName(ProjectFileBase visualStudioProject, string includedFile);

        protected bool IsSystemInclude(string includedFile)
        {
            bool isSystemInclude = false;

            foreach (string systemIncludeDirectory in AnalyzerSettings.Input.SystemIncludeDirectories)
            {
                if (includedFile.StartsWith(systemIncludeDirectory))
                {
                    isSystemInclude = true;
                }
            }
            return isSystemInclude;
        }

        protected bool IsExternalInclude(string includedFile)
        {
            bool isExternalInclude = false;

            foreach (ExternalIncludeDirectory externalIncludeDirectory in AnalyzerSettings.Input.ExternalIncludeDirectories)
            {
                if (includedFile.StartsWith(externalIncludeDirectory.Path))
                {
                    isExternalInclude = true;
                }
            }

            return isExternalInclude;
        }

        protected bool IsInterfaceInclude(string includedFile)
        {
            bool isInterfaceInclude = false;

            foreach (string interfaceIncludeDirectory in AnalyzerSettings.Input.InterfaceIncludeDirectories)
            {
                if (includedFile.StartsWith(interfaceIncludeDirectory))
                {
                    isInterfaceInclude = true;
                }
            }
            return isInterfaceInclude;
        }
    }
}
