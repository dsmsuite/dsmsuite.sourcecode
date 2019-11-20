using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public static class SolidColorBrushMultiplier
    {
        public static SolidColorBrush Multiply(this SolidColorBrush color, double multiplicationFactor)
        {
            float factor = (float)multiplicationFactor;
            return new SolidColorBrush(Color.Multiply(color.Color, factor));
        }
    }
}
