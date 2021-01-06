using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;
using Microsoft.Build.Evaluation;
using System.IO;
using DsmSuite.Analyzer.DotNet.Lib;
using Microsoft.Build.Construction;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class CsProjectFile : ProjectFileBase
    {
        private readonly BinaryFile _assembly;
        private readonly Dictionary<string, string> _globalProperties = new Dictionary<string, string>();
        private string _toolsVersion;

        public CsProjectFile(string solutionFolder, string solutionDir, string solutionName, string projectPath, AnalyzerSettings analyzerSettings, DotNetResolver resolver) :
            base(solutionFolder, solutionDir, solutionName, projectPath, analyzerSettings, resolver)
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
                            BinaryFile assembly = new BinaryFile(assemblyFilename, null);
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

                Success = true;
            }

            CloseProject(project);
        }

        public override bool Success { get; protected set; }

        public override IEnumerable<DotNetType> DotNetTypes => (_assembly != null) ? _assembly.Types : new List<DotNetType>();
        public override IEnumerable<DotNetRelation> DotNetRelations => (_assembly != null) ? _assembly.Relations : new List<DotNetRelation>();

        protected override Project OpenProject()
        {
            Project project = null;
            try
            {
                DetermineGlobalProperties();
                project = new Project(ProjectFileInfo.FullName, _globalProperties, _toolsVersion);
            }
            catch (Exception e)
            {
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }
            return project;
        }

        private void DetermineGlobalProperties()
        {
            try
            {
                ProjectRootElement project = ProjectRootElement.Open(ProjectFileInfo.FullName);
                if (project != null)
                {
                    _toolsVersion = project.ToolsVersion;

                    foreach (ProjectItemElement item in project.Items)
                    {
                        if (item.ElementName == "ProjectConfiguration")
                        {
                            string[] projectConfiguration = item.Include.Split('|'); // eg. "Release|x64"
                            _globalProperties["Configuration"] = projectConfiguration[0];
                            _globalProperties["Platform"] = projectConfiguration[1];
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Open project failed project={ProjectFileInfo.FullName}", e);
            }
        }
    }
}
