using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmElementModel : IDsmElementModelFileCallback
    {
        private readonly Dictionary<int /*id*/, DsmElement> _elementsById;
        private readonly Dictionary<int /*id*/, DsmElement> _deletedElementsById;
        private int _lastElementId;
        private readonly DsmElement _root;

        public event EventHandler<IDsmElement> ElementUnregistered;
        public event EventHandler<IDsmElement> ElementReregistered;

        public DsmElementModel()
        {
            _elementsById = new Dictionary<int, DsmElement>();
            _deletedElementsById = new Dictionary<int, DsmElement>();
            _lastElementId = 0;
            _root = new DsmElement(0, "", "");
        }

        public void Clear()
        {
            _elementsById.Clear();
            _deletedElementsById.Clear();
            _root.Children.Clear();
            _lastElementId = 0;
        }

        public void ClearHistory()
        {
            _deletedElementsById.Clear();
        }

        public IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId, bool deleted)
        {
            Logger.LogDataModelMessage($"Import element id={id} name={name} type={type} order={order} expanded={expanded} parentId={parentId}");

            if (id > _lastElementId)
            {
                _lastElementId = id;
            }
            return AddElement(id, name, type, order, expanded, parentId, deleted);
        }

        public IDsmElement AddElement(string name, string type, int? parentId)
        {
            Logger.LogDataModelMessage($"Add element name={name} type={type} parentId={parentId}");

            string fullname = name;
            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    ElementName elementName = new ElementName(_elementsById[parentId.Value].Fullname);
                    elementName.AddNamePart(name);
                    fullname = elementName.FullName;
                }
            }

            IDsmElement element = FindElementByFullname(fullname);
            if (element == null)
            {
                _lastElementId++;
                element = AddElement(_lastElementId, name, type, 0, false, parentId, false);
            }

            return element;
        }

        public void EditElementName(IDsmElement element, string name)
        {
            DsmElement editedElement = element as DsmElement;
            if (editedElement != null)
            {
                editedElement.Name = name;
            }
        }

        public void EditElementType(IDsmElement element, string type)
        {
            DsmElement editedElement = element as DsmElement;
            if (editedElement != null)
            {
                editedElement.Type = type;
            }
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement parent)
        {
            Logger.LogDataModelMessage($"Change element parent name={element.Name} from {element.Parent.Fullname} to {parent.Fullname}");

            DsmElement currentParent = element.Parent as DsmElement;
            DsmElement newParent = parent as DsmElement;
            if ((currentParent != null) && (newParent != null))
            {
                currentParent.RemoveChild(element);
                ElementUnregistered?.Invoke(this, element);
                newParent.AddChild(element);
                ElementReregistered?.Invoke(this, element);
            }
        }

        public void RemoveElement(int elementId)
        {
            Logger.LogDataModelMessage($"Remove element id={elementId}");

            if (_elementsById.ContainsKey(elementId))
            {
                DsmElement element = _elementsById[elementId];
                UnregisterElement(element);

                CollapseIfNoChildrenLeft(element);
            }
        }

        public void UnremoveElement(int elementId)
        {
            Logger.LogDataModelMessage($"Restore element id={elementId}");

            if (_deletedElementsById.ContainsKey(elementId))
            {
                DsmElement element = _deletedElementsById[elementId];
                ReregisterElement(element);
            }
        }

        public IEnumerable<IDsmElement> GetElements()
        {
            return _elementsById.Values;
        }

        public int GetElementCount()
        {
            return _elementsById.Count;
        }

        public IEnumerable<IDsmElement> GetExportedRootElements()
        {
            return _root.Children;
        }

        public int GetExportedElementCount()
        {
            return _elementsById.Count + _deletedElementsById.Count;
        }

        public void AssignElementOrder()
        {
            Logger.LogDataModelMessage("AssignElementOrder");

            int order = 1;
            foreach (IDsmElement root in _root.Children)
            {
                DsmElement rootElement = root as DsmElement;
                if (rootElement != null)
                {
                    AssignElementOrder(rootElement, ref order);
                }
            }
        }

        public IDsmElement FindElementById(int elementId)
        {
            return _elementsById.ContainsKey(elementId) ? _elementsById[elementId] : null;
        }

        public IDsmElement FindElementByFullname(string fullname)
        {
            IEnumerable<IDsmElement> elementWithName = from element in _elementsById.Values
                                                       where element.Fullname == fullname
                                                       select element;

            return elementWithName.FirstOrDefault();
        }

        public IEnumerable<IDsmElement> SearchElements(string text)
        {
            return from element in _elementsById.Values
                   where element.Fullname.Contains(text) && !element.IsDeleted
                   select element;
        }

        public IDsmElement GetDeletedElementById(int id)
        {
            return _deletedElementsById.ContainsKey(id) ? _deletedElementsById[id] : null;
        }

        public void ReorderChildren(IDsmElement element, IElementSequence sequence)
        {
            DsmElement parent = element as DsmElement;
            if (parent != null)
            {
                List<IDsmElement> clonedChildren = new List<IDsmElement>(parent.Children);

                foreach (IDsmElement child in clonedChildren)
                {
                    parent.RemoveChild(child);
                }

                for (int i = 0; i < sequence.GetNumberOfElements(); i++)
                {
                    parent.AddChild(clonedChildren[sequence.GetIndex(i)]);
                }
            }
            AssignElementOrder();
        }

        public bool Swap(IDsmElement element1, IDsmElement element2)
        {
            bool swapped = false;

            if (element1?.Parent == element2?.Parent)
            {
                DsmElement parent = element1?.Parent as DsmElement;
                if (parent != null)
                {
                    swapped = parent.Swap(element1, element2);
                }
            }

            return swapped;
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            IDsmElement nextSibling = null;
            DsmElement parent = element?.Parent as DsmElement;
            if (parent != null)
            {
                int index = parent.Children.IndexOf(element);

                if (index < parent.Children.Count - 1)
                {
                    nextSibling = parent.Children[index + 1];
                }
            }
            return nextSibling;
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            IDsmElement previousSibling = null;
            DsmElement parent = element?.Parent as DsmElement;
            if (parent != null)
            {
                int index = parent.Children.IndexOf(element);

                if (index > 0)
                {
                    previousSibling = parent.Children[index - 1];
                }
            }
            return previousSibling;
        }

        private IDsmElement AddElement(int id, string name, string type, int order, bool expanded, int? parentId, bool deleted)
        {
            DsmElement element = new DsmElement(id, name, type) { Order = order, IsExpanded = expanded, IsDeleted = deleted };

            if (parentId.HasValue)
            {
                DsmElement parent = null;
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    parent = _elementsById[parentId.Value];
                }

                if (_deletedElementsById.ContainsKey(parentId.Value))
                {
                    parent = _deletedElementsById[parentId.Value];
                }

                if (parent != null)
                {
                    parent.AddChild(element);
                }
                else
                {
                    Logger.LogError($"Parent not found id={id}");
                }
            }
            else
            {
                _root.AddChild(element);
            }

            if (deleted)
            {
                UnregisterElement(element);
            }
            else
            {
                RegisterElement(element);
            }

            return element;
        }

        private void AssignElementOrder(DsmElement element, ref int order)
        {
            element.Order = order;
            order++;

            foreach (IDsmElement child in element.Children)
            {
                DsmElement childElement = child as DsmElement;
                if (childElement != null)
                {
                    AssignElementOrder(childElement, ref order);
                }
            }
        }

        private void CollapseIfNoChildrenLeft(IDsmElement element)
        {
            DsmElement parent = element.Parent as DsmElement;
            if (parent != null)
            {
                if (parent.Children.Count == 0)
                {
                    parent.IsExpanded = false;
                }
            }
        }

        private void RegisterElement(DsmElement element)
        {
            _elementsById[element.Id] = element;
        }

        private void UnregisterElement(DsmElement element)
        {
            element.IsDeleted = true;
            _deletedElementsById[element.Id] = element;
            _elementsById.Remove(element.Id);
            ElementUnregistered?.Invoke(this, element);

            foreach (IDsmElement child in element.ExportedChildren)
            {
                UnregisterElement(child as DsmElement);
            }
        }

        private void ReregisterElement(DsmElement element)
        {
            foreach (IDsmElement child in element.ExportedChildren)
            {
                ReregisterElement(child as DsmElement);
            }

            _elementsById[element.Id] = element;
            _deletedElementsById.Remove(element.Id);
            element.IsDeleted = false;

            ElementReregistered?.Invoke(this, element);
        }
    }
}
