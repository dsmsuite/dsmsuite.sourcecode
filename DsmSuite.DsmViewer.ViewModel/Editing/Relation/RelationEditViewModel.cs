using System.Collections.Generic;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Relation
{
    public class RelationEditViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _relation;
        private string _selectedConsumerType;
        private readonly IDsmElement _consumer;
        private string _providerConsumerType;
        private readonly IDsmElement _provider;
        private string _type;
        private int _weight;
        private string _help;

        public RelationEditViewModel(IDsmApplication application, IDsmRelation relation, IDsmElement consumer, IDsmElement provider)
        {
            _application = application;
            _relation = relation;
            _consumer = consumer;
            _provider = provider;

            ElementTypes = new List<string>();

            ConsumerSearch = new ElementSearchViewModel(application);
            ProviderSearch = new ElementSearchViewModel(application);
        }

        public string Title { get; }

        public List<string> ElementTypes { get; }

        public string SelectedConsumerType
        {
            get { return _selectedConsumerType; }
            private set { _selectedConsumerType = value; OnPropertyChanged(); }
        }

        public ElementSearchViewModel ConsumerSearch { get; }

        public string SelectedProviderType
        {
            get { return _providerConsumerType; }
            private set { _providerConsumerType = value; OnPropertyChanged(); }
        }

        public ElementSearchViewModel ProviderSearch { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(); }
        }
    }
}
