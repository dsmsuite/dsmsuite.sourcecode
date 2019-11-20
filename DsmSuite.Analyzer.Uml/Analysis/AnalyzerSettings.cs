using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Uml.Analysis
{
    /// <summary>
    /// Settings used during analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings
    {
        private bool _loggingEnabled;
        private string _inputFilename;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings umlAnalyzerSettings = new AnalyzerSettings
            {
                LoggingEnabled = true,
                InputFilename = "Model.eap",
                OutputFilename = "Output.dsi",
                CompressOutputFile = true
            };

            return umlAnalyzerSettings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        public string InputFilename
        {
            get { return _inputFilename; }
            set { _inputFilename = value; }
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
            InputFilename = FilePath.ResolveFile(settingFilePath, InputFilename);
            OutputFilename = FilePath.ResolveFile(settingFilePath, OutputFilename);
        }
    }
}
