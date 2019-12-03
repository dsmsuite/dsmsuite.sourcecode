using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Data
{

    public class DsmResolvedRelation : IDsmResolvedRelation
    {
        public DsmResolvedRelation(Dictionary<int, DsmElement> elementsById, IDsmRelation relation)
        {
            Consumer = elementsById.ContainsKey(relation.ConsumerId) ? elementsById[relation.ConsumerId] : null;
            Provider = elementsById.ContainsKey(relation.ProviderId) ? elementsById[relation.ProviderId] : null;
            Type = relation.Type;
            Weight = relation.Weight;
        }

        public IDsmElement Consumer { get; }
        public IDsmElement Provider { get; }
        public string Type { get; }
        public int Weight { get; }
    }
}
