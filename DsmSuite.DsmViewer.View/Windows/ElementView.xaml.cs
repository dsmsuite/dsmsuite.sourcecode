using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Editing;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for ElementEditView.xaml
    /// </summary>
    public partial class ElementView : Window
    {
        public ElementView()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
