using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase
    {
        public RelationListItemViewModel(int index, IDsmResolvedRelation relation)
        {
            Index = index;
            ConsumerName = relation.ConsumerElement.Fullname;
            ConsumerType = relation.ConsumerElement.Type;
            ProviderName = relation.ProviderElement.Fullname;
            ProviderType = relation.ProviderElement.Type;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;
        }

        public int Index { get; }

        public string ConsumerName { get; }
        public string ConsumerType { get; }
        public string ProviderName { get; }
        public string ProviderType { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }
    }
}
