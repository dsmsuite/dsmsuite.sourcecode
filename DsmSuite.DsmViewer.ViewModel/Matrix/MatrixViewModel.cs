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
        private ObservableCollection<ElementTreeItemViewModel> _providers;
        private List<ElementTreeItemViewModel> _leafViewModels;
        private int? _selectedRow;
        private int? _selectedColumn;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private int _matrixSize;
        private bool _isMetricsViewExpanded;

        private List<List<MatrixColor>> _cellColors;
        private List<List<int>> _cellWeights;
        private List<MatrixColor> _columnColors;
        private List<int> _columnElementIds;
        private List<int> _metrics;

        private int? _selectedConsumerId;
        private int? _selectedProviderId;

        public MatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IEnumerable<IDsmElement> selectedElements)
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

            ToggleMetricsViewExpandedCommand = new RelayCommand<object>(ToggleMetricsViewExpandedExecute, ToggleMetricsViewExpandedCanExecute);

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
        public ICommand ToggleMetricsViewExpandedCommand { get; }

        public int MatrixSize
        {
            get { return _matrixSize; }
            set { _matrixSize = value; OnPropertyChanged(); }
        }

        public bool IsMetricsViewExpanded
        {
            get { return _isMetricsViewExpanded; }
            set { _isMetricsViewExpanded = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ElementTreeItemViewModel> Providers
        {
            get { return _providers; }
            private set { _providers = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<MatrixColor> ColumnColors => _columnColors;
        public IReadOnlyList<int> ColumnElementIds => _columnElementIds;
        public IReadOnlyList<IList<MatrixColor>> CellColors => _cellColors;
        public IReadOnlyList<IReadOnlyList<int>> CellWeights => _cellWeights;
        public IReadOnlyList<int> Metrics => _metrics;

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set { _zoomLevel = value; OnPropertyChanged(); }
        }

        public void NavigateToSelectedElement(IDsmElement element)
        {
            ExpandElement(element);
            SelectElement(element);
        }

        private void SelectElement(IDsmElement element)
        {
            SelectElement(Providers, element);
        }

        private void SelectElement(IEnumerable<ElementTreeItemViewModel> tree, IDsmElement element)
        {
            foreach (ElementTreeItemViewModel treeItem in tree)
            {
                if (treeItem.Id == element.Id)
                {
                    SelectProviderTreeItem(treeItem);
                }
                else
                {
                    SelectElement(treeItem.Children, element);
                }
            }
        }

        private void ExpandElement(IDsmElement element)
        {
            IDsmElement current = element.Parent;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.Parent;
            }
            Reload();
        }

        public void Reload()
        {
            BackupSelectionBeforeReload();
            Providers = CreateProviderTree();
            _leafViewModels = FindLeafElements();
            DefineColumnColors();
            DefineColumnContent();
            DefineCellColors();
            DefineCellContent();
            DefineMetrics();
            MatrixSize = _leafViewModels.Count;
            RestoreSelectionAfterReload();
        }

        public void SelectRow(int? row)
        {
            SelectedRow = row;
            SelectedColumn = null;
        }

        public void SelectColumn(int? column)
        {
            SelectedRow = null;
            SelectedColumn = column;
        }

        private string _columnHeaderTooltip;

        public string ColumnHeaderTooltip
        {
            get { return _columnHeaderTooltip; }
            set { _columnHeaderTooltip = value; OnPropertyChanged(); }
        }

        private string _cellTooltip;

        public string CellTooltip
        {
            get { return _cellTooltip; }
            set { _cellTooltip = value; OnPropertyChanged(); }
        }

        public void SelectCell(int? row, int? columnn)
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

        public void HoverRow(int? row)
        {
            HoveredRow = row;
            HoveredColumn = null;
        }

        public void HoverColumn(int? column)
        {
            HoveredRow = null;
            HoveredColumn = column;
            UpdateColumnHeaderTooltip(column);
        }

        public void HoverCell(int? row, int? columnn)
        {
            HoveredRow = row;
            HoveredColumn = columnn;
            UpdateCellTooltip(row, columnn);
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

        public IDsmElement SelectedConsumer
        {
            get
            {
                IDsmElement selectedConsumer = null;
                if (SelectedColumn.HasValue)
                {
                    selectedConsumer = _leafViewModels[SelectedColumn.Value].Element;
                }
                return selectedConsumer;
            }
        }

        public IDsmElement SelectedProvider => SelectedProviderTreeItem?.Element;

        private ElementTreeItemViewModel _selectedProviderTreeItem;

        public void SelectProviderTreeItem(ElementTreeItemViewModel selectedProviderTreeItem)
        {
            SelectCell(null, null);
            for (int row = 0; row < _leafViewModels.Count; row++)
            {
                if (_leafViewModels[row] == selectedProviderTreeItem)
                {
                    SelectRow(row);
                }
            }
            _selectedProviderTreeItem = selectedProviderTreeItem;
        }

        public ElementTreeItemViewModel SelectedProviderTreeItem
        {
            get
            {
                ElementTreeItemViewModel selectedProviderTreeItem;
                if (SelectedRow.HasValue && (SelectedRow.Value < _leafViewModels.Count))
                {
                    selectedProviderTreeItem = _leafViewModels[SelectedRow.Value];
                }
                else
                {
                    selectedProviderTreeItem = _selectedProviderTreeItem;
                }
                return selectedProviderTreeItem;
            }
        }

        private ElementTreeItemViewModel _hoveredProviderTreeItem;

        public void HoverProviderTreeItem(ElementTreeItemViewModel hoveredProviderTreeItem)
        {
            HoverCell(null, null);
            for (int row = 0; row < _leafViewModels.Count; row++)
            {
                if (_leafViewModels[row] == hoveredProviderTreeItem)
                {
                    HoverRow(row);
                }
            }
            _hoveredProviderTreeItem = hoveredProviderTreeItem;
        }

        public ElementTreeItemViewModel HoveredProviderTreeItem => HoveredRow.HasValue ? _leafViewModels[HoveredRow.Value] : _hoveredProviderTreeItem;

        private ObservableCollection<ElementTreeItemViewModel> CreateProviderTree()
        {
            int depth = 0;
            var rootViewModels = new ObservableCollection<ElementTreeItemViewModel>();
            foreach (IDsmElement provider in _selectedElements)
            {
                ElementTreeItemViewModel viewModel = new ElementTreeItemViewModel(this, provider, depth);
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
                    ElementTreeItemViewModel childViewModel = new ElementTreeItemViewModel(this, child, viewModel.Depth + 1);
                    viewModel.Children.Add(childViewModel);
                    AddProviderTreeChilderen(childViewModel);
                }
            }
            else
            {
                viewModel.Children.Clear();
            }
        }

        private List<ElementTreeItemViewModel> FindLeafElements()
        {
            List<ElementTreeItemViewModel> leafElements = new List<ElementTreeItemViewModel>();

            foreach (ElementTreeItemViewModel selectedElement in Providers)
            {
                FindLeafElements(leafElements, selectedElement);
            }

            return leafElements;
        }

        private void FindLeafElements(List<ElementTreeItemViewModel> leafElements, ElementTreeItemViewModel element)
        {
            if (!element.IsExpanded)
            {
                leafElements.Add(element);
            }

            foreach (ElementTreeItemViewModel childElement in element.Children)
            {
                FindLeafElements(leafElements, childElement);
            }
        }

        private void DefineCellColors()
        {
            int size = _leafViewModels.Count;

            _cellColors = new List<List<MatrixColor>>();

            // Define background color
            for (int row = 0; row < size; row++)
            {
                _cellColors.Add(new List<MatrixColor>());
                for (int column = 0; column < size; column++)
                {
                    _cellColors[row].Add(MatrixColor.Background);
                }
            }

            // Define expanded block color
            for (int row = 0; row < size; row++)
            {
                IDsmElement element = _leafViewModels[row].Element;

                if (_application.IsFirstChild(element))
                {
                    int leafElements = 0;
                    CountLeafElements(element.Parent, ref leafElements);

                    if (leafElements > 0)
                    {
                        int parentDepth = _leafViewModels[row].Depth - 1;
                        MatrixColor expandedColor = MatrixColorConverter.GetColor(parentDepth);

                        int begin = row;
                        int end = row + leafElements;

                        for (int i = begin; i < end; i++)
                        {
                            for (int columnDelta = begin; columnDelta < end; columnDelta++)
                            {
                                _cellColors[i][columnDelta] = expandedColor;
                            }
                        }
                    }
                }
            }

            // Define diagonal color
            for (int row = 0; row < size; row++)
            {
                int depth = _leafViewModels[row].Depth;
                MatrixColor dialogColor = MatrixColorConverter.GetColor(depth);
                _cellColors[row][row] = dialogColor;
            }

            // Define cycle color
            for (int row = 0; row < size; row++)
            {
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _leafViewModels[column].Element;
                    IDsmElement provider = _leafViewModels[row].Element;
                    if (_application.IsCyclicDependency(consumer, provider) && _application.ShowCycles)
                    {
                        _cellColors[row][column] = MatrixColor.Highlight;
                    }
                }
            }
        }

        private void CountLeafElements(IDsmElement element, ref int count)
        {
            if (!element.IsExpanded)
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
            _columnColors = new List<MatrixColor>();
            foreach (ElementTreeItemViewModel provider in _leafViewModels)
            {
                _columnColors.Add(provider.Color);
            }
        }

        private void DefineColumnContent()
        {
            _columnElementIds = new List<int>();
            foreach (ElementTreeItemViewModel provider in _leafViewModels)
            {
                _columnElementIds.Add(provider.Element.Order);
            }
        }

        private void DefineCellContent()
        {
            _cellWeights = new List<List<int>>();
            int size = _leafViewModels.Count;

            for (int row = 0; row < size; row++)
            {
                _cellWeights.Add(new List<int>());
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _leafViewModels[column].Element;
                    IDsmElement provider = _leafViewModels[row].Element;
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineMetrics()
        {
            _metrics = new List<int>();
            foreach (ElementTreeItemViewModel provider in _leafViewModels)
            {
                int size = _application.GetElementSize(provider.Element);
                _metrics.Add(size);
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

        private void ToggleMetricsViewExpandedExecute(object parameter)
        {
            IsMetricsViewExpanded = !IsMetricsViewExpanded;
        }

        private bool ToggleMetricsViewExpandedCanExecute(object parameter)
        {
            return true;
        }

        private void BackupSelectionBeforeReload()
        {
            _selectedConsumerId = SelectedConsumer?.Id;
            _selectedProviderId = SelectedProvider?.Id;
        }

        private void RestoreSelectionAfterReload()
        {
            for (int i = 0; i < _leafViewModels.Count; i++)
            {
                if (_selectedProviderId.HasValue && (_selectedProviderId.Value == _leafViewModels[i].Id))
                {
                    SelectRow(i);
                }

                if (_selectedConsumerId.HasValue && (_selectedConsumerId.Value == _leafViewModels[i].Id))
                {
                    SelectColumn(i);
                }
            }
        }

        private void UpdateColumnHeaderTooltip(int? column)
        {
            if (column.HasValue)
            {
                IDsmElement element = _leafViewModels[column.Value].Element;
                if (element != null)
                {
                    ColumnHeaderTooltip = $"[{element.Order}] {element.Fullname}";
                }
            }
        }

        private void UpdateCellTooltip(int? row, int? column)
        {
            if (row.HasValue && column.HasValue)
            {
                IDsmElement consumer = _leafViewModels[column.Value].Element;
                IDsmElement provider = _leafViewModels[row.Value].Element;

                if ((consumer != null) && (provider != null))
                {
                    int weight = _application.GetDependencyWeight(consumer, provider);

                    CellTooltip = $"[{consumer.Order}] {consumer.Fullname} to [{consumer.Order}] {consumer.Fullname} weight={weight}";
                }
            }
        }
    }
}
