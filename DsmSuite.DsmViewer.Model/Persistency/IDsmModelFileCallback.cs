using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmModelFileCallback
    {
        IMetaDataItem ImportMetaDataItem(string group, string name, string value);
        IDsmAction ImportAction(int id, string type, string data);
        IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parent);
        IDsmRelation ImportRelation(int id, int consumer, int provider, string type, int weight);

        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsmAction> GetActions();
        int GetActionCount();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string group);
        IEnumerable<IDsmElement> GetRootElements();
        int GetElementCount();
        IEnumerable<IDsmRelation> GetRelations();
        int GetRelationCount();
    }
}
