using DsmSuite.Analyzer.Transformations.Settings;
using DsmSuite.Common.Util;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.Cpp.Settings
{
    [Serializable]
    public class InputSettings
    {
        public string RootDirectory { get; set; }
        public List<string> SourceDirectories { get; set; }
        public List<string> ExternalIncludeDirectories { get; set; }
        public List<string> IgnorePaths { get; set; }
    }

    /// <summary>
    /// Because this analyzer does not uses include paths, included files can be ambiguous. This enum defines how to resolve this.
    /// Each option has its downsides. 
    /// -Ignore dependencies which are ambiguous. Not all actual dependencies are reported in the DSM.
    /// -Try to find the best matching file. If the algorithm gets it wrong, incorrect dependencies will be reported in the DSM.
    /// -Add possible choices. More than actual dependencies are reported in the DSM.
    /// </summary>
    [Serializable]
    public enum ResolveMethod
    {
        Ignore,
        AddBestMatch,
        AddAll,
    }

    [Serializable]
    public class AnalysisSettings
    {
        public ResolveMethod ResolveMethod { get; set; }
    }

    [Serializable]
    public enum TransformationModuleMergeStrategy
    {
        None,
        MoveHeaderFileToSourceFile,
        MergeHeaderAndSourceFileDirectory
    }

    [Serializable]
    public class TransformationSettings
    {
        public List<string> IgnoredNames { get; set; }
        public bool AddTransitiveIncludes { get; set; }
        public TransformationModuleMergeStrategy ModuleMergeStrategy { get; set; }
        public List<TransformationModuleMergeRule> ModuleMergeRules { get; set; }
    }

    [Serializable]
    public class OutputSettings
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
    }

    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings
    {
        public LogLevel LogLevel { get; set; }
        public InputSettings Input { get; set; }
        public AnalysisSettings Analysis { get; set; }
        public TransformationSettings Transformation { get; set; }
        public OutputSettings Output { get; set; }

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LogLevel = LogLevel.Error,
                Input = new InputSettings(),
                Analysis = new AnalysisSettings(),
                Transformation = new TransformationSettings(),
                Output = new OutputSettings(),
            };

            analyzerSettings.Input.RootDirectory = @"C:\";
            analyzerSettings.Input.SourceDirectories = new List<string> { @"C:\" };
            analyzerSettings.Input.ExternalIncludeDirectories = new List<string> { @"C:\" };
            analyzerSettings.Input.IgnorePaths = new List<string> { @"C:\" };

            analyzerSettings.Analysis.ResolveMethod = ResolveMethod.AddBestMatch;
            analyzerSettings.Transformation.IgnoredNames = new List<string>();
            analyzerSettings.Transformation.AddTransitiveIncludes = false;
            analyzerSettings.Transformation.ModuleMergeStrategy = TransformationModuleMergeStrategy.None;
            analyzerSettings.Transformation.ModuleMergeRules = new List<TransformationModuleMergeRule>
            {
                new TransformationModuleMergeRule() {From = "inc.", To = "src."}
            };

            analyzerSettings.Output.Filename = "Output.dsi";
            analyzerSettings.Output.Compress = true;

            return analyzerSettings;
        }

        public static void WriteToFile(string filename, AnalyzerSettings analyzerSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(AnalyzerSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, analyzerSettings);
            }
        }

        public static AnalyzerSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AnalyzerSettings));

            AnalyzerSettings analyzerSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                analyzerSettings = (AnalyzerSettings)serializer.Deserialize(reader);
            }

            analyzerSettings.ResolvePaths(Path.GetDirectoryName(filename));
            return analyzerSettings;
        }

        private void ResolvePaths(string settingFilePath)
        {
            Input.RootDirectory = FilePath.ResolveFile(settingFilePath, Input.RootDirectory);
            Input.SourceDirectories = FilePath.ResolveFiles(settingFilePath, Input.SourceDirectories);
            Input.ExternalIncludeDirectories = FilePath.ResolveFiles(settingFilePath, Input.ExternalIncludeDirectories);
            Input.IgnorePaths = FilePath.ResolveFiles(settingFilePath, Input.IgnorePaths);
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
