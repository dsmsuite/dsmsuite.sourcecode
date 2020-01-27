using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Jdeps.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using System;

namespace DsmSuite.Analyzer.Jdeps
{
    public static class Program
    {
        private static AnalyzerSettings _analyzerSettings;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.Jdeps <settingsfile>");
            }
            else
            {
                FileInfo settingsFileInfo = new FileInfo(args[0]);
                if (!settingsFileInfo.Exists)
                {
                    AnalyzerSettings.WriteToFile(settingsFileInfo.FullName, AnalyzerSettings.CreateDefault());
                    Logger.LogUserMessage("Settings file does not exist. Default one created");
                }
                else
                {
                    _analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), _analyzerSettings.LoggingEnabled);

                    if (!File.Exists(_analyzerSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{_analyzerSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor executor = new ConsoleActionExecutor($"Analyzing grapviz dot file '{_analyzerSettings.InputFilename}'", settingsFileInfo.FullName);
                        executor.Execute(Analyze);
                        Logger.LogUserMessage($" Total elapsed time={executor.ElapsedTime}");
                    }
                }
            }
        }

        static void Analyze(IProgress<ProgressInfo> progress)
        {
            DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, _analyzerSettings, progress);
            analyzer.Analyze();
            model.Save(_analyzerSettings.OutputFilename, _analyzerSettings.CompressOutputFile, null);
            AnalyzerLogger.Flush();
        }
    }
}
