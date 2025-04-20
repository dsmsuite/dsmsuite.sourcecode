using System.Globalization;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class MetricsExpandedToButtonContentConverter : IValueConverter
    {
        private static readonly string LeftPointingTriangle = '\u25C0'.ToString();
        private static readonly string RightPointingTriangle = '\u25B6'.ToString();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool expanded = (bool)value;
            return expanded ? LeftPointingTriangle : RightPointingTriangle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

