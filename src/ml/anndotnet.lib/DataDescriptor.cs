//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
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
using ANNdotNET.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ANNdotNET.Lib
{
    /// <summary>
    /// Class implement DataDescriptor one of few ANNdotNET Project Components. It holds information about
    /// Raw dataset path, parser for dataset, and meta-data for the columns.
    /// </summary>
    public class DataDescriptor
    {
        #region Properties
        public DataParser Parser { get; set; }

        public string DataPath { get; set; }

        public List<VariableDescriptor> Columns { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns string of metadata information 
        /// </summary>
        /// <param name="excludeNone">return only valid column and ignore unknown</param>
        /// <returns></returns>
        public string ToMetadataString(bool excludeNone)
        {
            if (Columns == null)
                return "";
            var strColumns = "";
            int intCounter = 1;

            var colls = Columns;
            if (excludeNone)
                colls = Columns.Where(x => x.Type != MLDataType.None && x.Kind != DataKind.None).ToList();

            foreach (var c in colls)
            {
                var strCounter = "";
                if (intCounter < 10)
                    strCounter = $"0{intCounter}";
                else
                    strCounter = $"{intCounter}";
                //store categories in case of category column
                var strClasses = c.Classes == null ? "" : string.Join(";", c.Classes);
                strColumns += $"|Column{strCounter}:{c.Name};{c.Type};{c.Kind};{c.MissingValue};{strClasses} ";
                intCounter++;
            }

            return strColumns;
        }


        public static List<string> GetOutputClasses(string strMetadata)
        {
            return MLFactory.GetOutputClasses(strMetadata);
        }

        public static List<string> GetColumnClasses(string columnData)
        {
            return MLFactory.GetColumnClasses(columnData);
        }
        #endregion
    }
}
