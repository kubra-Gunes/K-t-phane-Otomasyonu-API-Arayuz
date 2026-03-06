using System;
using System.Globalization;
using System.Windows.Data;

namespace KutuphaneOtomasyonu.WPF.Converters
{
    public class StringToNullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (int.TryParse(input, out int result))
                return result;

            return null; // Hatalı giriş olursa da null döndür.
        }
    }
}
