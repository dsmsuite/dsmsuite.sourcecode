using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for RowHeaderView.xaml
    /// </summary>
    public partial class RowHeaderView : UserControl
    {
        public RowHeaderView()
        {
            InitializeComponent();
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnRowMouseDown(object sender, MouseButtonEventArgs e)
        {
            ElementViewModel provider = GetProvider(sender);
            if (provider != null)
            {
                GetViewModel()?.SelectProvider(provider);
            }
        }

        private void OnRowMouseEnter(object sender, MouseEventArgs e)
        {
            ElementViewModel provider = GetProvider(sender);
            if (provider != null)
            {
                GetViewModel()?.HoverProvider(provider);
            }
        }

        private void OnRowMouseLeave(object sender, MouseEventArgs e)
        {
            GetViewModel()?.HoverProvider(null);
        }

        private static ElementViewModel GetProvider(object sender)
        {
            StackPanel control = sender as StackPanel;
            return control?.DataContext as ElementViewModel;
        }

        private MatrixViewModel GetViewModel()
        {
            return DataContext as MatrixViewModel;
        }
    }
}
