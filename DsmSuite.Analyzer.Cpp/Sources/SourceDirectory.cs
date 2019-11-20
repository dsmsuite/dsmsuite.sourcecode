using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.Analysis;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Cpp.Sources
{
    public class SourceDirectory
    {
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IIncludeResolveStrategy _includeResolveStrategy;
        private readonly List<string> _includeDirectories = new List<string>();
        private readonly List<SourceFile> _sourceFiles = new List<SourceFile>();

        public SourceDirectory(AnalyzerSettings analyzerSettings)
        {
            _analyzerSettings = analyzerSettings;

            switch (_analyzerSettings.ResolveMethod)
            {
                case ResolveMethod.AddBestMatch:
                    _includeResolveStrategy = new BestMatchIncludeFileResolveStrategy(_includeDirectories);
                    break;
                case ResolveMethod.AddAll:
                    _includeResolveStrategy = new AllIncludeFileResolveStrategy(_includeDirectories);
                    break;
                case ResolveMethod.Ignore:
                    _includeResolveStrategy = new IgnoreIncludeFileResolveStrategy(_includeDirectories);
                    break;
                default:
                    _includeResolveStrategy = new IgnoreIncludeFileResolveStrategy(_includeDirectories);
                    break;
            }
        }
        
        public void Analyze()
        {
            // Use all source directories as include directories
            foreach (string sourceCodeDirectory in _analyzerSettings.SourceDirectories)
            {
                RecursiveFindIncludeDirectories(sourceCodeDirectory);
            }

            // Find additional include directories
            foreach (string includeDirectory in _analyzerSettings.ExternalIncludeDirectories)
            {
                RecursiveFindIncludeDirectories(includeDirectory);
            }

            foreach (string sourceCodeDirectory in _analyzerSettings.SourceDirectories)
            {
                RecursiveFindSourceFiles(sourceCodeDirectory);
            }

            AnalyzeFoundSourceFiles();
        }

        public ICollection<SourceFile> SourceFiles => _sourceFiles;

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

        private void AnalyzeFoundSourceFiles()
        {
            foreach (SourceFile sourceFile in _sourceFiles)
            {
                sourceFile.Analyze(_includeResolveStrategy);
            }
        }
        
        private bool IgnoreDirectory(string directory)
        {
            bool ignore = false;

            foreach (string ignoredDirectory in _analyzerSettings.IgnorePaths)
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
