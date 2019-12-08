using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Collections.Generic;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Lists;
using DsmSuite.DsmViewer.ViewModel.Main;

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
        private ObservableCollection<ElementTreeItemViewModel> _providers;
        private IList<ElementViewModel> _consumers;
        private IList<IList<CellViewModel>> _dependencies;

        public MatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IEnumerable<IDsmElement> selectedElements)
        {
            _mainViewModel = mainViewModel;
            _application = application;
            _selectedElements = selectedElements;

            MoveUpCommand = mainViewModel.MoveUpCommand;
            MoveDownCommand = mainViewModel.MoveDownCommand;
            PartitionCommand = mainViewModel.PartitionCommand;
            ElementInternalsMatrixCommand = mainViewModel.ElementInternalsMatrixCommand;
            ElementContextMatrixCommand = mainViewModel.ElementContextMatrixCommand;
            RelationMatrixCommand = mainViewModel.RelationMatrixCommand;

            ToggleElementExpandedCommand = mainViewModel.ToggleElementExpandedCommand;

            CellRelationsReportCommand = new RelayCommand<object>(RelationsReportExecute, RelationsReportCanExecute);
            CellConsumersReportCommand = new RelayCommand<object>(CellConsumersReportExecute, CellConsumersReportCanExecute);
            CellProvidersReportCommand = new RelayCommand<object>(CellProvidersReportExecute, CellProvidersReportCanExecute);
            ElementConsumersReportCommand = new RelayCommand<object>(ElementConsumersReportExecute, ElementConsumersReportCanExecute);
            ElementProvidedInterfacesReportCommand = new RelayCommand<object>(ElementProvidedInterfacesReportExecute, ElementProvidedInterfacesReportCanExecute);
            ElementRequiredInterfacesReportCommand = new RelayCommand<object>(ElementRequiredInterfacesReportExecute, ElementRequiredInterfacesReportCanExecute);

            Providers = CreateProviderTree();
            Update();
            ZoomLevel = 1.0;
        }

        public ICommand CellRelationsReportCommand { get; }
        public ICommand CellConsumersReportCommand { get; }
        public ICommand CellProvidersReportCommand { get; }
        public ICommand ElementConsumersReportCommand { get; }
        public ICommand ElementProvidedInterfacesReportCommand { get; }
        public ICommand ElementRequiredInterfacesReportCommand { get; }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; OnPropertyChanged(); }
        }

        public void Reload()
        {
            Providers = CreateProviderTree();
            Update();
        }
        
        public void Update()
        {
            Providers = _providers; // To trigger update tree view
            var providerLeafs = FindProviderLeafElementViewModels(Providers);
            Consumers = FindConsumerLeafElementViewModels(Providers);
            Dependencies = UpdateCells(providerLeafs, Consumers);
        }

        public ElementViewModel SelectedConsumer
        {
            get
            {
                return _selectedConsumer;
            }
            set
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
            set
            {
                _selectedProvider = value; OnPropertyChanged();
            }
        }

        public void SelectConsumer(ElementViewModel consumer)
        {
            SelectedConsumer = consumer;
            SelectedProvider = null;
        }

        public void SelectProvider(ElementViewModel provider)
        {
            SelectedConsumer = null;
            SelectedProvider = provider;
        }

        public void SelectCell(ElementViewModel consumer, ElementViewModel provider)
        {
            SelectedConsumer = consumer;
            SelectedProvider = provider;
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

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand PartitionCommand { get; }
        public ICommand ElementInternalsMatrixCommand { get; }
        public ICommand ElementContextMatrixCommand { get; }
        public ICommand RelationMatrixCommand { get; }
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
            var rows = new ObservableCollection<ElementTreeItemViewModel>();
            int depth = 0;
            foreach (IDsmElement provider in _selectedElements)
            {
                ElementTreeItemViewModel viewModel = new ElementTreeItemViewModel(this, provider, ElementRole.Provider, depth);
                rows.Add(viewModel);
            }
            return rows;
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
                leafElementViewModels.Add(new ElementViewModel(this, treeViewItem.Element, ElementRole.Consumer, treeViewItem.Element.Depth));
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
                    rowCellViewModels.Add(new CellViewModel(this, consumer, provider, weight, cyclic, row, column));
                    column++;
                }
                cellViewModels.Add(rowCellViewModels);
                row++;
            }

            return cellViewModels;
        }

        private void CellConsumersReportExecute(object parameter)
        {
            string title = $"Consumers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var elements = _application.GetRelationConsumers(SelectedConsumer.Element, SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }


        private bool CellConsumersReportCanExecute(object parameter)
        {
            return true;
        }

        private void CellProvidersReportExecute(object parameter)
        {
            string title = $"Providers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var elements = _application.GetRelationProviders(SelectedConsumer.Element, SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool CellProvidersReportCanExecute(object parameter)
        {
            return true;
        }

        private void ElementConsumersReportExecute(object parameter)
        {
            string title = $"Consumers of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementConsumers(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ElementConsumersReportCanExecute(object parameter)
        {
            return true;
        }

        private void ElementProvidedInterfacesReportExecute(object parameter)
        {
            string title = $"Provided interface of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementProvidedElements(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }
        
        private bool ElementProvidedInterfacesReportCanExecute(object parameter)
        {
            return true;
        }

        private void ElementRequiredInterfacesReportExecute(object parameter)
        {
            string title = $"Required interface of {SelectedProvider.Element.Fullname}";

            var elements = _application.GetElementProviders(SelectedProvider.Element);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ElementRequiredInterfacesReportCanExecute(object parameter)
        {
            return true;
        }

        private void RelationsReportExecute(object parameter)
        {
            string title = $"Relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            var relations = _application.FindRelations(SelectedConsumer.Element, SelectedProvider.Element);

            RelationListViewModel relationsListViewModel = new RelationListViewModel(title, relations);
            _mainViewModel.NotifyRelationsReportReady(relationsListViewModel);
        }
        
        private bool RelationsReportCanExecute(object parameter)
        {
            return true;
        }
    }
}
