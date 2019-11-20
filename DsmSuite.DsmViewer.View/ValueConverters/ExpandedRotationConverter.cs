using System;
using System.Globalization;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class ExpandedRotationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool expanded = (bool) value;
            return expanded ? -90 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
