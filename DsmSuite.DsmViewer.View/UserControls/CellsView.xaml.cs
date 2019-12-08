using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.UserControls
{
    /// <summary>
    /// Interaction logic for MatrixView.xaml
    /// </summary>
    public partial class CellsView
    {
        private double _matrixCellSize;
        public CellsView()
        {
            InitializeComponent();
            _matrixCellSize = (double) FindResource("MatrixCellSize");
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnCellMouseDown(object sender, MouseButtonEventArgs e)
        {
            ElementViewModel consumer = GetConsumer(sender);
            ElementViewModel provider = GetProvider(sender);
            if ((consumer != null) && (provider != null))
            {
                GetViewModel()?.SelectCell(consumer, provider);

                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        break;
                    case MouseButton.Right:
                        break;
                }
            }
        }

        private void OnCellMouseEnter(object sender, MouseEventArgs e)
        {
            ElementViewModel consumer = GetConsumer(sender);
            ElementViewModel provider = GetProvider(sender);
            if ((consumer != null) && (provider != null))
            {
                GetViewModel()?.HoverCell(consumer, provider);
            }
        }

        private void OnCellMouseLeave(object sender, MouseEventArgs e)
        {
            GetViewModel()?.HoverCell(null, null);
        }

        private static ElementViewModel GetConsumer(object sender)
        {
            TextBlock control = sender as TextBlock;
            CellViewModel selectedItem = control?.DataContext as CellViewModel;
            return selectedItem?.Consumer;
        }

        private static ElementViewModel GetProvider(object sender)
        {
            TextBlock control = sender as TextBlock;
            CellViewModel selectedItem = control?.DataContext as CellViewModel;
            return selectedItem?.Provider;
        }

        private MatrixViewModel GetViewModel()
        {
            return DataContext as MatrixViewModel;
        }
    }
}
