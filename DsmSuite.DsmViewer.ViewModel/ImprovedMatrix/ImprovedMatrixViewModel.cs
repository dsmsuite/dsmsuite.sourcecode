using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Lists;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.ViewModel.ImprovedMatrix
{
    public class ImprovedMatrixViewModel : ViewModelBase
    {
        private double _zoomLevel;
        private readonly IMainViewModel _mainViewModel;
        private readonly IDsmApplication _application;
        private readonly IList<IDsmElement> _selectedElements;
        private ObservableCollection<MatrixElementViewModel> _providers;
        private List<MatrixElementViewModel> _leafElements;
        private List<List<DsmColor>> _cellColors;
        private List<List<int>> _cellWeights;
        private List<int> _columnElementIds;
        private List<DsmColor> _columnColors;
        private int? _selectedRow;
        private int? _selectedColumn;
        private int? _hoveredRow;
        private int? _hoveredColumn;

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
            DefineColumnColors();
            DefineColumnContent();
            DefineCellColors();
            DefineCellContent();
        }


        public ObservableCollection<MatrixElementViewModel> Providers
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

        public void SelectRow(int row)
        {
            SelectedRow = row;
            SelectedColumn = null;
        }

        public void SelectColumn(int column)
        {
            SelectedRow = null;
            SelectedColumn = column;
        }

        public void SelectCell(int row, int columnn)
        {
            SelectedRow = row;
            SelectedColumn = columnn;
        }

        public int? SelectedRow
        {
            get { return _selectedRow; }
            private set { _selectedRow = value; OnPropertyChanged(); }
        }

        public int? SelectedColumn
        {
            get { return _selectedColumn; }
            private set { _selectedColumn = value; OnPropertyChanged(); }
        }

        public void HoverRow(int row)
        {
            HoveredRow = row;
            HoveredColumn = null;
        }

        public void HoverColumn(int column)
        {
            HoveredRow = null;
            HoveredColumn = column;
        }

        public void HoverCell(int row, int columnn)
        {
            HoveredRow = row;
            HoveredColumn = columnn;
        }

        public int? HoveredRow
        {
            get { return _hoveredRow; }
            private set { _hoveredRow = value; OnPropertyChanged(); }
        }

        public int? HoveredColumn
        {
            get { return _hoveredColumn; }
            private set { _hoveredColumn = value; OnPropertyChanged(); }
        }

        private IDsmElement SelectedConsumer
        {
            get
            {
                IDsmElement selectedConsumer = null;
                if (_selectedColumn.HasValue)

                {
                    selectedConsumer = _leafElements[_selectedColumn.Value].Element;
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
                    selectedProvider = _leafElements[_selectedRow.Value].Element;
                }
                return selectedProvider;
            }
        }

        public IReadOnlyList<IList<DsmColor>> CellColors => _cellColors;

        public IReadOnlyList<IReadOnlyList<int>> CellWeights => _cellWeights;

        public IReadOnlyList<int> ColumnElementIds => _columnElementIds;

        public IReadOnlyList<DsmColor> ColumnColors => _columnColors;

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
                    IDsmElement consumer = _leafElements[column].Element;
                    IDsmElement provider = _leafElements[row].Element;
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineCellColors()
        {
            int size = _leafElements.Count;

            _cellColors = new List<List<DsmColor>>();

            // Define back ground color
            for (int row = 0; row < size; row++)
            {
                _cellColors.Add(new List<DsmColor>());
                for (int column = 0; column < size; column++)
                {
                    _cellColors[row].Add(DsmColor.Background);
                }
            }

            // Define expanded block color
            int d = 0;
            for (int row = 0; row < size; row++)
            {
                int leafElements = 0;
                CountLeafElements(_leafElements[row].Element, ref leafElements);

                if (leafElements > 0)
                {

                    for (int rowDelta = 0; rowDelta < leafElements; rowDelta++)
                    {
                        for (int columnDelta = 0; columnDelta < leafElements; columnDelta++)
                        {
                            _cellColors[row + rowDelta][row + columnDelta] = GetRowColor(_leafElements[row].Depth - 1);
                        }
                    }
                }
            }

            // Define diagonal color
            for (int row = 0; row < size; row++)
            {
                _cellColors[row][row] = GetRowColor(_leafElements[row].Depth);
            }

            // Define cycle color
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _leafElements[column].Element;
                    IDsmElement provider = _leafElements[row].Element;
                    if (_application.IsCyclicDependency(consumer, provider) && _application.ShowCycles)
                    {
                        _cellColors[row][column] = DsmColor.AccentColor;
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


        private void DefineColumnColors()
        {
            _columnColors = new List<DsmColor>();
            foreach (MatrixElementViewModel provider in _leafElements)
            {
                _columnColors.Add(provider.Color);
            }
        }

        private void DefineColumnContent()
        {
            _columnElementIds = new List<int>();
            foreach (MatrixElementViewModel provider in _leafElements)
            {
                _columnElementIds.Add(provider.Element.Id);
            }
        }

        private List<MatrixElementViewModel> FindLeafElements()
        {
            List<MatrixElementViewModel> leafElements = new List<MatrixElementViewModel>();

            foreach (MatrixElementViewModel selectedElement in Providers)
            {
                FindLeafElements(leafElements, selectedElement);
            }

            return leafElements;
        }

        private void FindLeafElements(List<MatrixElementViewModel> leafElements, MatrixElementViewModel element)
        {
            if (!element.IsExpanded)
            {
                leafElements.Add(element);
            }

            foreach (MatrixElementViewModel childElement in element.Children)
            {
                FindLeafElements(leafElements, childElement);
            }
        }

        private ObservableCollection<MatrixElementViewModel> CreateProviderTree()
        {
            int depth = 0;
            var rootViewModels = new ObservableCollection<MatrixElementViewModel>();
            foreach (IDsmElement provider in _selectedElements)
            {
                DsmColor color = GetRowColor(depth);
                MatrixElementViewModel viewModel = new MatrixElementViewModel(this, provider, depth, color);
                rootViewModels.Add(viewModel);
                AddProviderTreeChilderen(viewModel);
            }
            return rootViewModels;
        }

        private static DsmColor GetRowColor(int depth)
        {
            DsmColor color;
            switch (depth % 4)
            {
                case 0:
                    color = DsmColor.StandardColor1;
                    break;
                case 1:
                    color = DsmColor.StandardColor2;
                    break;
                case 2:
                    color = DsmColor.StandardColor3;
                    break;
                case 3:
                    color = DsmColor.StandardColor4;
                    break;
                default:
                    color = DsmColor.Background;
                    break;
            }
            return color;
        }

        private void AddProviderTreeChilderen(MatrixElementViewModel viewModel)
        {
            if (viewModel.IsExpanded)
            {
                foreach (IDsmElement child in viewModel.Element.Children)
                {
                    int depth = viewModel.Depth + 1;
                    DsmColor color = GetRowColor(depth);
                    MatrixElementViewModel childViewModel = new MatrixElementViewModel(this, child, depth, color);
                    viewModel.Children.Add(childViewModel);
                    AddProviderTreeChilderen(childViewModel);
                }
            }
            else
            {
                viewModel.Children.Clear();
            }
        }
    }
}
