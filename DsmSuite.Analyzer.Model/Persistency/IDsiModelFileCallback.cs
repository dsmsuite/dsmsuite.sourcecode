using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileCallback
    {
        void ImportMetaDataItem(string groupName, IDsiMetaDataItem metaDataItem);
        void ImportElement(IDsiElement element);
        void ImportRelation(IDsiRelation relation);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName);
        IEnumerable<IDsiElement> GetElements();
        IEnumerable<IDsiRelation> GetRelations();
    }
}
