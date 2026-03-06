// KutuphaneOtomasyonu.WPF.Converters/BooleanToTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace KutuphaneOtomasyonu.WPF.Converters
{
    public class BooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is object[] parameters && parameters.Length >= 3)
            {
                // MultiBinding'den gelen değerler
                bool isOverdue = (bool)parameters[0];
                string trueText = parameters[1]?.ToString() ?? "Evet";
                string falseText = parameters[2]?.ToString() ?? "Hayır";

                return isOverdue ? trueText : falseText;
            }
            else if (value is bool singleBoolValue)
            {
                // Tek bir boolean değer ve parametre varsa
                if (parameter is string paramString)
                {
                    string[] texts = paramString.Split('|');
                    if (texts.Length == 2)
                    {
                        return singleBoolValue ? texts[0] : texts[1];
                    }
                }
                return singleBoolValue ? "Evet" : "Hayır"; // Varsayılan
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}