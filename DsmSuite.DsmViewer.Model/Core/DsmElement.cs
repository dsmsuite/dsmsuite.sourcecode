using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Core
{
    /// <summary>
    /// Represent an element in the dsm hierarchy.
    /// </summary>
    public class DsmElement : IDsmElement
    {
        private char _typeId;
        private readonly List<IDsmElement> _children = new List<IDsmElement>();
        private DsmElement _parent;
        private static readonly NameRegistration ElementTypeNameRegistration = new NameRegistration();
        private static readonly NameRegistration ElementPropertyNameRegistration = new NameRegistration();

        public DsmElement(int id, string name, string type, IDictionary<string, string> properties, int order = 0, bool isExpanded = false)
        {
            Id = id;
            Name = name;
            _typeId = ElementTypeNameRegistration.RegisterName(type);
            Properties = (properties != null) ? properties : new Dictionary<string, string>();
            Order = order;
            IsExpanded = isExpanded;
            IsIncludedInTree = true;
            Dependencies = new DsmDependencies(this);

            foreach (string key in Properties.Keys)
            {
                ElementPropertyNameRegistration.RegisterName(key);
            }
        }

        public DsmDependencies Dependencies { get; }

        public int Id { get; }

        public int Order { get; set; }

        public string Type
        {
            get { return ElementTypeNameRegistration.GetRegisteredName(_typeId); }
            set { _typeId = ElementTypeNameRegistration.RegisterName(value); }
        }

        public string Name { get; set; }

        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredElementPropertyNames()
        {
            return ElementPropertyNameRegistration.GetRegisteredNames();
        }

        public bool IsDeleted { get; set; }

        public bool IsBookmarked { get; set; }

        public bool IsRoot => Parent == null;

        public string Fullname
        {
            get
            {
                string fullname = Name;
                IDsmElement parent = Parent;
                while (parent != null)
                {
                    if (parent.Name.Length > 0)
                    {
                        fullname = parent.Name + "." + fullname;
                    }
                    parent = parent.Parent;
                }
                return fullname;
            }
        }

        public string GetRelativeName(IDsmElement element)
        {
            string fullname = Name;
            IDsmElement parent = Parent;
            while ((parent != element) && (parent != null))
            {
                if (parent.Name.Length > 0)
                {
                    fullname = parent.Name + "." + fullname;
                }
                parent = parent.Parent;
            }
            return fullname;
        }

        public bool IsExpanded { get; set; }

        public bool IsMatch { get; set; }

        public bool IsIncludedInTree { get; set; }

        public IDsmElement Parent => _parent;

        public bool IsRecursiveChildOf(IDsmElement element)
        {
            bool isRecursiveChildOf = false;

            IDsmElement parent = Parent;
            while ((parent != null) && !isRecursiveChildOf)
            {
                if (parent == element)
                {
                    isRecursiveChildOf = true;
                }

                parent = parent.Parent;
            }
            return isRecursiveChildOf;
        }

        public IList<IDsmElement> Children => _children.Where(child => ((child.IsDeleted == false) && child.IsIncludedInTree)).ToList();

        public IList<IDsmElement> AllChildren => _children;

        public int IndexOfChild(IDsmElement child)
        {
            return _children.IndexOf(child);
        }

        public bool ContainsChildWithName(string name)
        {
            bool containsChildWithName = false;
            foreach (IDsmElement child in Children)
            {
                if (child.Name == name)
                {
                    containsChildWithName = true;
                }
            }

            return containsChildWithName;
        }

        public bool HasChildren => Children.Count > 0;

        public void InsertChildAtEnd(IDsmElement child)
        {
            _children.Add(child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = this;
            }
        }

        public void InsertChildAtIndex(IDsmElement child, int index)
        {
            int rangeLimitedIndex = index;
            rangeLimitedIndex = Math.Min(rangeLimitedIndex, _children.Count);
            rangeLimitedIndex = Math.Max(rangeLimitedIndex, 0);
            _children.Insert(rangeLimitedIndex, child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = this;
            }
        }

        public void RemoveChild(IDsmElement child)
        {
            _children.Remove(child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = null;
            }
        }

        public void RemoveAllChildren()
        {
            _children.Clear();
        }

        public IDictionary<int, DsmElement> GetElementAndItsChildren()
        {
            Dictionary<int, DsmElement> elements = new Dictionary<int, DsmElement>();
            GetElementAndItsChildren(this, elements);
            return elements;
        }

        private void GetElementAndItsChildren(IDsmElement element, Dictionary<int, DsmElement> elements)
        {
            if (!element.IsDeleted)
            {
                elements[element.Id] = element as DsmElement;
            }

            foreach (IDsmElement child in element.Children)
            {
                GetElementAndItsChildren(child, elements);
            }
        }

        public bool Swap(IDsmElement element1, IDsmElement element2)
        {
            bool swapped = false;

            if (_children.Contains(element1) && _children.Contains(element2))
            {
                int index1 = _children.IndexOf(element1);
                int index2 = _children.IndexOf(element2);

                _children[index2] = element1;
                _children[index1] = element2;

                swapped = true;
            }

            return swapped;
        }

        public int CompareTo(object obj)
        {
            DsmElement element = obj as DsmElement;
            return Id.CompareTo(element?.Id);
        }

        public static IEnumerable<string> GetTypeNames()
        {
            return ElementTypeNameRegistration.GetRegisteredNames();
        }
    }
}
