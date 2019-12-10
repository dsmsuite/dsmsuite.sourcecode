using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Data;
using System.Reflection;
using DsmSuite.Common.Util;
using System.Linq;

namespace DsmSuite.Analyzer.Model.Core
{
    public class MetaDataModel
    {
        private readonly string _defaultGroupName;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<DsiMetaDataItem>> _metaDataGroups;

        public MetaDataModel(string defaultGroupName, Assembly executingAssembly)
        {
            _defaultGroupName = defaultGroupName;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<DsiMetaDataItem>>();

            AddMetaDataItem("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Import meta data group={group} name={name} value={value}");

            DsiMetaDataItem item = FindItem(group, name);
            if (item != null)
            {
                item.Value = value;
            }
            else
            {
                GetMetaDataGroupItemList(group).Add(new DsiMetaDataItem(name, value));
            }
        }

        public void AddMetaDataItem(string name, string value)
        {
            Logger.LogDataModelMessage($"Add metadata group={_defaultGroupName} name={name} value={value}");

            DsiMetaDataItem item = FindItem(_defaultGroupName, name);
            if (item != null)
            {
                item.Value = value;
            }
            else
            {
                GetMetaDataGroupItemList(_defaultGroupName).Add(new DsiMetaDataItem(name, value));
            }
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return GetMetaDataGroupItemList(group);
        }

        private IList<DsiMetaDataItem> GetMetaDataGroupItemList(string group)
        {
            if (!_metaDataGroups.ContainsKey(group))
            {
                _metaDataGroupNames.Add(group);
                _metaDataGroups[group] = new List<DsiMetaDataItem>();
            }

            return _metaDataGroups[group];
        }

        private DsiMetaDataItem FindItem(string groupName, string name)
        {
            return (from item in GetMetaDataGroupItemList(groupName)
                    where item.Name == name
                    select item).FirstOrDefault();
        }
    }
}
