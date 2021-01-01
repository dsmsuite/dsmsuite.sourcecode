
using System.Windows;

namespace DsmSuite.DsmViewer.View.Editing
{
    /// <summary>
    /// Interaction logic for ElementEditTypeDialog.xaml
    /// </summary>
    public partial class ElementEditTypeDialog
    {
        public ElementEditTypeDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
