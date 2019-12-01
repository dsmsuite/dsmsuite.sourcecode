using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiModelFileWritterCallback
    {
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataItems(string groupName);
        IEnumerable<IElement> GetElements();
        IEnumerable<IRelation> GetRelations();
    }
}
