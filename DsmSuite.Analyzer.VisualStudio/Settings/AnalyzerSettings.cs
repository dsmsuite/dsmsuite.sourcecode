using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.Transformations.Settings;

namespace DsmSuite.Analyzer.VisualStudio.Settings
{
    [Serializable]
    public class InputSettings
    {
        public string Filename { get; set; }
        public string RootDirectory { get; set; }
        public List<string> SystemIncludeDirectories { get; set; }
        public List<ExternalIncludeDirectory> ExternalIncludeDirectories { get; set; }
        public List<string> InterfaceIncludeDirectories { get; set; }
    }

    [Serializable]
    public class AnalysisSettings
    {
        public string ToolsVersion { get; set; }
        public ViewMode ViewMode { get; set; }
    }

    [Serializable]
    public class TransformationSettings
    {
        public List<string> IgnoredNames { get; set; }
        public bool AddTransitiveIncludes { get; set; }
        public TransformationHeaderSourceFileMergeStrategy HeaderSourceFileMergeStrategy { get; set; }
        public List<TransformationMergeRule> MergeHeaderAndSourceFileDirectoryRules { get; set; }
    }

    [Serializable]
    public class OutputSettings
    {
        public string Filename { get; set; }
        public bool Compress { get; set; }
    }

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

    [Serializable]
    public enum TransformationHeaderSourceFileMergeStrategy
    {
        None,
        MoveHeaderFileToSourceFile,
        MergeHeaderAndSourceFileDirectory
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

            analyzerSettings.Input.Filename = @"C:\Example.sln";
            analyzerSettings.Input.RootDirectory = @"C:\";
            analyzerSettings.Input.SystemIncludeDirectories = new List<string>();
            analyzerSettings.Input.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\8.1\Include\um");
            analyzerSettings.Input.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\8.1\Include\shared");
            analyzerSettings.Input.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\ucrt");
            analyzerSettings.Input.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include");
            analyzerSettings.Input.SystemIncludeDirectories.Add(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\atlmfc\include");


            analyzerSettings.Input.InterfaceIncludeDirectories = new List<string>();
            analyzerSettings.Input.ExternalIncludeDirectories = new List<ExternalIncludeDirectory>();
            analyzerSettings.Input.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory
            {
                Path = @"C\:External",
                ResolveAs = "External"
            });

            analyzerSettings.Analysis.ToolsVersion = "14.0";
            analyzerSettings.Analysis.ViewMode = ViewMode.LogicalView;

            analyzerSettings.Transformation.IgnoredNames = new List<string>();
            analyzerSettings.Transformation.AddTransitiveIncludes = false;
            analyzerSettings.Transformation.HeaderSourceFileMergeStrategy = TransformationHeaderSourceFileMergeStrategy.None;
            analyzerSettings.Transformation.MergeHeaderAndSourceFileDirectoryRules = new List<TransformationMergeRule>
            {
                new TransformationMergeRule() {From = "Header Files.", To = "Source Files."}
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
            Input.Filename = FilePath.ResolveFile(settingFilePath, Input.Filename);
            Input.RootDirectory = FilePath.ResolveFile(settingFilePath, Input.RootDirectory);
            Input.SystemIncludeDirectories = FilePath.ResolveFiles(settingFilePath, Input.SystemIncludeDirectories);
            foreach (ExternalIncludeDirectory externalIncludeDirectory in Input.ExternalIncludeDirectories)
            {
                externalIncludeDirectory.Path = FilePath.ResolveFile(settingFilePath, externalIncludeDirectory.Path);
            }
            Input.InterfaceIncludeDirectories = FilePath.ResolveFiles(settingFilePath, Input.InterfaceIncludeDirectories);
            Output.Filename = FilePath.ResolveFile(settingFilePath, Output.Filename);
        }
    }
}
