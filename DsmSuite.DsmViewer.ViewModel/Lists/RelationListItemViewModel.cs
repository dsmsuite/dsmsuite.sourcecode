using System;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListItemViewModel : ViewModelBase, IComparable
    {
        public RelationListItemViewModel(IDsmApplication application, IDsmRelation relation)
        {
            Relation = relation;

            ConsumerPath = relation.Consumer.Parent.Fullname;
            ConsumerName = relation.Consumer.Name;
            ConsumerType = relation.Consumer.Type;
            ProviderPath = relation.Provider.Parent.Fullname;
            ProviderName = relation.Provider.Name;
            ProviderType = relation.Provider.Type;
            RelationType = relation.Type;
            RelationWeight = relation.Weight;
            Properties = relation.Properties;

            CycleType cycleType = application.IsCyclicDependency(relation.Consumer, relation.Provider);
            switch (cycleType)
            {
                case CycleType.Hierarchical:
                    Cyclic = "Hierarchical";
                    break;
                case CycleType.System:
                    Cyclic = "System";
                    break;
                case CycleType.None:
                default:
                    Cyclic = "";
                    break;
            }
        }

        public IDsmRelation Relation { get; set; }

        public int Index { get; set; }

        public string ConsumerPath { get; }
        public string ConsumerName { get; }
        public string ConsumerType { get; }
        public string ProviderPath { get; }
        public string ProviderName { get; }
        public string ProviderType { get; }
        public string RelationType { get; }
        public int RelationWeight { get; }
        public string Cyclic { get; }
        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredRelationPropertyNames()
        {
            return Relation.DiscoveredRelationPropertyNames();
        }

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
