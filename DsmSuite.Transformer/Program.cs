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
        private static TransformerSettings _transformerSettings;

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
                    _transformerSettings = TransformerSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), _transformerSettings.LoggingEnabled);

                    if (!File.Exists(_transformerSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{_transformerSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor executor = new ConsoleActionExecutor("Performing transformations", settingsFileInfo.FullName);
                        executor.Execute(Transform);
                    }
                }
            }
        }

        static void Transform(IProgress<ProgressInfo> progress)
        {
            DsiModel model = new DsiModel("Transformer", Assembly.GetExecutingAssembly());
            model.Load(_transformerSettings.InputFilename, null);
            Transformation.Transformer transformer = new Transformation.Transformer(model, _transformerSettings);
            transformer.Transform(progress);
            model.Save(_transformerSettings.OutputFilename, _transformerSettings.CompressOutputFile, null);
            AnalyzerLogger.Flush();
        }
    }
}
