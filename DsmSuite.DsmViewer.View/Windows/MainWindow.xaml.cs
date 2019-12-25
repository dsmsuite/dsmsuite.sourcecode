using System.Reflection;
using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.View.Dialogs;
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
using DsmSuite.DsmViewer.ViewModel.Editing.Snapshot;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _mainViewModel;
        private ProgressWindow _progressWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void OpenModel(string filename)
        {
            _mainViewModel.OpenFileCommand.Execute(filename);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            _mainViewModel = new MainViewModel(application);
            _mainViewModel.ReportCreated += OnReportCreated;
            _mainViewModel.ElementsReportReady += OnElementsReportReady;
            _mainViewModel.RelationsReportReady += OnRelationsReportReady;
            _mainViewModel.ProgressViewModel.BusyChanged += OnProgressViewModelBusyChanged;

            _mainViewModel.ElementCreateStarted += OnElementCreateStarted;
            _mainViewModel.ElementEditNameStarted += OnElementEditNameStarted;
            _mainViewModel.ElementEditTypeStarted += OnElementEditTypeStarted;

            _mainViewModel.RelationCreateStarted += OnRelationCreateStarted;
            _mainViewModel.RelationEditWeightStarted += OnRelationEditWeightStarted;
            _mainViewModel.RelationEditTypeStarted += OnRelationEditTypeStarted;

            _mainViewModel.SnapshotMakeStarted += OnSnapshotMakeStarted;

            _mainViewModel.ActionsVisible += OnActionsVisible;
            _mainViewModel.SettingsVisible += OnSettingsVisible;
            DataContext = _mainViewModel;

            OpenModelFile();
        }

        private void OnSettingsVisible(object sender, SettingsViewModel viewModel)
        {
            SettingsView view = new SettingsView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OnActionsVisible(object sender, ViewModel.Lists.ActionListViewModel viewModel)
        {
            ActionListView view = new ActionListView {DataContext = viewModel};
            view.Show();
        }

        private void OnSnapshotMakeStarted(object sender, SnapshotMakeViewModel viewModel)
        {
            SnapshotCreateDialog view = new SnapshotCreateDialog { DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnElementCreateStarted(object sender, ElementCreateViewModel viewModel)
        {
            ElementCreateDialog view = new ElementCreateDialog { DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnElementEditNameStarted(object sender, ElementEditNameViewModel viewModel)
        {
            ElementEditNameDialog view = new ElementEditNameDialog { DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnElementEditTypeStarted(object sender, ElementEditTypeViewModel viewModel)
        {
            ElementEditTypeDialog view = new ElementEditTypeDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OnRelationCreateStarted(object sender, RelationCreateViewModel viewModel)
        {
            RelationCreateDialog view = new RelationCreateDialog { DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnRelationEditWeightStarted(object sender, RelationEditWeightViewModel viewModel)
        {
            RelationEditWeightDialog view = new RelationEditWeightDialog { DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnRelationEditTypeStarted(object sender, RelationEditTypeViewModel viewModel)
        {
            RelationEditTypeDialog view = new RelationEditTypeDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OpenModelFile()
        {
            App app = System.Windows.Application.Current as App;
            if ((app != null) && (app.CommandLineArguments.Length == 1))
            {
                string filename = app.CommandLineArguments[0];
                if (filename.EndsWith(".dsm"))
                {
                    _mainViewModel.OpenFileCommand.Execute(filename);
                }
            }
        }

        private void OnElementsReportReady(object sender, ViewModel.Lists.ElementListViewModel e)
        {
            ElementListView view = new ElementListView
            {
                DataContext = e,
                Owner = this
            };
            view.Show();
        }

        private void OnRelationsReportReady(object sender, ViewModel.Lists.RelationListViewModel e)
        {
            RelationListView view = new RelationListView
            {
                DataContext = e,
                Owner = this
            };
            view.Show();
        }

        private void OnProgressViewModelBusyChanged(object sender, bool visible)
        {
            if (visible)
            {
                if (_progressWindow == null)
                {
                    _progressWindow = new ProgressWindow
                    {
                        DataContext = _mainViewModel.ProgressViewModel,
                        Owner = this
                    };
                    _progressWindow.ShowDialog();
                }
            }
            else
            {
                _progressWindow.Close();
                _progressWindow = null;
            }
        }

        private void OnReportCreated(object sender, ReportViewModel e)
        {
            ReportView reportView = new ReportView { DataContext = e };
            reportView.Show();
        }
    }
}
