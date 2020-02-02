using System;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Analyzer.Cpp <settingsfile>");
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

                    Logger.LogUserMessage("Input directories:");
                    foreach (string sourceDirectory in analyzerSettings.SourceDirectories)
                    {
                        Logger.LogUserMessage($" {sourceDirectory}");
                    }
                    Logger.LogUserMessage($"Resolve method: {analyzerSettings.ResolveMethod}");
                    DsiModel model = new DsiModel("Analyzer", Assembly.GetExecutingAssembly());
                    Analysis.Analyzer analyzer = new Analysis.Analyzer(model, analyzerSettings);
                    analyzer.Analyze();
                    model.Save(analyzerSettings.OutputFilename, analyzerSettings.CompressOutputFile, null);
                    Logger.LogUserMessage($"Found elements={model.GetElementCount()} relations={model.GetRelationCount()} resolvedRelations={model.ResolvedRelationPercentage:0.0}% ambiguousRelations={model.AmbiguousRelationPercentage:0.0}%");
                    Logger.LogUserMessage($"Output file: {analyzerSettings.OutputFilename} compressed={analyzerSettings.CompressOutputFile}");
                }
            }
        }
    }
}
