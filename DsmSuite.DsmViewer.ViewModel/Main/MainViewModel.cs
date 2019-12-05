using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using DsmSuite.DsmViewer.Reporting;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using DsmSuite.DsmViewer.Application;
using DsmSuite.DsmViewer.ViewModel.Lists;
using System.Linq;
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

        public MainViewModel(IDsmApplication application)
        {
            _application = application;
            _application.Modified += OnModelModified;
            OpenFileCommand = new RelayCommand<object>(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = new RelayCommand<object>(SaveFileExecute, SaveFileCanExecute);

            HomeCommand = new RelayCommand<object>(HomeExecute, HomeCanExecute);

            MoveUpCommand = new RelayCommand<object>(MoveUpExecute, MoveUpCanExecute);
            MoveDownCommand = new RelayCommand<object>(MoveDownExecute, MoveDownCanExecute);
            PartitionCommand = new RelayCommand<object>(PartitionExecute, PartitionCanExecute);
            ElementInternalsMatrixCommand = new RelayCommand<object>(ElementInternalsMatrixExecute, ElementInternalsMatrixCanExecute);
            ElementContextMatrixCommand = new RelayCommand<object>(ElementContextMatrixExecute, ElementContextMatrixCanExecute);

            RelationMatrixCommand = new RelayCommand<object>(RelationMatrixExecute, RelationMatrixCanExecute);
            ZoomInCommand = new RelayCommand<object>(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand<object>(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = new RelayCommand<object>(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            OverviewReportCommand = new RelayCommand<object>(OverviewReportExecute, OverviewReportCanExecute);

            NavigateToNextCommand = new RelayCommand<object>(NavigateToNextExecute, NavigateToNextCanExecute);
            NavigateToPreviousCommand = new RelayCommand<object>(NavigateToPreviousExecute, NavigateToPreviousCanExecute);

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

        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand PartitionCommand { get; }
        public ICommand ElementInternalsMatrixCommand { get; }
        public ICommand ElementContextMatrixCommand { get; }
        public ICommand RelationMatrixCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand OverviewReportCommand { get; }
        public ICommand NavigateToNextCommand { get; }
        public ICommand NavigateToPreviousCommand { get; }

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

        private void PartitionExecute(object parameter)
        {
            _application.Sort(SelectedProvider?.Element, "Partition");
            ActiveMatrix.Reload();
        }

        private bool PartitionCanExecute(object parameter)
        {
            return _application.HasChildren(SelectedProvider?.Element);
        }

        private void ElementInternalsMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement> { SelectedProvider?.Element };
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ElementInternalsMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void ElementContextMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement>();
            selectedElements.Add(SelectedProvider?.Element);
            selectedElements.AddRange(_application.GetElementConsumers(SelectedProvider?.Element));
            selectedElements.AddRange(_application.GetElementProviders(SelectedProvider?.Element));
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ElementContextMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void RelationMatrixExecute(object parameter)
        {
            List<IDsmElement> selectedElements = new List<IDsmElement>
            {
                SelectedProvider?.Element,
                SelectedConsumer?.Element
            };
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool RelationMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void MoveUpExecute(object parameter)
        {
            _application.MoveUp(SelectedProvider?.Element);
            ActiveMatrix.Reload();
        }

        private bool MoveUpCanExecute(object parameter)
        {
            IDsmElement current = SelectedProvider?.Element;
            IDsmElement previous = current?.PreviousSibling;
            return (current != null) && (previous != null);
        }

        private void MoveDownExecute(object parameter)
        {
            _application.MoveDown(SelectedProvider?.Element);
            ActiveMatrix.Reload();
        }

        private bool MoveDownCanExecute(object parameter)
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
            _foundElements = _application.SearchExecute(SearchText).ToList();
            int count = _foundElements.Count();
            if (count == 0)
            {
                SearchState = SearchState.NoMatch;

                if (SearchText.Length > 0)
                {
                    SearchResult = "No elements found";
                }
                else
                {
                    SearchResult = "";
                }
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


    }
}
