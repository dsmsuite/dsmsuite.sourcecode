using System.Diagnostics;
using DsmSuite.Analyzer.Cpp.Sources;
using DsmSuite.Analyzer.Data;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Cpp.Analysis
{
    /// <summary>
    /// C++ code analyzer which uses #includes to analyze dependencies between source files.
    /// </summary>
    public class Analyzer
    {
        private readonly IDataModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly SourceDirectory _sourceDirectory;

        public Analyzer(IDataModel model, AnalyzerSettings analyzerSettings)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _sourceDirectory = new SourceDirectory(_analyzerSettings);
        }

        public void Analyze()
        {
            Logger.LogUserMessage("Analyzing");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            AnalyzeSourceFiles();
            RegisterSourceFiles();
            RegisterDirectIncludeRelations();

            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            Logger.LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            Logger.LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");

            stopWatch.Stop();
            Logger.LogUserMessage($" total elapsed time={stopWatch.Elapsed}");
        }

        private void AnalyzeSourceFiles()
        {
            Logger.LogUserMessage("Analyzing source files");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _sourceDirectory.Analyze();

            stopWatch.Stop();
            Logger.LogUserMessage($"elapsed time={stopWatch.Elapsed}");
        }

        private void RegisterSourceFiles()
        {
            Logger.LogUserMessage("Register source files");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (SourceFile sourceFile in _sourceDirectory.SourceFiles)
            {
                RegisterSourceFile(sourceFile);
            }

            stopWatch.Stop();
            Logger.LogUserMessage($"elapsed time={stopWatch.Elapsed}");
        }

        private void RegisterDirectIncludeRelations()
        {
            Logger.LogUserMessage("Register direct includes relations");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (SourceFile sourceFile in _sourceDirectory.SourceFiles)
            {
                RegisterDirectIncludeRelations(sourceFile);
            }

            stopWatch.Stop();
            Logger.LogUserMessage($"elapsed time={stopWatch.Elapsed}");
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
                Logger.LogErrorFileNotFound(sourceFile.Name, "source directory");
            }
        }

        private void RegisterDirectIncludeRelations(SourceFile sourceFile)
        {
            string consumerName = ExtractSourceFileUniqueName(sourceFile.Name);

            foreach (string includedFile in sourceFile.Includes)
            {
                if (!IsExternalInclude(includedFile))
                {
                    string providerName = ExtractSourceFileUniqueName(includedFile);
                    _model.AddRelation(consumerName, providerName, "include", 1, "analyze relations");
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
    }
}
