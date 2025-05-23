﻿using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Sources
{
    public class SourceDirectory
    {
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly List<string> _includeDirectories = new List<string>();
        private readonly List<SourceFile> _sourceFiles = new List<SourceFile>();

        public SourceDirectory(AnalyzerSettings analyzerSettings)
        {
            _analyzerSettings = analyzerSettings;
        }

        public void Analyze()
        {
            FindIncludeDirectories();
            FindSourceFiles();
        }

        private void FindIncludeDirectories()
        {
            foreach (string sourceCodeDirectory in _analyzerSettings.Input.SourceDirectories)
            {
                RecursiveFindIncludeDirectories(sourceCodeDirectory);
            }

            foreach (string includeDirectory in _analyzerSettings.Input.ExternalIncludeDirectories)
            {
                RecursiveFindIncludeDirectories(includeDirectory);
            }
        }

        private void FindSourceFiles()
        {
            foreach (string sourceCodeDirectory in _analyzerSettings.Input.SourceDirectories)
            {
                RecursiveFindSourceFiles(sourceCodeDirectory);
            }
        }

        public IList<SourceFile> SourceFiles => _sourceFiles;

        public IList<string> IncludeDirectories => _includeDirectories;

        private void RecursiveFindIncludeDirectories(string includeDirectory)
        {
            _includeDirectories.Add(includeDirectory);

            foreach (string additionalIncludeSubDirectory in Directory.EnumerateDirectories(includeDirectory))
            {
                RecursiveFindIncludeDirectories(additionalIncludeSubDirectory);
            }
        }

        private void RecursiveFindSourceFiles(string sourceCodeDirectory)
        {
            if (!IgnoreDirectory(sourceCodeDirectory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(sourceCodeDirectory);
                FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    SourceFile sourceFile = new SourceFile(file);
                    if (sourceFile.IsSourceCodeFile)
                    {
                        Logger.LogInfo("Source code file found:" + file.FullName);
                        _sourceFiles.Add(sourceFile);
                    }
                }

                foreach (string sourceCodeSubDirectory in Directory.EnumerateDirectories(sourceCodeDirectory))
                {
                    RecursiveFindSourceFiles(sourceCodeSubDirectory);
                }
            }
        }

        private bool IgnoreDirectory(string directory)
        {
            bool ignore = false;

            foreach (string ignoredDirectory in _analyzerSettings.Input.IgnorePaths)
            {
                if (directory.StartsWith(ignoredDirectory))
                {
                    ignore = true;
                }
            }

            return ignore;
        }
    }
}
