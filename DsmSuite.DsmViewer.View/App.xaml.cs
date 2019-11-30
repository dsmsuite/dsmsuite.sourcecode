using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        private readonly string settingFile = "ViewerSettings.xml";

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.LogAssemblyInfo(Assembly.GetExecutingAssembly());

            FileInfo settingsFileInfo = new FileInfo(settingFile);
            if (settingsFileInfo.Exists)
            {
                ViewerSettings viewerSettings = ViewerSettings.ReadFromFile(settingsFileInfo.FullName);
                Logger.LoggingEnabled = viewerSettings.LoggingEnabled;
            }
            
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new LoggingTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            Logger.LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            Logger.LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");

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
