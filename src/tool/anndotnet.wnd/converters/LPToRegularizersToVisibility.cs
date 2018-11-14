using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace anndotnet.wnd.converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class LPToRegularizersToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            if (value.ToString().Equals("AdamLearner", StringComparison.CurrentCultureIgnoreCase) || value.ToString().Equals("FSAdaGradLearner", StringComparison.CurrentCultureIgnoreCase))
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
           
        }  

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}
