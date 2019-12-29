using System.Windows;
using System.Windows.Controls;
using DsmSuite.DsmViewer.ViewModel.Matrix;


namespace DsmSuite.DsmViewer.View.Matrix
{
    /// <summary>
    /// Interaction logic for MatrixRowMetricsSelectorView.xaml
    /// </summary>
    public partial class MatrixRowMetricsSelectorView
    {
        private MatrixViewModel _viewModel;

        public MatrixRowMetricsSelectorView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as MatrixViewModel;
        }
    }
}
