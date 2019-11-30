using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
        private static readonly Dictionary<string, HashSet<string>> PathsNotResolvedLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludePathsNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludeFilesNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> DataModelRelationNotResolvedLogMessages;

        static AnalyzerLogger()
        {
            FilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
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
            string logFile = GetLogfile("ambigiousIncludes.log");
            string message = "Include file ambiguous: " + includeFile + " in " + sourceFile;
            WriteLine(logFile, message);

            foreach (Tuple<string, bool> candidate in candidates)
            {
                string details = " resolved=" + candidate.Item2 + " file=" + candidate.Item1;
                WriteLine(logFile, details);
            }
        }

        public static void LogTransformation(string actionName, string description)
        {
            WriteLine(GetLogfile("transformation.log"), actionName + ": " + description);
        }

        public static void LogDataModelAction(string message,
                    [CallerFilePath] string file = "",
                    [CallerMemberName] string method = "",
                    [CallerLineNumber] int lineNumber = 0)
        {
            WriteLine(GetLogfile("dataModelActions.log"), FormatLine(file, method, lineNumber, "action", message));
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
            Flush(FilesNotFoundLogMessages, "Files not found", "filesNotFound");
            Flush(PathsNotResolvedLogMessages, "Relative paths not resolved", "pathsNotResolved");
            Flush(IncludePathsNotFoundLogMessages, "Absolute paths not found", "includePathsNotFound");
            Flush(IncludeFilesNotFoundLogMessages, "Includes files not found", "includeFilesNotFound");
            Flush(DataModelRelationNotResolvedLogMessages, "Relations not resolved", "dataModelRelationsNotResolved");
        }

        private static void Flush(Dictionary<string, HashSet<string>> messages, string title, string filename)
        {
            string overviewFilename = GetLogfile(filename + "Overview.txt");
            string detailsFilename = GetLogfile(filename + "Details.txt");

            int totalOccurances = 0;

            List<string> keys = messages.Keys.ToList();
            keys.Sort();

            if (keys.Count > 0)
            {
                WriteLine(overviewFilename, title);
                WriteLine(detailsFilename, title);

                WriteLine(overviewFilename, "--------------------------------------------");
                WriteLine(detailsFilename, "---------------------------------------------");
            }
            
            foreach (string key in keys)
            {
                int occurances = messages[key].Count;
                totalOccurances += occurances;
                WriteLine(overviewFilename, $"{key} {occurances} occurances");
                WriteLine(detailsFilename, $"{key} {occurances} occurances");
                foreach (string message in messages[key])
                {
                    WriteLine(detailsFilename, "  " + message);
                }
            }
            if (keys.Count > 0)
            {
                WriteLine(overviewFilename, $"{keys.Count} items found in {totalOccurances} occurances");
                WriteLine(detailsFilename, $"{keys.Count} items found in {totalOccurances} occurances");
            }
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

        private static void WriteLine(string filename, string line)
        {
            if (Logger.LoggingEnabled)
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
            return Path.GetFullPath(Path.Combine(Logger.LogDirectory.FullName, relativeLogfile));
        }
    }
}
