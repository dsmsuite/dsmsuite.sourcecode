using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmModelFileCallback
    {
        IDsmMetaDataItem ImportMetaDataItem(string group, string name, string value);
        IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parent);
        IDsmRelation ImportRelation(int id, int consumer, int provider, string type, int weight);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsmMetaDataItem> GetMetaDataGroupItems(string group);
        IEnumerable<IDsmElement> GetRootElements();
        int GetElementCount();
        IEnumerable<IDsmRelation> GetRelations();
        int GetRelationCount();
    }
}
