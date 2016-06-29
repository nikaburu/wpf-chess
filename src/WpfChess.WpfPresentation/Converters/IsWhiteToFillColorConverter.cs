using System;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfChess.WpfPresentation.Converters
{
    class IsWhiteToFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
