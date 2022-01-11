using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchElementFoundToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IDsmElement foundElememt = (IDsmElement)value;
            return (foundElememt != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
