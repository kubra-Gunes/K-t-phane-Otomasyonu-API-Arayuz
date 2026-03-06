// KutuphaneOtomasyonu.WPF/Converters/StringToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KutuphaneOtomasyonu.WPF.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Eğer string boş veya sadece boşluk ise (TextBox Tag metnini göstermek için), Visible döndür
            if (value is string text && string.IsNullOrWhiteSpace(text))
            {
                return Visibility.Visible;
            }
            // Eğer string dolu ise (TextBox boş değilse), Collapsed döndür (Tag metnini gizle)
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}