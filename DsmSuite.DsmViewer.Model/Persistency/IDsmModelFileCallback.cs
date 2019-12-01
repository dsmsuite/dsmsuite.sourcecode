using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmModelFileCallback
    {
        void ImportMetaDataItem(string groupName, IMetaDataItem metaDataItem);
        void ImportElement(IElement element);
        void ImportRelation(IRelation relation);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);
        IEnumerable<IElement> GetElements();
        IEnumerable<IRelation> GetRelations();
    }
}
