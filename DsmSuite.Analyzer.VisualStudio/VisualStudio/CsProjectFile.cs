using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;
using System.IO;
using DsmSuite.Analyzer.DotNet.Lib;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class CsProjectFile : ProjectFileBase
    {
        private BinaryFile _assembly;

        public CsProjectFile(string solutionFolder, string solutionDir, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver) :
            base(solutionFolder, solutionDir, projectPath, analyzerSettings, resolver)
        {
            Project project = OpenProject();

            if (project != null)
            {
                foreach (var property in project.AllEvaluatedProperties)
                {
                    if (property.Name == "TargetPath")
                    {
                        FileInfo fileInfo = new FileInfo(property.EvaluatedValue);
                        string assemblyFilename = fileInfo.FullName;

                        if (File.Exists(assemblyFilename))
                        {
                            BinaryFile assembly = new BinaryFile(assemblyFilename, new List<string>(), null);
                            if (assembly.IsAssembly)
                            {
                                _assembly = assembly;
                            }
                        }
                    }
                }
            }

            CloseProject(project);
        }

        public override BinaryFile BuildAssembly => _assembly;

        public override void Analyze()
        {
            Project project = OpenProject();

            if ((project != null) && (_assembly != null))
            {
                ICollection<ProjectItem> comReferences = project.GetItems("COMReference");
                foreach (ProjectItem comReference in comReferences)
                {
                    Console.WriteLine(comReference.EvaluatedInclude);
                }

                _assembly.FindTypes(Resolver);
                _assembly.FindRelations();
            }

            CloseProject(project);
        }

        public override IEnumerable<DotNetType> DotNetTypes => (_assembly != null) ? _assembly.Types : new List<DotNetType>();
        public override IEnumerable<DotNetRelation> DotNetRelations => (_assembly != null) ? _assembly.Relations : new List<DotNetRelation>();

        protected override Project OpenProject()
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
    }
}
