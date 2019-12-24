using System;
using System.Windows.Data;
using System.Windows.Media;
using WpfChess.WpfPresentation.Helpers;

namespace WpfChess.WpfPresentation.Converters
{
    public class IsWhiteConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
            {
                return new SolidColorBrush(GameSettingHelper.WhiteColor);
            }

            return new SolidColorBrush(GameSettingHelper.BlackColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
