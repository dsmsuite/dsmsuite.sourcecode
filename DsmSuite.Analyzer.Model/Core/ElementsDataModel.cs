using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.Analyzer.Model.Core
{
    public class ElementsDataModel
    {
        private readonly Dictionary<string, IDsiElement> _elementsByName;
        private readonly Dictionary<int, IDsiElement> _elementsById;
        private readonly Dictionary<string, int> _elementTypeCount;

        public ElementsDataModel()
        {
            _elementsByName = new Dictionary<string, IDsiElement>();
            _elementsById = new Dictionary<int, IDsiElement>();
            _elementTypeCount = new Dictionary<string, int>();
        }

        public void ImportElement(int id, string name, string type, string source)
        {
            Logger.LogDataModelMessage($"Import element id={id} name={name} type={type} source={source}");

            DsiElement element = new DsiElement(id, name, type, source);
            _elementsByName[element.Name] = element;
            _elementsById[element.Id] = element;
            IncrementElementTypeCount(element.Type);
        }

        public IDsiElement AddElement(string name, string type, string source)
        {
            Logger.LogDataModelMessage($"Add element name={name} type={type} source={source}");

            string key = name.ToLower();
            if (!_elementsByName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByName.Count + 1;
                DsiElement element = new DsiElement(id, name, type, source);
                _elementsByName[key] = element;
                _elementsById[id] = element;
                return element;
            }
            else
            {
                return null;
            }
        }

        public void RemoveElement(IDsiElement element)
        {
            Logger.LogDataModelMessage($"Remove element id={element.Id} name={element.Name} type={element.Type} source={element.Source}");

            string key = element.Name.ToLower();
            _elementsByName.Remove(key);
            _elementsById.Remove(element.Id);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            Logger.LogDataModelMessage("Rename element id={element.Id} from {element.Name} to {newName}");

            DsiElement e = element as DsiElement;
            if (e != null)
            {
                string oldKey = e.Name.ToLower();
                _elementsByName.Remove(oldKey);
                e.Name = newName;
                string newKey = e.Name.ToLower();
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

        public int TotalElementCount => _elementsByName.Values.Count;

        private void IncrementElementTypeCount(string type)
        {
            if (!_elementTypeCount.ContainsKey(type))
            {
                _elementTypeCount[type] = 0;
            }
            _elementTypeCount[type]++;
        }
    }
}
