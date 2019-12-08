using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Editing;

namespace DsmSuite.DsmViewer.View.Windows
{
    /// <summary>
    /// Interaction logic for ElementCreateView.xaml
    /// </summary>
    public partial class ElementCreateView : Window
    {
        public ElementCreateView()
        {
            InitializeComponent();
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
