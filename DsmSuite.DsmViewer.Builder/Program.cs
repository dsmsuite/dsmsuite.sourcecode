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
            if (!File.Exists(_builderSettings.Input.Filename))
            {
                result = false;
                Logger.LogUserMessage($"Input file '{_builderSettings.Input.Filename}' does not exist.");
            }
            return result;
        }

        protected override void LogInputParameters()
        {
            Logger.LogUserMessage($"Input filename:{_builderSettings.Input.Filename}");
        }

        protected override void Action()
        {
            DsmModel model = new DsmModel("Builder", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            application.ImportDsiModel(_builderSettings.Input.Filename,
                                    _builderSettings.Output.Filename,
                                    _builderSettings.Transformation.ApplyPartitioningAlgorithm,
                                    _builderSettings.Output.Compress,
                                    this);
        }

        protected override void LogOutputParameters()
        {
            Logger.LogUserMessage($"Output file: {_builderSettings.Output.Filename} compressed={_builderSettings.Output.Compress}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init(Assembly.GetExecutingAssembly(), true);

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
                    Logger.LogLevel = builderSettings.LogLevel;

                    ConsoleAction action = new ConsoleAction(builderSettings);
                    action.Execute();
                }
            }

            Logger.Flush();
        }
    }
}
