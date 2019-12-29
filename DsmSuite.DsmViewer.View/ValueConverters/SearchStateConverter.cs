using DsmSuite.DsmViewer.ViewModel.Main;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class SearchStateConverter : IValueConverter
    {
        public SolidColorBrush NoMatchBrush { get; set; }
        public SolidColorBrush OneMatchBrush { get; set; }
        public SolidColorBrush ManyMatchesBrush { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush;
            SearchState searchState = (SearchState)value;
            switch(searchState)
            {
                case SearchState.NoMatch:
                    brush = NoMatchBrush;
                    break;
                case SearchState.OneMatch:
                    brush = OneMatchBrush;
                    break;
                case SearchState.ManyMatches:
                    brush = ManyMatchesBrush;
                    break;
                default:
                    brush = NoMatchBrush;
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
