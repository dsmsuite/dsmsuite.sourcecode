using DsmSuite.DsmViewer.ViewModel.Main;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush;
            SearchState searchState = (SearchState)value;
            switch(searchState)
            {
                case SearchState.NoMatch:
                    brush = new SolidColorBrush(Colors.Red);
                    break;
                case SearchState.ManyMatches:
                    brush = new SolidColorBrush(Colors.DarkGray);
                    break;
                case SearchState.OneMatch:
                    brush = new SolidColorBrush(Colors.Black);
                    break;
                default:
                    brush = new SolidColorBrush(Colors.LightGray);
                    break;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
