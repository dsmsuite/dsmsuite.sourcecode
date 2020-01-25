using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.Settings
{
    [Serializable]
    public class ExternalIncludeDirectory
    {
        public string Path { get; set; }

        public string ResolveAs { get; set; }
    }

    [Serializable]
    public enum ViewMode
    {
        LogicalView,
        PhysicalView
    }

    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings
    {
        private bool _loggingEnabled;
        private string _inputFilename;
        private string _rootDirectory;
        private List<string> _systemIncludeDirectories;
        private List<string> _interfaceIncludeDirectories;
        private List<ExternalIncludeDirectory> _externalIncludeDirectories;
        private ViewMode _viewMode;
        private string _toolsVersion;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LoggingEnabled = true,
                InputFilename = @"C:\Example.sln",
                RootDirectory = "",
                SystemIncludeDirectories = new List<string>(),
                InterfaceIncludeDirectories = new List<string>(),
                ExternalIncludeDirectories = new List<ExternalIncludeDirectory>(),
                ViewMode = ViewMode.LogicalView,
                ToolsVersion = "14.0",
                OutputFilename = "Output.dsi",
                CompressOutputFile = true
            };

            analyzerSettings.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\8.1\Include\um");
            analyzerSettings.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\8.1\Include\shared");
            analyzerSettings.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\ucrt");
            analyzerSettings.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include");
            analyzerSettings.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\atlmfc\include");

            analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory
            {
                Path = @"C\:External",
                ResolveAs = "External"
            });

            return analyzerSettings;
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

        public string RootDirectory
        {
            get { return _rootDirectory; }
            set { _rootDirectory = value; }
        }

        public List<string> SystemIncludeDirectories
        {
            get { return _systemIncludeDirectories; }
            set { _systemIncludeDirectories = value; }
        }

        public List<string> InterfaceIncludeDirectories
        {
            get { return _interfaceIncludeDirectories; }
            set { _interfaceIncludeDirectories = value; }
        }

        public List<ExternalIncludeDirectory> ExternalIncludeDirectories
        {
            get { return _externalIncludeDirectories; }
            set { _externalIncludeDirectories = value; }
        }

        public ViewMode ViewMode
        {
            get { return _viewMode; }
            set { _viewMode = value; }
        }

        public string ToolsVersion
        {
            get { return _toolsVersion; }
            set { _toolsVersion = value; }
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
            RootDirectory = FilePath.ResolveFile(settingFilePath, RootDirectory);
            SystemIncludeDirectories = FilePath.ResolveFiles(settingFilePath, SystemIncludeDirectories);
            foreach (ExternalIncludeDirectory externalIncludeDirectory in ExternalIncludeDirectories)
            {
                externalIncludeDirectory.Path = FilePath.ResolveFile(settingFilePath, externalIncludeDirectory.Path);
            }
            InterfaceIncludeDirectories = FilePath.ResolveFiles(settingFilePath, InterfaceIncludeDirectories);
            OutputFilename = FilePath.ResolveFile(settingFilePath, OutputFilename);
        }
    }
}
