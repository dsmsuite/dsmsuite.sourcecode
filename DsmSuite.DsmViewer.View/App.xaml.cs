using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Windows.Threading;

namespace DsmSuite.DsmViewer.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        public static Theme Skin { get; set; } = Theme.Dark;
        public static bool ShowCycles { get; set; }

        public string[] CommandLineArguments { get; protected set; }

        static App()
        {
            ViewerSetting.Read();
            Logger.EnableLogging(Assembly.GetExecutingAssembly(), ViewerSetting.LoggingEnabled);
            Skin = ViewerSetting.Theme;
            ShowCycles = ViewerSetting.ShowCycles;
        }

        public App()
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
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
            ViewerSetting.Write();
            base.OnExit(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogException("Unhandled exception", e.Exception);
            e.Handled = true;
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
