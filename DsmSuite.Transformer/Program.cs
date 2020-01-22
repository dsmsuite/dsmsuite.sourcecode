using System;
using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.Transformer <settingsfile>");
            }
            else
            {
                FileInfo settingsFileInfo = new FileInfo(args[0]);
                if (!settingsFileInfo.Exists)
                {
                    TransformerSettings.WriteToFile(settingsFileInfo.FullName, TransformerSettings.CreateDefault());
                    Logger.LogUserMessage("Settings file does not exist. Default one created");
                }
                else
                {
                    TransformerSettings transformerSettings = TransformerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), transformerSettings.LoggingEnabled);

                    if (!File.Exists(transformerSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{transformerSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleProgressIndicator progressIndicator = new ConsoleProgressIndicator();
                        var progress = new Progress<ProgressInfo>(p =>
                        {
                            progressIndicator.Update(p);
                        });

                        DsiModel model = new DsiModel("Transformer", Assembly.GetExecutingAssembly());
                        model.Load(transformerSettings.InputFilename, null);
                        Transformation.Transformer transformer = new Transformation.Transformer(model, transformerSettings);
                        transformer.Transform(progress);
                        model.Save(transformerSettings.OutputFilename, transformerSettings.CompressOutputFile, null);
                    }
                }
            }

            AnalyzerLogger.Flush();
        }
    }
}
