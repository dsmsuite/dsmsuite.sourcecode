using DsmSuite.Common.Util;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.Analyzer.DotNet.Settings
{
    [Serializable]
    public class InputSettings
    {
        public List<string> AssemblyDirectories { get; set; }

        public List<string> IncludeAssemblyNames { get; set; }
    }

    [Serializable]
    public class TransformationSettings
    {
        public List<string> IgnoredNames { get; set; }

        public List<string> IncludedNames { get; set; }
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
        public TransformationSettings Transformation { get; set; }
        public OutputSettings Output { get; set; }

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LogLevel = LogLevel.Error,
                Input = new InputSettings(),
                Transformation = new TransformationSettings(),
                Output = new OutputSettings(),
            };

            analyzerSettings.Input.AssemblyDirectories = new List<string>();
            analyzerSettings.Input.IncludeAssemblyNames = new List<string>();

            analyzerSettings.Transformation.IgnoredNames = new List<string>
            {
                // Ignore Microsoft stuff
                "^System.",
                "^Microsoft.",
                // Ignore COM/C++ Interop stuff
                "^Interop",
                // Ignore anonymous classes
                "<",
                "^_"
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
            for (int i = 0; i < Input.AssemblyDirectories.Count; i++)
            {
                Input.AssemblyDirectories[i] = FilePath.ResolveFile(settingFilePath, Input.AssemblyDirectories[i]);
            }
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
