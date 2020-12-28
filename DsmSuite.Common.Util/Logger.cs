using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DsmSuite.Common.Util
{
    /// <summary>
    /// Provides logging to be used for diagnostic purposes
    /// </summary>
    public class Logger
    {
        private static Assembly _assembly;
        private static string _logPath;
        private static readonly Dictionary<string, HashSet<string>> DataModelRelationNotResolvedLogMessages;

        public static DirectoryInfo LogDirectory { get; private set; }

        static Logger()
        {
            DataModelRelationNotResolvedLogMessages = new Dictionary<string, HashSet<string>>();
        }

        public static void Init(Assembly assembly, bool logInCurrentDirectory)
        {
            _assembly = assembly;
            if (logInCurrentDirectory)
            {
                _logPath = Directory.GetCurrentDirectory();
            }
            else
            {
                _logPath = @"C:\Temp\DsmSuiteLogging\";
            }

            LogLevel = LogLevel.None;
        }

        public static LogLevel LogLevel { get; set; }



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

        public static void LogAssemblyInfo()
        {
            string name = _assembly.GetName().Name;
            string version = _assembly.GetName().Version.ToString();
            DateTime buildDate = new FileInfo(_assembly.Location).LastWriteTime;
            LogUserMessage(name + " version =" + version + " build=" + buildDate);
        }

        public static void LogUserMessage(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.WriteLine(message);
            LogToFile(LogLevel.User, "userMessages.log", message);
        }

        public static void LogError(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile(LogLevel.Error, "errorMessages.log", FormatLine(sourceFile, method, lineNumber, "error", message));
        }

        public static void LogException(string message, Exception e,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile(LogLevel.Error, "exceptions.log", FormatLine(sourceFile, method, lineNumber, message, e.Message));
            LogToFile(LogLevel.Error, "exceptions.log", e.StackTrace);
            LogToFile(LogLevel.Error, "exceptions.log", "");
        }

        public static void LogWarning(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile(LogLevel.Warning, "warningMessages.log", FormatLine(sourceFile, method, lineNumber, "error", message));
        }

        public static void LogInfo(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile(LogLevel.Info, "infoMessages.log", FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogDataModelMessage(string message,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string method = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogToFile(LogLevel.Data, "dataModelMessages.log", FormatLine(sourceFile, method, lineNumber, "info", message));
        }

        public static void LogErrorDataModelRelationNotResolved(string consumerName, string providerName)
        {
            string key = providerName;
            string message = " From " + consumerName;
            if (!DataModelRelationNotResolvedLogMessages.ContainsKey(key))
            {
                DataModelRelationNotResolvedLogMessages[key] = new HashSet<string>();
            }
            DataModelRelationNotResolvedLogMessages[key].Add(message);
        }

        public static void Flush()
        {
            Flush(LogLevel.Data, DataModelRelationNotResolvedLogMessages, "Relations not resolved", "dataModelRelationsNotResolved", 0);
        }

        public static void LogToFile(LogLevel level, string logFilename, string line)
        {
            if (LogLevel >= level)
            {
                string path = GetLogFullPath(logFilename);
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter writetext = new StreamWriter(fs))
                {
                    writetext.WriteLine(line);
                }
            }
        }

        private static void Flush(LogLevel loglevel, Dictionary<string, HashSet<string>> messages, string title, string filename, int minCount)
        {
            string overviewFilename = filename + "Overview.txt";
            string detailsFilename = filename + "Details.txt";

            int totalOccurances = 0;

            List<string> keys = messages.Keys.ToList();
            keys.Sort();

            if (keys.Count > 0)
            {
                Logger.LogToFile(loglevel, overviewFilename, title);
                Logger.LogToFile(loglevel, detailsFilename, title);

                Logger.LogToFile(loglevel, overviewFilename, "--------------------------------------------");
                Logger.LogToFile(loglevel, detailsFilename, "---------------------------------------------");
            }

            foreach (string key in keys)
            {
                int occurances = messages[key].Count;

                if (occurances > minCount)
                {
                    totalOccurances += occurances;
                    Logger.LogToFile(loglevel, overviewFilename, $"{key} {occurances} occurances");
                    Logger.LogToFile(loglevel, detailsFilename, $"{key} {occurances} occurances");
                    foreach (string message in messages[key])
                    {
                        Logger.LogToFile(loglevel, detailsFilename, "  " + message);
                    }
                }
            }

            if (keys.Count > 0)
            {
                Logger.LogToFile(loglevel, overviewFilename, $"{keys.Count} items found in {totalOccurances} occurances");
                Logger.LogToFile(loglevel, detailsFilename, $"{keys.Count} items found in {totalOccurances} occurances");
            }
        }

        private static DirectoryInfo CreateLogDirectory()
        {
            DateTime now = DateTime.Now;
            string timestamp = $"{now.Year:0000}-{now.Month:00}-{now.Day:00}-{now.Hour:00}-{now.Minute:00}-{now.Second:00}";
            string assemblyName = _assembly.GetName().Name;
            return Directory.CreateDirectory($@"{_logPath}\{assemblyName}_{timestamp}\");
        }

        private static string GetLogFullPath(string logFilename)
        {
            if (LogDirectory == null)
            {
                LogDirectory = CreateLogDirectory();
            }

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
