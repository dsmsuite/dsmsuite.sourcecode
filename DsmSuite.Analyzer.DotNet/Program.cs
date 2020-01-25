using System;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.DotNet.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.DotNet
{
    public static class Program
    {
        private static AnalyzerSettings _analyzerSettings;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.DotNet <settingsfile>");
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

                    if (!Directory.Exists(_analyzerSettings.AssemblyDirectory))
                    {
                        Logger.LogUserMessage($"Input directory '{_analyzerSettings.AssemblyDirectory}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor executor = new ConsoleActionExecutor($"Analyzing .Net binaries in '{_analyzerSettings.AssemblyDirectory}'");
                        executor.Execute(Analyze);
                    }
                }
            }
        }

        static void Analyze(IProgress<ProgressInfo> progress)
        {
            DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
            Analysis.Analyzer analyzer = new Analysis.Analyzer(model, _analyzerSettings);
            analyzer.Analyze(progress);
            model.Save(_analyzerSettings.OutputFilename, _analyzerSettings.CompressOutputFile, null);
            AnalyzerLogger.Flush();
        }
    }
}
