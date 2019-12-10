using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Util;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.Analyzer.Model.Core
{
    public class MetaDataModel
    {
        private readonly string _defaultGroupName;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<MetaDataItem>> _metaDataGroups;

        public MetaDataModel(string defaultGroupName, Assembly executingAssembly)
        {
            _defaultGroupName = defaultGroupName;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<MetaDataItem>>();

            AddMetaDataItem("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Import meta data group={group} name={name} value={value}");

            MetaDataItem item = FindItem(group, name);
            if (item != null)
            {
                item.Value = value;
            }
            else
            {
                GetMetaDataGroupItemList(group).Add(new MetaDataItem(name, value));
            }
        }

        public void AddMetaDataItem(string name, string value)
        {
            Logger.LogDataModelMessage($"Add metadata group={_defaultGroupName} name={name} value={value}");

            MetaDataItem item = FindItem(_defaultGroupName, name);
            if (item != null)
            {
                item.Value = value;
            }
            else
            {
                GetMetaDataGroupItemList(_defaultGroupName).Add(new MetaDataItem(name, value));
            }
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return GetMetaDataGroupItemList(group);
        }

        private IList<MetaDataItem> GetMetaDataGroupItemList(string group)
        {
            if (!_metaDataGroups.ContainsKey(group))
            {
                _metaDataGroupNames.Add(group);
                _metaDataGroups[group] = new List<MetaDataItem>();
            }

            return _metaDataGroups[group];
        }

        private MetaDataItem FindItem(string groupName, string name)
        {
            return (from item in GetMetaDataGroupItemList(groupName)
                    where item.Name == name
                    select item).FirstOrDefault();
        }
    }
}
