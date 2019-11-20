using System;
using System.Globalization;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class DependencyWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int weight = (int) value;
            if (weight > 0)
            {
                return weight.ToString();
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
