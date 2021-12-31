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
using DataProcessing.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataProcessing.Wnd
{
    /// <summary>
    /// Class implements export ANNdotNET Model configuration to Excel. The class exporta Training and Validation datasets to excel-
    /// </summary>
    public static class ExportData
    {
        public static DataFrame WriteFiles (string fileName, (bool cntk, bool randomize, char delimiter, int testRows, bool precentige) options, ANNDataSet fulldata)
        {
            try
            {

                //get dataset based on options 
                var ds = fulldata.GetDataSet(options.randomize);
                ds.RowsToValidation = options.testRows;
                ds.IsPrecentige = options.precentige;
                //create experiment based created dataset
                var exp = new DataFrame(ds);

                //saving processed data in to file
                var dirPath = Path.GetDirectoryName(fileName);
                var name = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var trainPath = Path.Combine(dirPath, name + "_train" + ext);
                var testPath = Path.Combine(dirPath, name + "_test" + ext);

                exp.WriteToFile(trainPath, false, options.delimiter, options.cntk);
                if (options.testRows > 0)
                    exp.WriteToFile(testPath, true, options.delimiter, options.cntk);

                return exp;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool MakeDataSets (string fileName, (bool cntk, bool randomize, char delimiter, int testRows, bool precentige) options, ANNDataSet fulldata)
        {
            try
            {

                //get dataset based on options 
                var ds = fulldata.GetDataSet(options.randomize);
                ds.RowsToValidation = options.testRows;
                ds.IsPrecentige = options.precentige;
                //create experiment based created dataset
                var exp = new DataFrame(ds);

                //saving processed data in to file
                var dirPath = Path.GetDirectoryName(fileName);
                var name = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var trainPath = Path.Combine(dirPath, name + "_train" + ext);
                var testPath = Path.Combine(dirPath, name + "_valid" + ext);

                exp.WriteToFile(trainPath, false, options.delimiter, options.cntk);
                if (options.testRows > 0)
                    exp.WriteToFile(testPath, true, options.delimiter, options.cntk);

                return true;

            }
            catch (Exception )
            {

                throw;
            }
        }
        public static bool MakeAnnDataSets(string fileName, (bool cntk, bool randomize, char delimiter, int testRows, bool precentige) options, ANNDataSet fulldata)
        {
            try
            {

                //get dataset based on options 
                var ds = fulldata.GetDataSet(options.randomize);
                ds.RowsToValidation = options.testRows;
                ds.IsPrecentige = options.precentige;
                //create experiment based created dataset
                var exp = new DataFrame(ds);

                //saving processed data in to file
                var dirPath = Path.GetDirectoryName(fileName);
                var name = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var trainPath = Path.Combine(dirPath, name + "_train" + ext);
                var testPath = Path.Combine(dirPath, name + "_valid" + ext);

                exp.WriteToFile(trainPath, false, options.delimiter, options.cntk);
                if (options.testRows > 0)
                    exp.WriteToFile(testPath, true, options.delimiter, options.cntk);

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> PrepareDataSet(DataFrame exp)
        {
            
            List<string> strData = new List<string>();
            //
            var cols = exp.GetColumns();
            for(int i=0; i<exp.GetRowCount(); i++)
            {
                string line ="";
                string numValues = "";
                //first write numeric columns
                var numCols = cols.Where(x=>x.ColumnDataType== ColumnType.Numeric && x.IsOutput==false).ToList();
                for (int j = 0; j < numCols.Count; j++)
                {
                    var col = numCols[j];
                    numValues +=col.GetData(i)+" ";
                }
                if (numCols.Count > 0)
                    line = $"|NumFeatures {numValues}";
                //then for each column make a new group
                var catCols = cols.Where(x => x.ColumnDataType == ColumnType.Category && x.IsOutput == false).ToList();
                for (int j = 0; j < catCols.Count; j++)
                {
                    var col = catCols[j];
                    col.Name = col.Name.Trim();
                    var str = $"|{col.Name} {string.Join(" ", col.GetEncodedValue(i))} ";
                    line += str;
                }

                //then write label column
                var labCols = cols.Where(x => x.IsOutput == true).ToList();
                for (int j = 0; j < labCols.Count; j++)
                {
                    var col = labCols[j];
                    col.Name = col.Name.Trim();
                    if (col.ColumnDataType== ColumnType.Category)
                    {
                        var str = $"|{col.Name} {string.Join(" ", col.GetEncodedValue(i))} ";
                        line += str;
                    }
                    else
                    {
                        var str = $"|{col.Name} {col.GetData(i)} ";
                        line += str;
                    }  
                }

                strData.Add(line);
            }
            return strData;
        }
    }
}
