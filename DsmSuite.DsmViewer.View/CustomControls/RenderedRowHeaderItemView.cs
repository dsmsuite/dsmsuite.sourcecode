using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.CustomControls
{
    public class RenderedRowHeaderItemView : MatrixFrameworkElement
    {
        private static readonly string BlackRightPointingTriangle = '\u25B6'.ToString();
        private static readonly string BlackDownPointingTriangle = '\u25BC'.ToString();
        private static readonly FormattedText BlackRightPointingTriangleFormattedText;
        private static readonly FormattedText BlackDownPointingTriangleFormattedText;

        private readonly RenderTheme _renderTheme;
        private ElementTreeItemViewModel _viewModel;

        static RenderedRowHeaderItemView()
        {
            BlackRightPointingTriangleFormattedText = new FormattedText(BlackRightPointingTriangle,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                10,
                Brushes.Black);

            BlackDownPointingTriangleFormattedText = new FormattedText(BlackDownPointingTriangle,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                10,
                Brushes.Black);
        }

        public RenderedRowHeaderItemView(RenderTheme renderTheme)
        {
            _renderTheme = renderTheme;

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ElementTreeItemViewModel;
            if (_viewModel != null)
            {
                ToolTip = _viewModel.Description;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(ElementTreeItemViewModel.IsHovered)) ||
                (e.PropertyName == nameof(ElementTreeItemViewModel.IsSelected)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if ((_viewModel != null) && (ActualWidth > 2.0) && (ActualHeight > 2.0))
            {
                SolidColorBrush background = _renderTheme.GetBackground(_viewModel.Color, false, _viewModel.IsHovered,
                    _viewModel.IsSelected);
                Rect backgroundRect = new Rect(1.0, 1.0, ActualWidth - 2.0, ActualHeight - 2.0);
                dc.DrawRectangle(background, null, backgroundRect);

                string text = _viewModel.Name + " - " + _viewModel.Order;
                if (_viewModel.IsExpanded)
                {
                    Point textLocation = new Point(backgroundRect.X + 10.0, backgroundRect.Y - 20.0);
                    DrawRotatedText(dc, textLocation, text, backgroundRect.Height - 20.0);
                }
                else
                {
                    Point textLocation = new Point(backgroundRect.X + 20.0, backgroundRect.Y + 15.0);
                    DrawText(dc, textLocation, text, backgroundRect.Width - 25.0);
                }

                Point expanderLocation = new Point(backgroundRect.X + 1.0, backgroundRect.Y + 1.0);
                DrawExpander(dc, expanderLocation);
            }
        }

        private void DrawExpander(DrawingContext dc, Point location)
        {
            if (_viewModel.IsExpandable)
            {
                dc.DrawText(
                    _viewModel.IsExpanded
                        ? BlackDownPointingTriangleFormattedText
                        : BlackRightPointingTriangleFormattedText, location);
            }
        }
    }
}