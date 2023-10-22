using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;


namespace Anndotnet.App.Converter
{
    public class PathToImageConverter : IValueConverter
    {
        private static readonly string? Assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var uri = new Uri($"avares://{Assembly}"+value.ToString());
            return new Bitmap(AssetLoader.Open(uri));

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
