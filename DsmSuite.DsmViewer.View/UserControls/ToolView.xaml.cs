using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Main;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for ControlAndInfoView.xaml
    /// </summary>
    public partial class ToolView
    {
        private MainViewModel _mainViewModel;

        public ToolView()
        {
            InitializeComponent();
        }

        private void ToolView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel = DataContext as MainViewModel;
        }

        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            string filename = GetSelectedFile();
            if (_mainViewModel.OpenFileCommand.CanExecute(filename))
            {
                _mainViewModel.OpenFileCommand.Execute(filename);
            }
        }

        private string GetSelectedFile()
        {
            string selectedFile = null;

            OpenFileDialog dlg = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "dsm",
                Filter = "DSM model|*.dsm|DSI import|*.dsi|All Types|*.dsm;*.dsi",
                Title = "Open DSM project"
            };
            
            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = dlg.FileName;
            }

            return selectedFile;
        }
    }
}
