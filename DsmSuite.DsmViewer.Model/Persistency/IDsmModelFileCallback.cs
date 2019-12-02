using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmModelFileCallback
    {
        void ImportMetaDataItem(string groupName, IDsmMetaDataItem metaDataItem);
        void ImportElement(IDsmElement element);
        void ImportRelation(IDsmRelation relation);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName);
        IEnumerable<IDsmElement> GetElements();
        IEnumerable<IDsmRelation> GetRelations();
    }
}
