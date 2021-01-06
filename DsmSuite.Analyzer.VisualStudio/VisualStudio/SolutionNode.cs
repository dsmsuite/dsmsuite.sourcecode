using Microsoft.Build.Construction;

namespace DsmSuite.Analyzer.VisualStudio.VisualStudio
{
    public class SolutionNode
    {
        private SolutionNode _parent;

        public SolutionNode(ProjectInSolution project)
        {
            Guid = project.ProjectGuid;
            Name = project.ProjectName;
            AbsolutePath = project.AbsolutePath;
            ParentGuid = project.ParentProjectGuid;
            Type = project.ProjectType;
        }

        public string Guid { get; }
        public string Name { get; }
        public string AbsolutePath { get; }
        public string ParentGuid { get; }
        public SolutionProjectType Type { get; }

        public void AddChild(SolutionNode child)
        {
            if (child != null)
            {
                child._parent = this;
            }
        }

        public string SolutionFolder
        {
            get
            {
                string solutionFolder = "";
                SolutionNode parent = _parent;
                while (parent != null)
                {

                    if (parent.Name.Length > 0)
                    {
                        if (solutionFolder.Length > 0)
                        {
                            solutionFolder = parent.Name + "." + solutionFolder;
                        }
                        else
                        {
                            solutionFolder = parent.Name;
                        }
                    }

                    parent = parent._parent;
                }
                return solutionFolder;
            }
        }
    }
}  

