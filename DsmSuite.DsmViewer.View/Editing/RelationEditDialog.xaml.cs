using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.View.Editing
{
    /// <summary>
    /// Interaction logic for RelationEditTypeDialog.xaml
    /// </summary>
    public partial class RelationEditDialog
    {
        public RelationEditDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
