using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmRelationModelFileCallback
    {
        IDsmRelation ImportRelation(int id, int consumer, int provider, string type, int weight);

        IEnumerable<IDsmRelation> GetExportedRelations();
        int GetExportedRelationCount();
    }
}
