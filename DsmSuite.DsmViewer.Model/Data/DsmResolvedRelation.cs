using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Data
{

    public class DsmResolvedRelation : IDsmResolvedRelation
    {
        public DsmResolvedRelation(Dictionary<int, DsmElement> elementsById, IDsmRelation relation)
        {
            Id = relation.Id;
            Consumer = elementsById.ContainsKey(relation.Consumer) ? elementsById[relation.Consumer] : null;
            Provider = elementsById.ContainsKey(relation.Provider) ? elementsById[relation.Provider] : null;
            Type = relation.Type;
            Weight = relation.Weight;
        }

        public int Id { get; }
        public IDsmElement Consumer { get; }
        public IDsmElement Provider { get; }
        public string Type { get; }
        public int Weight { get; }
    }
}
