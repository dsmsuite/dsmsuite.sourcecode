using System;
using System.Globalization;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class ExpandableIconConverter : IValueConverter
    {
        private static readonly string Arrow = '\u25BC'.ToString();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool expanded = (bool)value;
            return expanded ? Arrow : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
