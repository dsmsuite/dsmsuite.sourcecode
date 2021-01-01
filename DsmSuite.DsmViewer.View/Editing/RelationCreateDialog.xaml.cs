using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.View.Editing
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
