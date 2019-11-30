using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DsmSuite.Common.Util
{
    /// <summary>
    /// Provides logging to be used for diagnostic purposes
    /// </summary>
    public class Logger
    {
        public static DirectoryInfo LogDirectory { get; private set; }

        static Logger()
        {
            DateTime now = DateTime.Now;
            string timestamp = $"{now.Year:0000}-{now.Month:00}-{now.Day:00}-{now.Hour:00}-{now.Minute:00}-{now.Second:00}";
            LogDirectory = Directory.CreateDirectory(@"C:\Temp\DsmSuiteTracing" + @"\Log_" + timestamp + @"\");
        }

        public static void EnableLogging(Assembly assembly)
        {
            LoggingEnabled = true;
            DateTime now = DateTime.Now;
            string timestamp = $"{now.Year:0000}-{now.Month:00}-{now.Day:00}-{now.Hour:00}-{now.Minute:00}-{now.Second:00}";
            string assemblyName = assembly.GetName().Name;
            LogDirectory = Directory.CreateDirectory($@"C:\Temp\DsmSuiteLogging\{assemblyName}_{timestamp}\");
        }

        public static bool LoggingEnabled { get; private set; }

        public static void LogAssemblyInfo(Assembly assembly)
        {
            string name = assembly.GetName().Name;
            string version = assembly.GetName().Version.ToString();
            DateTime buildDate = new FileInfo(assembly.Location).LastWriteTime;
            LogUserMessage(name + " version=" + version + " build=" + buildDate);
        }

        public static void LogUserMessage(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.WriteLine(message);
            WriteLine(true, GetLogfile("userMessages.log"), FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogInfo(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            WriteLine(LoggingEnabled, GetLogfile("infoMessages.log"),
                FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogError(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            WriteLine(LoggingEnabled, GetLogfile("errorMessages.log"),
                FormatLine(sourceFile, method, lineNumber, "error", message));
        }

        public static void LogException(string message, Exception e,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            WriteLine(LoggingEnabled, GetLogfile("errorMessages.log"),
                FormatLine(sourceFile, method, lineNumber, "error", message));

            WriteLine(LoggingEnabled, GetLogfile("exceptions.log"),
                FormatLine(sourceFile, method, lineNumber, message, e.Message));
            WriteLine(LoggingEnabled, GetLogfile("exceptions.log"), e.StackTrace);
            WriteLine(LoggingEnabled, GetLogfile("exceptions.log"), "");
        }

        public static void LogResourceUsage()
        {
            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");
        }

        private static string FormatLine(string sourceFile, string method, int lineNumber, string catagory, string text)
        {
            return StripPath(sourceFile) + " " + method + "() line=" + lineNumber + " " + catagory + "=" + text;
        }

        private static string StripPath(string sourceFile)
        {
            char[] separators = { '\\' };
            string[] parts = sourceFile.Split(separators);
            return parts[parts.Length - 1];
        }

        private static void WriteLine(bool enabled, string filename, string line)
        {
            if (enabled)
            {
                FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                using (StreamWriter writetext = new StreamWriter(fs))
                {
                    writetext.WriteLine(line);
                }
            }
        }

        private static string GetLogfile(string relativeLogfile)
        {
            return Path.GetFullPath(Path.Combine(LogDirectory.FullName, relativeLogfile));
        }
    }
}
