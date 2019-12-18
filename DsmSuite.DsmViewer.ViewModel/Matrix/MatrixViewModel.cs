using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Collections.Generic;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Lists;
using DsmSuite.DsmViewer.ViewModel.Main;
using System;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class MatrixViewModel : ViewModelBase
    {
        private double _zoomLevel;
        private readonly IMainViewModel _mainViewModel;
        private readonly IDsmApplication _application;
        private readonly IEnumerable<IDsmElement> _selectedElements;
        private ElementViewModel _selectedConsumer;
        private ElementViewModel _selectedProvider;
        private ElementViewModel _hoveredConsumer;
        private ElementViewModel _hoveredProvider;
        private int? _selectedConsumerId;
        private int? _selectedProviderId;
        private ObservableCollection<ElementTreeItemViewModel> _providers;
        private IList<ElementViewModel> _consumers;
        private IList<IList<CellViewModel>> _dependencies;

        public MatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IEnumerable<IDsmElement> selectedElements)
        {
            _mainViewModel = mainViewModel;
            _application = application;
            _selectedElements = selectedElements;

            ToggleElementExpandedCommand = mainViewModel.ToggleElementExpandedCommand;

            MoveCommand = mainViewModel.MoveElementCommand;
            MoveUpCommand = mainViewModel.MoveUpElementCommand;
            MoveDownCommand = mainViewModel.MoveDownElementCommand;
            PartitionCommand = mainViewModel.PartitionElementCommand;

            ShowCellRelationsCommand = new RelayCommand<object>(ShowCellRelationsExecute, ShowCellRelationsCanExecute);
            ShowCellConsumersCommand = new RelayCommand<object>(ShowCellConsumersExecute, ShowCellConsumersCanExecute);
            ShowCellProvidersCommand = new RelayCommand<object>(ShowCellProvidersExecute, ShowCellProvidersCanExecute);
            ShowElementDetailMatrixCommand = mainViewModel.ShowElementDetailMatrixCommand;
            ShowElementContextMatrixCommand = mainViewModel.ShowElementContextMatrixCommand;

            ShowElementConsumersCommand = new RelayCommand<object>(ShowElementConsumersExecute, ShowConsumersCanExecute);
            ShowElementProvidedInterfacesCommand = new RelayCommand<object>(ShowProvidedInterfacesExecute, ShowElementProvidedInterfacesCanExecute);
            ShowElementRequiredInterfacesCommand = new RelayCommand<object>(ShowElementRequiredInterfacesExecute, ShowElementRequiredInterfacesCanExecute);
            ShowCellDetailMatrixCommand = mainViewModel.ShowCellDetailMatrixCommand;

            CreateElementCommand = mainViewModel.CreateElementCommand;
            DeleteElementCommand = mainViewModel.DeleteElementCommand;
            MoveElementCommand = mainViewModel.MoveElementCommand;
            EditElementCommand = mainViewModel.EditElementCommand;

            CreateRelationCommand = mainViewModel.CreateRelationCommand;
            DeleteRelationCommand = mainViewModel.DeleteRelationCommand;
            EditRelationCommand = mainViewModel.EditRelationCommand;

            Reload();

            ZoomLevel = 1.0;
        }

        public ICommand ShowCellRelationsCommand { get; }
        public ICommand ShowCellConsumersCommand { get; }
        public ICommand ShowCellProvidersCommand { get; }
        public ICommand ShowElementConsumersCommand { get; }
        public ICommand ShowElementProvidedInterfacesCommand { get; }
        public ICommand ShowElementRequiredInterfacesCommand { get; }
        public ICommand CreateElementCommand { get; }
        public ICommand DeleteElementCommand { get; }
        public ICommand MoveElementCommand { get; }
        public ICommand EditElementCommand { get; }
        public ICommand CreateRelationCommand { get; }
        public ICommand DeleteRelationCommand { get; }
        public ICommand EditRelationCommand { get; }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; OnPropertyChanged(); }
        }

        public void Reload()
        {
            Providers = CreateProviderTree();
            var providerLeafs = FindProviderLeafElementViewModels(Providers);
            Consumers = FindConsumerLeafElementViewModels(Providers);
            Dependencies = UpdateCells(providerLeafs, Consumers);
            RestoreSelections(Providers);
        }
        
        public ElementViewModel SelectedConsumer
        {
            get
            {
                return _selectedConsumer;
            }
            private set
            {
                _selectedConsumer = value; OnPropertyChanged();
            }
        }

        public ElementViewModel SelectedProvider
        {
            get
            {
                return _selectedProvider;
            }
            private set
            {
                _selectedProvider = value; OnPropertyChanged();
            }
        }

        public void SelectConsumer(ElementViewModel consumer)
        {
            SelectedConsumer = consumer;
            SelectedProvider = null;

            _selectedConsumerId = consumer.Id;
            _selectedProviderId = null;
        }

        public void SelectProvider(ElementViewModel provider)
        {
            SelectedConsumer = null;
            SelectedProvider = provider;

            _selectedConsumerId = null;
            _selectedProviderId = provider.Id;
        }

        public void SelectCell(ElementViewModel consumer, ElementViewModel provider)
        {
            SelectedConsumer = consumer;
            SelectedProvider = provider;

            _selectedConsumerId = consumer.Id;
            _selectedProviderId = provider.Id;
        }

        public ElementViewModel HoveredConsumer
        {
            get
            {
                return _hoveredConsumer;
            }
            private set
            {
                _hoveredConsumer = value; OnPropertyChanged();
            }
        }

        public ElementViewModel HoveredProvider
        {
            get
            {
                return _hoveredProvider;
            }
            private set
            {
                _hoveredProvider = value; OnPropertyChanged();
            }
        }

        public void HoverConsumer(ElementViewModel consumer)
        {
            HoveredConsumer = consumer;
            HoveredProvider = null;
        }

        public void HoverProvider(ElementViewModel provider)
        {
            HoveredConsumer = null;
            HoveredProvider = provider;
        }

        public void HoverCell(ElementViewModel consumer, ElementViewModel provider)
        {
            HoveredConsumer = consumer;
            HoveredProvider = provider;
        }

        public ICommand MoveCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand PartitionCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }

        public ObservableCollection<ElementTreeItemViewModel> Providers
        {
            get { return _providers; }
            private set { _providers = value; OnPropertyChanged(); }
        }

        public IList<ElementViewModel> Consumers
        {
            get { return _consumers; }
            private set { _consumers = value; OnPropertyChanged(); }
        }

        public IList<IList<CellViewModel>> Dependencies
        {
            get { return _dependencies; }
            private set { _dependencies = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ElementTreeItemViewModel> CreateProviderTree()
        {
            int depth = 0;
            var rootViewModels = new ObservableCollection<ElementTreeItemViewModel>();
            foreach (IDsmElement provider in _selectedElements)
            {
                ElementTreeItemViewModel viewModel = new ElementTreeItemViewModel(this, provider, ElementRole.Provider, depth);
                rootViewModels.Add(viewModel);
                AddProviderTreeChilderen(viewModel);
            }
            return rootViewModels;
        }

        private void AddProviderTreeChilderen(ElementTreeItemViewModel viewModel)
        {
            if (viewModel.Element.IsExpanded)
            {
                foreach (IDsmElement child in viewModel.Element.Children)
                {
                    ElementTreeItemViewModel childViewModel = new ElementTreeItemViewModel(this, child, ElementRole.Provider, viewModel.Depth + 1);
                    viewModel.Children.Add(childViewModel);
                    AddProviderTreeChilderen(childViewModel);
                }
            }
            else
            {
                viewModel.Children.Clear();
            }
        }

        private void RestoreSelections(ObservableCollection<ElementTreeItemViewModel> tree)
        {
            foreach (ElementTreeItemViewModel viewModel in tree)
            {
                if (viewModel.Id == _selectedConsumerId)
                {
                    SelectedConsumer = viewModel;
                }

                if (viewModel.Id == _selectedProviderId)
                {
                    SelectedProvider = viewModel;
                }

                RestoreSelections(viewModel.Children);
            }
        }

        private IList<ElementViewModel> FindProviderLeafElementViewModels(ObservableCollection<ElementTreeItemViewModel> tree)
        {
            List<ElementViewModel> leafElementViewModels = new List<ElementViewModel>();

            foreach (ElementTreeItemViewModel treeViewItem in tree)
            {
                FindProviderLeafElementViewModels(leafElementViewModels, treeViewItem);
            }

            return leafElementViewModels;
        }

        private void FindProviderLeafElementViewModels(IList<ElementViewModel> leafElementViewModels, ElementTreeItemViewModel treeViewItem)
        {
            if (!treeViewItem.IsExpanded)
            {
                leafElementViewModels.Add(treeViewItem);
            }

            foreach (ElementTreeItemViewModel child in treeViewItem.Children)
            {
                FindProviderLeafElementViewModels(leafElementViewModels, child);
            }
        }

        private IList<ElementViewModel> FindConsumerLeafElementViewModels(IList<ElementTreeItemViewModel> tree)
        {
            List<ElementViewModel> leafElementViewModels = new List<ElementViewModel>();

            foreach (ElementTreeItemViewModel treeViewItem in tree)
            {
                FindConsumerLeafElementViewModels(leafElementViewModels, treeViewItem);
            }

            return leafElementViewModels;
        }

        private void FindConsumerLeafElementViewModels(IList<ElementViewModel> leafElementViewModels, ElementTreeItemViewModel treeViewItem)
        {
            if (!treeViewItem.IsExpanded)
            {
                leafElementViewModels.Add(new ElementViewModel(this, treeViewItem.Element, ElementRole.Consumer, treeViewItem.Depth));
            }

            foreach (ElementTreeItemViewModel child in treeViewItem.Children)
            {
                FindConsumerLeafElementViewModels(leafElementViewModels, child);
            }
        }

        private IList<IList<CellViewModel>> UpdateCells(IList<ElementViewModel> providers, IList<ElementViewModel> consumers)
        {
            List<IList<CellViewModel>> cellViewModels = new List<IList<CellViewModel>>();

            int row = 0;
            foreach (ElementViewModel provider in providers)
            {
                int column = 0;
                List<CellViewModel> rowCellViewModels = new List<CellViewModel>();
                foreach (ElementViewModel consumer in consumers)
                {
                    int weight = _application.GetDependencyWeight(consumer.Element, provider.Element);
                    bool cyclic = _application.IsCyclicDependency(consumer.Element, provider.Element);

                    int depth = 0;
                    if (provider.Element.Id == consumer.Element.Id)
                    {
                        depth = provider.Depth;
                    }
                    else
                    {
                        if ((consumer.Element.Parent.Id == provider.Element.Parent.Id) &&
                            (consumer.Depth == provider.Depth))
                        {
                            depth = provider.Depth - 1;
                        }
                    }

                    int color = Math.Abs(depth % 4);

                    rowCellViewModels.Add(new CellViewModel(this, consumer, provider, weight, cyclic, row, column, color));
                    column++;
                }
                cellViewModels.Add(rowCellViewModels);
                row++;
            }

            return cellViewModels;
        }

        private void ShowCellConsumersExecute(object parameter)
        {
            string title = $"Consumers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var elements = _application.GetRelationConsumers(SelectedConsumer.Element, SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }


        private bool ShowCellConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellProvidersExecute(object parameter)
        {
            string title = $"Providers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var elements = _application.GetRelationProviders(SelectedConsumer.Element, SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowCellProvidersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementConsumersExecute(object parameter)
        {
            string title = $"Consumers of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementConsumers(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowProvidedInterfacesExecute(object parameter)
        {
            string title = $"Provided interface of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementProvidedElements(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }
        
        private bool ShowElementProvidedInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementRequiredInterfacesExecute(object parameter)
        {
            string title = $"Required interface of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementProviders(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowElementRequiredInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellRelationsExecute(object parameter)
        {
            string title = $"Relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var relations = _application.FindResolvedRelations(SelectedConsumer.Element, SelectedProvider.Element);

            RelationListViewModel relationsListViewModel = new RelationListViewModel(title, relations);
            _mainViewModel.NotifyRelationsReportReady(relationsListViewModel);
        }
        
        private bool ShowCellRelationsCanExecute(object parameter)
        {
            return true;
        }
    }
}
