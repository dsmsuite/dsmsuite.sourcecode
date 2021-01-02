using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiRelationModelFileCallback
    {
        IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight, string annotation);
        IEnumerable<IDsiRelation> GetRelations();
        int CurrentRelationCount { get; }
    }
}
