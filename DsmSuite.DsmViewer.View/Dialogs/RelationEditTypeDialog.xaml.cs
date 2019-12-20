using System.Windows;

namespace DsmSuite.DsmViewer.View.Dialogs
{
    /// <summary>
    /// Interaction logic for RelationEditTypeDialog.xaml
    /// </summary>
    public partial class RelationEditTypeDialog
    {
        public RelationEditTypeDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
