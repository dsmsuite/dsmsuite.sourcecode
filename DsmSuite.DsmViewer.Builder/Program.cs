using System.IO;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Builder
{
    class Program
    {
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
                    BuilderSettings builderSettings = BuilderSettings.ReadFromFile(settingsFileInfo.FullName);
                    Logger.LoggingEnabled = builderSettings.LoggingEnabled;

                    if (!File.Exists(builderSettings.InputFilename))
                    {
                        Logger.LogUserMessage($"Input file '{builderSettings.InputFilename}' does not exist.");
                    }
                    else
                    {
                        DsmModel model = new DsmModel("Builder", Assembly.GetExecutingAssembly());
                        Builder builder = new Builder(model, builderSettings);
                        builder.BuildModel();
                        model.SaveModel(builderSettings.OutputFilename, builderSettings.CompressOutputFile, null);
                    }
                }
            }
        }
    }
}
