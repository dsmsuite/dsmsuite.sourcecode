using System.Collections.Generic;
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

        public DsmElement(int id, string name, string type, int order = 0 , bool isExpanded = false)
        {
            Id = id;
            Name = name;
            _typeId = TypeRegistration.AddTypeName(type);
            Order = order;
            IsExpanded = isExpanded;
        }

        /// <summary>
        /// Number uniquely identifying element.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Number identifying sequential order of the element in element tree.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Type of element.
        /// </summary>
        public string Type
        {
            get { return TypeRegistration.GetTypeName(_typeId); }
            set { _typeId = TypeRegistration.AddTypeName(value); }
        }

        /// <summary>
        /// Name of the element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full name of the element based on its position in the element hierarchy
        /// </summary>
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

        /// <summary>
        /// Is the element expanded in the viewer.
        /// </summary>
        public bool IsExpanded { get; set; }

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

        /// <summary>
        /// Get the previous element with the same parent.
        /// </summary>
        public IDsmElement PreviousSibling
        {
            get
            {
                IDsmElement previousSibling = null;
                if (_parent != null)
                {
                    int index = _parent._children.IndexOf(this);

                    if (index > 0)
                    {
                        previousSibling = _parent._children[index - 1];
                    }
                }
                return previousSibling;
            }
        }

        /// <summary>
        /// Get the next element with the same parent.
        /// </summary>
        public IDsmElement NextSibling
        {
            get
            {
                IDsmElement nextSibling = null;
                if (_parent != null)
                {
                    int index = _parent._children.IndexOf(this);

                    if (index < Parent.Children.Count - 1)
                    {
                        nextSibling = _parent._children[index + 1];
                    }
                }
                return nextSibling;
            }
        }

        /// <summary>
        /// Get the first child.
        /// </summary>
        public IDsmElement FirstChild => _children.Count > 0 ? _children[0] : null;

        /// <summary>
        /// Get the last child.
        /// </summary>
        public IDsmElement LastChild => _children.Count > 0 ? _children[_children.Count-1] : null;

        /// <summary>
        /// Children of the element.
        /// </summary>
        public IList<IDsmElement> Children => _children;

        /// <summary>
        /// Parent of the element.
        /// </summary>
        public IDsmElement Parent => _parent;

        /// <summary>
        /// Has the element any children.
        /// </summary>
        public bool HasChildren => Children.Count > 0;

        /// <summary>
        /// Add a child to the element.
        /// </summary>
        /// <param name="child">The child to be added</param>
        public void AddChild(IDsmElement child)
        {
            Children.Add(child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = this;
            }
        }

        /// <summary>
        /// Remove a child from the element.
        /// </summary>
        /// <param name="child">The child to be removed</param>
        public void RemoveChild(IDsmElement child)
        {
            Children.Remove(child);
            DsmElement c = child as DsmElement;
            if (c != null)
            {
                c._parent = null;
            }
        }

        public int CompareTo(object obj)
        {
            DsmElement element = obj as DsmElement;
            return Id.CompareTo(element?.Id);
        }
    }
}
