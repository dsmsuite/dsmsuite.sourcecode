using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase
    {
        public RelationListItemViewModel(int index, IDsmRelation relation)
        {
            Index = index;
            ConsumerName = relation.Consumer.Fullname;
            ConsumerType = relation.Consumer.Type;
            ProviderName = relation.Provider.Fullname;
            ProviderType = relation.Provider.Type;
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
