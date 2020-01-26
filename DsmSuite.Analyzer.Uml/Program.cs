using System;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Uml.Settings;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Uml
{
    public static class Program
    {
        private static AnalyzerSettings _analyzerSettings;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.Uml <settingsfile>");
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
                        ConsoleActionExecutor executor = new ConsoleActionExecutor($"Analyzing UML model '{_analyzerSettings.InputFilename}'");
                        executor.Execute(Analyze);
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
            Logger.LogUserMessage($" Found elements={model.GetElementCount()} relations={model.GetRelationCount()} confidence={model.ResolvedRelationPercentage}");
        }
    }
}
