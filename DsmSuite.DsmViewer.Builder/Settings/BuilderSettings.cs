using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Builder.Settings
{
    [Serializable]
    public class InputSettings
    {
        public string Filename { get; set; }
    }

    [Serializable]
    public class TransformationSettings
    {
        public bool ApplyPartitioningAlgorithm { get; set; }
    }

    [Serializable]
    public class OutputSettings
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
    }

    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class BuilderSettings
    {
        public LogLevel LogLevel { get; set; }
        public InputSettings Input { get; set; }
        public TransformationSettings Transformation { get; set; }
        public OutputSettings Output { get; set; }

        public static BuilderSettings CreateDefault()
        {
            BuilderSettings builderSettings = new BuilderSettings
            {
                LogLevel = LogLevel.None,
                Input = new InputSettings(),
                Transformation = new TransformationSettings(),
                Output = new OutputSettings(),
            };

            builderSettings.Input.Filename = "Input.dsi";

            builderSettings.Transformation.ApplyPartitioningAlgorithm = false;

            builderSettings.Output.Filename = "Output.dsm";
            builderSettings.Output.Compress = true;

            return builderSettings;
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
            Input.Filename = Resolve(settingFilePath, Input.Filename);
            Output.Filename = Resolve(settingFilePath, Output.Filename);
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
