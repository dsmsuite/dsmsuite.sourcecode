using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;
using System.IO;
using DsmSuite.Analyzer.DotNet.Lib;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using Microsoft.Build.Framework;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class CsProjectFile : ProjectFileBase
    {
        public CsProjectFile(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings) :
            base(solutionFolder, solutionDir, projectPath, analyzerSettings)
        {
        }

        public override void Analyze()
        {
            Project project = OpenProject();

            if (project != null)
            {
                ICollection<ProjectItem> comReferences = project.GetItems("COMReference");

                foreach (ProjectItem comReference in comReferences)
                {
                    Console.WriteLine(comReference.EvaluatedInclude);
                }

                foreach (var property in project.AllEvaluatedProperties)
                {
                    if (property.Name == "TargetPath")
                    {
                        FileInfo fileInfo = new FileInfo(property.EvaluatedValue);
                        string assemblyFilename = fileInfo.FullName;

                        if (File.Exists(assemblyFilename))
                        {
                            AssemblyResolver resolver = new AssemblyResolver();
                            AssemblyFile assembly = new AssemblyFile(assemblyFilename, new List<string>(), null);
                            resolver.AddSearchPath(assembly);
                            assembly.FindTypes(resolver);
                            assembly.FindRelations();
                        }
                    }
                }
            }

            CloseProject(project);
        }

        private Project OpenProject()
        {
            Project project = null;
            try
            {
                Dictionary<string, string> globalProperties = new Dictionary<string, string>();
                project = new Project(ProjectFileInfo.FullName, globalProperties, AnalyzerSettings.ToolsVersion);
                foreach (var item in project.AllEvaluatedItems)
                {
                    if (item.ItemType == "Configuration")
                    {
                        string[] projectConfiguration = item.EvaluatedInclude.Split('|'); // eg. "Release|x64"
                        globalProperties["Configuration"] = projectConfiguration[0];
                        globalProperties["Platform"] = projectConfiguration[1];
                        break;
                    }
                }
                UpdateConfiguration(project);
            }
            catch (Exception e)
            {
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }
            return project;
        }

        private void UpdateConfiguration(Project project)
        {
            foreach (ProjectItem item in project.Items)
            {
                if (item.ItemType == "Configuration")
                {
                    string configuration = null;
                    string platform = null;
                    foreach (ProjectMetadata meta in item.Metadata)
                    {
                        if (meta.Name == "Configuration")
                        {
                            configuration = meta.UnevaluatedValue;
                        }

                        if (meta.Name == "Platform")
                        {
                            platform = meta.UnevaluatedValue;
                        }
                    }

                    if ((configuration != null) && (platform != null))
                    {
                        project.SetGlobalProperty("Configuration", configuration);
                        project.SetGlobalProperty("Platform", platform);
                    }

                }
            }
            project.ReevaluateIfNecessary();
        }

        private void CloseProject(Project project)
        {
            try
            {
                project.ProjectCollection.UnloadProject(project);
            }
            catch (Exception e)
            {
                Logger.LogException($"Exception while closing project={ProjectFileInfo.FullName}", e);
            }
        }
    }
}
