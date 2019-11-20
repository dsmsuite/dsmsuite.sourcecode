using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for ColumnHeaderView.xaml
    /// </summary>
    public partial class ColumnHeaderView : UserControl
    {
        public ColumnHeaderView()
        {
            InitializeComponent();
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnColumnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ElementViewModel consumer = GetConsumer(sender);
            if (consumer != null)
            {
                GetViewModel()?.SelectConsumer(consumer);
            }
        }
        
        private void OnColumnMouseEnter(object sender, MouseEventArgs e)
        {
            ElementViewModel consumer = GetConsumer(sender);
            if (consumer != null)
            {
                GetViewModel()?.HoverConsumer(consumer);
            }
        }

        private void OnColumnMouseLeave(object sender, MouseEventArgs e)
        {
            GetViewModel()?.HoverConsumer(null);
        }

        private static ElementViewModel GetConsumer(object sender)
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
