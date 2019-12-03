namespace DsmSuite.DsmViewer.Model.Core
{
    public class HierarchicalName
    {
        public HierarchicalName()
        {
            FullName = "";
        }

        public HierarchicalName(string fullname)
        {
            FullName = fullname;
        }

        public HierarchicalName(string parentName, string name)
        {
            FullName = parentName;
            Add(name);
        }

        public void Add(string name)
        {
            if (FullName.Length > 0)
            {
                FullName += ".";
            }
            FullName += name;
        }

        public string FullName { get; private set; }

        public string[] Elements => FullName.Split('.');

        public int ElementCount => Elements.Length;

        public string Name => Elements[Elements.Length - 1];

        public string ParentName
        {
            get
            {
                string parentName = "";
                int beginIndex = 0;
                int endIndex = FullName.Length - Name.Length - 1;
                if (endIndex > 0)
                {
                    parentName = FullName.Substring(beginIndex, endIndex);
                }
                return parentName;
            }
        }
    }
}
