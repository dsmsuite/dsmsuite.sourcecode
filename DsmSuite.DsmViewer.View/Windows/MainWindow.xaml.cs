using System.Reflection;
using System.Windows;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.Application;

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

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DsmModel model = new DsmModel("Viewer", Assembly.GetExecutingAssembly());
            DsmApplication application = new DsmApplication(model);
            _mainViewModel = new MainViewModel(application);
            _mainViewModel.ReportCreated += OnReportCreated;
            _mainViewModel.ElementsReportReady += OnElementsReportReady;
            _mainViewModel.RelationsReportReady += OnRelationsReportReady;
            DataContext = _mainViewModel;

            _mainViewModel.ProgressViewModel.BusyChanged += OnProgressViewModelBusyChanged;
        }

        private void OnElementsReportReady(object sender, ViewModel.Lists.ElementListViewModel e)
        {
            ElementListView view = new ElementListView();
            view.DataContext = e;
            view.Owner = this;
            view.ShowDialog();
        }

        private void OnRelationsReportReady(object sender, ViewModel.Lists.RelationListViewModel e)
        {
            RelationListView view = new RelationListView();
            view.DataContext = e;
            view.Owner = this;
            view.ShowDialog();
        }

        private void OnProgressViewModelBusyChanged(object sender, bool visible)
        {
            if (visible)
            {
                if (_progressWindow == null)
                {
                    _progressWindow = new ProgressWindow();
                    _progressWindow.DataContext = _mainViewModel.ProgressViewModel;
                    _progressWindow.Owner = this;
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
