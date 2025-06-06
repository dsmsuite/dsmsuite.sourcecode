﻿using DsmSuite.DsmViewer.Application.Core;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.View.Editing;
using DsmSuite.DsmViewer.View.Lists;
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Snapshot;
using DsmSuite.DsmViewer.ViewModel.Main;
using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SettingsView = DsmSuite.DsmViewer.View.Settings.SettingsView;

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
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_mainViewModel.IsModified)
            {
                e.Cancel = MessageBox.Show("Are you sure to exit?", "You have unsaved changes", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
            }
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
            _mainViewModel.ElementsReportReady += OnElementsReportReady;
            _mainViewModel.RelationsReportReady += OnRelationsReportReady;
            _mainViewModel.ProgressViewModel.BusyChanged += OnProgressViewModelBusyChanged;

            _mainViewModel.ElementEditStarted += OnElementEditStarted;

            _mainViewModel.SnapshotMakeStarted += OnSnapshotMakeStarted;

            _mainViewModel.ActionsVisible += OnActionsVisible;
            _mainViewModel.SettingsVisible += OnSettingsVisible;

            _mainViewModel.ScreenshotRequested += OnScreenshotRequested;
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
            ActionListView view = new ActionListView { DataContext = viewModel };
            view.Show();
        }

        private void OnSnapshotMakeStarted(object sender, SnapshotMakeViewModel viewModel)
        {
            SnapshotCreateDialog view = new SnapshotCreateDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OnElementEditStarted(object sender, ElementEditViewModel viewModel)
        {
            ElementEditDialog view = new ElementEditDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OpenModelFile()
        {
            App app = System.Windows.Application.Current as App;
            if ((app != null) && (app.CommandLineArguments.Length == 1))
            {
                string filename = app.CommandLineArguments[0];
                if (filename.EndsWith(".dsm") || filename.EndsWith(".dsi"))
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

        private void OnScreenshotRequested(object sender, System.EventArgs e)
        {
            const int leftMargin = 5;
            const int topMargin = 70;
            const int bottomMargin = 2;
            int width = (int)(Matrix.UsedWidth * _mainViewModel.ActiveMatrix.ZoomLevel) + leftMargin;
            int height = (int)(Matrix.UsedHeight * _mainViewModel.ActiveMatrix.ZoomLevel) + topMargin;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(Matrix);
            Int32Rect rect = new Int32Rect(leftMargin, topMargin, width - leftMargin, height - topMargin - bottomMargin);
            CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, rect);
            Clipboard.SetImage(croppedBitmap);
        }
    }
}
