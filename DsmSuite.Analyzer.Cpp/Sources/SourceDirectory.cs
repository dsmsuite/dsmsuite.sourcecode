using System;
using System.Collections.Generic;
using System.IO;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Sources
{
    public class SourceDirectory
    {
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly IIncludeResolveStrategy _includeResolveStrategy;
        private readonly List<string> _includeDirectories = new List<string>();
        private readonly List<SourceFile> _sourceFiles = new List<SourceFile>();
        private int _analyzedSourceFiles;

        public SourceDirectory(AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _analyzerSettings = analyzerSettings;
            _progress = progress;

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
            FindIncludeDirectories();
            FindSourceFiles();
            AnalyzeFoundSourceFiles();
        }

        private void FindIncludeDirectories()
        {
            foreach (string sourceCodeDirectory in _analyzerSettings.SourceDirectories)
            {
                RecursiveFindIncludeDirectories(sourceCodeDirectory);
            }

            foreach (string includeDirectory in _analyzerSettings.ExternalIncludeDirectories)
            {
                RecursiveFindIncludeDirectories(includeDirectory);
            }
            UpdateProgress("Finding source directories", _includeDirectories.Count, "directories", true);
        }

        private void FindSourceFiles()
        {
            foreach (string sourceCodeDirectory in _analyzerSettings.SourceDirectories)
            {
                RecursiveFindSourceFiles(sourceCodeDirectory);
            }
            UpdateProgress("Finding source files", _sourceFiles.Count, "source files", false);
        }

        public ICollection<SourceFile> SourceFiles => _sourceFiles;

        private void RecursiveFindIncludeDirectories(string includeDirectory)
        {
            _includeDirectories.Add(includeDirectory);
            UpdateProgress("Finding source directories", _includeDirectories.Count, "directories", false);

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
            _analyzedSourceFiles = 0;
            foreach (SourceFile sourceFile in _sourceFiles)
            {
                sourceFile.Analyze(_includeResolveStrategy);
                _analyzedSourceFiles++;
                UpdateProgress("Analyzing source files", _analyzedSourceFiles, "files", false);
            }
            UpdateProgress("Analyzing source files", _analyzedSourceFiles, "files", true);
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

        private void UpdateProgress(string actionText, int itemCount, string itemType, bool done)
        {
            if (_progress != null)
            {
                ProgressInfo progressInfoInfo = new ProgressInfo
                {
                    ActionText = actionText,
                    TotalItemCount = 0,
                    CurrentItemCount = itemCount,
                    ItemType = itemType,
                    Percentage = null,
                    Done = done
                };

                _progress.Report(progressInfoInfo);
            }
        }
    }
}
