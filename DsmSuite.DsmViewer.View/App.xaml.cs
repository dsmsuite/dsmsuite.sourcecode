using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.View.Settings;

namespace DsmSuite.DsmViewer.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        public static Theme Skin { get; set; } = Theme.Dark;

        public string[] CommandLineArguments { get; protected set; }

        static App()
        {
            string applicationSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DsmSuite");
            if (!Directory.Exists(applicationSettingsFolder))
            {
                Directory.CreateDirectory(applicationSettingsFolder);
            }

            string settingsFilePath = Path.Combine(applicationSettingsFolder, "ViewerSettings.xml");
            FileInfo settingsFileInfo = new FileInfo(settingsFilePath);
            if (!settingsFileInfo.Exists)
            {
                ViewerSettings.WriteToFile(settingsFilePath, ViewerSettings.CreateDefault());
            }
            else
            {
                ViewerSettings viewerSettings = ViewerSettings.ReadFromFile(settingsFileInfo.FullName);
                Logger.EnableLogging(Assembly.GetExecutingAssembly(), viewerSettings.LoggingEnabled);
                Skin = viewerSettings.Theme;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.LogAssemblyInfo(Assembly.GetExecutingAssembly());

            CommandLineArguments = e.Args;


            
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new LoggingTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.LogResourceUsage();

            base.OnExit(e);
        }

        public class LoggingTraceListener : TraceListener
        {
            public override void Write(string message)
            {
            }

            public override void WriteLine(string message)
            {
                Logger.LogError(message);
            }
        }
    }
}
