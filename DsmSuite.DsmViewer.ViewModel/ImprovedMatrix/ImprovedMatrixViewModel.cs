using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Lists;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.ViewModel.ImprovedMatrix
{
    public class ImprovedMatrixViewModel : ViewModelBase
    {
        private double _zoomLevel;
        private readonly IMainViewModel _mainViewModel;
        private readonly IDsmApplication _application;
        private readonly IList<IDsmElement> _selectedElements;
        private ObservableCollection<ImprovedElementTreeItemViewModel> _providers;
        private IList<IDsmElement> _leafElements;
        private IList<List<Color>> _cellColors;
        private IList<List<int>> _cellWeights;
        private int? _selectedRow;
        private int? _selectedColumn;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private int _matrixSize;

        public ImprovedMatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IList<IDsmElement> selectedElements)
        {
            _mainViewModel = mainViewModel;
            _application = application;
            _selectedElements = selectedElements;

            ToggleElementExpandedCommand = mainViewModel.ToggleElementExpandedCommand;

            ChangeElementParentCommand = mainViewModel.MoveElementCommand;
            MoveUpElementCommand = mainViewModel.MoveUpElementCommand;
            MoveDownElementCommand = mainViewModel.MoveDownElementCommand;
            SortElementCommand = mainViewModel.SortElementCommand;

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
            ChangeElementNameCommand = mainViewModel.ChangeElementNameCommand;
            ChangeElementTypeCommand = mainViewModel.ChangeElementTypeCommand;

            CreateRelationCommand = mainViewModel.CreateRelationCommand;
            DeleteRelationCommand = mainViewModel.DeleteRelationCommand;
            ChangeRelationWeightCommand = mainViewModel.ChangeRelationWeightCommand;
            ChangeRelationTypeCommand = mainViewModel.ChangeRelationTypeCommand;

            Reload();

            ZoomLevel = 1.0;

        }

        public void Reload()
        {
            Providers = CreateProviderTree();
            _leafElements = FindLeafElements();

            DefineCellColors();
            DefineCellContent();
        }

        private IList<IDsmElement> FindLeafElements()
        {
            List<IDsmElement> leafElements = new List<IDsmElement>();

            foreach (IDsmElement selectedElement in _selectedElements)
            {
                FindLeafElements(leafElements, selectedElement);
            }

            return leafElements;
        }

        private void FindLeafElements(IList<IDsmElement> leafElements, IDsmElement element)
        {
            if (!element.IsExpanded)
            {
                leafElements.Add(element);
            }

            foreach (IDsmElement childElement in element.Children)
            {
                FindLeafElements(leafElements, childElement);
            }
        }

        private ObservableCollection<ImprovedElementTreeItemViewModel> CreateProviderTree()
        {
            int depth = 0;
            var rootViewModels = new ObservableCollection<ImprovedElementTreeItemViewModel>();
            foreach (IDsmElement provider in _selectedElements)
            {
                ImprovedElementTreeItemViewModel viewModel = new ImprovedElementTreeItemViewModel(this, provider, ElementRole.Provider, depth);
                rootViewModels.Add(viewModel);
                AddProviderTreeChilderen(viewModel);
            }
            return rootViewModels;
        }

        private void AddProviderTreeChilderen(ImprovedElementTreeItemViewModel viewModel)
        {
            if (viewModel.IsExpanded)
            {
                foreach (IDsmElement child in viewModel.Element.Children)
                {
                    ImprovedElementTreeItemViewModel childViewModel = new ImprovedElementTreeItemViewModel(this, child, ElementRole.Provider, viewModel.Depth + 1);
                    viewModel.Children.Add(childViewModel);
                    AddProviderTreeChilderen(childViewModel);
                }
            }
            else
            {
                viewModel.Children.Clear();
            }
        }

        public ObservableCollection<ImprovedElementTreeItemViewModel> Providers
        {
            get { return _providers; }
            private set { _providers = value; OnPropertyChanged(); }
        }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            private set { _zoomLevel = value; OnPropertyChanged(); }
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
        public ICommand ChangeElementNameCommand { get; }
        public ICommand ChangeElementTypeCommand { get; }
        public ICommand CreateRelationCommand { get; }
        public ICommand DeleteRelationCommand { get; }
        public ICommand ChangeRelationWeightCommand { get; }
        public ICommand ChangeRelationTypeCommand { get; }
        public ICommand ChangeElementParentCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }

        public int? SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; OnPropertyChanged(); }
        }

        public int? SelectedColumn
        {
            get { return _selectedColumn; }
            set { _selectedColumn = value; OnPropertyChanged(); }
        }

        public int? HoveredRow
        {
            get { return _hoveredRow; }
            set { _hoveredRow = value; OnPropertyChanged(); }
        }

        public int? HoveredColumn
        {
            get { return _hoveredColumn; }
            set { _hoveredColumn = value; OnPropertyChanged(); }
        }

        private IDsmElement SelectedConsumer
        {
            get
            {
                IDsmElement selectedConsumer = null;
                if (_selectedColumn.HasValue)

                {
                    selectedConsumer = _leafElements[_selectedColumn.Value];
                }
                return selectedConsumer;
            }
        }

        private IDsmElement SelectedProvider
        {
            get
            {
                IDsmElement selectedProvider = null;
                if (_selectedRow.HasValue)

                {
                    selectedProvider = _leafElements[_selectedRow.Value];
                }
                return selectedProvider;
            }
        }

        private void ShowCellConsumersExecute(object parameter)
        {
            string title = $"Consumers in relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";

            var elements = _application.GetRelationConsumers(SelectedConsumer, SelectedProvider);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }


        private bool ShowCellConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellProvidersExecute(object parameter)
        {
            string title = $"Providers in relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";

            var elements = _application.GetRelationProviders(SelectedConsumer, SelectedProvider);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowCellProvidersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementConsumersExecute(object parameter)
        {
            string title = $"Consumers of {SelectedProvider.Fullname}";

            var elements = _application.GetElementConsumers(SelectedProvider);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowProvidedInterfacesExecute(object parameter)
        {
            string title = $"Provided interface of {SelectedProvider.Fullname}";

            var elements = _application.GetElementProvidedElements(SelectedProvider);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowElementProvidedInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementRequiredInterfacesExecute(object parameter)
        {
            string title = $"Required interface of {SelectedProvider.Fullname}";

            var elements = _application.GetElementProviders(SelectedProvider);

            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            _mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowElementRequiredInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellRelationsExecute(object parameter)
        {
            string title = $"Relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";

            var relations = _application.FindResolvedRelations(SelectedConsumer, SelectedProvider);

            RelationListViewModel relationsListViewModel = new RelationListViewModel(title, relations);
            _mainViewModel.NotifyRelationsReportReady(relationsListViewModel);
        }

        private bool ShowCellRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void DefineCellContent()
        {
            _cellWeights = new List<List<int>>();
            int size = _leafElements.Count;

            for (int row = 0; row < size; row++)
            {
                _cellWeights.Add(new List<int>());
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _leafElements[column];
                    IDsmElement provider = _leafElements[row];
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineCellColors()
        {
            int size = _leafElements.Count;

            _cellColors = new List<List<Color>>();


            // Define back ground color
            for (int row = 0; row < size; row++)
            {
                _cellColors.Add(new List<Color>());
                for (int column = 0; column < size; column++)
                {
                    _cellColors[row].Add(Colors.White);
                }
            }

            // Define expanded block color
            int d = 0;
            for (int row = 0; row < size; row++)
            {
                int depth = 0;

                if (depth > d)
                {
                    int leafElements = 0;

                    CountLeafElements(_leafElements[row], ref leafElements);
                    for (int rowDelta = 0; rowDelta < leafElements; rowDelta++)
                    {
                        for (int columnDelta = 0; columnDelta < leafElements; columnDelta++)
                        {
                            _cellColors[row + rowDelta][row + columnDelta] = Colors.Red;
                        }
                    }
                }
                d = depth;
            }

            // Define dialog color
            for (int row = 0; row < size; row++)
            {
                _cellColors[row][row] = Colors.Yellow;
            }

            // Define cycle color
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _leafElements[column];
                    IDsmElement provider = _leafElements[row];
                    if (_application.IsCyclicDependency(consumer, provider) && _application.ShowCycles)
                    {
                        _cellColors[row][column] = Colors.Green;
                    }
                }
            }

        }

        private void CountLeafElements(IDsmElement element, ref int count)
        {
            if ((element.Children.Count == 0) || !element.IsExpanded)
            {
                count++;
            }
            else
            {
                foreach (IDsmElement child in element.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }
        }


    }
}
