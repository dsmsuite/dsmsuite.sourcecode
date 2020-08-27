using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DsmSuite.DsmViewer.View.ValueConverters
{
    public class MatrixSizeToVisbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int matrixSize = (int)value;
            return (matrixSize > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
