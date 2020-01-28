using System.Diagnostics;
using System.IO;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Builder.Settings;
using DsmSuite.DsmViewer.Model.Core;
using System;

namespace DsmSuite.DsmViewer.Builder
{
    class Program
    {
        private static BuilderSettings _builderSettings;

        static void Main(string[] args)
        {
            Logger.LogAssemblyInfo(Assembly.GetExecutingAssembly());

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
                    _builderSettings = BuilderSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.EnableLogging(Assembly.GetExecutingAssembly(), _builderSettings.LoggingEnabled);

                    if (!File.Exists(_builderSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{_builderSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        ConsoleActionExecutor<BuilderSettings> executor = new ConsoleActionExecutor<BuilderSettings>($"Building dsm {_builderSettings.InputFilename}", _builderSettings);
                        executor.Execute(Build);
                    }
                }
            }
        }

        static void Build(BuilderSettings settings, IProgress<ProgressInfo> progress)
        {
            DsmModel model = new DsmModel("Builder", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            application.ImportModel(_builderSettings.InputFilename,
                                    _builderSettings.OutputFilename,
                                    _builderSettings.ApplyPartitioningAlgorithm,
                                    _builderSettings.RecordChanges,
                                    _builderSettings.CompressOutputFile,
                                    progress);
        }
    }
}
