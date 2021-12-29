using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Persistency;

namespace DsmSuite.Analyzer.Model.Core
{
    public class DsiElementModel : IDsiElementModelFileCallback
    {
        private readonly Dictionary<string, IDsiElement> _elementsByName;
        private readonly Dictionary<int, IDsiElement> _elementsById;
        private readonly Dictionary<string, int> _elementTypeCount;

        public event EventHandler<int> ElementRemoved;

        public DsiElementModel()
        {
            _elementsByName = new Dictionary<string, IDsiElement>();
            _elementsById = new Dictionary<int, IDsiElement>();
            _elementTypeCount = new Dictionary<string, int>();
        }

        public void Clear()
        {
            _elementsByName.Clear();
            _elementsById.Clear();
            _elementTypeCount.Clear();
        }

        public IDsiElement ImportElement(int id, string name, string type, IDictionary<string, string> properties)
        {
            Logger.LogDataModelMessage($"Import element id={id} name={name} type={type}");

            DsiElement element = new DsiElement(id, name, type, properties);
            string key = CreateKey(element.Name);
            _elementsByName[key] = element;
            _elementsById[element.Id] = element;
            IncrementElementTypeCount(element.Type);
            return element;
        }

        public IDsiElement AddElement(string name, string type, IDictionary<string, string> properties)
        {
            Logger.LogDataModelMessage($"Add element name={name} type={type}");

            string key = CreateKey(name);
            if (!_elementsByName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByName.Count + 1;
                DsiElement element = new DsiElement(id, name, type, properties);
                _elementsByName[key] = element;
                _elementsById[id] = element;
                return element;
            }
            else
            {
                return null;
            }
        }

        public void IgnoreElement(string name, string type)
        {
            Logger.LogDataModelMessage($"Ignore element name={name} type={type}");
        }
        
        public void RemoveElement(IDsiElement element)
        {
            Logger.LogDataModelMessage($"Remove element id={element.Id} name={element.Name} type={element.Type}");

            string key = element.Name.ToLower();
            _elementsByName.Remove(key);
            _elementsById.Remove(element.Id);

            ElementRemoved?.Invoke(this, element.Id);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            Logger.LogDataModelMessage("Rename element id={element.Id} from {element.Name} to {newName}");

            DsiElement e = element as DsiElement;
            if (e != null)
            {
                string oldKey = CreateKey(e.Name);
                _elementsByName.Remove(oldKey);
                e.Name = newName;
                string newKey = CreateKey(e.Name);
                _elementsByName[newKey] = e;
            }
        }

        public IDsiElement FindElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IDsiElement FindElementByName(string name)
        {
            string key = name.ToLower();
            return _elementsByName.ContainsKey(key) ? _elementsByName[key] : null;
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            return _elementsById.Values;
        }

        public ICollection<string> GetElementTypes()
        {
            return _elementTypeCount.Keys;
        }

        public int GetElementTypeCount(string type)
        {
            if (_elementTypeCount.ContainsKey(type))
            {
                return _elementTypeCount[type];
            }
            else
            {
                return 0;
            }
        }

        public int CurrentElementCount => _elementsByName.Values.Count;

        private void IncrementElementTypeCount(string type)
        {
            if (!_elementTypeCount.ContainsKey(type))
            {
                _elementTypeCount[type] = 0;
            }
            _elementTypeCount[type]++;
        }

        private string CreateKey(string name)
        {
            return name.ToLower();
        }
    }
}
