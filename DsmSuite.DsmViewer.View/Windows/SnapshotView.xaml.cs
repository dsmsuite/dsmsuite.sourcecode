using System.Windows;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for SnapshotView.xaml
    /// </summary>
    public partial class SnapshotView : Window
    {
        public SnapshotView()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
