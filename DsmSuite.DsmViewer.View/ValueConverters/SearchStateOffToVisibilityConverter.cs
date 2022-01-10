using DsmSuite.DsmViewer.ViewModel.Main;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchStateOffToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SearchState searchState = (SearchState)value;
            return (searchState == SearchState.Off) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
