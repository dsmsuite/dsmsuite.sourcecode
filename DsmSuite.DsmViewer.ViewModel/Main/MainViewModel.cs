using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using DsmSuite.DsmViewer.ViewModel.Lists;
using System.Linq;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        public void NotifyReportCreated(ReportViewModel report)
        {
            ReportCreated?.Invoke(this, report);
        }

        public void NotifyElementsReportReady(ElementListViewModel report)
        {
            ElementsReportReady?.Invoke(this, report);
        }

        public void NotifyRelationsReportReady(RelationListViewModel report)
        {
            RelationsReportReady?.Invoke(this, report);
        }

        public event EventHandler<ReportViewModel> ReportCreated;
        public event EventHandler<ElementListViewModel> ElementsReportReady;
        public event EventHandler<RelationListViewModel> RelationsReportReady;

        private readonly IDsmApplication _application;
        private string _modelFilename;
        private string _title;
        private string _searchText;
        private List<IDsmElement> _foundElements;
        private int? _foundElementIndex;
        private SearchState _searchState;
        private string _searchResult;

        private bool _showCycles;
        private bool _isModified;
        private bool _isLoaded;
        private readonly double _minZoom = 0.50;
        private readonly double _maxZoom = 2.00;
        private readonly double _zoomFactor = 1.25;
        private string _reportText;

        private MatrixViewModel _activeMatrix;
        private readonly ProgressViewModel _progressViewModel;
        private string _redoText;
        private string _undoText;

        public MainViewModel(IDsmApplication application)
        {
            _application = application;
            _application.Modified += OnModelModified;
            _application.ActionPerformed += OnActionPerformed;

            OpenFileCommand = new RelayCommand<object>(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = new RelayCommand<object>(SaveFileExecute, SaveFileCanExecute);

            HomeCommand = new RelayCommand<object>(HomeExecute, HomeCanExecute);

            MoveUpElementCommand = new RelayCommand<object>(MoveUpElementExecute, MoveUpElementCanExecute);
            MoveDownElementCommand = new RelayCommand<object>(MoveDownElementExecute, MoveDownElementCanExecute);
            PartitionElementCommand = new RelayCommand<object>(PartitionElementExecute, PartitionElementCanExecute);

            ShowElementDetailMatrixCommand = new RelayCommand<object>(ShowElementDetailMatrixExecute, ShowElementDetailMatrixCanExecute);
            ShowElementContextMatrixCommand = new RelayCommand<object>(ShowElementContextMatrixExecute, ShowElementContextMatrixCanExecute);
            ShowCellDetailMatrixCommand = new RelayCommand<object>(ShowCellDetailMatrixExecute, ShowCellDetailMatrixCanExecute);

            ZoomInCommand = new RelayCommand<object>(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand<object>(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = new RelayCommand<object>(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            UndoCommand = new RelayCommand<object>(UndoExecute, UndoCanExecute);
            RedoCommand = new RelayCommand<object>(RedoExecute, RedoCanExecute);
            
            OverviewReportCommand = new RelayCommand<object>(OverviewReportExecute, OverviewReportCanExecute);

            NavigateToNextCommand = new RelayCommand<object>(NavigateToNextExecute, NavigateToNextCanExecute);
            NavigateToPreviousCommand = new RelayCommand<object>(NavigateToPreviousExecute, NavigateToPreviousCanExecute);

            CreateElementCommand = new RelayCommand<object>(CreateElementExecute, CreateElementCanExecute);
            DeleteElementCommand = new RelayCommand<object>(DeleteElementExecute, DeleteElementCanExecute);
            MoveElementCommand = new RelayCommand<object>(MoveElementExecute, MoveElementCanExecute);
            RenameElementCommand = new RelayCommand<object>(RenameElementExecute, RenameElementCanExecute);
            CreateRelationCommand = new RelayCommand<object>(CreateRelationExecute, CreateRelationCanExecute);
            DeleteRelationCommand = new RelayCommand<object>(DeleteRelationExecute, DeleteRelationCanExecute);

            ModelFilename = "";
            Title = "DSM Viewer";
            ShowCycles = false;
            IsModified = false;
            IsLoaded = false;
            SearchText = "";

            _progressViewModel = new ProgressViewModel();
        }

        private void OnModelModified(object sender, bool e)
        {
            IsModified = e;
        }

        public ElementViewModel SelectedConsumer => ActiveMatrix?.SelectedConsumer;

        public ElementViewModel SelectedProvider => ActiveMatrix?.SelectedProvider;

        public ElementTreeItemViewModel SelectedProviderTreeItem => ActiveMatrix?.SelectedProvider as ElementTreeItemViewModel;

        public MatrixViewModel ActiveMatrix
        {
            get { return _activeMatrix; }
            set { _activeMatrix = value; OnPropertyChanged(); }
        }

        public string ReportText
        {
            get { return _reportText; }
            set { _reportText = value; OnPropertyChanged(); }
        }

        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand HomeCommand { get; }

        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand PartitionElementCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand OverviewReportCommand { get; }
        public ICommand NavigateToNextCommand { get; }
        public ICommand NavigateToPreviousCommand { get; }

        public ICommand CreateElementCommand { get; }
        public ICommand DeleteElementCommand { get; }
        public ICommand MoveElementCommand { get; }
        public ICommand RenameElementCommand { get; }
        public ICommand CreateRelationCommand { get; }
        public ICommand DeleteRelationCommand { get; }

        public string ModelFilename
        {
            get { return _modelFilename; }
            set { _modelFilename = value; OnPropertyChanged(); }
        }

        public bool ShowCycles
        {
            get { return _showCycles; }
            set { _showCycles = value; OnPropertyChanged(); }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set { _isModified = value; OnPropertyChanged(); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); OnRunSearch(); }
        }

        public SearchState SearchState
        {
            get { return _searchState; }
            set { _searchState = value; OnPropertyChanged(); }
        }

        public string SearchResult
        {
            get { return _searchResult; }
            set { _searchResult = value; OnPropertyChanged(); }
        }

        public ProgressViewModel ProgressViewModel => _progressViewModel;

        private async void OpenFileExecute(object parameter)
        {
            var progress = new Progress<DsmProgressInfo>(p =>
            {
                _progressViewModel.Update(p.ElementCount, p.RelationCount, p.Progress);
            });

            _progressViewModel.Action = "Reading";
            string fileToOpen = parameter as string;
            if (fileToOpen != null)
            {
                await _application.OpenModel(fileToOpen, progress);
                ModelFilename = fileToOpen;
                Title = $"DSM Viewer ({ModelFilename})";
                IsLoaded = true;
                ActiveMatrix = new MatrixViewModel(this, _application, _application.RootElements);
            }
        }

        private bool OpenFileCanExecute(object parameter)
        {
            string fileToOpen = parameter as string;
            if (fileToOpen != null)
            {
                return File.Exists(fileToOpen);
            }
            else
            {
                return false;
            }
        }

        private async void SaveFileExecute(object parameter)
        {
            var progress = new Progress<DsmProgressInfo>(p =>
            {
                _progressViewModel.Update(p.ElementCount, p.RelationCount, p.Progress);
            });

            _progressViewModel.Action = "Reading";
            await _application.SaveModel(ModelFilename, progress);
        }

        private bool SaveFileCanExecute(object parameter)
        {
            return _application.IsModified;
        }

        private void HomeExecute(object parameter)
        {
            ActiveMatrix = new MatrixViewModel(this, _application, _application.RootElements);
        }

        private bool HomeCanExecute(object parameter)
        {
            return IsLoaded;
        }

        private void PartitionElementExecute(object parameter)
        {
            _application.Sort(SelectedProvider?.Element, "Partition");
            ActiveMatrix.Reload();
        }

        private bool PartitionElementCanExecute(object parameter)
        {
            return _application.HasChildren(SelectedProvider?.Element);
        }

        private void ShowElementDetailMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement> { SelectedProvider?.Element };
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ShowElementDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementContextMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement> {SelectedProvider?.Element};
            selectedElements.AddRange(_application.GetElementConsumers(SelectedProvider?.Element));
            selectedElements.AddRange(_application.GetElementProviders(SelectedProvider?.Element));
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ShowElementContextMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellDetailMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement>
            {
                SelectedProvider?.Element,
                SelectedConsumer?.Element
            };
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ShowCellDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void MoveUpElementExecute(object parameter)
        {
            _application.MoveUp(SelectedProvider?.Element);
            ActiveMatrix.Reload();
        }

        private bool MoveUpElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider?.Element;
            IDsmElement previous = current?.PreviousSibling;
            return (current != null) && (previous != null);
        }

        private void MoveDownElementExecute(object parameter)
        {
            _application.MoveDown(SelectedProvider?.Element);
            ActiveMatrix.Reload();
        }

        private bool MoveDownElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider?.Element;
            IDsmElement next = current?.NextSibling;
            return (current != null) && (next != null);
        }

        private void ZoomInExecute(object parameter)
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel *= _zoomFactor;
            }
        }

        private bool ZoomInCanExecute(object parameter)
        {
            return ActiveMatrix?.ZoomLevel < _maxZoom;
        }

        private void ZoomOutExecute(object parameter)
        {
            if (ActiveMatrix != null)
            {
                ActiveMatrix.ZoomLevel /= _zoomFactor;
            }
        }

        private bool ZoomOutCanExecute(object parameter)
        {
            return ActiveMatrix?.ZoomLevel > _minZoom;
        }

        private void ToggleElementExpandedExecute(object parameter)
        {
            ActiveMatrix.SelectedProvider = ActiveMatrix.HoveredProvider;
            if ((SelectedProviderTreeItem != null) && (SelectedProviderTreeItem.IsExpandable))
            {
                SelectedProviderTreeItem.IsExpanded = !SelectedProviderTreeItem.IsExpanded;
            }

            ActiveMatrix.Update();
        }

        private bool ToggleElementExpandedCanExecute(object parameter)
        {
            return true;
        }

        public string UndoText
        {
            get { return _undoText; }
            set { _undoText = value; OnPropertyChanged(); }
        }

        private void UndoExecute(object parameter)
        {
            _application.Undo();
            ActiveMatrix.Reload();
        }

        private bool UndoCanExecute(object parameter)
        {
            return _application.CanUndo();
        }

        private void RedoExecute(object parameter)
        {
            _application.Redo(); 
            ActiveMatrix.Reload();
        }

        public string RedoText
        {
            get { return _redoText; }
            set { _redoText = value; OnPropertyChanged(); }
        }

        private bool RedoCanExecute(object parameter)
        {
            return _application.CanRedo();
        }

        private void OverviewReportExecute(object parameter)
        {
            string content = _application.GetOverviewReport();

            ReportViewModel reportViewModel = new ReportViewModel() { Title = "Overview", Content = content };
            ReportCreated?.Invoke(this, reportViewModel);
        }

        private bool OverviewReportCanExecute(object parameter)
        {
            return true;
        }

        private void OnRunSearch()
        {
            _foundElements = _application.SearchElements(SearchText).ToList();
            int count = _foundElements.Count;
            if (count == 0)
            {
                SearchState = SearchState.NoMatch;
                SearchResult = SearchText.Length > 0 ? "No elements found" : "";
            }
            else if (count == 1)
            {
                IDsmElement foundElement = _foundElements.FirstOrDefault();
                if ((foundElement != null) && (foundElement.Fullname != SearchText))
                {
                    SearchText = foundElement.Fullname;
                }
                SearchState = SearchState.OneMatch;
                SearchResult = "1 element found";
            }
            else if (count > 1)
            {
                SearchState = SearchState.ManyMatches;
                SearchResult = $"{count} elements found";
            }
        }

        private void NavigateToNextExecute(object parameter)
        {
            if (!_foundElementIndex.HasValue)
            {
                _foundElementIndex = 0;
            }
            else
            {
                if (_foundElementIndex < _foundElements.Count - 1)
                {
                    _foundElementIndex++;
                }
            }

            NavigateToSelectedElement();
        }

        private bool NavigateToNextCanExecute(object parameter)
        {
            bool canExecute = false;

            if (_foundElements.Count > 0)
            {
                if (_foundElementIndex.HasValue)
                {
                    if (_foundElementIndex.Value < _foundElements.Count - 1)
                    {
                        canExecute = true;
                    }
                }
                else
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        private void NavigateToPreviousExecute(object parameter)
        {
            if (_foundElementIndex.HasValue)
            {
                _foundElementIndex--;
            }

            NavigateToSelectedElement();
        }

        private void NavigateToSelectedElement()
        {
            if (_foundElementIndex.HasValue)
            {
                IDsmElement foundElement = _foundElements[_foundElementIndex.Value];
                ExpandElement(foundElement);
                SelectElement(ActiveMatrix.Providers, foundElement);
            }
        }

        private bool NavigateToPreviousCanExecute(object parameter)
        {
            return _foundElements.Count > 0 && _foundElementIndex > 0;
        }

        private void ExpandElement(IDsmElement element)
        {
            IDsmElement current = element.Parent;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.Parent;
            }
            ActiveMatrix?.Reload();
        }

        private void SelectElement(IEnumerable<ElementTreeItemViewModel> providers, IDsmElement element)
        {
            foreach (ElementTreeItemViewModel item in providers)
            {
                if (element.Id == item.Id)
                {
                    if (ActiveMatrix != null)
                    {
                        ActiveMatrix.SelectedProvider = item;
                    }
                }
                else
                {
                    SelectElement(item.Children, element);
                }
            }
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            UndoText = $"Undo {_application.GetUndoActionDescription()}";
            RedoText = $"Redo {_application.GetRedoActionDescription()}";
        }

        private void CreateElementExecute(object parameter)
        {
            string name = "Name";
            string type = "Type";
            _application.CreateElement(name, type, SelectedProvider.Element);
            ActiveMatrix.Reload();
        }

        private bool CreateElementCanExecute(object parameter)
        {
            return true;
        }

        private void DeleteElementExecute(object parameter)
        {
            _application.DeleteElement(SelectedProvider.Element);
            ActiveMatrix.Reload();
        }

        private bool DeleteElementCanExecute(object parameter)
        {
            return true;
        }

        private void RenameElementExecute(object parameter)
        {
            string newName = "NewName";
            _application.RenameElement(SelectedProvider.Element, newName);
            ActiveMatrix.Reload();
        }

        private bool RenameElementCanExecute(object parameter)
        {
            return true;
        }

        private void MoveElementExecute(object parameter)
        {
            IDsmElement newParent = null;
            _application.MoveElement(SelectedProvider.Element, newParent);
            ActiveMatrix.Reload();
        }

        private bool MoveElementCanExecute(object parameter)
        {
            return true;
        }

        private void CreateRelationExecute(object parameter)
        {
            IDsmElement consumer = SelectedConsumer.Element;
            IDsmElement provider = SelectedProvider.Element;
            _application.CreateRelation(consumer, provider, "type", 1);
            ActiveMatrix.Reload();
        }

        private bool CreateRelationCanExecute(object parameter)
        {
            return true;
        }

        private void DeleteRelationExecute(object parameter)
        {
            IDsmRelation relation = null;
            _application.DeleteRelation(relation);
            ActiveMatrix.Reload();
        }

        private bool DeleteRelationCanExecute(object parameter)
        {
            return true;
        }
    }
}
