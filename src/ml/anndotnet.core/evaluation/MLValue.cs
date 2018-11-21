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
using CNTK;
using NNetwork.Core;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Set of helper methods for Value extraction
    /// </summary>
    public class MLValue
    {
        /// <summary>
        /// Returns the value from list of values. In case of 1D it returns just a element. Regression
        /// In case of two element, it returns element with higher value. Binary Classification
        /// In case of more than 2 element it returns the index of the greater value. Misclassification
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public static float GetResult(IList<float> rowData)
        {

            //parse output and return appropriate value based on problem type
            if (rowData.Count > 2)
            {
                float lValue = rowData.IndexOf(rowData.Max());
                return lValue;
            }
            else if (rowData.Count == 2)
            {
                float lValue = rowData.IndexOf(rowData.Max());
                return lValue;
            }
            else
            {
                var lValue = rowData.First();
                return lValue;
            }
        }

        /// <summary>
        /// Returns array of values based on variable and data CNTK objects
        /// </summary>
        /// <param name="var"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<List<float>> GetValues(Variable var, Value data)
        {
            var retVal = new List<List<float>>();
            var vec = data.GetDenseData<float>(var).ToList();
            foreach (var el in vec)
            {
                //
                var n = var.Shape.Dimensions.First();
                for (int i = 0; i < el.Count; i += n)
                {
                    var rows = el.Skip(i).Take(n).ToList();
                    retVal.Add(rows);
                }
            }
            //
            return retVal;
        }
        
        // <summary>
        /// Exports result into csv file format
        /// </summary>
        /// <param name="actualValues"></param>
        /// <param name="predictedValues"></param>
        /// <param name="resultExportPath"></param>
        public static void ValueToFile(List<List<float>> actualValues, List<List<float>> predictedValues, string resultExportPath)
        {
            try
            {
                List<string> lines = new List<string>();
                var actual = "";
                var predicted = "";
                //
                for (int i = 0; i < predictedValues[0].Count; i++)
                {
                    actual += $"actual{i + 1} ";
                    predicted += $"predicted{i + 1} ";
                }
                //   
                lines.Add($"{actual}{predicted}");

                //export and unscaled the values
                for (int i = 0; i < predictedValues.Count; i++)
                {
                    string strActual = "";
                    string strPredicted = "";
                    for (int j = 0; j < predictedValues[0].Count; j++)
                    {
                        strActual += $"{ actualValues[i][j]} ";
                        strPredicted += $"{predictedValues[i][j]} ";
                    }
                    //
                    lines.Add($"{strActual}{strPredicted}");
                }
                //save result to csv file
                File.WriteAllLines(resultExportPath, lines);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
