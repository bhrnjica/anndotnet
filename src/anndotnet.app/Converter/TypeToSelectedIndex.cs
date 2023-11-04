using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using Daany;


namespace Anndotnet.App.Converter
{
    public class TypeToSelectedIndex : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value.ToString()=="String")
            {
                return 3;
            }
            else if (value.ToString() == "Categorical")
            {
                return 2;
            }
            else if (value.ToString() == "Float")
            {
                return 1;
            }
            else
            {
                return 0;

            }
            
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value.ToString()=="3")
            {
                return "String";
            }

            else if (value.ToString() == "2")
            {
                return "Categorical";
            }
            else if (value.ToString() == "1")
            {
                return "Float";
            }
            else
            {
                return "Integer";
            }
        }
    }
}
