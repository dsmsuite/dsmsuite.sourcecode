using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
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
        private IList<List<Color>> _cellColors;
        private IList<List<int>> _cellWeights;
        private int? _selectedRow;
        private int? _selectedColumn;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private int _matrixSize;

        public ImprovedMatrixViewModel(IMainViewModel mainViewModel, IDsmApplication application, IList<IDsmElement> selectedElements)
        {
            _cellColors = new List<List<Color>>();
            _cellWeights = new List<List<int>>();

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
            DefineCellColors();
            DefineCellContent();
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

        private void ShowCellConsumersExecute(object parameter)
        {
            //string title = $"Consumers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            //var elements = _application.GetRelationConsumers(SelectedConsumer.Element, SelectedProvider.Element);

            //ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            //_mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }


        private bool ShowCellConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellProvidersExecute(object parameter)
        {
            //string title = $"Providers in relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            //var elements = _application.GetRelationProviders(SelectedConsumer.Element, SelectedProvider.Element);

            //ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            //_mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowCellProvidersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementConsumersExecute(object parameter)
        {
            //string title = $"Consumers of {SelectedProvider.Element.Fullname}";

            //var elements = _application.GetElementConsumers(SelectedProvider.Element);

            //ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            //_mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowConsumersCanExecute(object parameter)
        {
            return true;
        }

        private void ShowProvidedInterfacesExecute(object parameter)
        {
            //string title = $"Provided interface of {SelectedProvider.Element.Fullname}";

            //var elements = _application.GetElementProvidedElements(SelectedProvider.Element);

            //ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            //_mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowElementProvidedInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementRequiredInterfacesExecute(object parameter)
        {
            //string title = $"Required interface of {SelectedProvider.Element.Fullname}";

            //var elements = _application.GetElementProviders(SelectedProvider.Element);

            //ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            //_mainViewModel.NotifyElementsReportReady(elementListViewModel);
        }

        private bool ShowElementRequiredInterfacesCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellRelationsExecute(object parameter)
        {
            //string title = $"Relations between consumer {SelectedConsumer.Element.Fullname} and provider {SelectedProvider.Element.Fullname}";

            //var relations = _application.FindResolvedRelations(SelectedConsumer.Element, SelectedProvider.Element);

            //RelationListViewModel relationsListViewModel = new RelationListViewModel(title, relations);
            //_mainViewModel.NotifyRelationsReportReady(relationsListViewModel);
        }

        private bool ShowCellRelationsCanExecute(object parameter)
        {
            return true;
        }

        private void DefineCellContent()
        {
            int size = _selectedElements.Count;

            for (int row = 0; row < size; row++)
            {
                _cellWeights.Add(new List<int>());
                for (int column = 0; column < size; column++)
                {
                    IDsmElement consumer = _selectedElements[column];
                    IDsmElement provider = _selectedElements[row];
                    int weight = _application.GetDependencyWeight(consumer, provider);
                    _cellWeights[row].Add(weight);
                }
            }
        }

        private void DefineCellColors()
        {
            int size = _selectedElements.Count;

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

                    CountLeafElements(_selectedElements[row], ref leafElements);
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
                    IDsmElement consumer = _selectedElements[column];
                    IDsmElement provider = _selectedElements[row];
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
