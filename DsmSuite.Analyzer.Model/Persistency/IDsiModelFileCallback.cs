using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileCallback
    {
        IDsiMetaDataItem ImportMetaDataItem(string groupName, string name, string value);
        IDsiElement ImportElement(int id, string name, string type, string source);
        IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName);
        IEnumerable<IDsiElement> GetElements();
        IEnumerable<IDsiRelation> GetRelations();
    }
}
