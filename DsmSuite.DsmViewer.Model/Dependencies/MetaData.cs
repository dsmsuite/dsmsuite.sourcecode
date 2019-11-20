using System.Collections.Generic;
using System.Linq;

namespace DsmSuite.DsmViewer.Model.Dependencies
{
    public class MetaData
    {
        private readonly Dictionary<string, Dictionary<string, string>> _metaData;

        public MetaData()
        {
            _metaData = new Dictionary<string, Dictionary<string, string>>();
        }

        public void Clear()
        {
            _metaData.Clear();
        }

        public void AddMetaData(string group, string name, string value)
        {
            if (!_metaData.ContainsKey(group))
            {
                _metaData[group] = new Dictionary<string, string>();
            }

            _metaData[group][name] = value;
        }

        public IList<string> GetGroups()
        {
            return _metaData.Keys.OrderBy(x => x).ToList();
        }

        public IList<string> GetNames(string group)
        {
            if (!_metaData.ContainsKey(group))
            {
                return new List<string>();
            }
            else
            {
                return _metaData[group].Keys.OrderBy(x => x).ToList();
            }
        }

        public string GetValue(string group, string name)
        {
            string value = "";
            if (_metaData.ContainsKey(group) && _metaData[group].ContainsKey(name))
            {
                value = _metaData[group][name];
            }
            return value;
        }
    }
}
