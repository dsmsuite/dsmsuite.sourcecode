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
                    AnalyzerSettings analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    if (analyzerSettings.LoggingEnabled)
                    {
                        Logger.EnableLogging(Assembly.GetExecutingAssembly());
                    }

                    if (!Directory.Exists(analyzerSettings.AssemblyDirectory))
                    {
                        Logger.LogUserMessage($"Input directory '{analyzerSettings.AssemblyDirectory}' does not exist.");
                    }
                    else
                    {
                        DataModel model = new DataModel("Analyzer", Assembly.GetExecutingAssembly());
                        Analysis.Analyzer analyzer = new Analysis.Analyzer(model, analyzerSettings);
                        analyzer.Analyze();
                        model.Save(analyzerSettings.OutputFilename, analyzerSettings.CompressOutputFile);
                    }
                }
            }

            AnalyzerLogger.Flush();
        }
    }
}
