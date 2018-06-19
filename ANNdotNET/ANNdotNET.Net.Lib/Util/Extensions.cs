//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET.Net.Lib.Util
{
    public static class Extensions
    {
        /// <summary>
        /// Parses specific datatime string including time zone
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <param name="tryFormat"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(this string strDateTime, string tryFormat)
        {           
            if (DateTime.TryParseExact(strDateTime, tryFormat /*"yyyy-MM-dd HH:mm:ss"*/, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeLocal, out DateTime dateTime))
            {
                return dateTime;
            }
            else//try to parse date with time zone
            {
                var strDate= strDateTime.Substring(0,19);
                var zone = strDateTime.Substring(19);
                if (DateTime.TryParseExact(strDate, tryFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeLocal, out DateTime dateTime1))
                {
                    var zz = zone.Substring(1);
                    var timeOfDay = TimeSpan.ParseExact(zz,"hh\\:mm", CultureInfo.InvariantCulture);
                    if(zone[0]=='-')
                    {
                        return (dateTime1 + timeOfDay);
                    }
                    else
                    {
                        return (dateTime1 - timeOfDay);
                    }
                }
                else//try to parse date with time zone
                {

                    //we put min date in order to skip this row caused with invalid date
                    return DateTime.MinValue;
                }
               
            }
        }
    }
}
