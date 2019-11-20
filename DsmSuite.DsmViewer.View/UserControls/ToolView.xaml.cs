using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Main;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for ControlAndInfoView.xaml
    /// </summary>
    public partial class ToolView : UserControl
    {
        private MainViewModel _mainViewModel;

        public ToolView()
        {
            InitializeComponent();
        }

        private void ToolView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainViewModel = this.DataContext as MainViewModel;
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

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.AddExtension = true;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "dsm";
            dlg.Filter = "DSM project files (*.dsm)|*.dsm|All files (*.*)|*.*";
            dlg.Title = "Open DSM project";

            bool? result = dlg.ShowDialog();

            if (result.HasValue && result.Value)
            {
                selectedFile = dlg.FileName;
            }

            return selectedFile;
        }
    }
}
