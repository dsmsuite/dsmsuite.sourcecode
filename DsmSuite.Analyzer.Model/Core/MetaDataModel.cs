using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Data;
using System.Reflection;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Model.Core
{
    public class MetaDataModel
    {
        private readonly string _processStep;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<IDsiMetaDataItem>> _metaDataGroups;

        public MetaDataModel(string processStep, Assembly executingAssembly)
        {
            _processStep = processStep;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<IDsiMetaDataItem>>();

            AddMetaData("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void AddMetaData(string name, string value)
        {
            Logger.LogDataModelMessage($"Add metadata group={_processStep} name={name} value={value}");

            GetMetaDataGroupItemList(_processStep).Add(new DsiMetaDataItem(name, value));
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Import meta data group={group} name={name} value={value}");

            GetMetaDataGroupItemList(group).Add(new DsiMetaDataItem(name, value));
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return GetMetaDataGroupItemList(group);
        }

        private IList<IDsiMetaDataItem> GetMetaDataGroupItemList(string group)
        {
            if (!_metaDataGroups.ContainsKey(group))
            {
                _metaDataGroupNames.Add(group);
                _metaDataGroups[group] = new List<IDsiMetaDataItem>();
            }

            return _metaDataGroups[group];
        }
    }
}
