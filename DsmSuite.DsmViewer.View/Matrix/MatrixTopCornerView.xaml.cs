using System.Windows;
using System.Windows.Controls;
using DsmSuite.DsmViewer.ViewModel.Matrix;


namespace DsmSuite.DsmViewer.View.Matrix
{
    /// <summary>
    /// Interaction logic for MatrixTopCornerView.xaml
    /// </summary>
    public partial class MatrixTopCornerView
    {
        private MatrixViewModel _viewModel;

        public MatrixTopCornerView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as MatrixViewModel;
        }

        private void OnClearSelection(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectCell(null, null);
            _viewModel.SelectTreeItem(null);
        }
    }
}
