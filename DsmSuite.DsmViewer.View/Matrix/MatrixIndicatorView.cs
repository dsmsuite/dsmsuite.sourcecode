using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Matrix;
using System.Windows.Input;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixIndicatorView : MatrixFrameworkElement
    {
        private MatrixViewModel _viewModel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredRow;
        private readonly double _pitch;
        private readonly double _offset;

        public MatrixIndicatorView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(5.0, _theme.MatrixCellSize));
            _hoveredRow = null;
            _pitch = _theme.MatrixCellSize + 2.0;
            _offset = 1.0;

            DataContextChanged += OnDataContextChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as MatrixViewModel;
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnPropertyChanged;
                InvalidateVisual();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            if (_hoveredRow != row)
            {
                _hoveredRow = row;
                _viewModel.HoverRow(row);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.HoverColumn(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            _viewModel.SelectRow(row);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(MatrixViewModel.MatrixSize)) ||
                (e.PropertyName == nameof(MatrixViewModel.HoveredColumn)) ||
                (e.PropertyName == nameof(MatrixViewModel.SelectedColumn)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_viewModel != null)
            {
                int matrixSize = _viewModel.MatrixSize;
                for (int row = 0; row < matrixSize; row++)
                {
                    _rect.X = 0;
                    _rect.Y = _offset + row * _pitch;

                    bool isHovered = _viewModel.HoveredRow.HasValue && (row == _viewModel.HoveredRow.Value);
                    bool isSelected = _viewModel.SelectedRow.HasValue && (row == _viewModel.SelectedRow.Value);
                    MatrixColor color = _viewModel.ColumnColors[row];
                    SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                    if (_viewModel.RowIsConsumer[row])
                    {
                        if (_viewModel.RowIsProvider[row])
                        {
                            dc.DrawRectangle(_theme.MatrixColorHierarchicalCycle, null, _rect);
                        }
                        else
                        {
                            dc.DrawRectangle(_theme.MatrixColorConsumer, null, _rect);
                        }
                    }
                    else
                    {
                        if (_viewModel.RowIsProvider[row])
                        {
                            dc.DrawRectangle(_theme.MatrixColorProvider, null, _rect);
                        }
                        else
                        {
                            dc.DrawRectangle(background, null, _rect);
                        }
                    }
                }

                Height = _theme.MatrixCellSize * matrixSize + 2.0;
                Width = 5.0;
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }
    }
}
