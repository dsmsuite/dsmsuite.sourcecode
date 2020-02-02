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

        public static void EnableLogging(Assembly assembly, bool enabled)
        {
            LoggingEnabled = enabled;
            LogDirectory = CreateLogDirectory(assembly);
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
            LogToFile("userMessages.log", FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogDataModelMessage(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile("dataModelMessages.log", FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogInfo(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile("infoMessages.log", FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogError(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile("errorMessages.log", FormatLine(sourceFile, method, lineNumber, "error", message));
        }

        public static void LogException(string message, Exception e,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile("errorMessages.log", FormatLine(sourceFile, method, lineNumber, "error", message));

            LogToFileAlways("exceptions.log", FormatLine(sourceFile, method, lineNumber, message, e.Message));
            LogToFileAlways("exceptions.log", e.StackTrace);
            LogToFileAlways("exceptions.log", "");
        }

        public static void LogResourceUsage()
        {
            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            LogUserMessage($"Peak physical memory usage {peakWorkingSetMb:0.000}MB");
            LogUserMessage($"Peak paged memory usage    {peakPagedMemMb:0.000}MB");
            LogUserMessage($"Peak virtual memory usage  {peakVirtualMemMb:0.000}MB");
        }

        public static void LogToFileAlways(string logFilename, string line)
        {
            string path = GetLogFullPath(logFilename);
            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            using (StreamWriter writetext = new StreamWriter(fs))
            {
                writetext.WriteLine(line);
            }
        }

        public static void LogToFile(string logFilename, string line)
        {
            if (LoggingEnabled)
            {
                string path = GetLogFullPath(logFilename);
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter writetext = new StreamWriter(fs))
                {
                    writetext.WriteLine(line);
                }
            }
        }

        private static DirectoryInfo CreateLogDirectory(Assembly assembly)
        {
            DateTime now = DateTime.Now;
            string timestamp =
                $"{now.Year:0000}-{now.Month:00}-{now.Day:00}-{now.Hour:00}-{now.Minute:00}-{now.Second:00}";
            string assemblyName = assembly.GetName().Name;
            return Directory.CreateDirectory($@"C:\Temp\DsmSuiteLogging\{assemblyName}_{timestamp}\");
        }



        private static string GetLogFullPath(string logFilename)
        {
            return Path.GetFullPath(Path.Combine(LogDirectory.FullName, logFilename));
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
    }
}
