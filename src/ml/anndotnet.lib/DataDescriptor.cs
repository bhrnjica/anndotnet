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
                colls = Columns.Where(x => x.Type != DataType.None && x.Kind != DataKind.None).ToList();

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
            if (string.IsNullOrEmpty(strMetadata))
                return null;
            

            var columns = strMetadata.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            var labelColumn = columns.Where(x => x.Contains("Label")).FirstOrDefault();
            if (string.IsNullOrEmpty(labelColumn))
                return null;

            return GetColumnClasses(labelColumn);
        }

        public static List<string> GetColumnClasses(string columnData)
        {
            var lst = new List<string>();
            var paramName = columnData.Split(MLFactory.m_ParameterNameSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (paramName == null || paramName.Length != 2)
                return null;


            var par = paramName[1].Split(MLFactory.m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries);

            lst = par.Skip(4).Select(x=>x.Trim(' ')).ToList();
            return lst;
        }
        #endregion
    }
}
