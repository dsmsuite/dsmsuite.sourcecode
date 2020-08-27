using System.Windows.Controls;

namespace DsmSuite.DsmViewer.View.Matrix
{
    /// <summary>
    /// Interaction logic for MatrixView.xaml
    /// </summary>
    public partial class MatrixView
    {
        public MatrixView()
        {
            InitializeComponent();
        }

        public double UsedWidth => RowHeaderView.ActualWidth + IndicatorView.ActualWidth + Splitter.ActualWidth + /*RowMetricsView.ActualWidth*/ + CellsView.UsedWidth;

        public double UsedHeight => ColumnHeaderView.ActualHeight + CellsView.UsedHeight;

        private void CellsViewOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Canvas.SetLeft(ColumnHeaderView, -e.HorizontalOffset);
            Canvas.SetTop(RowHeaderView, -e.VerticalOffset);
            Canvas.SetTop(IndicatorView, -e.VerticalOffset);
            Canvas.SetTop(RowMetricsView, -e.VerticalOffset);
        }
    }
}
