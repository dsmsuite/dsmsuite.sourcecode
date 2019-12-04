using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Builder.Settings
{
    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class BuilderSettings
    {
        private bool _loggingEnabled;
        private string _inputFilename;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static BuilderSettings CreateDefault()
        {
            BuilderSettings settings = new BuilderSettings
            {
                LoggingEnabled = false,
                InputFilename = "Input.dsi",
                OutputFilename = "Output.dsm",
                CompressOutputFile = true
            };

            return settings;
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

        public static void WriteToFile(string filename, BuilderSettings builderSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(BuilderSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, builderSettings);
            }
        }

        public static BuilderSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BuilderSettings));

            BuilderSettings builderSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                builderSettings = (BuilderSettings)serializer.Deserialize(reader);
            }

            builderSettings.ResolvePaths(Path.GetDirectoryName(filename));
            return builderSettings;
        }

        private void ResolvePaths(string settingFilePath)
        {
            InputFilename = Resolve(settingFilePath, InputFilename);
            OutputFilename = Resolve(settingFilePath, OutputFilename);
        }

        private static string Resolve(string path, string filename)
        {
            if (File.Exists(filename))
            {
                return filename;
            }
            else
            {
                string absoluteFilename = Path.GetFullPath(Path.Combine(path, filename));
                Logger.LogUserMessage("Resolve file: path=" + path + " file=" + filename + " as file=" + absoluteFilename);
                return absoluteFilename;
            }
        }
    }
}
