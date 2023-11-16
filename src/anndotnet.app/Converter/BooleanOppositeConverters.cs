using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using Daany;


namespace Anndotnet.App.Converter
{
    public class BooleanOppositeConverters : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || (bool)value == false)
            {
                return true;
            }
            
            else
            {
                return false;

            }
            
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
           return null;
        }
    }
}
