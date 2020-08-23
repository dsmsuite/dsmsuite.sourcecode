using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Queries
{
    public class DsmResolvedRelation : IDsmResolvedRelation
    {
        public DsmResolvedRelation(IDsmModel model, IDsmRelation relation)
        {
            Id = relation.Id;
            ConsumerElement = model.GetElementById(relation.Consumer.Id);
            ProviderElement = model.GetElementById(relation.Provider.Id);
            Type = relation.Type;
            Weight = relation.Weight;
        }

        public int Id { get; }
        public IDsmElement ConsumerElement { get; }
        public IDsmElement ProviderElement { get; }
        public string Type { get; }
        public int Weight { get; }
    }
}
