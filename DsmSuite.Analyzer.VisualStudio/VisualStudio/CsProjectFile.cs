using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.Analyzer.DotNet.Analysis;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;

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
            //Project project = OpenProject();

            //if (project != null)
            //{
            //    foreach (var property in project.AllEvaluatedProperties)
            //    {
            //        if (property.Name == "TargetPath")
            //        {
            //            FileInfo fileInfo = new FileInfo(property.EvaluatedValue);
            //            string assemblyFilename = fileInfo.FullName;
            //        }
            //    }
            //}
        }

        private Project OpenProject()
        {
            Project project = null;
            try
            {
                Dictionary<string, string> globalProperties = new Dictionary<string, string>();
                project = new Project(ProjectFileInfo.FullName, globalProperties, AnalyzerSettings.ToolsVersion);
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
                if (item.ItemType == "ProjectConfiguration")
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
    }
}
