using System;
using System.Linq;
using DsmSuite.Analyzer.Cpp.IncludeResolve;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Analyzer.Cpp.Sources;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Analysis
{
    /// <summary>
    /// C++ code analyzer which uses #includes to analyze dependencies between source files.
    /// </summary>
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly SourceDirectory _sourceDirectory;
        private int _analyzedSourceFiles;
        private int _progressPercentage;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
            _sourceDirectory = new SourceDirectory(_analyzerSettings);
        }

        public void Analyze()
        {
            FindSourceFiles();
            AnalyzeSourceFiles();
            RegisterSourceFiles();
            RegisterDirectIncludeRelations();
            AnalyzerLogger.Flush();
        }

        private void FindSourceFiles()
        {
            _sourceDirectory.Analyze();
        }

        private void AnalyzeSourceFiles()
        {
            IIncludeResolveStrategy includeResolveStrategy = GetIncludeResolveStrategy();

            _analyzedSourceFiles = 0;
            foreach (SourceFile sourceFile in _sourceDirectory.SourceFiles)
            {
                sourceFile.Analyze(includeResolveStrategy);
                _analyzedSourceFiles++;
                UpdateProgress("Analyzing source files", _sourceDirectory.SourceFiles.Count, _analyzedSourceFiles, "files");
            }
        }

        private IIncludeResolveStrategy GetIncludeResolveStrategy()
        {
            IIncludeResolveStrategy includeResolveStrategy;
            switch (_analyzerSettings.ResolveMethod)
            {
                case ResolveMethod.AddBestMatch:
                    includeResolveStrategy = new BestMatchIncludeFileResolveStrategy(_sourceDirectory.IncludeDirectories);
                    break;
                case ResolveMethod.AddAll:
                    includeResolveStrategy = new AllIncludeFileResolveStrategy(_sourceDirectory.IncludeDirectories);
                    break;
                case ResolveMethod.Ignore:
                    includeResolveStrategy = new IgnoreIncludeFileResolveStrategy(_sourceDirectory.IncludeDirectories);
                    break;
                default:
                    includeResolveStrategy = new IgnoreIncludeFileResolveStrategy(_sourceDirectory.IncludeDirectories);
                    break;
            }
            return includeResolveStrategy;
        }

        private void RegisterSourceFiles()
        {
            foreach (SourceFile sourceFile in _sourceDirectory.SourceFiles)
            {
                RegisterSourceFile(sourceFile);
            }
        }

        private void RegisterDirectIncludeRelations()
        {
            foreach (SourceFile sourceFile in _sourceDirectory.SourceFiles)
            {
                RegisterDirectIncludeRelations(sourceFile);
            }
        }

        private void RegisterSourceFile(SourceFile sourceFile)
        {
            if (sourceFile.FileInfo.Exists)
            {
                string name = ExtractSourceFileUniqueName(sourceFile.Name);
                string type = sourceFile.Extension;
                _model.AddElement(name, type, sourceFile.FileInfo.FullName);
            }
            else
            {
                AnalyzerLogger.LogErrorFileNotFound(sourceFile.Name, "source directory");
            }
        }

        private void RegisterDirectIncludeRelations(SourceFile sourceFile)
        {
            string consumerName = ExtractSourceFileUniqueName(sourceFile.Name);

            foreach (string includedFile in sourceFile.Includes)
            {
                if (!IsExternalInclude(includedFile))
                {
                    RegisterRelation(includedFile, consumerName);
                }
            }

            foreach (string unresolvedIncludedFile in sourceFile.UnresolvedIncludes)
            {
                if (!IsExternalInclude(unresolvedIncludedFile))
                {
                    _model.SkipRelation(sourceFile.Name, unresolvedIncludedFile, "include", "not resolved");
                }
            }
        }

        private void RegisterRelation(string includedFile, string consumerName)
        {
            string providerName = ExtractSourceFileUniqueName(includedFile);
            _model.AddRelation(consumerName, providerName, "include", 1, "analyze relations");
        }

        private bool IsExternalInclude(string includedFile)
        {
            bool isSystemInclude = false;

            foreach (string externalIncludeDirectories in _analyzerSettings.ExternalIncludeDirectories)
            {
                if (includedFile.StartsWith(externalIncludeDirectories))
                {
                    isSystemInclude = true;
                }
            }
            return isSystemInclude;
        }

        private string ExtractSourceFileUniqueName(string filename)
        {
            int start = _analyzerSettings.RootDirectory.Length + 1;

            string relativePath = filename.Substring(start);
            string logicalName = relativePath.Replace("\\", ".");

            return logicalName;
        }

        private void UpdateProgress(string actionText, int totalItemCount, int itemCount, string itemType)
        {
            if (_progress != null)
            {
                int currentProgressPercentage = 0;
                if (itemCount > 0)
                {
                    currentProgressPercentage = itemCount*100/totalItemCount;
                }

                if (_progressPercentage != currentProgressPercentage)
                {
                    _progressPercentage = currentProgressPercentage;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ActionText = actionText,
                        TotalItemCount = _sourceDirectory.SourceFiles.Count,
                        CurrentItemCount = itemCount,
                        ItemType = itemType,
                        Percentage = currentProgressPercentage,
                        Done = currentProgressPercentage == 100
                    };

                    _progress.Report(progressInfoInfo);
                }
            }
        }
    }
}
