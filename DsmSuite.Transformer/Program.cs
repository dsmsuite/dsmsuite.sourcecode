using System.IO;
using System.Reflection;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer
{
    public class ConsoleAction : ConsoleActionBase
    {
        private readonly TransformerSettings _transformerSettings;

        public ConsoleAction(TransformerSettings transformerSettings) : base("Transforming model")
        {
            _transformerSettings = transformerSettings;
        }

        protected override bool CheckPrecondition()
        {
            bool result = true;
            if (!File.Exists(_transformerSettings.InputFilename))
            {
                result = false;
                Logger.LogUserMessage($"Input file '{_transformerSettings.InputFilename}' does not exist.");
            }
            return result;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage($"Input filename:{_transformerSettings.InputFilename}");
        }

        protected override void Action()
        {
            DsiModel model = new DsiModel("Transformer", Assembly.GetExecutingAssembly());
            model.Load(_transformerSettings.InputFilename, this);
            Transformation.Transformer transformer = new Transformation.Transformer(model, _transformerSettings, this);
            transformer.Transform();
            model.Save(_transformerSettings.OutputFilename, _transformerSettings.CompressOutputFile, this);

        }

        protected override void LogOutputParameters()
        {
            Logger.LogUserMessage($"Output file: {_transformerSettings.OutputFilename} compressed={_transformerSettings.CompressOutputFile}");
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);

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
                    Logger.LoggingEnabled = transformerSettings.LoggingEnabled;

                    ConsoleAction action = new ConsoleAction(transformerSettings);
                    action.Execute();
                }
            }
        }
    }
}
