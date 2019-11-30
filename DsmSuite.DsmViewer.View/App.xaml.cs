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
        private readonly string settingFile = "ViewerSettings.xml";

        public string[] CommandLineArguments { get; protected set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.LogAssemblyInfo(Assembly.GetExecutingAssembly());

            CommandLineArguments = e.Args;

            FileInfo settingsFileInfo = new FileInfo(settingFile);
            if (settingsFileInfo.Exists)
            {
                ViewerSettings viewerSettings = ViewerSettings.ReadFromFile(settingsFileInfo.FullName);
                if (viewerSettings.LoggingEnabled)
                {
                    Logger.EnableLogging(Assembly.GetExecutingAssembly());
                }
            }
            
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
