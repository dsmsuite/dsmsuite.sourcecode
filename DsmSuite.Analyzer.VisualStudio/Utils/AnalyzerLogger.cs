using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Util
{
    /// <summary>
    /// Provides logging to be used for diagnostic purposes
    /// </summary>
    public static class AnalyzerLogger
    {
        private static readonly Dictionary<string, HashSet<string>> FilesNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> FilesFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> PathsNotResolvedLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludePathsNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludeFilesNotFoundLogMessages;

        static AnalyzerLogger()
        {
            FilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            FilesFoundLogMessages = new Dictionary<string, HashSet<string>>();
            PathsNotResolvedLogMessages = new Dictionary<string, HashSet<string>>();
            IncludePathsNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            IncludeFilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
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

        public static void LogFileFound(string filename, string context)
        {
            string key = filename;
            string message = "In " + context;
            if (!FilesFoundLogMessages.ContainsKey(key))
            {
                FilesFoundLogMessages[key] = new HashSet<string>();
            }
            FilesFoundLogMessages[key].Add(message);
        }

        public static void LogErrorPathNotResolved(string relativePath, string context)
        {
            string key = relativePath;
            string message = "In " + context;
            if (!PathsNotResolvedLogMessages.ContainsKey(key))
            {
                PathsNotResolvedLogMessages[key] = new HashSet<string>();
            }
            PathsNotResolvedLogMessages[key].Add(message);
        }

        public static void LogErrorIncludePathNotFound(string includePath, string context)
        {
            string key = includePath;
            string message = "In " + context;
            if (!IncludePathsNotFoundLogMessages.ContainsKey(key))
            {
                IncludePathsNotFoundLogMessages[key] = new HashSet<string>();
            }
            IncludePathsNotFoundLogMessages[key].Add(message);
        }

        public static void LogErrorIncludeFileNotFound(string includeFile, string context)
        {
            string key = includeFile;
            string message = "In " + context;
            if (!IncludeFilesNotFoundLogMessages.ContainsKey(key))
            {
                IncludeFilesNotFoundLogMessages[key] = new HashSet<string>();
            }
            IncludeFilesNotFoundLogMessages[key].Add(message);
        }
        
        public static void Flush()
        {
            Flush(LogLevel.Error, FilesNotFoundLogMessages, "Files not found", "filesNotFound", 0);
            Flush(LogLevel.Info, FilesFoundLogMessages, "Duplicate files found", "duplicateFilesFound", 1);
            Flush(LogLevel.Error, PathsNotResolvedLogMessages, "Relative paths not resolved", "pathsNotResolved", 0);
            Flush(LogLevel.Error, IncludePathsNotFoundLogMessages, "Absolute paths not found", "includePathsNotFound", 0);
            Flush(LogLevel.Error, IncludeFilesNotFoundLogMessages, "Includes files not found", "includeFilesNotFound", 0);
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
