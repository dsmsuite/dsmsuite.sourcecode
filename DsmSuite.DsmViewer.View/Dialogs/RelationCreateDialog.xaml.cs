using System.Windows;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for RelationCreateDialog.xaml
    /// </summary>
    public partial class RelationCreateDialog
    {
        public RelationCreateDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
