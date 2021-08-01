namespace DsmSuite.Common.Util
{
    public class ElementName
    {
        public ElementName()
        {
            FullName = "";
        }

        public ElementName(string fullname)
        {
            FullName = fullname;
        }

        public ElementName(string parentName, string name)
        {
            FullName = parentName;
            AddNamePart(name);
        }

        public void AddNamePart(string name)
        {
            if (FullName.Length > 0)
            {
                FullName += ".";
            }
            FullName += name;
        }

        public string FullName { get; private set; }

        public string[] NameParts => FullName.Split('.');

        public int NamePartCount => NameParts.Length;

        public string LastNamePart => NameParts[NameParts.Length - 1];

        public string ParentName
        {
            get
            {
                string parentName = "";
                int beginIndex = 0;
                int endIndex = FullName.Length - LastNamePart.Length - 1;
                if (endIndex > 0)
                {
                    parentName = FullName.Substring(beginIndex, endIndex);
                }
                return parentName;
            }
        }
    }
}
