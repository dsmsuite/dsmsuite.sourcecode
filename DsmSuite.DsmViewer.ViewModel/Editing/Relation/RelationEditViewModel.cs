using System;
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
        private readonly RelationEditViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _selectedRelation;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;
        private string _selectedRelationType;
        private int _weight;
        private string _help;

        private static string _lastSelectedRelationType = "";
        private static string _lastSelectedConsumerElementType = "";
        private static string _lastSelectedProviderElementType = "";

        public event EventHandler<IDsmRelation> RelationUpdated;

        public RelationEditViewModel(RelationEditViewModelType viewModelType, IDsmApplication application, IDsmRelation selectedRelation, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _viewModelType = viewModelType;
            _application = application;

            switch (_viewModelType)
            {
                case RelationEditViewModelType.Modify:
                    Title = "Modify relation";
                    _selectedRelation = selectedRelation;
                    _selectedConsumer = _selectedRelation.Consumer;
                    _selectedProvider = _selectedRelation.Provider;
                    SelectedRelationType = _selectedRelation.Type;
                    Weight = _selectedRelation.Weight;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptModifyExecute, AcceptCanExecute);
                    break;
                case RelationEditViewModelType.Add:
                    Title = "Add relation";
                    _selectedRelation = null;
                    _selectedConsumer = selectedConsumer;
                    _selectedProvider = selectedProvider;
                    SelectedRelationType = _lastSelectedRelationType;
                    Weight = 1;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptAddExecute, AcceptCanExecute);
                    break;
                default:
                    break;
            }

            ConsumerSearchViewModel = new ElementSearchViewModel(application, _selectedConsumer, _lastSelectedConsumerElementType, false);
            ProviderSearchViewModel = new ElementSearchViewModel(application, _selectedProvider, _lastSelectedProviderElementType, false);
            RelationTypes = new List<string>(application.GetRelationTypes());
        }

        public string Title { get; }

        public ElementSearchViewModel ConsumerSearchViewModel { get; }

        public ElementSearchViewModel ProviderSearchViewModel { get; }

        public List<string> RelationTypes { get; }

        public string SelectedRelationType
        {
            get { return _selectedRelationType; }
            set { _selectedRelationType = value; _lastSelectedRelationType = value;  OnPropertyChanged(); }
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

        private void AcceptModifyExecute(object parameter)
        {
            bool relationUpdated = false;
            if (_selectedRelation.Type != SelectedRelationType)
            {
                _application.ChangeRelationType(_selectedRelation, SelectedRelationType);
                relationUpdated = true;
            }

            if (_selectedRelation.Weight != Weight)
            {
                _application.ChangeRelationWeight(_selectedRelation, Weight);
                relationUpdated = true;
            }

            if (relationUpdated)
            {
                InvokeRelationUpdated(_selectedRelation);
            }
        }

        private void AcceptAddExecute(object parameter)
        {
            IDsmRelation createdRelation = _application.CreateRelation(ConsumerSearchViewModel.SelectedElement, ProviderSearchViewModel.SelectedElement, SelectedRelationType, Weight);
            InvokeRelationUpdated(createdRelation);
        }

        private bool AcceptCanExecute(object parameter)
        {
            if (ConsumerSearchViewModel.SelectedElement == null)
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
        
        private void InvokeRelationUpdated(IDsmRelation updateRelation)
        {
            _lastSelectedConsumerElementType = ConsumerSearchViewModel.SelectedElementType;
            _lastSelectedProviderElementType = ProviderSearchViewModel.SelectedElementType;
            RelationUpdated?.Invoke(this, updateRelation);
        }
    }
}
