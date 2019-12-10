using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class MetaDataModel
    {
        private readonly string _defaultGroupName;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<IDsmMetaDataItem>> _metaDataGroups;

        public MetaDataModel(string defaultGroupName, Assembly executingAssembly)
        {
            _defaultGroupName = defaultGroupName;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<IDsmMetaDataItem>>();

            AddMetaData("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void Clear()
        {
            _metaDataGroupNames.Clear();
            _metaDataGroups.Clear();
        }

        public IDsmMetaDataItem AddMetaData(string name, string value)
        {
            Logger.LogDataModelMessage($"Add meta data group={_defaultGroupName} name={name} value={value}");

            DsmMetaDataItem metaDataItem = new DsmMetaDataItem(name, value);
            GetMetaDataGroupItemList(_defaultGroupName).Add(metaDataItem);
            return metaDataItem;
        }

        public IDsmMetaDataItem AddMetaData(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Add meta data group={group} name={name} value={value}");

            DsmMetaDataItem metaDataItem = new DsmMetaDataItem(name, value);
            GetMetaDataGroupItemList(group).Add(metaDataItem);
            return metaDataItem;
        }

        public IDsmMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Import meta data group={group} name={name} value={value}");

            DsmMetaDataItem metaDataItem = new DsmMetaDataItem(name, value);
            GetMetaDataGroupItemList(group).Add(metaDataItem);
            return metaDataItem;
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return GetMetaDataGroupItemList(groupName);
        }

        private IList<IDsmMetaDataItem> GetMetaDataGroupItemList(string groupName)
        {
            if (!_metaDataGroups.ContainsKey(groupName))
            {
                _metaDataGroupNames.Add(groupName);
                _metaDataGroups[groupName] = new List<IDsmMetaDataItem>();
            }

            return _metaDataGroups[groupName];
        }
    }
}
