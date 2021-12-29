using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiRelationModelFileCallback
    {
        IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties);
        IEnumerable<IDsiRelation> GetRelations();
        int CurrentRelationCount { get; }
    }
}
