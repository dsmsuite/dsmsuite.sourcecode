using System.Collections.Generic;
using System.Linq;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;

namespace DsmSuite.Analyzer.VisualStudio.Utils
{
    /// <summary>
    /// Provides logging to be used for diagnostic purposes
    /// </summary>
    public static class AnalyzerLogger
    {
        private static readonly Dictionary<string, HashSet<string>> FilesNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> FilesFoundInVisualStudioProjectLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludeFilesNotFoundInVisualStudioProjectLogMessages;
        private static readonly Dictionary<string, HashSet<string>> PathsNotResolvedLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludePathsNotFoundLogMessages;
        private static readonly Dictionary<string, HashSet<string>> IncludeFilesNotFoundLogMessages;

        static AnalyzerLogger()
        {
            FilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            FilesFoundInVisualStudioProjectLogMessages = new Dictionary<string, HashSet<string>>();
            IncludeFilesNotFoundInVisualStudioProjectLogMessages = new Dictionary<string, HashSet<string>>();
            PathsNotResolvedLogMessages = new Dictionary<string, HashSet<string>>();
            IncludePathsNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
            IncludeFilesNotFoundLogMessages = new Dictionary<string, HashSet<string>>();
        }
       
        public static void LogErrorFileNotFound(string filename, string visualStudioProject)
        {
            string key = filename;
            string message = "In " + visualStudioProject;
            if (!FilesNotFoundLogMessages.ContainsKey(key))
            {
                FilesNotFoundLogMessages[key] = new HashSet<string>();
            }
            FilesNotFoundLogMessages[key].Add(message);
        }

        public static void LogFileFoundInVisualStudioProject(string filename, string visualStudioProject)
        {
            string key = filename;
            string message = "In " + visualStudioProject;
            if (!FilesFoundInVisualStudioProjectLogMessages.ContainsKey(key))
            {
                FilesFoundInVisualStudioProjectLogMessages[key] = new HashSet<string>();
            }
            FilesFoundInVisualStudioProjectLogMessages[key].Add(message);
        }

        public static void LogIncludeFileNotFoundInVisualStudioProject(string filename, string visualStudioProject)
        {
            string key = filename;
            string message = "In " + visualStudioProject;
            if (!IncludeFilesNotFoundInVisualStudioProjectLogMessages.ContainsKey(key))
            {
                IncludeFilesNotFoundInVisualStudioProjectLogMessages[key] = new HashSet<string>();
            }
            IncludeFilesNotFoundInVisualStudioProjectLogMessages[key].Add(message);
        }

        public static void LogErrorPathNotResolved(string relativePath, string visualStudioProject)
        {
            string key = relativePath;
            string message = "In " + visualStudioProject;
            if (!PathsNotResolvedLogMessages.ContainsKey(key))
            {
                PathsNotResolvedLogMessages[key] = new HashSet<string>();
            }
            PathsNotResolvedLogMessages[key].Add(message);
        }

        public static void LogErrorIncludePathNotFound(string includePath, string visualStudioProject)
        {
            string key = includePath;
            string message = "In " + visualStudioProject;
            if (!IncludePathsNotFoundLogMessages.ContainsKey(key))
            {
                IncludePathsNotFoundLogMessages[key] = new HashSet<string>();
            }
            IncludePathsNotFoundLogMessages[key].Add(message);
        }

        public static void LogErrorIncludeFileNotFound(string includeFile, string visualStudioProject)
        {
            string key = includeFile;
            string message = "In " + visualStudioProject;
            if (!IncludeFilesNotFoundLogMessages.ContainsKey(key))
            {
                IncludeFilesNotFoundLogMessages[key] = new HashSet<string>();
            }
            IncludeFilesNotFoundLogMessages[key].Add(message);
        }

        public static void LogProjectStatus(string projectName, string status)
        {
            Logger.LogToFile(LogLevel.Info, "foundVisualStudioProjects.log", $"{projectName} status={status}");
        }
        
