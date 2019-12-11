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
        private readonly Assembly _executingAssembly;
        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<MetaDataItem>> _metaDataGroups;

        public MetaDataModel(string defaultGroupName, Assembly executingAssembly)
        {
            _defaultGroupName = defaultGroupName;
            _executingAssembly = executingAssembly;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<MetaDataItem>>();

            AddDefaultItems();
        }

        public void Clear()
        {
            _metaDataGroupNames.Clear();
            _metaDataGroups.Clear();

            AddDefaultItems();
        }

        public IMetaDataItem AddMetaDataItemToDefaultGroup(string name, string value)
        {
            return AddMetaDataItem(_defaultGroupName, name, value);
        }

        public IMetaDataItem AddMetaDataItem(string groupName, string name, string value)
        {
            Logger.LogDataModelMessage($"Add metadata group={groupName} name={name} value={value}");

            MetaDataItem item = FindItem(groupName, name);
            if (item != null)
            {
                item.Value = value;
            }
            else
            {
                item = new MetaDataItem(name, value);
                GetMetaDataGroupItemList(groupName).Add(item);
            }
            return item;
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return GetMetaDataGroupItemList(groupName);
        }

        private IList<MetaDataItem> GetMetaDataGroupItemList(string groupName)
        {
            if (!_metaDataGroups.ContainsKey(groupName))
            {
                _metaDataGroupNames.Add(groupName);
                _metaDataGroups[groupName] = new List<MetaDataItem>();
            }

            return _metaDataGroups[groupName];
        }

        private MetaDataItem FindItem(string groupName, string name)
        {
            return (from item in GetMetaDataGroupItemList(groupName)
                    where item.Name == name
                    select item).FirstOrDefault();
        }

        private void AddDefaultItems()
        {
            AddMetaDataItemToDefaultGroup("Executable", SystemInfo.GetExecutableInfo(_executingAssembly));
        }
    }
}
