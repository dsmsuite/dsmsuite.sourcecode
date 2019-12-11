using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmElementsDataModel
    {
        private readonly Dictionary<int /*id*/, IDsmElement> _elementsById;
        private readonly Dictionary<int /*id*/, IDsmElement> _deletedElementsById;
        private readonly IList<IDsmElement> _rootElements;
        private int _lastElementId;

        public DsmElementsDataModel()
        {
            _elementsById = new Dictionary<int, IDsmElement>();
            _deletedElementsById = new Dictionary<int, IDsmElement>();
            _rootElements = new List<IDsmElement>();
            _lastElementId = 0;
        }

        public void Clear()
        {
            _elementsById.Clear();
            _rootElements.Clear();
            _lastElementId = 0;
        }

        public IEnumerable<IDsmElement> RootElements => _rootElements;

        public IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            Logger.LogDataModelMessage($"Import element id={id} name={name} type={type} order={order} expanded={expanded} parentId={parentId}");

            if (id > _lastElementId)
            {
                _lastElementId = id;
            }
            return AddElement(id, name, type, order, expanded, parentId);
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

            IDsmElement element = GetElementByFullname(fullname);
            if (element == null)
            {
                _lastElementId++;
                element = AddElement(_lastElementId, name, type, 0, false, parentId);
            }

            return element;
        }

        public void EditElement(IDsmElement element, string name, string type)
        {
            DsmElement editedElement = element as DsmElement;
            if (editedElement != null)
            {
                editedElement.Name = name;
                editedElement.Type = type;
            }
        }

        public void ChangeParent(IDsmElement element, IDsmElement parent)
        {
            Logger.LogDataModelMessage($"Change element parent name={element.Name} from {element.Parent.Fullname} to {parent.Fullname}");

            DsmElement currentParent = element.Parent as DsmElement;
            DsmElement newParent = parent as DsmElement;
            if ((currentParent != null) && (newParent != null))
            {
                currentParent.RemoveChild(element);
                newParent.AddChild(element);
            }
        }

        public void RemoveElement(int id)
        {
            Logger.LogDataModelMessage($"Remove element id={id}");

            if (_elementsById.ContainsKey(id))
            {
                IDsmElement element = _elementsById[id];
                DsmElement parent = element.Parent as DsmElement;
                if (parent != null)
                {
                    parent.RemoveChild(element);
                    if (!parent.HasChildren)
                    {
                        parent.IsExpanded = false;
                    }
                }

                UnregisterElement(element);
            }
        }

        public void UnremoveElement(int id)
        {
            Logger.LogDataModelMessage($"Restore element id={id}");
            if (_deletedElementsById.ContainsKey(id))
            {
                IDsmElement element = _deletedElementsById[id];
                RegisterElement(element);
            }
        }


        public IEnumerable<IDsmElement> GetRootElements()
        {
            return _rootElements;
        }

        public int GetElementCount()
        {
            return _elementsById.Count;
        }

        public void AssignElementOrder()
        {
            Logger.LogDataModelMessage("AssignElementOrder");

            int order = 1;
            foreach (IDsmElement root in _rootElements)
            {
                DsmElement rootElement = root as DsmElement;
                if (rootElement != null)
                {
                    AssignElementOrder(rootElement, ref order);
                }
            }
        }

        public int ElementCount => _elementsById.Count;

        public IDsmElement GetElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            IEnumerable<IDsmElement> elementWithName = from element in _elementsById.Values
                                                       where element.Fullname == fullname
                                                       select element;

            return elementWithName.FirstOrDefault();
        }

        public IEnumerable<IDsmElement> SearchElements(string text)
        {
            return from element in _elementsById.Values
                   where element.Fullname.Contains(text)
                   select element;
        }

        public void ReorderChildren(IDsmElement element, IVector permutationVector)
        {
            DsmElement parent = element as DsmElement;
            if (parent != null)
            {
                List<IDsmElement> clonedChildren = new List<IDsmElement>(parent.Children);

                foreach (IDsmElement child in clonedChildren)
                {
                    parent.RemoveChild(child);
                }

                for (int i = 0; i < permutationVector.Size(); i++)
                {
                    parent.AddChild(clonedChildren[permutationVector.Get(i)]);
                }
            }
            AssignElementOrder();
        }

        public bool Swap(IDsmElement element1, IDsmElement element2)
        {
            bool swapped = false;

            if (element1.Parent == element2.Parent)
            {
                DsmElement parent = element1.Parent as DsmElement;
                if (parent != null)
                {
                    if (parent.Swap(element1, element2))
                    {
                        swapped = true;
                    }
                }
            }

            AssignElementOrder();

            return swapped;
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            IDsmElement next = null;
            if (element != null)
            {
                next = element.NextSibling;
            }
            return next;
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            IDsmElement previous = null;
            if (element != null)
            {
                previous = element.PreviousSibling;
            }
            return previous;
        }

        public List<int> GetIdsOfElementAndItsChidren(IDsmElement element)
        {
            List<int> ids = new List<int>();
            GetIdsOfElementAndItsChidren(element, ids);
            return ids;
        }

        public void GetIdsOfElementAndItsChidren(IDsmElement element, List<int> ids)
        {
            ids.Add(element.Id);

            foreach (IDsmElement child in element.Children)
            {
                GetIdsOfElementAndItsChidren(child, ids);
            }
        }

        private IDsmElement AddElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            DsmElement element = null;

            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    element = new DsmElement(id, name, type) { Order = order, IsExpanded = expanded };

                    if (_elementsById.ContainsKey(parentId.Value))
                    {
                        DsmElement parent = _elementsById[parentId.Value] as DsmElement;
                        if (parent != null)
                        {
                            parent.AddChild(element);
                            RegisterElement(element);
                        }
                    }
                    else
                    {
                        Logger.LogError($"Parent not found id={id}");
                    }
                }
            }
            else
            {
                element = new DsmElement(id, name, type) { Order = order, IsExpanded = expanded };
                _rootElements.Add(element);
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

        private void RegisterElement(IDsmElement element)
        {
            _elementsById[element.Id] = element;
        }

        private void UnregisterElement(IDsmElement element)
        {
            _elementsById.Remove(element.Id);
            _deletedElementsById[element.Id] = element;

            foreach (IDsmElement child in element.Children)
            {
                UnregisterElement(child);
            }
        }
    }
}
