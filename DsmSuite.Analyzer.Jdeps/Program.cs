using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Jdeps.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using System;
using System.Diagnostics;

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
                        Logger.LogUserMessage($"Analyzing file={analyzerSettings.InputFilename}");

                        ConsoleProgressIndicator progressIndicator = new ConsoleProgressIndicator();
                        var progress = new Progress<ProgressInfo>(p =>
                        {
                            progressIndicator.UpdateProgress(p);
                        });

                        DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
                        Analysis.Analyzer analyzer = new Analysis.Analyzer(model, analyzerSettings, progress);
                        analyzer.Analyze();
                        model.Save(analyzerSettings.OutputFilename, analyzerSettings.CompressOutputFile, null);

                        progressIndicator.Done();

                        AnalyzerLogger.Flush();
                    }
                }
            }
        }
    }
}
