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

        public static void LogErrorIncludeFileAmbigious(string sourceFile,
                                                        string includeFile,
                                                        List<Tuple<string, bool>> candidates)
        {
            string logFile = "ambigiousIncludes.log";
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
    }
}
