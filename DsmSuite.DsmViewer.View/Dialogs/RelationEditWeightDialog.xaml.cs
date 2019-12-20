using System.Windows;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for RelationEditWeightDialog.xaml
    /// </summary>
    public partial class RelationEditWeightDialog
    {
        public RelationEditWeightDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
