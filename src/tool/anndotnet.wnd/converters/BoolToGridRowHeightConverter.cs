//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using NNetwork.Core.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace anndotnet.wnd.converters
{
    [ValueConversion(typeof(bool), typeof(GridLength))]
    public class BoolToGridRowHeightConverter : IValueConverter
    {
        /// <summary>
        /// converter to show or hide corresponded fields specific for each layer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            var type = (LayerType)value;

            int row;
            int.TryParse(parameter.ToString(), out row);
            

            if (row == 1)
            {
                if (type == LayerType.Normalization)
                    return new GridLength(0);
                else if (type != LayerType.Drop)
                    return GridLength.Auto;
                else
                    return new GridLength(0);
            }
            if (row == 2)
            {
                if (type == LayerType.LSTM)
                    return GridLength.Auto;
                else
                    return new GridLength(0);
            }
            if (row == 3)
            {
                if (type == LayerType.Drop)
                    return GridLength.Auto;
                else
                    return new GridLength(0);
            }

            return new GridLength(0);

        }  

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}
