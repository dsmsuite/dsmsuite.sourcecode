using System;
using System.IO;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Builder.Settings;
using DsmSuite.DsmViewer.Model.Core;

namespace DsmSuite.DsmViewer.Builder
{
    public class ConsoleAction : ConsoleActionBase
    {
        private readonly BuilderSettings _builderSettings;

        public ConsoleAction(BuilderSettings builderSettings) : base("Building DSM")
        {
            _builderSettings = builderSettings;
        }

        protected override bool CheckPrecondition()
        {
            bool result = true;
            if (!File.Exists(_builderSettings.InputFilename))
            {
                result = false;
                Logger.LogUserMessage($"Input file '{_builderSettings.InputFilename}' does not exist.");
            }
            return result;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage($"Input filename:{_builderSettings.InputFilename}");
        }

        protected override void Action()
        {
            DsmModel model = new DsmModel("Builder", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            application.ImportModel(_builderSettings.InputFilename,
                                    _builderSettings.OutputFilename,
                                    _builderSettings.ApplyPartitioningAlgorithm,
                                    _builderSettings.RecordChanges,
                                    _builderSettings.CompressOutputFile,
                                    this);
        }

        protected override void LogOutputParameters()
        {
            Logger.LogUserMessage($"Output file: {_builderSettings.OutputFilename} compressed={_builderSettings.CompressOutputFile}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly());

            if (args.Length < 1)
            {
                Logger.LogUserMessage("Usage: DsmSuite.DsmViewer.Builder <settingsfile>");
            }
            else
            {
                FileInfo settingsFileInfo = new FileInfo(args[0]);
                if (!settingsFileInfo.Exists)
                {
                    BuilderSettings.WriteToFile(settingsFileInfo.FullName, BuilderSettings.CreateDefault());
                    Logger.LogUserMessage("Settings file does not exist. Default one created");
                }
                else
                {
                    BuilderSettings builderSettings = BuilderSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.LoggingEnabled = builderSettings.LoggingEnabled;

                    ConsoleAction action = new ConsoleAction(builderSettings);
                    action.Execute();
                }
            }
        }
    }
}
