using System;
using System.Globalization;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class MetricsExpandedToViewWidthConverter : IValueConverter
    {
        public double ViewWidth { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool expanded = (bool)value;
            return expanded ? ViewWidth : 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

