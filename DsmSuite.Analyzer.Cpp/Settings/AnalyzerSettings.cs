using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Settings
{
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

    /// <summary>
    /// Settings used during code analysis. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class AnalyzerSettings
    {
        private bool _loggingEnabled;
        private string _rootDirectory;
        private List<string> _sourceDirectories;
        private List<string> _externalIncludeDirectories;
        private List<string> _ignorePaths;
        private ResolveMethod _resolveMethod;
        private string _outputFilename;
        private bool _compressOutputFile;

        public static AnalyzerSettings CreateDefault()
        {
            AnalyzerSettings analyzerSettings = new AnalyzerSettings
            {
                LoggingEnabled = true,
                RootDirectory = @"C:\",
                SourceDirectories = new List<string>(),
                ExternalIncludeDirectories = new List<string>(),
                IgnorePaths = new List<string>(),
                ResolveMethod = ResolveMethod.AddBestMatch,
                OutputFilename = "Output.dsi",
                CompressOutputFile = true
            };

            analyzerSettings.SourceDirectories.Add(@"C:\");
            analyzerSettings.ExternalIncludeDirectories.Add(@"C:\");
            analyzerSettings.IgnorePaths.Add(@"C:\");

            return analyzerSettings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        public string RootDirectory
        {
            get { return _rootDirectory; }
            set { _rootDirectory = value; }
        }

        public List<string> SourceDirectories
        {
            get { return _sourceDirectories; }
            set { _sourceDirectories = value; }
        }

        public List<string> ExternalIncludeDirectories
        {
            get { return _externalIncludeDirectories; }
            set { _externalIncludeDirectories = value; }
        }

        public List<string> IgnorePaths
        {
            get { return _ignorePaths; }
            set { _ignorePaths = value; }
        }

        public ResolveMethod ResolveMethod
        {
            get { return _resolveMethod; }
            set { _resolveMethod = value; }
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
            RootDirectory = FilePath.ResolveFile(settingFilePath, RootDirectory);
            SourceDirectories = FilePath.ResolveFiles(settingFilePath, SourceDirectories);
            ExternalIncludeDirectories = FilePath.ResolveFiles(settingFilePath, ExternalIncludeDirectories);
            IgnorePaths = FilePath.ResolveFiles(settingFilePath, IgnorePaths);
            OutputFilename = FilePath.ResolveFile(settingFilePath, OutputFilename);
        }
    }
}
