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
                    AnalyzerSettings analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), analyzerSettings.LoggingEnabled);

                    if (!File.Exists(analyzerSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{analyzerSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor<AnalyzerSettings> executor = new ConsoleActionExecutor<AnalyzerSettings>($"Analyzing grapviz dot file '{analyzerSettings.InputFilename}'", analyzerSettings);
                        executor.Execute(Analyze);
                        Logger.LogUserMessage($" Total elapsed time={executor.ElapsedTime}");
                    }
                }
            }
        }

        static void Analyze(AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            Logger.LogUserMessage($"Input filename:{analyzerSettings.InputFilename}");
            DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, analyzerSettings, progress);
            analyzer.Analyze();
            model.Save(analyzerSettings.OutputFilename, analyzerSettings.CompressOutputFile, null);
            Logger.LogUserMessage($"Found elements={model.GetElementCount()} relations={model.GetRelationCount()} resolvedRelations={model.ResolvedRelationPercentage:0.0}% ambiguousRelations={model.AmbiguousRelationPercentage:0.0}%");
            Logger.LogUserMessage($"Output file: {analyzerSettings.OutputFilename} compressed={analyzerSettings.CompressOutputFile}");

        }
    }
}
