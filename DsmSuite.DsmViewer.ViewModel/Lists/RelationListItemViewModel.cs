using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase
    {
        public RelationListItemViewModel(int index, IDsmResolvedRelation relation)
        {
            Index = index;
            ConsumerName = relation.ConsumerElement.Fullname;
            ProviderName = relation.ProviderElement.Fullname;
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
