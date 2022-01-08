using System.Collections.Generic;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
using System;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ViewModelBase
    {
        private readonly RelationsListViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _selectedConsumer;
        private readonly IDsmElement _selectedProvider;
        private ObservableCollection<RelationListItemViewModel> _relations;
        private RelationListItemViewModel _selectedRelation;

        public event EventHandler<RelationEditViewModel> RelationAddStarted;
        public event EventHandler<RelationEditViewModel> RelationEditStarted;

        public RelationListViewModel(RelationsListViewModelType viewModelType, IDsmApplication application, IDsmElement selectedConsumer, IDsmElement selectedProvider)
        {
            _viewModelType = viewModelType;
            _application = application;
            _selectedConsumer = selectedConsumer;
            _selectedProvider = selectedProvider;

            Title = "Relation List";
            switch (viewModelType)
            {
                case RelationsListViewModelType.ElementIngoingRelations:
                    SubTitle = $"Ingoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddConsumerRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ElementOutgoingRelations:
                    SubTitle = $"Outgoing relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddProviderRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ElementInternalRelations:
                    SubTitle = $"Internal relations of {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddInternalRelationExecute, AddRelationCanExecute);
                    break;
                case RelationsListViewModelType.ConsumerProviderRelations:
                    SubTitle = $"Relations between consumer {_selectedConsumer.Fullname} and provider {_selectedProvider.Fullname}";
                    AddRelationCommand = new RelayCommand<object>(AddConsumerProviderRelationExecute, AddRelationCanExecute);
                    break;
                default:
                    SubTitle = "";
                    break;
            }

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
            DeleteRelationCommand = new RelayCommand<object>(DeleteRelationExecute, DeleteRelationCanExecute);
            EditRelationCommand = new RelayCommand<object>(EditRelationExecute, EditRelationCanExecute);

            UpdateRelations(null);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<RelationListItemViewModel> Relations
        {
            get { return _relations; }
            private set { _relations = value; OnPropertyChanged(); }
        }

        public RelationListItemViewModel SelectedRelation
        {
            get { return _selectedRelation; }
            set { _selectedRelation = value; OnPropertyChanged(); }
        }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand DeleteRelationCommand { get; }
        public ICommand EditRelationCommand { get; }
        public ICommand AddRelationCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RelationListItemViewModel viewModel in Relations)
            {
                builder.AppendLine($"{viewModel.Index,-5}, {viewModel.ConsumerName,-100}, {viewModel.ProviderName,-100}, {viewModel.RelationType,-30}, {viewModel.RelationWeight,-10}, {viewModel.Properties,-150}");
            }
            Clipboard.SetText(builder.ToString());
        }

        private void DeleteRelationExecute(object parameter)
        {
            _application.DeleteRelation(SelectedRelation.Relation);
            UpdateRelations(SelectedRelation.Relation);
        }

        private bool DeleteRelationCanExecute(object parameter)
        {
            return SelectedRelation != null;
        }

        private void EditRelationExecute(object parameter)
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Modify, _application, SelectedRelation.Relation, null, null);
            RelationEditStarted?.Invoke(this, relationEditViewModel);
        }

        private bool EditRelationCanExecute(object parameter)
        {
            return SelectedRelation != null;
        }

        private void AddConsumerRelationExecute(object parameter)
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _application.RootElement, _selectedProvider);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddProviderRelationExecute(object parameter)
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _selectedProvider, _application.RootElement);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddInternalRelationExecute(object parameter)
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _selectedProvider, _selectedProvider);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private void AddConsumerProviderRelationExecute(object parameter)
        {
            RelationEditViewModel relationEditViewModel = new RelationEditViewModel(RelationEditViewModelType.Add, _application, null, _selectedConsumer, _selectedProvider);
            relationEditViewModel.RelationUpdated += OnRelationUpdated;
            RelationAddStarted?.Invoke(this, relationEditViewModel);
        }

        private bool AddRelationCanExecute(object parameter)
        {
            return true;
        }

        private void OnRelationUpdated(object sender, IDsmRelation updatedRelation)
        {
            UpdateRelations(updatedRelation);
        }

        private void UpdateRelations(IDsmRelation updatedRelation)
        {
            RelationListItemViewModel selectedRelationListItemViewModel = null;
            IEnumerable<IDsmRelation> relations;
            switch (_viewModelType)
            {
                case RelationsListViewModelType.ElementIngoingRelations:
                    relations = _application.FindIngoingRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ElementOutgoingRelations:
                    relations = _application.FindOutgoingRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ElementInternalRelations:
                    relations = _application.FindInternalRelations(_selectedProvider);
                    break;
                case RelationsListViewModelType.ConsumerProviderRelations:
                    relations = _application.FindResolvedRelations(_selectedConsumer, _selectedProvider);
                    break;
                default:
                    relations = new List<IDsmRelation>();
                    break;
            }

            List<RelationListItemViewModel> relationViewModels = new List<RelationListItemViewModel>();

            foreach (IDsmRelation relation in relations)
            {
                RelationListItemViewModel relationListItemViewModel = new RelationListItemViewModel(_application, relation);
                relationViewModels.Add(relationListItemViewModel);

                if (updatedRelation != null)
                {
                    if (relation.Id == updatedRelation.Id)
                    {
                        selectedRelationListItemViewModel = relationListItemViewModel;
                    }
                }
            }

            relationViewModels.Sort();

            int index = 1;
            foreach (RelationListItemViewModel viewModel in relationViewModels)
            {
                viewModel.Index = index;
                index++;
            }

            Relations = new ObservableCollection<RelationListItemViewModel>(relationViewModels);
            SelectedRelation = selectedRelationListItemViewModel;
        }
    }
}
