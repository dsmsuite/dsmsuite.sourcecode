using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class CellColorConverter : IMultiValueConverter
    {
        private readonly SolidColorBrush[] _brushes =  new SolidColorBrush[5];

        public SolidColorBrush Brush1
        {
            set { _brushes[0] = value; }
            get { return _brushes[0]; }
        }
        public SolidColorBrush Brush2
        {
            set { _brushes[1] = value; }
            get { return _brushes[1]; }
        }
        public SolidColorBrush Brush3
        {
            set { _brushes[2] = value; }
            get { return _brushes[2]; }
        }
        public SolidColorBrush Brush4
        {
            set { _brushes[3] = value; }
            get { return _brushes[3]; }
        }

        public SolidColorBrush BrushCyclic { get; set; }

        public double HighlightFactorHovered { get; set; }
        public double HighlightFactorSelected { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.White);

            if (values.Length == 4)
            {
                int colorIndex = (int)values[0];
                bool selected = (bool)values[1];
                bool hovered = (bool)values[2];
                bool cylic = (bool)values[3];

                brush = cylic ? BrushCyclic : _brushes[colorIndex];

                if (hovered)
                {
                    brush = brush.Multiply(HighlightFactorHovered);
                }

                if (selected)
                {
                    brush = brush.Multiply(HighlightFactorSelected);
                }
            }

            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
