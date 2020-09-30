using System.Windows;

namespace DsmSuite.DsmViewer.View.Settings
{
    /// <summary>
    /// Interaction logic for SearchSettingsDialog.xaml
    /// </summary>
    public partial class SearchSettingsView 
    {
        public SearchSettingsView()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
