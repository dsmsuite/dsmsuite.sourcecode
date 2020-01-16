using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.Transformer.Settings
{
    [Serializable]
    public class AddTransitiveRelationsSettings
    {
        public bool Enabled { get; set; }
    }

    [Serializable]
    public class MoveElementRule
    {
        public string From { get; set; }

        public string To { get; set; }
    }

    [Serializable]
    public class MoveElementsSettings
    {
        public bool Enabled { get; set; }
        public List<MoveElementRule> Rules { get; set; }
    }

    [Serializable]
    public class MoveHeaderElementsSettings
    {
        public bool Enabled { get; set; }
    }

    [Serializable]
    public class IncludeFilterSettings
    {
        public bool Enabled { get; set; }
        public List<string> Names { get; set; }
    }

    [Serializable]
    public class SplitProductAndTestElementsSettings
    {
        public bool Enabled { get; set; }
        public string TestElementIdentifier { get; set; }
        public string ProductElementIdentifier { get; set; }
    }

    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class TransformerSettings
    {
        private bool _loggingEnabled;
        private string _inputFilename;
        private AddTransitiveRelationsSettings _addTransitiveRelationsSettings;
        private MoveElementsSettings _moveElementsSettings;
        private MoveHeaderElementsSettings _moveHeaderElementsSettings;
        private SplitProductAndTestElementsSettings _splitProductAndTestElementsSettings;
        private IncludeFilterSettings _includeFilterSettings;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static TransformerSettings CreateDefault()
        {
            TransformerSettings transformerSettings = new TransformerSettings
            {
                LoggingEnabled = true,
                InputFilename = @"C:\exampleIn.dsi",
                AddTransitiveRelationsSettings = new AddTransitiveRelationsSettings(),
                MoveElementsSettings = new MoveElementsSettings(),
                MoveHeaderElementsSettings = new MoveHeaderElementsSettings(),
                SplitProductAndTestElementsSettings = new SplitProductAndTestElementsSettings(),
                IncludeFilterSettings = new IncludeFilterSettings(),
                OutputFilename = @"C:\exampleOut.dsi",
                CompressOutputFile = true
            };

            transformerSettings.MoveElementsSettings.Rules = new List<MoveElementRule>
            {
                new MoveElementRule() {From = "Header Files.", To = "Source Files."}
            };

            transformerSettings.SplitProductAndTestElementsSettings.ProductElementIdentifier = "Src";
            transformerSettings.SplitProductAndTestElementsSettings.TestElementIdentifier = "Test";

            transformerSettings.IncludeFilterSettings.Names = new List<string> {"SomeName"};

            return transformerSettings;
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

        public AddTransitiveRelationsSettings AddTransitiveRelationsSettings
        {
            get { return _addTransitiveRelationsSettings; }
            set { _addTransitiveRelationsSettings = value; }
        }

        public MoveElementsSettings MoveElementsSettings
        {
            get { return _moveElementsSettings; }
            set { _moveElementsSettings = value; }
        }

        public MoveHeaderElementsSettings MoveHeaderElementsSettings
        {
            get { return _moveHeaderElementsSettings; }
            set { _moveHeaderElementsSettings = value; }
        }

        public SplitProductAndTestElementsSettings SplitProductAndTestElementsSettings
        {
            get { return _splitProductAndTestElementsSettings; }
            set { _splitProductAndTestElementsSettings = value; }
        }

        public IncludeFilterSettings IncludeFilterSettings
        {
            get { return _includeFilterSettings; }
            set { _includeFilterSettings = value; }
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

        public static void WriteToFile(string filename, TransformerSettings transformerSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(TransformerSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, transformerSettings);
            }
        }

        public static TransformerSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TransformerSettings));

            TransformerSettings transformerSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                transformerSettings = (TransformerSettings)serializer.Deserialize(reader);
            }

            transformerSettings.ResolvePaths(Path.GetDirectoryName(filename));
            return transformerSettings;
        }

        private void ResolvePaths(string settingFilePath)
        {
            InputFilename = FilePath.ResolveFile(settingFilePath, InputFilename);
            OutputFilename = FilePath.ResolveFile(settingFilePath, OutputFilename);
        }
    }
}
