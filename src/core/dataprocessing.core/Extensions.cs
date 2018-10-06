//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool                                                  //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See license section of  https://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataProcessing.Core
{
    public static class Extensions
    {
        
        public static string Description(this Enum This)
        {
            Type type = This.GetType();

            string name = Enum.GetName(type, This);

            MemberInfo member = type.GetMembers()
                .Where(w => w.Name == name)
                .FirstOrDefault();

            DescriptionAttribute attribute = member != null
                ? member.GetCustomAttributes(true)
                    .Where(w => w.GetType() == typeof(DescriptionAttribute))
                    .FirstOrDefault() as DescriptionAttribute
                : null;

            return attribute != null ? attribute.Description : name;
        }

        /// <summary>
        /// Row based dataset convert into column based dataset
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[][] toColumnVector<T>(this T[][] input)
        {
            var colVecData = new List<T[]>();
            for (int j = 0; j < input[0].Length; j++)
            {
                var col = new T[input.Length];

                for (int i = 0; i < input.Length; i++)
                {
                    col[i] = input[i][j];

                }
                colVecData.Add(col);
            }
            return colVecData.ToArray();
        }
        /// <summary>
        /// Row based dataset convert into column based dataset
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[][] toColumnVector<T>(this List<List<T>> input)
        {
            var colVecData = new List<T[]>();
            for (int j = 0; j < input[0].Count; j++)
            {
                var col = new T[input.Count];

                for (int i = 0; i < input.Count; i++)
                {
                    col[i] = input[i][j];

                }
                colVecData.Add(col);
            }
            return colVecData.ToArray();
        }
    }
}
