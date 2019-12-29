using System.Windows;
using System.Windows.Media;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixTheme
    {
        private SolidColorBrush[] _brushes;
        private readonly FrameworkElement _frameworkElement;

        public MatrixTheme(FrameworkElement frameworkElement)
        {
            _frameworkElement = frameworkElement;
            MatrixCellSize = (double)_frameworkElement.FindResource("MatrixCellSize");
            MatrixHeaderHeight = (double)_frameworkElement.FindResource("MatrixHeaderHeight");
            MatrixInfoViewWidth = (double)_frameworkElement.FindResource("MatrixInfoViewWidth");
            TextColor = (SolidColorBrush)_frameworkElement.FindResource("TextColor");
        }

        public double MatrixCellSize { get; }
        public double MatrixHeaderHeight { get; }
        public double MatrixInfoViewWidth { get; }
        public SolidColorBrush TextColor { get; }

        public SolidColorBrush GetBackground(MatrixColor color, bool isHovered, bool isSelected)
        {
            UpdateBrushes();

            int colorIndex;
            switch (color)
            {
                case MatrixColor.Background:
                    colorIndex = 0;
                    break;
                case MatrixColor.Color1:
                    colorIndex = 4;
                    break;
                case MatrixColor.Color2:
                    colorIndex = 8;
                    break;
                case MatrixColor.Color3:
                    colorIndex = 12;
                    break;
                case MatrixColor.Color4:
                    colorIndex = 16;
                    break;
                case MatrixColor.Highlight:
                    colorIndex = 20;
                    break;
                default:
                    colorIndex = 0;
                    break;
            }

            if (isHovered)
            {
                colorIndex += 1;
            }

            if (isSelected)
            {
                colorIndex += 2;
            }

            return _brushes[colorIndex];
        }

        private void UpdateBrushes()
        {
            if (_brushes == null)
            {
                _brushes = new SolidColorBrush[24];

                SolidColorBrush brushBackground = (SolidColorBrush)_frameworkElement.FindResource("MatrixColorBackground");
                SolidColorBrush brush1 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor1");
                SolidColorBrush brush2 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor2");
                SolidColorBrush brush3 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor3");
                SolidColorBrush brush4 = (SolidColorBrush)_frameworkElement.FindResource("MatrixColor4");
                SolidColorBrush brushCyclic = (SolidColorBrush)_frameworkElement.FindResource("MatrixColorCyclic");
                double highlightFactorHovered = (double)_frameworkElement.FindResource("HighlightFactorHovered");
                double highlightFactorSelected = (double)_frameworkElement.FindResource("HighlightFactorSelected");

                SetBrush(0, brushBackground, highlightFactorHovered, highlightFactorSelected);
                SetBrush(1, brush1, highlightFactorHovered, highlightFactorSelected);
                SetBrush(2, brush2, highlightFactorHovered, highlightFactorSelected);
                SetBrush(3, brush3, highlightFactorHovered, highlightFactorSelected);
                SetBrush(4, brush4, highlightFactorHovered, highlightFactorSelected);
                SetBrush(5, brushCyclic, highlightFactorHovered, highlightFactorSelected);
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
