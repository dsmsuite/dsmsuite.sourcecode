using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Reporting;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using System.Windows.Threading;
using DsmSuite.DsmViewer.Application;
using DsmSuite.DsmViewer.Util;
using DsmSuite.DsmViewer.ViewModel.Lists;

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
        private readonly IDsmModel _model;
        private string _modelFilename;
        private string _title;

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
            _model = application.Model;
            _model.Modified += OnModelModified;
            OpenFileCommand = new RelayCommand<object>(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = new RelayCommand<object>(SaveFileExecute, SaveFileCanExecute);

            HomeCommand = new RelayCommand<object>(HomeExecute, HomeCanExecute);

            MoveUpCommand = new RelayCommand<object>(MoveUpExecute, MoveUpCanExecute);
            MoveDownCommand = new RelayCommand<object>(MoveDownExecute, MoveDownCanExecute);
            PartitionCommand = new RelayCommand<object>(PartitionExecute, PartitionCanExecute);
            ElementDetailMatrixCommand = new RelayCommand<object>(ElementDetailMatrixExecute, ElementDetailMatrixCanExecute);

            RelationDetailMatrixCommand = new RelayCommand<object>(RelationDetailMatrixExecute, RelationDetailMatrixCanExecute);
            ZoomInCommand = new RelayCommand<object>(ZoomInExecute, ZoomInCanExecute);
            ZoomOutCommand = new RelayCommand<object>(ZoomOutExecute, ZoomOutCanExecute);
            ToggleElementExpandedCommand = new RelayCommand<object>(ToggleElementExpandedExecute, ToggleElementExpandedCanExecute);

            OverviewReportCommand = new RelayCommand<object>(OverviewReportExecute, OverviewReportCanExecute);

            ModelFilename = "";
            Title = "DSM Viewer";
            ShowCycles = false;
            IsModified = false;
            IsLoaded = false;

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
        public ICommand ElementDetailMatrixCommand { get; }
        public ICommand RelationDetailMatrixCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand OverviewReportCommand { get; }

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

        public ProgressViewModel ProgressViewModel
        {
            get { return _progressViewModel; }
        }

        private async void OpenFileExecute(object parameter)
        {
            var progress = new Progress<ProgressInfo>(p =>
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
            var progress = new Progress<ProgressInfo>(p =>
            {
                _progressViewModel.Update(p.ElementCount, p.RelationCount, p.Progress);
            });

            _progressViewModel.Action = "Reading";
            await _application.SaveModel(ModelFilename, progress);
        }

        private bool SaveFileCanExecute(object parameter)
        {
            return _model.IsModified;
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
            _application.Sort(SelectedProvider?.Element,"Partition");
            ActiveMatrix.Reload();
        }

        private bool PartitionCanExecute(object parameter)
        {
            return _application.HasChildren(SelectedProvider?.Element);
        }

        private void ElementDetailMatrixExecute(object parameter)
        {
            List<IElement> selectedElements = new List<IElement>();
            selectedElements.Add(SelectedProvider?.Element);
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool ElementDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void RelationDetailMatrixExecute(object parameter)
        {
            List<IElement> selectedElements = new List<IElement>();
            selectedElements.Add(SelectedProvider?.Element);
            selectedElements.Add(SelectedConsumer?.Element);
            ActiveMatrix = new MatrixViewModel(this, _application, selectedElements);
        }

        private bool RelationDetailMatrixCanExecute(object parameter)
        {
            return true;
        }

        private void MoveUpExecute(object parameter)
        {
            IElement current = SelectedProvider?.Element;
            IElement previous = current?.PreviousSibling;
            if ((current != null) && (previous != null))
            {
                _model.Swap(current, previous);
                ActiveMatrix.Reload();
            }
        }

        private bool MoveUpCanExecute(object parameter)
        {
            IElement current = SelectedProvider?.Element;
            IElement previous = current?.PreviousSibling;
            return (current != null) && (previous != null);
        }

        private void MoveDownExecute(object parameter)
        {
            IElement current = SelectedProvider?.Element;
            IElement next = current?.NextSibling;
            if ((current != null) && (next != null))
            {
                _model.Swap(current, next);
                ActiveMatrix.Reload();
            }
        }

        private bool MoveDownCanExecute(object parameter)
        {
            IElement current = SelectedProvider?.Element;
            IElement next = current?.NextSibling;
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
            OverviewReport report = new OverviewReport(_model);
            string content = report.WriteReport();

            ReportViewModel reportViewModel = new ReportViewModel() { Title = "Overview", Content = content };
            ReportCreated?.Invoke(this, reportViewModel);
        }

        private bool OverviewReportCanExecute(object parameter)
        {
            return true;
        }
    }
}
