using System.Windows;

namespace DsmSuite.DsmViewer.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ElementCreateDialog.xaml
    /// </summary>
    public partial class ElementCreateDialog
    {
        public ElementCreateDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
