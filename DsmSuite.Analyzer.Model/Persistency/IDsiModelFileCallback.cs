using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileCallback
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
