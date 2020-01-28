using System;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Util;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.VisualStudio <settingsfile>");
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
                    AnalyzerSettings _analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), _analyzerSettings.LoggingEnabled);

                    if (!File.Exists(_analyzerSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{_analyzerSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor<AnalyzerSettings> executor = new ConsoleActionExecutor<AnalyzerSettings>($"Analyzing Visual Studio solution '{_analyzerSettings.InputFilename}'", _analyzerSettings);
                        executor.Execute(Analyze);
                    }
                }
            }
        }

        static void Analyze(AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, analyzerSettings);
            analyzer.Analyze(progress);
            model.Save(analyzerSettings.OutputFilename, analyzerSettings.CompressOutputFile, null);
            AnalyzerLogger.Flush();
        }
    }
}
