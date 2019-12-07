using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileCallback
    {
        void ImportMetaDataItem(string group, string name, string value);
        void ImportElement(int id, string name, string type, string source);
        void ImportRelation(int consumer, int provider, string type, int weight);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string group);
        IEnumerable<IDsiElement> GetElements();
        IEnumerable<IDsiRelation> GetRelations();
    }
}
