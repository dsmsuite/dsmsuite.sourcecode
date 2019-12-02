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
        IDsmMetaDataItem ImportMetaDataItem(string groupName, string name, string value);
        IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId);
        IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight);

        IList<string> GetMetaDataGroups();
        IList<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName);
        IList<IDsmElement> GetElements();
        IList<IDsmRelation> GetRelations();
    }
}
