using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmRelationModelFileCallback
    {
        IDsmRelation ImportRelation(int id, IDsmElement consumer, IDsmElement provider, string type, int weight, bool deleted);

        IEnumerable<IDsmRelation> GetExportedRelations();
        int GetExportedRelationCount();
    }
}
