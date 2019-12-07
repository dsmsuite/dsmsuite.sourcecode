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
                    AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
                    analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path=@"D\:External", ResolveAs = "External"});
                    analyzerSettings.SolutionGroups[0].SolutionFilenames.Add("MySolution.sln");
                    AnalyzerSettings.WriteToFile(settingsFileInfo.FullName, analyzerSettings);
                    Logger.LogUserMessage("Settings file does not exist. Default one created");
                }
                else
                {
                    AnalyzerSettings analyzerSettings = AnalyzerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), analyzerSettings.LoggingEnabled);

                    bool allFound = true;
                    foreach (SolutionGroup solutionGroup in analyzerSettings.SolutionGroups)
                    {
                        foreach (string solutionFilename in solutionGroup.SolutionFilenames)
                        {
                            if (!File.Exists(solutionFilename))
                            {
                                Logger.LogUserMessage($"Input file '{solutionFilename}' does not exist.");
                                allFound = false;
                            }
                        }
                    }

                    if (allFound)
                    {
                        DsiDataModel model = new DsiDataModel("Analyzer", Assembly.GetExecutingAssembly());
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
