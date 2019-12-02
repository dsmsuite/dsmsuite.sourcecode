using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Data
{

    public class DsmResolvedRelation : IDsmResolvedRelation
    {
        public DsmResolvedRelation(DependencyModel dependencyModel, IDsmRelation relation)
        {
            Consumer = dependencyModel.GetElementById(relation.ConsumerId);
            Provider = dependencyModel.GetElementById(relation.ProviderId);
            Type = relation.Type;
            Weight = relation.Weight;
        }

        public IDsmElement Consumer { get; }
        public IDsmElement Provider { get; }
        public string Type { get; }
        public int Weight { get; }
    }
}