        public static void LogProjectProperties(string filename, Project project)
        {
            Logger.LogToFile(LogLevel.All, filename, "Properties");
            Logger.LogToFile(LogLevel.All, filename, "--------------------------------------------------------------");
            Logger.LogToFile(LogLevel.All, filename, "");

            foreach (ProjectProperty projectProperty in project.AllEvaluatedProperties)
            {
                Logger.LogToFile(LogLevel.All, filename, $" property={projectProperty.Name}");
                Logger.LogToFile(LogLevel.All, filename, $"  unevaluatedValue={projectProperty.UnevaluatedValue}");
                Logger.LogToFile(LogLevel.All, filename, $"  evaluatedValue={projectProperty.EvaluatedValue}");
                Logger.LogToFile(LogLevel.All, filename, "");
            }

            Logger.LogToFile(LogLevel.All, filename, "Item Definitions");
            Logger.LogToFile(LogLevel.All, filename, "--------------------------------------------------------------");
            Logger.LogToFile(LogLevel.All, filename, "");

            foreach (ProjectMetadata projectMetadata in project.AllEvaluatedItemDefinitionMetadata)
            {
                Logger.LogToFile(LogLevel.All, filename, $" itemDefinition={projectMetadata.Name}");
                Logger.LogToFile(LogLevel.All, filename, $"  unevaluatedValue={projectMetadata.UnevaluatedValue}");
                Logger.LogToFile(LogLevel.All, filename, $"  evaluatedValue={projectMetadata.EvaluatedValue}");
                Logger.LogToFile(LogLevel.All, filename, "");
            }

            Logger.LogToFile(LogLevel.All, filename, "Items");
            Logger.LogToFile(LogLevel.All, filename, "--------------------------------------------------------------");
            Logger.LogToFile(LogLevel.All, filename, "");

            foreach (ProjectItem projectItem in project.AllEvaluatedItems)
            {
                Logger.LogToFile(LogLevel.All, filename, $" item={projectItem.ItemType}");
                Logger.LogToFile(LogLevel.All, filename, $"  unevaluatedValue={projectItem.UnevaluatedInclude}");
                Logger.LogToFile(LogLevel.All, filename, $"  evaluatedValue={projectItem.EvaluatedInclude}");
                Logger.LogToFile(LogLevel.All, filename, "");
            }
        }

        public static void Flush()
        {
            Flush(LogLevel.Error, FilesFoundInVisualStudioProjectLogMessages, "Files found in multiple visual studio projects", "filesFoundInMultipleVisualProjects", 1);
            Flush(LogLevel.Error, IncludeFilesNotFoundInVisualStudioProjectLogMessages, "Includes files not found in any visual studio projects", "includeFilesNotFoundInAnyVisualStudioProject", 0);
            Flush(LogLevel.Error, FilesNotFoundLogMessages, "Files not found", "filesNotFound", 0);
            Flush(LogLevel.Error, PathsNotResolvedLogMessages, "Relative paths not resolved", "pathsNotResolved", 0);
            Flush(LogLevel.Error, IncludePathsNotFoundLogMessages, "Absolute paths not found", "includePathsNotFound", 0);
            Flush(LogLevel.Error, IncludeFilesNotFoundLogMessages, "Includes files not found", "includeFilesNotFound", 0);
        }

        private static void Flush(LogLevel logLevel, Dictionary<string, HashSet<string>> messages, string title, string filename, int minCount)
        {
            string overviewFilename = filename + "Overview.txt";
            string detailsFilename = filename + "Details.txt";

            int totalOccurrences = 0;

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
                int occurrences = messages[key].Count;

                if (occurrences > minCount)
                {
                    totalOccurrences += occurrences;
                    Logger.LogToFile(logLevel, overviewFilename, $"{key} {occurrences} occurrences");
                    Logger.LogToFile(logLevel, detailsFilename, $"{key} {occurrences} occurrences");
                    foreach (string message in messages[key])
                    {
                        Logger.LogToFile(logLevel, detailsFilename, "  " + message);
                    }
                }
            }

            if (keys.Count > 0)
            {
                Logger.LogToFile(logLevel, overviewFilename, $"{keys.Count} items found in {totalOccurrences} occurrences");
                Logger.LogToFile(logLevel, detailsFilename, $"{keys.Count} items found in {totalOccurrences} occurrences");
            }
        }
    }
}
