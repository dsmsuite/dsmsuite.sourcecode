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
        private static readonly Dictionary<string, HashSet<string>> DataModelRelationNotResolvedLogMessages;

        static AnalyzerLogger()
        {
            FilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            FilesFoundLogMessages = new Dictionary<string, HashSet<string>>();
            PathsNotResolvedLogMessages = new Dictionary<string, HashSet<string>>();
            IncludePathsNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            IncludeFilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            DataModelRelationNotResolvedLogMessages = new Dictionary<string, HashSet<string>>();
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
            if (!FilesNotFoundLogMessages.ContainsKey(key))
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

        public static void LogErrorIncludeFileAmbigious(string sourceFile,
                                                        string includeFile,
                                                        List<Tuple<string, bool>> candidates)
        {
            string logFile = "ambigiousIncludes.log";
            string message = "Include file ambiguous: " + includeFile + " in " + sourceFile;
            Logger.LogToFile(logFile, message);

            foreach (Tuple<string, bool> candidate in candidates)
            {
                string details = " resolved=" + candidate.Item2 + " file=" + candidate.Item1;
                Logger.LogToFile(logFile, details);
            }
        }

        public static void LogTransformation(string actionName, string description)
        {
            Logger.LogToFile("transformation.log", actionName + ": " + description);
        }

        public static void LogDataModelRelationNotResolved(string consumerName, string providerName)
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
            Flush(FilesNotFoundLogMessages, "Files not found", "filesNotFound", 0);
            Flush(FilesFoundLogMessages, "Duplicate files found", "duplicateFilesFound", 1);
            Flush(PathsNotResolvedLogMessages, "Relative paths not resolved", "pathsNotResolved", 0);
            Flush(IncludePathsNotFoundLogMessages, "Absolute paths not found", "includePathsNotFound", 0);
            Flush(IncludeFilesNotFoundLogMessages, "Includes files not found", "includeFilesNotFound", 0);
            Flush(DataModelRelationNotResolvedLogMessages, "Relations not resolved", "dataModelRelationsNotResolved", 0);
        }

        private static void Flush(Dictionary<string, HashSet<string>> messages, string title, string filename, int minCount)
        {
            string overviewFilename = filename + "Overview.txt";
            string detailsFilename = filename + "Details.txt";

            int totalOccurances = 0;

            List<string> keys = messages.Keys.ToList();
            keys.Sort();

            if (keys.Count > 0)
            {
                Logger.LogToFile(overviewFilename, title);
                Logger.LogToFile(detailsFilename, title);

                Logger.LogToFile(overviewFilename, "--------------------------------------------");
                Logger.LogToFile(detailsFilename, "---------------------------------------------");
            }
            
            foreach (string key in keys)
            {
                int occurances = messages[key].Count;

                if (occurances >= minCount)
                {
                    totalOccurances += occurances;
                    Logger.LogToFile(overviewFilename, $"{key} {occurances} occurances");
                    Logger.LogToFile(detailsFilename, $"{key} {occurances} occurances");
                    foreach (string message in messages[key])
                    {
                        Logger.LogToFile(detailsFilename, "  " + message);
                    }
                }
            }

            if (keys.Count > 0)
            {
                Logger.LogToFile(overviewFilename, $"{keys.Count} items found in {totalOccurances} occurances");
                Logger.LogToFile(detailsFilename, $"{keys.Count} items found in {totalOccurances} occurances");
            }
        }
    }
}
