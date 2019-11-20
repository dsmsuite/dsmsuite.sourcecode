using System.Windows;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.CustomControls
{
    public class RenderedColumHeaderItemView : MatrixFrameworkElement
    {
        private readonly RenderTheme _renderTheme;
        private readonly int _column;
        private ElementViewModel _viewModel;
        private Rect _rect;

        public RenderedColumHeaderItemView(RenderTheme renderTheme, int column)
        {
            _renderTheme = renderTheme;
            _column = column;
            _rect = new Rect(new Size(_renderTheme.MatrixCellSize, _renderTheme.MatrixHeaderHeight));
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ElementViewModel;
            if (_viewModel != null)
            {
                ToolTip = _viewModel.Description;

                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(ElementViewModel.IsHovered)) || 
                (e.PropertyName == nameof(ElementViewModel.IsSelected)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_viewModel != null)
            {
                double pitch = _renderTheme.MatrixCellSize + 2.0;
                _rect.X = 1.0 + _column * pitch;
                _rect.Y = 0;

                SolidColorBrush background = _renderTheme.GetBackground(_viewModel.Color, false, _viewModel.IsHovered, _viewModel.IsSelected);
                dc.DrawRectangle(background, null, _rect);

                Point location = new Point(_rect.X + 10.0, _rect.Y - 5.0);
                DrawRotatedText(dc, location, _viewModel.Order.ToString(), _rect.Height - 2.0);
            }
        }
    }
}


