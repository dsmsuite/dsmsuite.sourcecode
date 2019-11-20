using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase
    {
        public RelationListItemViewModel(int index, IRelation relation)
        {
            Index = index;
            ConsumerName = relation.Consumer.Fullname;
            ProviderName = relation.Provider.Fullname;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;
        }

        public int Index { get; }

        public string ConsumerName { get; }
        public string ProviderName { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }
    }
}
