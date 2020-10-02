using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private static readonly TypeRegistration TypeRegistration = new TypeRegistration();

        public DsmElement(int id, string name, string type, int order = 0, bool isExpanded = false)
        {
            Id = id;
            Name = name;
            _typeId = TypeRegistration.AddTypeName(type);
            Order = order;
            IsExpanded = isExpanded;
            IsIncludedInTree = true;
            Dependencies = new DsmDependencies(this);
        }

        public DsmDependencies Dependencies { get; }

        public int Id { get; }

        public int Order { get; set; }

        public string Type
        {
            get { return TypeRegistration.GetTypeName(_typeId); }
            set { _typeId = TypeRegistration.AddTypeName(value); }
        }

        public string Name { get; set; }

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

        public IList<IDsmElement> Children => _children.Where(child => ((child.IsDeleted == false) && (child.IsIncludedInTree == true))).ToList();

        public int ChildCount => Children.Count;

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

        public void AddChild(IDsmElement child)
        {
            _children.Add(child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = this;
            }
        }

        public void InsertChild(int index, IDsmElement child)
        {
            int newIndex = Math.Min(index, _children.Count);
            _children.Insert(newIndex, child);
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
    }
}
