using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using Daany;


namespace Anndotnet.App.Converter
{
    public class MlTypeToSelectedIndex : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value.ToString()=="None")
            {
                return 0;
            }
            else if (value.ToString() == "Feature")
            {
                return 1;
            }
            else
            {
                return 2;

            }
            
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || value.ToString()=="0")
            {
                return "Ignore";
            }

            else if (value.ToString() == "1")
            {
                return "Feature";
            }
            else
            {
                return "Label";
            }
        }
    }
}
