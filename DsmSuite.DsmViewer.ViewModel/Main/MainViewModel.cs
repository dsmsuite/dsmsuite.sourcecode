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
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
using DsmSuite.DsmViewer.ViewModel.Editing.Snapshot;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Reflection;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        public void NotifyElementsReportReady(string title, IEnumerable<IDsmElement> elements)
        {
            ElementListViewModel elementListViewModel = new ElementListViewModel(title, elements);
            ElementsReportReady?.Invoke(this, elementListViewModel);
        }

        public void NotifyRelationsReportReady(string title, IEnumerable<IDsmRelation> relations)
        {
            RelationListViewModel viewModel = new RelationListViewModel(title, relations);
            RelationsReportReady?.Invoke(this, viewModel);
        }

        public event EventHandler<ElementCreateViewModel> ElementCreateStarted;
        public event EventHandler<ElementEditNameViewModel> ElementEditNameStarted;
        public event EventHandler<ElementEditTypeViewModel> ElementEditTypeStarted;
        public event EventHandler<ElementEditAnnotationViewModel> ElementEditAnnotationStarted;
        
        public event EventHandler<RelationCreateViewModel> RelationCreateStarted;
        public event EventHandler<RelationEditWeightViewModel> RelationEditWeightStarted;
        public event EventHandler<RelationEditTypeViewModel> RelationEditTypeStarted;

        public event EventHandler<SnapshotMakeViewModel> SnapshotMakeStarted;

        public event EventHandler<ElementListViewModel> ElementsReportReady;
        public event EventHandler<RelationListViewModel> RelationsReportReady;

        public event EventHandler<ActionListViewModel> ActionsVisible;

        public event EventHandler<SettingsViewModel> SettingsVisible;
        public event EventHandler<SearchSettingsViewModel> SearchSettingsVisible;

        public event EventHandler ScreenshotRequested;

        private readonly IDsmApplication _application;
        private string _modelFilename;
        private string _title;
        private string _version;
        private string _searchText;
        private SearchState _searchState;
        private string _searchResult;

        private bool _isModified;
        private bool _isLoaded;
        private readonly double _minZoom = 0.50;
        private readonly double _maxZoom = 2.00;
        private readonly double _zoomFactor = 1.25;
        private string _reportText;

        private MatrixViewModel _activeMatrix;
        private IndicatorViewMode _indicatorViewMode;
        private readonly ProgressViewModel _progressViewModel;
        private string _redoText;
        private string _undoText;
        private string _selectedSortAlgorithm;

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
            SortElementCommand = new RelayCommand<object>(SortElementExecute, SortElementCanExecute);

            ShowBookmarkedElementsCommand = new RelayCommand<object>(ShowBookmarkedElementsCommandExecute, ShowBookmarkedElementsCommandCanExecute);
            ShowAnnotatedElementsCommand = new RelayCommand<object>(ShowAnnotatedElementExecute, ShowAnnotatedElementCanExecute);

            ToggleElementBookmarkCommand = new RelayCommand<object>(ToggleElementBookmarkExecute, ToggleElementBookmarkCanExecute);
            ChangeElementAnnotationCommand = new RelayCommand<object>(ChangeElementAnnotationExecute, ChangeElementAnnotationCanExecute);

            ShowElementDetailMatrixCommand = new RelayCommand<object>(ShowElementDetailMatrixExecute, ShowElementDetailMatrixCanExecute);
            ShowElementContextMatrixCommand = new RelayCommand<object>(ShowElementContextMatrixExecute, ShowElementContextMatrixCanExecute);
            ShowCellDetailMatrixCommand = new RelayCommand<object>(ShowCellDetailMatrixExecute, ShowCellDetailMatrixCanExecute);

            ZoomInCommand = new RelayCommand<object>(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand<object>(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = new RelayCommand<object>(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            UndoCommand = new RelayCommand<object>(UndoExecute, UndoCanExecute);
            RedoCommand = new RelayCommand<object>(RedoExecute, RedoCanExecute);

            CreateElementCommand = new RelayCommand<object>(CreateElementExecute, CreateElementCanExecute);
            DeleteElementCommand = new RelayCommand<object>(DeleteElementExecute, DeleteElementCanExecute);
            ChangeElementParentCommand = new RelayCommand<object>(MoveElementExecute, MoveElementCanExecute);
            ChangeElementNameCommand = new RelayCommand<object>(ChangeElementNameExecute, ChangeElementNameCanExecute);
            ChangeElementTypeCommand = new RelayCommand<object>(ChangeElementTypeExecute, ChangeElementTypeCanExecute);

            CreateRelationCommand = new RelayCommand<object>(CreateRelationExecute, CreateRelationCanExecute);
            DeleteRelationCommand = new RelayCommand<object>(DeleteRelationExecute, DeleteRelationCanExecute);
            ChangeRelationWeightCommand = new RelayCommand<object>(ChangeRelationWeightExecute, ChangeRelationWeightCanExecute);
            ChangeRelationTypeCommand = new RelayCommand<object>(ChangeRelationTypeExecute, ChangeRelationTypeCanExecute);
 
            MakeSnapshotCommand = new RelayCommand<object>(MakeSnapshotExecute, MakeSnapshotCanExecute);
            ShowHistoryCommand = new RelayCommand<object>(ShowHistoryExecute, ShowHistoryCanExecute);
            ShowSettingsCommand = new RelayCommand<object>(ShowSettingsExecute, ShowSettingsCanExecute);

            TakeScreenshotCommand = new RelayCommand<object>(TakeScreenshotExecute);
            ClearSearchCommand = new RelayCommand<object>(ClearSearchExecute);
            SearchSettingsCommand = new RelayCommand<object>(SearchSettingExecute);

            _modelFilename = "";
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _title = $"DSM Viewer - {_version}";

            _isModified = false;
            _isLoaded = false;

            _searchText = "";
            _searchState = SearchState.NoMatch;

            _selectedSortAlgorithm = SupportedSortAlgorithms[0];

            _progressViewModel = new ProgressViewModel();

            IndicatorViewMode = IndicatorViewMode.ConsumersProviders;

            ActiveMatrix = new MatrixViewModel(this, _application, new List<IDsmElement>());
        }

        private void OnModelModified(object sender, bool e)
        {
            IsModified = e;
        }

        public IDsmElement SelectedConsumer => ActiveMatrix?.SelectedConsumer;

        public IDsmElement SelectedProvider => ActiveMatrix?.SelectedProvider;

        public ElementTreeItemViewModel SelectedProviderTreeItem => ActiveMatrix?.SelectedTreeItem;

        public MatrixViewModel ActiveMatrix
        {
            get { return _activeMatrix; }
            set { _activeMatrix = value; OnPropertyChanged(); }
        }

        private bool _isMetricsViewExpanded;

        public IndicatorViewMode IndicatorViewMode
        {
            get { return _indicatorViewMode; }
            set { _indicatorViewMode = value; OnPropertyChanged(); }
        }

        public bool IsMetricsViewExpanded
        {
            get { return _isMetricsViewExpanded; }
            set { _isMetricsViewExpanded = value; OnPropertyChanged(); }
        }

        public string ReportText
        {
            get { return _reportText; }
            set { _reportText = value; OnPropertyChanged(); }
        }

        public List<string> SupportedSortAlgorithms => _application.GetSupportedSortAlgorithms().ToList();

        public string SelectedSortAlgorithm
        {
            get { return _selectedSortAlgorithm; }
            set { _selectedSortAlgorithm = value; OnPropertyChanged(); }
        }

        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand HomeCommand { get; }

        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }

        public ICommand ShowBookmarkedElementsCommand { get; }
        public ICommand ShowAnnotatedElementsCommand { get; }

        public ICommand ToggleElementBookmarkCommand { get; }
        public ICommand ChangeElementAnnotationCommand { get; }

        public ICommand SortElementCommand { get; }
        public ICommand ShowElementDetailMatrixCommand { get; }
        public ICommand ShowElementContextMatrixCommand { get; }
        public ICommand ShowCellDetailMatrixCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand CreateElementCommand { get; }
        public ICommand DeleteElementCommand { get; }
        public ICommand ChangeElementParentCommand { get; }
        public ICommand ChangeElementNameCommand { get; }
        public ICommand ChangeElementTypeCommand { get; }
        public ICommand CreateRelationCommand { get; }
        public ICommand DeleteRelationCommand { get; }
        public ICommand ChangeRelationWeightCommand { get; }
        public ICommand ChangeRelationTypeCommand { get; }

        public ICommand MakeSnapshotCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand TakeScreenshotCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SearchSettingsCommand { get; }

        public string ModelFilename
        {
            get { return _modelFilename; }
            set { _modelFilename = value; OnPropertyChanged(); }
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

        public string Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); OnSearchTextUpdated(); }
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
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewModel.Update(p);
            });

            _progressViewModel.Action = "Reading";
            string fileToOpen = parameter as string;
            if (fileToOpen != null)
            {
                FileInfo fileInfo = new FileInfo(fileToOpen);
                ModelFilename = fileToOpen.Replace(fileInfo.Extension, ".dsm");

                Title = $"DSM Viewer - {_version} ({ModelFilename})";

                switch (fileInfo.Extension)
                {
                    case ".dsm":
                        await _application.OpenModel(fileToOpen, progress);
                        IsLoaded = true;
                        break;
                    case ".dsi":
                        await _application.AsyncImportDsiModel(fileToOpen, ModelFilename, false, false, true, progress);
                        IsLoaded = true;
                        break;
                    case ".dot":
                        await _application.AsyncImportGraphVizModel(fileToOpen, ModelFilename, false, false, true, progress);
                        IsLoaded = true;
                        break;
                    case ".grapML":
                        await _application.AsyncImportGraphMlModel(fileToOpen, ModelFilename, false, false, true, progress);
                        IsLoaded = true;
                        break;
                }
                ActiveMatrix = new MatrixViewModel(this, _application, GetRootElements());
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
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewModel.Update(p);
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
            IncludeAllInTree();
            ActiveMatrix.Reload();
        }

        private IEnumerable<IDsmElement> GetRootElements()
        {
            return new List<IDsmElement> { _application.RootElement };
        }

        private bool HomeCanExecute(object parameter)
        {
            return IsLoaded;
        }

        private void SortElementExecute(object parameter)
        {
            _application.Sort(SelectedProvider, SelectedSortAlgorithm);
        }

        private bool SortElementCanExecute(object parameter)
        {
            return _application.HasChildren(SelectedProvider);
        }

        private void ShowElementDetailMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            ActiveMatrix.Reload();
        }

        private bool ShowElementDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowElementContextMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);

            foreach (IDsmElement consumer in _application.GetElementConsumers(SelectedProvider))
            {
                IncludeInTree(consumer);
            }

            foreach (IDsmElement provider in _application.GetElementProviders(SelectedProvider))
            {
                IncludeInTree(provider);
            }

            ActiveMatrix.Reload();
        }

        private bool ShowElementContextMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ShowCellDetailMatrixExecute(object parameter)
        {
            ExcludeAllFromTree();
            IncludeInTree(SelectedProvider);
            IncludeInTree(SelectedConsumer);

            ActiveMatrix.Reload();
        }

        private bool ShowCellDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void MoveUpElementExecute(object parameter)
        {
            _application.MoveUp(SelectedProvider);
        }

        private bool MoveUpElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider;
            IDsmElement previous = _application.PreviousSibling(current);
            return (current != null) && (previous != null);
        }

        private void MoveDownElementExecute(object parameter)
        {
            _application.MoveDown(SelectedProvider);
        }

        private bool MoveDownElementCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider;
            IDsmElement next = _application.NextSibling(current);
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
            ActiveMatrix.SelectTreeItem(ActiveMatrix.HoveredTreeItem);
            if ((SelectedProviderTreeItem != null) && (SelectedProviderTreeItem.IsExpandable))
            {
                SelectedProviderTreeItem.IsExpanded = !SelectedProviderTreeItem.IsExpanded;
            }

            ActiveMatrix.Reload();
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
        }

        private bool UndoCanExecute(object parameter)
        {
            return _application.CanUndo();
        }

        private void RedoExecute(object parameter)
        {
            _application.Redo();
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

        private void SelectDefaultIndicatorMode()
        {
            IndicatorViewMode = string.IsNullOrEmpty(SearchText) ? IndicatorViewMode.ConsumersProviders : IndicatorViewMode.SearchResults;
            ActiveMatrix?.Reload();
        }

        private void OnSearchTextUpdated()
        {
            SelectDefaultIndicatorMode();

            int count = ActiveMatrix.HighlighMatchingElements(SearchText);

            if (count == 0)
            {
                SearchState = SearchState.NoMatch;
                SearchResult = SearchText.Length > 0 ? "None found" : "";
            }
            else
            {
                SearchState = SearchState.Match;
                SearchResult = $"{count} found";
            }
            ActiveMatrix.Reload();
        }

        private bool HasPrefix(string prefix, ref string searchText)
        {
            bool hasPrefix = searchText.StartsWith(prefix);
            searchText = SearchText.Replace(prefix, string.Empty);
            return hasPrefix;
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            UndoText = $"Undo {_application.GetUndoActionDescription()}";
            RedoText = $"Redo {_application.GetRedoActionDescription()}";
            ActiveMatrix?.Reload();
        }

        private void CreateElementExecute(object parameter)
        {
            ElementCreateViewModel elementCreateViewModel = new ElementCreateViewModel(_application, SelectedProvider);
            ElementCreateStarted?.Invoke(this, elementCreateViewModel);
        }

        private bool CreateElementCanExecute(object parameter)
        {
            return true;
        }

        private void DeleteElementExecute(object parameter)
        {
            _application.DeleteElement(SelectedProvider);
        }

        private bool DeleteElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void ChangeElementNameExecute(object parameter)
        {
            ElementEditNameViewModel elementEditViewModel = new ElementEditNameViewModel(_application, SelectedProvider);
            ElementEditNameStarted?.Invoke(this, elementEditViewModel);
        }

        private bool ChangeElementNameCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void ChangeElementTypeExecute(object parameter)
        {
            ElementEditTypeViewModel elementEditViewModel = new ElementEditTypeViewModel(_application, SelectedProvider);
            ElementEditTypeStarted?.Invoke(this, elementEditViewModel);
        }

        private bool ChangeElementTypeCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void MoveElementExecute(object parameter)
        {
            Tuple<IDsmElement, IDsmElement,int> moveParameter = parameter as Tuple<IDsmElement, IDsmElement, int>;
            if (moveParameter != null)
            {
                _application.ChangeElementParent(moveParameter.Item1, moveParameter.Item2, moveParameter.Item3);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool MoveElementCanExecute(object parameter)
        {
            bool canExecute = false;
            if (SelectedProvider != null)
            {
                canExecute = !SelectedProvider.IsRoot;
            }
            return canExecute;
        }

        private void ChangeRelationWeightExecute(object parameter)
        {
            if ((SelectedConsumer != null) && (SelectedProvider != null))
            {
                IEnumerable<IDsmRelation> relations = _application.FindRelations(SelectedConsumer, SelectedProvider);
                IDsmRelation relation = relations.FirstOrDefault();
                if (relation != null)
                {
                    RelationEditWeightViewModel relationEditViewModel = new RelationEditWeightViewModel(_application, relation);
                    RelationEditWeightStarted?.Invoke(this, relationEditViewModel);
                }
            }
        }

        private bool ChangeRelationWeightCanExecute(object parameter)
        {
            return ActiveMatrix?.SelectedCellHasRelationCount == 1;
        }

        private void ChangeRelationTypeExecute(object parameter)
        {
            if ((SelectedConsumer != null) && (SelectedProvider != null))
            {
                IEnumerable<IDsmRelation> relations = _application.FindRelations(SelectedConsumer, SelectedProvider);
                IDsmRelation relation = relations.FirstOrDefault();
                if (relation != null)
                {
                    RelationEditTypeViewModel relationEditViewModel = new RelationEditTypeViewModel(_application, relation);
                    RelationEditTypeStarted?.Invoke(this, relationEditViewModel);
                }
            }
        }

        private bool ChangeRelationTypeCanExecute(object parameter)
        {
            return ActiveMatrix?.SelectedCellHasRelationCount == 1;
        }

        private void CreateRelationExecute(object parameter)
        {
            RelationCreateViewModel relationCreateViewModel = new RelationCreateViewModel(_application, SelectedConsumer, SelectedProvider);
            RelationCreateStarted?.Invoke(this, relationCreateViewModel);
        }

        private bool CreateRelationCanExecute(object parameter)
        {
            return (SelectedConsumer != null) &&
                   (SelectedConsumer.HasChildren == false) &&
                   (SelectedProvider != null) &&
                   (SelectedProvider.HasChildren == false);
        }

        private void DeleteRelationExecute(object parameter)
        {
            if ((SelectedConsumer != null) && (SelectedProvider != null))
            {
                IEnumerable<IDsmRelation> relations = _application.FindRelations(SelectedConsumer, SelectedProvider);
                IDsmRelation relation = relations.FirstOrDefault();
                if (relation != null)
                {
                    _application.DeleteRelation(relation);
                }
            }
        }

        private bool DeleteRelationCanExecute(object parameter)
        {
            return ActiveMatrix?.SelectedCellHasRelationCount == 1;
        }

        private void ShowBookmarkedElementsCommandExecute(object parameter)
        {
            IndicatorViewMode = IndicatorViewMode.Bookmarks;
            ActiveMatrix?.Reload();
        }

        private bool ShowBookmarkedElementsCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ShowAnnotatedElementExecute(object parameter)
        {
            IndicatorViewMode = IndicatorViewMode.Annotations;
            ActiveMatrix?.Reload();
        }

        private bool ShowAnnotatedElementCanExecute(object parameter)
        {
            return true;
        }

        private void ToggleElementBookmarkExecute(object parameter)
        {
            if (SelectedProvider != null)
            {
                SelectedProvider.IsBookmarked = !SelectedProvider.IsBookmarked;
                ActiveMatrix?.Reload();
            }
        }

        private bool ToggleElementBookmarkCanExecute(object parameter)
        {
            return _indicatorViewMode == IndicatorViewMode.Bookmarks;
        }

        private void ChangeElementAnnotationExecute(object parameter)
        {
            if (SelectedProvider != null)
            {
                ElementEditAnnotationViewModel elementEditViewModel = new ElementEditAnnotationViewModel(_application, SelectedProvider);
                ElementEditAnnotationStarted?.Invoke(this, elementEditViewModel);
                ActiveMatrix?.Reload();
            }
        }

        private bool ChangeElementAnnotationCanExecute(object parameter)
        {
            return _indicatorViewMode == IndicatorViewMode.Annotations;
        }

        private void MakeSnapshotExecute(object parameter)
        {
            SnapshotMakeViewModel viewModel = new SnapshotMakeViewModel(_application);
            SnapshotMakeStarted?.Invoke(this, viewModel);
        }

        private bool MakeSnapshotCanExecute(object parameter)
        {
            return true;
        }

        private void ShowHistoryExecute(object parameter)
        {
            ActionListViewModel viewModel = new ActionListViewModel(_application);
            ActionsVisible?.Invoke(this, viewModel);
        }

        private bool ShowHistoryCanExecute(object parameter)
        {
            return true;
        }

        private void ShowSettingsExecute(object parameter)
        {
            SettingsViewModel viewModel = new SettingsViewModel(_application);
            SettingsVisible?.Invoke(this, viewModel);
            ActiveMatrix?.Reload();
        }

        private bool ShowSettingsCanExecute(object parameter)
        {
            return true;
        }

        private void TakeScreenshotExecute(object parameter)
        {
            ScreenshotRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSearchExecute(object parameter)
        {
            SearchText = "";
        }

        public void SearchSettingExecute(object parameter)
        {
            SearchSettingsViewModel viewModel = new SearchSettingsViewModel(_application);
            SearchSettingsVisible?.Invoke(this, viewModel);
            OnSearchTextUpdated();
        }

        private void ExcludeAllFromTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, false);
        }

        private void IncludeAllInTree()
        {
            UpdateChildrenIncludeInTree(_application.RootElement, true);
        }

        private void IncludeInTree(IDsmElement element)
        {
            UpdateChildrenIncludeInTree(element, true);
            UpdateParentsIncludeInTree(element, true);
        }

        private void UpdateChildrenIncludeInTree(IDsmElement element, bool included)
        {
            element.IsIncludedInTree = included;

            foreach (IDsmElement child in element.AllChildren)
            {
                UpdateChildrenIncludeInTree(child, included);
            }
        }

        private void UpdateParentsIncludeInTree(IDsmElement element, bool included)
        {
            IDsmElement current = element;
            do
            {
                current.IsIncludedInTree = included;
                current = current.Parent;
            } while (current != null);
        }
    }
}
