using System.Windows;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class RenderedCellView : MatrixFrameworkElement
    {
        private readonly RenderTheme _renderTheme;
        private CellViewModel _viewModel;
        private Rect _rect;

        public RenderedCellView(RenderTheme renderTheme)
        {
            _renderTheme = renderTheme;
            _rect = new Rect(new Size(_renderTheme.MatrixCellSize, _renderTheme.MatrixCellSize));
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as CellViewModel;
            if (_viewModel != null)
            {
                ToolTip = _viewModel.Description;

                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(CellViewModel.IsHovered)) || 
                (e.PropertyName == nameof(CellViewModel.IsSelected)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_viewModel != null)
            {
                double pitch = _renderTheme.MatrixCellSize + 2.0;
                _rect.X = 1.0 + _viewModel.Column * pitch;
                _rect.Y = 1.0 + _viewModel.Row * pitch;
                SolidColorBrush background = _renderTheme.GetBackground(_viewModel.Color, _viewModel.Cyclic,
                                                                        _viewModel.IsHovered, _viewModel.IsSelected);
                dc.DrawRectangle(background, null, _rect);

                if (_viewModel.Weight > 0)
                {
                    Point location = new Point
                    {
                        X = 1.0 + _viewModel.Column*pitch,
                        Y = 14.0 + _viewModel.Row*pitch
                    };
                    DrawText(dc, location, _viewModel.Weight.ToString(), _rect.Width - 2.0);
                }
            }
        }
    }
}
