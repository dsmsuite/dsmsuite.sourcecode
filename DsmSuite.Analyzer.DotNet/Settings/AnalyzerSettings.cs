using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.DotNet.Settings
{
    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings
    {
        private bool _loggingEnabled;
        private string _assemblyDirectory;
        private List<string> _externalNames;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LoggingEnabled = true,
                AssemblyDirectory = "",
                ExternalNames = new List<string>(),
                OutputFilename = "Output.dsi",
                CompressOutputFile = true
            };

            analyzerSettings.ExternalNames.Add("System.");
            analyzerSettings.ExternalNames.Add("Microsoft.");
            analyzerSettings.ExternalNames.Add("Interop/");
            analyzerSettings.ExternalNames.Add("&lt;");
            analyzerSettings.ExternalNames.Add("$");
            analyzerSettings.ExternalNames.Add("_");
            return analyzerSettings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        public string AssemblyDirectory
        {
            get { return _assemblyDirectory; }
            set { _assemblyDirectory = value; }
        }

        public List<string> ExternalNames
        {
            get { return _externalNames; }
            set { _externalNames = value; }
        }

        public string OutputFilename
        {
            get { return _outputFilename; }
            set { _outputFilename = value; }
        }

        public bool CompressOutputFile
        {
            get { return _compressOutputFile; }
            set { _compressOutputFile = value; }
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
            AssemblyDirectory = FilePath.ResolveFile(settingFilePath, AssemblyDirectory);
            OutputFilename = FilePath.ResolveFile(settingFilePath, OutputFilename);
        }
    }
}
