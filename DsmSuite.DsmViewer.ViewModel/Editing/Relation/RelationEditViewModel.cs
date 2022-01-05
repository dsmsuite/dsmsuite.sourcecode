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
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;
        private string _selectedRelationType;
        private int _weight;
        private string _help;

        public RelationEditViewModel(IDsmApplication application, IDsmRelation relation, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _application = application;
            _relation = relation;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            ConsumerSearchViewModel = new ElementSearchViewModel(application, _selectedConsumer);
            ProviderSearchViewModel = new ElementSearchViewModel(application, _selectedProvider);
            RelationTypes = new List<string>(application.GetRelationTypes());
            SelectedRelationType = null;
            Weight = 1;

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public ElementSearchViewModel ConsumerSearchViewModel { get; }

        public ElementSearchViewModel ProviderSearchViewModel { get; }

        public List<string> RelationTypes { get; }

        public string SelectedRelationType
        {
            get { return _selectedRelationType; }
            set { _selectedRelationType = value; OnPropertyChanged(); }
        }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(); }
        }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public ICommand AcceptChangeCommand { get; }

        private void AcceptChangeExecute(object parameter)
        {
            _application.CreateRelation(ConsumerSearchViewModel.SelectedElement, ProviderSearchViewModel.SelectedElement, SelectedRelationType, Weight);
        }

        private bool AcceptChangeCanExecute(object parameter)
        { 
            if (ConsumerSearchViewModel.SelectedElement ==  null)
            {
                Help = "No consumer selected";
                return false;
            }
            else if (ProviderSearchViewModel.SelectedElement == null)
            {
                Help = "No provider selected";
                return false;
            }
            else if (SelectedRelationType == null)
            {
                Help = "No relation type selected";
                return false;
            }
            else if (Weight < 0)
            {
                Help = "Weight can not be negative";
                return false;
            }
            else if (Weight == 0)
            {
                Help = "Weight can not be zero";
                return false;
            }
            else 
            {
                Help = "";
                return true;
            }
        }
    }
}
