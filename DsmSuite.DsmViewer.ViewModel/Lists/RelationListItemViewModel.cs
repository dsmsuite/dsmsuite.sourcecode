using System;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.Application.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase, IComparable
    {
        public RelationListItemViewModel(IDsmRelation relation)
        {
            ConsumerName = relation.Consumer.Fullname;
            ConsumerType = relation.Consumer.Type;
            ProviderName = relation.Provider.Fullname;
            ProviderType = relation.Provider.Type;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;
        }

        public int Index { get; set; }

        public string ConsumerName { get; }
        public string ConsumerType { get; }
        public string ProviderName { get; }
        public string ProviderType { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }

        public int CompareTo(object obj)
        {
            RelationListItemViewModel other = obj as RelationListItemViewModel;

            int compareConsumer = string.Compare(ConsumerName, other?.ConsumerName, StringComparison.Ordinal);
            int compareProvider = string.Compare(ProviderName, other?.ProviderName, StringComparison.Ordinal);

            if (compareConsumer != 0)
            {
                return compareConsumer;
            }
            else
            {
                return compareProvider;
            }
        }
    }
}
