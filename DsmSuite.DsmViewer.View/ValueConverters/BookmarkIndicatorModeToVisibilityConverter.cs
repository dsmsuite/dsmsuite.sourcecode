using DsmSuite.DsmViewer.ViewModel.Main;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class BookmarkIndicatorModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IndicatorViewMode viewMode = (IndicatorViewMode)value;
            return (viewMode == IndicatorViewMode.Bookmarks) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
