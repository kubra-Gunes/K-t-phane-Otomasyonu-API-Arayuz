// KutuphaneOtomasyonu.WPF.Converters/BooleanToTextMultiConverter.cs

using System;
using System.Globalization;
using System.Windows.Data;

namespace KutuphaneOtomasyonu.WPF.Converters
{
    public class BooleanToTextMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 && values[0] is bool isOverdue)
            {
                string overdueText = values[1]?.ToString();
                string onTimeText = values[2]?.ToString();

                return isOverdue ? overdueText : onTimeText;
            }

            return "Bilinmiyor";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}