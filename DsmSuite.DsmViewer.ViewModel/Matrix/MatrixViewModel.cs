using System.Collections.ObjectModel;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class MatrixViewModel : ViewModelBase, IMatrixViewModel
    {
        private double _zoomLevel;
        private readonly IMainViewModel _mainViewModel;
        private readonly IDsmApplication _application;
        private readonly IEnumerable<IDsmElement> _selectedElements;
        private ObservableCollection<ElementTreeItemViewModel> _elementViewModelTree;
        private List<ElementTreeItemViewModel> _elementViewModelLeafs;
        private ElementTreeItemViewModel _selectedTreeItem;
        private ElementTreeItemViewModel _hoveredTreeItem;
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
        private List<string> _metrics;
        private List<bool> _rowIsProvider;
        private List<bool> _rowIsConsumer;
        private List<bool> _rowIsMatch;
        private int? _selectedConsumerId;
        private int? _selectedProviderId;

        private string _columnHeaderTooltip;
        private string _cellTooltip;

        private readonly Dictionary<MetricType, string> _metricTypeNames;
        private string _selectedMetricTypeName;
        private MetricType _selectedMetricType;
        private string _searchText = "";

        public MatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IEnumerable<IDsmElement> selectedElements)
        {
            _mainViewModel = mainViewModel;
            _application = application;
            _selectedElements = selectedElements;

            ToggleElementExpandedCommand = mainViewModel.ToggleElementExpandedCommand;

            SortElementCommand = mainViewModel.SortElementCommand;
            MoveUpElementCommand = mainViewModel.MoveUpElementCommand;
            MoveDownElementCommand = mainViewModel.MoveDownElementCommand;

            CreateElementCommand = mainViewModel.CreateElementCommand;
            ChangeElementNameCommand = mainViewModel.ChangeElementNameCommand;
            ChangeElementTypeCommand = mainViewModel.ChangeElementTypeCommand;
            ChangeElementParentCommand = mainViewModel.ChangeElementParentCommand;
            DeleteElementCommand = mainViewModel.DeleteElementCommand;

            ShowElementIngoingRelationsCommand = new RelayCommand<object>(ShowElementIngoingRelationsExecute, ShowElementIngoingRelationsCanExecute);
            ShowElementOutgoingRelationCommand = new RelayCommand<object>(ShowElementOutgoingRelationExecute, ShowElementOutgoingRelationCanExecute);
            ShowElementinternalRelationsCommand = new RelayCommand<object>(ShowElementinternalRelationsExecute, ShowElementinternalRelationsCanExecute);

            ShowElementConsumersCommand = new RelayCommand<object>(ShowElementConsumersExecute, ShowConsumersCanExecute);
            ShowElementProvidedInterfacesCommand = new RelayCommand<object>(ShowProvidedInterfacesExecute, ShowElementProvidedInterfacesCanExecute);
            ShowElementRequiredInterfacesCommand = new RelayCommand<object>(ShowElementRequiredInterfacesExecute, ShowElementRequiredInterfacesCanExecute);
            ShowCellDetailMatrixCommand = mainViewModel.ShowCellDetailMatrixCommand;

            CreateRelationCommand = mainViewModel.CreateRelationCommand;
            ChangeRelationWeightCommand = mainViewModel.ChangeRelationWeightCommand;
            ChangeRelationTypeCommand = mainViewModel.ChangeRelationTypeCommand;
            DeleteRelationCommand = mainViewModel.DeleteRelationCommand;

            ShowCellRelationsCommand = new RelayCommand<object>(ShowCellRelationsExecute, ShowCellRelationsCanExecute);
            ShowCellConsumersCommand = new RelayCommand<object>(ShowCellConsumersExecute, ShowCellConsumersCanExecute);
            ShowCellProvidersCommand = new RelayCommand<object>(ShowCellProvidersExecute, ShowCellProvidersCanExecute);
            ShowElementDetailMatrixCommand = mainViewModel.ShowElementDetailMatrixCommand;
            ShowElementContextMatrixCommand = mainViewModel.ShowElementContextMatrixCommand;

            ToggleMetricsViewExpandedCommand = new RelayCommand<object>(ToggleMetricsViewExpandedExecute, ToggleMetricsViewExpandedCanExecute);

            PreviousMetricCommand = new RelayCommand<object>(PreviousMetricExecute, PreviousMetricCanExecute);
            NextMetricCommand = new RelayCommand<object>(NextMetricExecute, NextMetricCanExecute);

            Reload();

            ZoomLevel = 1.0;

            _metricTypeNames = new Dictionary<MetricType, string>();
            _metricTypeNames[MetricType.NumberOfElements] = "Internal Elements";
            _metricTypeNames[MetricType.IngoingRelations] = "Ingoing Relations";
            _metricTypeNames[MetricType.OutgoingRelations] = "Outgoing Relations";
            _metricTypeNames[MetricType.InternalRelations] = "Internal Relations";
            _metricTypeNames[MetricType.HierarchicalCycles] = "Hierarchical Cycles";
            _metricTypeNames[MetricType.SystemCycles] = "System Cycles";

            _selectedMetricType = MetricType.NumberOfElements;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        public int HighlighMatchingElements(string searchText)
        {
            _searchText = searchText;
            return UpdateMatchingRows();
        }

        public ICommand ToggleElementExpandedCommand { get; }

        public ICommand SortElementCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }

        public ICommand CreateElementCommand { get; }
        public ICommand ChangeElementNameCommand { get; }
        public ICommand ChangeElementTypeCommand { get; }
        public ICommand ChangeElementParentCommand { get; }
        public ICommand DeleteElementCommand { get; }

        public ICommand ShowElementIngoingRelationsCommand { get; }
        public ICommand ShowElementOutgoingRelationCommand { get; }
        public ICommand ShowElementinternalRelationsCommand { get; }

        public ICommand ShowElementConsumersCommand { get; }
        public ICommand ShowElementProvidedInterfacesCommand { get; }
        public ICommand ShowElementRequiredInterfacesCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }

        public ICommand CreateRelationCommand { get; }
        public ICommand ChangeRelationWeightCommand { get; }
        public ICommand ChangeRelationTypeCommand { get; }
        public ICommand DeleteRelationCommand { get; }

        public ICommand ShowCellRelationsCommand { get; }
        public ICommand ShowCellConsumersCommand { get; }
        public ICommand ShowCellProvidersCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }

        public ICommand PreviousMetricCommand { get; }
        public ICommand NextMetricCommand { get; }

        public ICommand ToggleMetricsViewExpandedCommand { get; }

        public string SelectedMetricTypeName
        {
            get { return _selectedMetricTypeName; }
            set
            {
                _selectedMetricTypeName = value;
                _selectedMetricType = _metricTypeNames.FirstOrDefault(x => x.Value == _selectedMetricTypeName).Key;
                Reload();
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<ElementTreeItemViewModel> ElementViewModelTree
        {
            get { return _elementViewModelTree; }
            private set { _elementViewModelTree = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<MatrixColor> ColumnColors => _columnColors;
        public IReadOnlyList<int> ColumnElementIds => _columnElementIds;
        public IReadOnlyList<IList<MatrixColor>> CellColors => _cellColors;
        public IReadOnlyList<IReadOnlyList<int>> CellWeights => _cellWeights;
        public IReadOnlyList<string> Metrics => _metrics;
        public IReadOnlyList<bool> RowIsProvider => _rowIsProvider;
        public IReadOnlyList<bool> RowIsConsumer => _rowIsConsumer;
        public IReadOnlyList<bool> RowIsMatch => _rowIsMatch;

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

        public void Reload()
        {
            BackupSelectionBeforeReload();
            ElementViewModelTree = CreateElementViewModelTree();
            _elementViewModelLeafs = FindLeafElementViewModels();
            DefineProviderRows();
            DefineConsumerRows();
            DefineMatchingRows();
            DefineColumnColors();
            DefineColumnContent();
            DefineCellColors();
            DefineCellContent();
            DefineMetrics();
            MatrixSize = _elementViewModelLeafs.Count;
            RestoreSelectionAfterReload();

            UpdateMatchingRows();
        }

        public void SelectTreeItem(ElementTreeItemViewModel selectedTreeItem)
        {
            SelectCell(null, null);
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                if (_elementViewModelLeafs[row] == selectedTreeItem)
                {
                    SelectRow(row);
                }
            }
            _selectedTreeItem = selectedTreeItem;
        }

        public ElementTreeItemViewModel SelectedTreeItem
        {
            get
            {
                ElementTreeItemViewModel selectedTreeItem;
                if (SelectedRow.HasValue && (SelectedRow.Value < _elementViewModelLeafs.Count))
                {
                    selectedTreeItem = _elementViewModelLeafs[SelectedRow.Value];
                }
                else
                {
                    selectedTreeItem = _selectedTreeItem;
                }
                return selectedTreeItem;
            }
        }

        public void HoverTreeItem(ElementTreeItemViewModel hoveredTreeItem)
        {
            HoverCell(null, null);
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                if (_elementViewModelLeafs[row] == hoveredTreeItem)
                {
                    HoverRow(row);
                }
            }
            _hoveredTreeItem = hoveredTreeItem;
        }

        public ElementTreeItemViewModel HoveredTreeItem
        {
            get
            {
                ElementTreeItemViewModel hoveredTreeItem;
                if (HoveredRow.HasValue && (HoveredRow.Value < _elementViewModelLeafs.Count))
                {
                    hoveredTreeItem = _elementViewModelLeafs[HoveredRow.Value];
                }
                else
                {
                    hoveredTreeItem = _hoveredTreeItem;
                }
                return hoveredTreeItem;
            }
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
            UpdateProviderRows();
            UpdateConsumerRows();
        }

        public void HoverColumn(int? column)
        {
            HoveredRow = null;
            HoveredColumn = column;
            UpdateColumnHeaderTooltip(column);
            UpdateProviderRows();
            UpdateConsumerRows();
        }

        public void HoverCell(int? row, int? columnn)
        {
            HoveredRow = row;
            HoveredColumn = columnn;
            UpdateCellTooltip(row, columnn);
            UpdateProviderRows();
            UpdateConsumerRows();
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
                    selectedConsumer = _elementViewModelLeafs[SelectedColumn.Value].Element;
                }
                return selectedConsumer;
            }
        }

        public IDsmElement SelectedProvider => SelectedTreeItem?.Element;

        public string ColumnHeaderTooltip
        {
            get { return _columnHeaderTooltip; }
            set { _columnHeaderTooltip = value; OnPropertyChanged(); }
        }

        public string CellTooltip
        {
            get { return _cellTooltip; }
            set { _cellTooltip = value; OnPropertyChanged(); }
        }

        public IEnumerable<string> MetricTypes
        {
            get
            {
                return _metricTypeNames.Values;
            }
        }

        private ObservableCollection<ElementTreeItemViewModel> CreateElementViewModelTree()
        {
            int depth = 0;
            ObservableCollection<ElementTreeItemViewModel> tree = new ObservableCollection<ElementTreeItemViewModel>();
            foreach (IDsmElement element in _selectedElements)
            {
                ElementTreeItemViewModel viewModel = new ElementTreeItemViewModel(this, element, depth);
                tree.Add(viewModel);
                AddElementViewModelChildren(viewModel);
            }
            return tree;
        }

        private void AddElementViewModelChildren(ElementTreeItemViewModel viewModel)
        {
            if (viewModel.Element.IsExpanded)
            {
                foreach (IDsmElement child in viewModel.Element.Children)
                {
                    ElementTreeItemViewModel childViewModel = new ElementTreeItemViewModel(this, child, viewModel.Depth + 1);
                    viewModel.AddChild(childViewModel);
                    AddElementViewModelChildren(childViewModel);
                }
            }
            else
            {
                viewModel.ClearChildren();
            }
        }

        private List<ElementTreeItemViewModel> FindLeafElementViewModels()
        {
            List<ElementTreeItemViewModel> leafViewModels = new List<ElementTreeItemViewModel>();

            foreach (ElementTreeItemViewModel viewModel in ElementViewModelTree)
            {
                FindLeafElementViewModels(leafViewModels, viewModel);
            }

            return leafViewModels;
        }

        private void FindLeafElementViewModels(List<ElementTreeItemViewModel> leafViewModels, ElementTreeItemViewModel viewModel)
        {
            if (!viewModel.IsExpanded)
            {
                leafViewModels.Add(viewModel);
            }

            foreach (ElementTreeItemViewModel childViewModel in viewModel.Children)
            {
                FindLeafElementViewModels(leafViewModels, childViewModel);
            }
        }

        private void DefineCellColors()
        {
            int matrixSize = _elementViewModelLeafs.Count;

            _cellColors = new List<List<MatrixColor>>();

            // Define background color
            for (int row = 0; row < matrixSize; row++)
            {
                _cellColors.Add(new List<MatrixColor>());
                for (int column = 0; column < matrixSize; column++)
                {
                    _cellColors[row].Add(MatrixColor.Background);
                }
            }

            // Define expanded block color
            for (int row = 0; row < matrixSize; row++)
            {
                ElementTreeItemViewModel viewModel = _elementViewModelLeafs[row];
                IDsmElement element = viewModel.Element;

                Stack<ElementTreeItemViewModel> viewModelHierarchy = new Stack<ElementTreeItemViewModel>();
                ElementTreeItemViewModel child = viewModel;
                ElementTreeItemViewModel parent = viewModel.Parent;
                while ((parent != null) && (parent.Children[0] == child))
                {
                    viewModelHierarchy.Push(parent);
                    child = parent;
                    parent = parent.Parent;
                }

                foreach (ElementTreeItemViewModel currentViewModel in viewModelHierarchy)
                {
                    int leafElements = 0;
                    CountLeafElements(currentViewModel.Element, ref leafElements);

                    if (leafElements > 0 && currentViewModel.Depth > 0)
                    {
                        MatrixColor expandedColor = MatrixColorConverter.GetColor(currentViewModel.Depth);

                        int begin = row;
                        int end = row + leafElements;

                        for (int rowDelta = begin; rowDelta < end; rowDelta++)
                        {
                            for (int columnDelta = begin; columnDelta < end; columnDelta++)
                            {
                                _cellColors[rowDelta][columnDelta] = expandedColor;
                            }
                        }
                    }
                }
            }

            // Define diagonal color
            for (int row = 0; row < matrixSize; row++)
            {
                int depth = _elementViewModelLeafs[row].Depth;
                MatrixColor dialogColor = MatrixColorConverter.GetColor(depth);
                _cellColors[row][row] = dialogColor;
            }

            // Define cycle color
            for (int row = 0; row < matrixSize; row++)
            {
                for (int column = 0; column < matrixSize; column++)
                {
                    IDsmElement consumer = _elementViewModelLeafs[column].Element;
                    IDsmElement provider = _elementViewModelLeafs[row].Element;
                    CycleType cycleType = _application.IsCyclicDependency(consumer, provider);
                    if (_application.ShowCycles)
                    {
                        if (cycleType == CycleType.Hierarchical)
                        {
                            _cellColors[row][column] = MatrixColor.HierarchicalCycle;
                        }

                        if (cycleType == CycleType.System)
                        {
                            _cellColors[row][column] = MatrixColor.SystemCycle;
                        }
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
            foreach (ElementTreeItemViewModel provider in _elementViewModelLeafs)
            {
                _columnColors.Add(provider.Color);
            }
        }

        private void DefineColumnContent()
        {
            _columnElementIds = new List<int>();
            foreach (ElementTreeItemViewModel provider in _elementViewModelLeafs)
            {
                _columnElementIds.Add(provider.Element.Order);
            }
        }

        private void DefineCellContent()
        {
            _cellWeights = new List<List<int>>();
            int matrixSize = _elementViewModelLeafs.Count;

            for (int row = 0; row < matrixSize; row++)
            {
                _cellWeights.Add(new List<int>());
                for (int column = 0; column < matrixSize; column++)
                {
                    IDsmElement consumer = _elementViewModelLeafs[column].Element;
                    IDsmElement provider = _elementViewModelLeafs[row].Element;
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineMetrics()
        {
            _metrics = new List<string>();
            switch (_selectedMetricType)
            {
                case MetricType.NumberOfElements:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.GetElementSize(viewModel.Element);
                        _metrics.Add(metric.ToString());
                    }
                    break;
                case MetricType.IngoingRelations:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.FindIngoingRelations(viewModel.Element).Count();
                        _metrics.Add(metric.ToString());
                    }
                    break;
                case MetricType.OutgoingRelations:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.FindOutgoingRelations(viewModel.Element).Count();
                        _metrics.Add(metric.ToString());
                    }
                    break;
                case MetricType.InternalRelations:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.FindInternalRelations(viewModel.Element).Count();
                        _metrics.Add(metric.ToString());
                    }
                    break;
                case MetricType.HierarchicalCycles:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.GetHierarchicalCycleCount(viewModel.Element);
                        if (metric > 0)
                        {
                            _metrics.Add(metric.ToString());
                        }
                        else
                        {
                            _metrics.Add("-");
                        }
                    }
                    break;
                case MetricType.SystemCycles:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        int metric = _application.GetSystemCycleCount(viewModel.Element);
                        if (metric > 0)
                        {
                            _metrics.Add(metric.ToString());
                        }
                        else
                        {
                            _metrics.Add("-");
                        }
                    }
                    break;

                default:
                    foreach (ElementTreeItemViewModel viewModel in _elementViewModelLeafs)
                    {
                        _metrics.Add("");
                    }
                    break;
            }
        }

        private void ShowCellConsumersExecute(object parameter)
        {
            string title = $"Consumers in relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";
            var elements = _application.GetRelationConsumers(SelectedConsumer, SelectedProvider);
            _mainViewModel.NotifyElementsReportReady(title, elements);
        }

        private bool ShowCellConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellProvidersExecute(object parameter)
        {
            string title = $"Providers in relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";
            var elements = _application.GetRelationProviders(SelectedConsumer, SelectedProvider);
            _mainViewModel.NotifyElementsReportReady(title, elements);
        }

        private bool ShowCellProvidersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementIngoingRelationsExecute(object parameter)
        {
            string title = $"Ingoing relations of {SelectedProvider.Fullname}";
            var relations = _application.FindIngoingRelations(SelectedProvider);
            _mainViewModel.NotifyRelationsReportReady(title, relations);
        }

        private bool ShowElementIngoingRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementOutgoingRelationExecute(object parameter)
        {
            string title = $"Outgoing relations of {SelectedProvider.Fullname}";
            var relations = _application.FindOutgoingRelations(SelectedProvider);
            _mainViewModel.NotifyRelationsReportReady(title, relations);
        }

        private bool ShowElementOutgoingRelationCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementinternalRelationsExecute(object parameter)
        {
            string title = $"Internal relations of {SelectedProvider.Fullname}";
            var relations = _application.FindInternalRelations(SelectedProvider);
            _mainViewModel.NotifyRelationsReportReady(title, relations);
        }

        private bool ShowElementinternalRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementConsumersExecute(object parameter)
        {
            string title = $"Consumers of {SelectedProvider.Fullname}";
            var elements = _application.GetElementConsumers(SelectedProvider);
            _mainViewModel.NotifyElementsReportReady(title, elements);
        }

        private bool ShowConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowProvidedInterfacesExecute(object parameter)
        {
            string title = $"Provided interface of {SelectedProvider.Fullname}";
            var elements = _application.GetElementProvidedElements(SelectedProvider);
            _mainViewModel.NotifyElementsReportReady(title, elements);
        }

        private bool ShowElementProvidedInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementRequiredInterfacesExecute(object parameter)
        {
            string title = $"Required interface of {SelectedProvider.Fullname}";
            var elements = _application.GetElementProviders(SelectedProvider);
            _mainViewModel.NotifyElementsReportReady(title, elements);
        }

        private bool ShowElementRequiredInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellRelationsExecute(object parameter)
        {
            string title = $"Relations between consumer {SelectedConsumer.Fullname} and provider {SelectedProvider.Fullname}";
            var relations = _application.FindResolvedRelations(SelectedConsumer, SelectedProvider);
            _mainViewModel.NotifyRelationsReportReady(title, relations);
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

        private void PreviousMetricExecute(object parameter)
        {
            _selectedMetricType--;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        private bool PreviousMetricCanExecute(object parameter)
        {
            return _selectedMetricType != MetricType.NumberOfElements;
        }

        private void NextMetricExecute(object parameter)
        {
            _selectedMetricType++;
            SelectedMetricTypeName = _metricTypeNames[_selectedMetricType];
        }

        private bool NextMetricCanExecute(object parameter)
        {
            return _selectedMetricType != MetricType.SystemCycles;
        }

        private void UpdateColumnHeaderTooltip(int? column)
        {
            if (column.HasValue)
            {
                IDsmElement element = _elementViewModelLeafs[column.Value].Element;
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
                IDsmElement consumer = _elementViewModelLeafs[column.Value].Element;
                IDsmElement provider = _elementViewModelLeafs[row.Value].Element;

                if ((consumer != null) && (provider != null))
                {
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    CycleType cycleType = _application.IsCyclicDependency(consumer, provider);

                    string cycleText = "";
                    if (cycleType == CycleType.Hierarchical)
                    {
                        cycleText = "with hierarchical cycle";
                    }
                    if (cycleType == CycleType.System)
                    {
                        cycleText = "with system cycle";
                    }
                    CellTooltip = $"[{consumer.Order}] {consumer.Fullname} to [{provider.Order}] {provider.Fullname} weight={weight} {cycleText}";
                }
            }
        }

        private void SelectElement(IDsmElement element)
        {
            SelectElement(ElementViewModelTree, element);
        }

        private void SelectElement(IEnumerable<ElementTreeItemViewModel> tree, IDsmElement element)
        {
            foreach (ElementTreeItemViewModel treeItem in tree)
            {
                if (treeItem.Id == element.Id)
                {
                    SelectTreeItem(treeItem);
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

        private void DefineProviderRows()
        {
            _rowIsProvider = new List<bool>();
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                _rowIsProvider.Add(false);
            }
        }

        private void UpdateProviderRows()
        {
            if (HoveredRow.HasValue)
            {
                for (int row = 0; row < _elementViewModelLeafs.Count; row++)
                {
                    _rowIsProvider[row] = _cellWeights[row][HoveredRow.Value] > 0;
                }
            }
            else
            {
                for (int row = 0; row < _elementViewModelLeafs.Count; row++)
                {
                    _rowIsProvider[row] = false;
                }
            }
        }

        private void DefineConsumerRows()
        {
            _rowIsConsumer = new List<bool>();
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                _rowIsConsumer.Add(false);
            }
        }

        private void UpdateConsumerRows()
        {
            if (HoveredRow.HasValue)
            {
                for (int row = 0; row < _elementViewModelLeafs.Count; row++)
                {
                    _rowIsConsumer[row] = _cellWeights[HoveredRow.Value][row] > 0;
                }
            }
            else
            {
                for (int row = 0; row < _elementViewModelLeafs.Count; row++)
                {
                    _rowIsConsumer[row] = false;
                }
            }
        }

        private void DefineMatchingRows()
        {
            _rowIsMatch = new List<bool>();
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                _rowIsMatch.Add(false);
            }
        }

        private int UpdateMatchingRows()
        {
            int count =  _application.SearchElements(_searchText);
            for (int row = 0; row < _elementViewModelLeafs.Count; row++)
            {
                _rowIsMatch[row] = _elementViewModelLeafs[row].Element.IsMatch;
            }
            return count;
        }

        private void BackupSelectionBeforeReload()
        {
            _selectedConsumerId = SelectedConsumer?.Id;
            _selectedProviderId = SelectedProvider?.Id;
        }

        private void RestoreSelectionAfterReload()
        {
            for (int i = 0; i < _elementViewModelLeafs.Count; i++)
            {
                if (_selectedProviderId.HasValue && (_selectedProviderId.Value == _elementViewModelLeafs[i].Id))
                {
                    SelectRow(i);
                }

                if (_selectedConsumerId.HasValue && (_selectedConsumerId.Value == _elementViewModelLeafs[i].Id))
                {
                    SelectColumn(i);
                }
            }
        }
    }
}
