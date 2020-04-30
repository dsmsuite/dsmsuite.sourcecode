using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixCellsView : MatrixFrameworkElement
    {
        private MatrixViewModel _viewModel;
        private readonly MatrixTheme _theme;
        private Rect _rect;
        private int? _hoveredRow;
        private int? _hoveredColumn;
        private readonly double _pitch;
        private readonly double _offset;
        private readonly double _verticalTextOffset = 16.0;

        public MatrixCellsView()
        {
            _theme = new MatrixTheme(this);
            _rect = new Rect(new Size(_theme.MatrixCellSize, _theme.MatrixCellSize));
            _hoveredRow = null;
            _hoveredColumn = null;
            _pitch = _theme.MatrixCellSize + _theme.SpacingWidth;
            _offset = _theme.SpacingWidth / 2;

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
            int column = GetHoveredColumn(e.GetPosition(this));
            if ((_hoveredRow != row) || (_hoveredColumn != column))
            {
                _hoveredRow = row;
                _hoveredColumn = column;
                _viewModel.HoverCell(row, column);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.HoverCell(null, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int row = GetHoveredRow(e.GetPosition(this));
            int column = GetHoveredColumn(e.GetPosition(this));
            _viewModel.SelectCell(row, column);
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewModel.CellTooltip))
            {
                ToolTip = _viewModel.CellTooltip;
            }

            if ((e.PropertyName == nameof(MatrixViewModel.MatrixSize)) ||
                (e.PropertyName == nameof(MatrixViewModel.HoveredRow)) ||
                (e.PropertyName == nameof(MatrixViewModel.SelectedRow)) ||
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
                    for (int column = 0; column < matrixSize; column++)
                    {
                        _rect.X = _offset + column * _pitch;
                        _rect.Y = _offset + row * _pitch;

                        bool isHovered = (_viewModel.HoveredRow.HasValue && (row == _viewModel.HoveredRow.Value)) ||
                                         (_viewModel.HoveredColumn.HasValue && (column == _viewModel.HoveredColumn.Value));
                        bool isSelected = (_viewModel.SelectedRow.HasValue && (row == _viewModel.SelectedRow.Value)) ||
                                          (_viewModel.SelectedColumn.HasValue && (column == _viewModel.SelectedColumn.Value));
                        MatrixColor color = _viewModel.CellColors[row][column];
                        SolidColorBrush background = _theme.GetBackground(color, isHovered, isSelected);

                        dc.DrawRectangle(background, null, _rect);

                        int weight = _viewModel.CellWeights[row][column];
                        if (weight > 0)
                        {
                            char infinity = '\u221E';
                            string content;
                            if (weight > 9999)
                            {
                                content = infinity.ToString();
                            }
                            else
                            {
                                content = weight.ToString();
                            }

                            double textWidth = MeasureText(content);

                            Point location = new Point
                            {
                                X = (column * _pitch) + (_pitch - textWidth) / 2,
                                Y = (row * _pitch) + _verticalTextOffset
                            };
                            DrawText(dc, content, location, _theme.TextColor,_rect.Width - _theme.SpacingWidth);
                        }
                    }
                }
                Height = _pitch * matrixSize + _theme.ScrollBarWidth;
                Width = Height; 
            }
        }

        private int GetHoveredRow(Point location)
        {
            double row = (location.Y - _offset) / _pitch;
            return (int)row;
        }

        private int GetHoveredColumn(Point location)
        {
            double column = (location.X - _offset) / _pitch;
            return (int)column;
        }
    }
}
