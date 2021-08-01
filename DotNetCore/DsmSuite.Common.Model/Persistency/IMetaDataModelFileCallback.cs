using System.Collections.Generic;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.Common.Model.Persistency
{
    public interface IMetaDataModelFileCallback
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);

        IEnumerable<string> GetExportedMetaDataGroups();
        IEnumerable<IMetaDataItem> GetExportedMetaDataGroupItems(string group);
    }
}
