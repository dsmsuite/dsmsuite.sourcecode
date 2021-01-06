using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Utils
{
    /// <summary>
    /// Provides logging to be used for diagnostic purposes
    /// </summary>
    public static class AnalyzerLogger
    {
        private static readonly Dictionary<string, HashSet<string>> FilesNotFoundLogMessages;

        static AnalyzerLogger()
        {
            FilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
        }
       
        public static void LogErrorFileNotFound(string filename, string context)
        {
            string key = filename;
            string message = "In " + context;
            if (!FilesNotFoundLogMessages.ContainsKey(key))
            {
                FilesNotFoundLogMessages[key] = new HashSet<string>();
            }
            FilesNotFoundLogMessages[key].Add(message);
        }

        public static void LogErrorIncludeFileAmbiguous(string sourceFile,
                                                        string includeFile,
                                                        List<Tuple<string, bool>> candidates)
        {
            string logFile = "ambiguousIncludes.log";
            string message = "Include file ambiguous: " + includeFile + " in " + sourceFile;
            Logger.LogToFile(LogLevel.Error, logFile, message);

            foreach (Tuple<string, bool> candidate in candidates)
            {
                string details = " resolved=" + candidate.Item2 + " file=" + candidate.Item1;
                Logger.LogToFile(LogLevel.Error, logFile, details);
            }
        }

        public static void Flush()
        {
            Flush(LogLevel.Error, FilesNotFoundLogMessages, "Files not found", "filesNotFound", 0);
        }

        private static void Flush(LogLevel logLevel, Dictionary<string, HashSet<string>> messages, string title, string filename, int minCount)
        {
            string overviewFilename = filename + "Overview.txt";
            string detailsFilename = filename + "Details.txt";

            int totalOccurrances = 0;

            List<string> keys = messages.Keys.ToList();
            keys.Sort();

            if (keys.Count > 0)
            {
                Logger.LogToFile(logLevel, overviewFilename, title);
                Logger.LogToFile(logLevel, detailsFilename, title);

                Logger.LogToFile(logLevel, overviewFilename, "--------------------------------------------");
                Logger.LogToFile(logLevel, detailsFilename, "---------------------------------------------");
            }
            
            foreach (string key in keys)
            {
                int occurrances = messages[key].Count;

                if (occurrances > minCount)
                {
                    totalOccurrances += occurrances;
                    Logger.LogToFile(logLevel, overviewFilename, $"{key} {occurrances} occurrances");
                    Logger.LogToFile(logLevel, detailsFilename, $"{key} {occurrances} occurrances");
                    foreach (string message in messages[key])
                    {
                        Logger.LogToFile(logLevel, detailsFilename, "  " + message);
                    }
                }
            }

            if (keys.Count > 0)
            {
                Logger.LogToFile(logLevel, overviewFilename, $"{keys.Count} items found in {totalOccurrances} occurrances");
                Logger.LogToFile(logLevel, detailsFilename, $"{keys.Count} items found in {totalOccurrances} occurrances");
            }
        }
    }
}
