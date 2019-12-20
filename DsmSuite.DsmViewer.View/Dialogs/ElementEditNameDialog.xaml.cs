using System.Windows;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for ElementEditNameDialog.xaml
    /// </summary>
    public partial class ElementEditNameDialog
    {
        public ElementEditNameDialog()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
