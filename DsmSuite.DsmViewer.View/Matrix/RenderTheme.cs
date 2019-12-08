using System.Windows;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class RenderTheme
    {
        private SolidColorBrush[] _brushes;
        private readonly FrameworkElement _frameworkElement;

        public RenderTheme(FrameworkElement frameworkElement)
        {
            _frameworkElement = frameworkElement;
            MatrixCellSize = (double)_frameworkElement.FindResource("MatrixCellSize");
            MatrixHeaderHeight = (double)_frameworkElement.FindResource("MatrixHeaderHeight");
        }

        public double MatrixCellSize { get; }
        public double MatrixHeaderHeight { get; }
        public SolidColorBrush GetBackground(int color, bool cyclic, bool isHovered, bool isSelected)
        {
            UpdateBrushes();

            int index = cyclic ? 16 : (color * 4);

            if (isHovered)
            {
                index += 1;
            }

            if (isSelected)
            {
                index += 2;
            }

            return _brushes[index];
        }

        private void UpdateBrushes()
        {
            if (_brushes == null)
            {
                _brushes = new SolidColorBrush[20];

                SolidColorBrush brush1 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor1");
                SolidColorBrush brush2 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor2");
                SolidColorBrush brush3 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor3");
                SolidColorBrush brush4 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor4");
                SolidColorBrush brushCyclic = (SolidColorBrush)_frameworkElement.FindResource("MatrixColorCyclic");
                double highlightFactorHovered = (double)_frameworkElement.FindResource("HighlightFactorHovered");
                double highlightFactorSelected = (double)_frameworkElement.FindResource("HighlightFactorSelected");

                SetBrush(0, brush1, highlightFactorHovered, highlightFactorSelected);
                SetBrush(1, brush2, highlightFactorHovered, highlightFactorSelected);
                SetBrush(2, brush3, highlightFactorHovered, highlightFactorSelected);
                SetBrush(3, brush4, highlightFactorHovered, highlightFactorSelected);
                SetBrush(4, brushCyclic, highlightFactorHovered, highlightFactorSelected);
            }
        }

        private void SetBrush(int colorIndex, SolidColorBrush brush, double highlightFactorHovered, double highlightFactorSelected)
        {
            int index = colorIndex*4;
            _brushes[index] = brush;
            _brushes[index+1] = GetHighlightBrush(brush, highlightFactorHovered);
            _brushes[index+2] = GetHighlightBrush(brush, highlightFactorSelected);
            _brushes[index+3] = GetHighlightBrush(brush, highlightFactorHovered * highlightFactorSelected);
        }

        public static SolidColorBrush GetHighlightBrush(SolidColorBrush color, double multiplicationFactor)
        {
            float factor = (float)multiplicationFactor;
            return new SolidColorBrush(Color.Multiply(color.Color, factor));
        }
    }
}
