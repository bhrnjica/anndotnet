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
using System.Text.RegularExpressions;

namespace DataProcessing.Core
{
    public enum MetaData
    {
        [Description("Name:")]
        Name,
        [Description("Type:")]
        CType,
        [Description("Encoding:")]
        Encoding,
        [Description("Variable:")]
        Variable, 
        [Description("Scaling:")]
        Scaling,//binary column values (0,1)
        [Description("Missing value:")]
        MissingValue,//categorical column values    
    }

    //Type of the column data
    public enum ColumnType
    {
        [Description("String")]
        Unknown = 0,
        [Description("None")]
        None,
        [Description("Numeric")]
        Numeric,//continuous column values
        [Description("Binary")]
        Binary,//binary column values (0,1)
        [Description("Category")]
        Category,//categorical column values    
    }
    //Types of categorical column  encoding 
    public enum CategoryEncoding
    {
        [Description("None")] //no encoding
        None,
        [Description("(0,1)")] //binary encoding with 0 and 1
        Binary1,
        [Description("(-1,1)")] //binary encoding with 0 and 1
        Binary2,
        [Description("N")]//for (4 classes) =1,2,3,4 
        Level,
        [Description("1:N")]//one hot vector (4 categories) = (1,0,0);(0,1,0);(0,0,1);
        OneHot,
        [Description("1:N-1(0)")] //category encoding (4 categories) = (1,0,0);(0,1,0);(0,0,1);(0,0,0)
        Dummy1,
        [Description("1:N-1(-1)")] //category encoding (4 categories) = (1,0,0);(0,1,0);(0,0,1);(-1,-1,-1)
        Dummy2,
        
    }

    //Scaling of the numerical column
    public enum Scaling
    {
        [Description("None")]
        None,
        [Description("MinMax")]
        MinMax,
        [Description("Gauss")]
        Gauss,
       
    }

    //Parameter type (obsolute) (for backward compatibility)
    public enum ParameterType
    {
        [Description("Ignored")]
        Ignored, // ignore columns in modeling
        [Description("Input")]
        Input, //-treat column as input parameter or feature
        [Description("Output")]
        Output, // - treat column as output value or label
    }

    //Parameter type
    public enum VariableType
    {
        [Description("None")]
        None, // ignore columns in modeling
        [Description("Feature")]
        Feature, //-treat column as input parameter or feature
        [Description("Label")]
        Label, // - treat column as output value or label
        
    }

    public enum MissingValue
    {
        [Description("None")]
        None,//remove the row from the experiment
        [Description("Ignore")]
        Ignore,//remove the row from the experiment
        [Description("Average")]
        Average,//recalculate the column and put average value in all missing rows
        [Description("Mode")]
        Mode,//recalculate the column and put most frequent value in all missing rows
        [Description("Random")]
        Random,//recalculate the column and put most random value in all missing rows
        [Description("Max")]
        Max,//recalculate the column and put Max value in all missing rows
        [Description("Min")]
        Min //recalculate the column and put Min value in all missing rows
    }

    //Class for defining Column metadata
    public class MetaColumn
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Encoding { get; set; }//encoding category column only
        public string Param { get; set; }
        public string Scale { get; set; }
        public string MissingValue { get; set; }
        public string[] Categories { get; set; }

        public bool IsIngored
        {
            get
            {
                return (
                    Type.IndexOf("none",System.StringComparison.OrdinalIgnoreCase)>=0 ||
                    Type.IndexOf("ignored", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Param.Contains(ParameterType.Ignored.ToString()) ||
                    Param.Contains(VariableType.None.ToString()));
            }
        }

        public override string ToString()
        {
            string retVal = "";
            retVal += Id.ToString() + ";";
            retVal += Index.ToString() + ";";
            retVal += Name + ";";
            retVal += Type + ";";
            retVal += Param + ";";
            retVal += Scale + ";";
            retVal += MissingValue;

            return retVal;
        }

    }

    //Statistic for Column
    public class Statistics
    {
        public double Mean;
        public double Median;
        public double Mode;
        public double Random;
        public double Range;
        public double Min;
        public double Max;
        public double StdDev;
        public List<string> Categories;
    }

    /// <summary>
    /// Class holds full information about data set including 
    /// metadata and values in original format
    /// </summary>
    public class ANNDataSet
    {
        public MetaColumn[] MetaData { get; set; }
        public string[][] Data { get; set; }
        public int TestRows { get; set; }
        public bool IsPrecentige { get; set; }
        public bool RandomizeData { get; set; }

        public ANNDataSet GetDataSet(bool randomizeData)
        {
            var ds = new ANNDataSet();
            var ls = new List<MetaColumn>();
            for(int i=0; i< MetaData.Length; i++)
            {
                if (MetaData[i].IsIngored)
                    continue;
                else
                {
                    var c = new MetaColumn();
                    c.Id = MetaData[i].Id;
                    c.Index = MetaData[i].Index;
                    c.MissingValue = MetaData[i].MissingValue;
                    c.Name = MetaData[i].Name;
                    c.Param = MetaData[i].Param;
                    c.Scale = MetaData[i].Scale;
                    c.Type = MetaData[i].Type;
                    c.Encoding = MetaData[i].Encoding;
                    ls.Add(c);
                }
               
            }

            //
            ds.MetaData = ls.ToArray();
            ds.IsPrecentige = IsPrecentige;
            ds.TestRows = TestRows;

            //
            string[][] data = new string[this.Data.Length][];
            for (int i = 0; i < this.Data.Length; i++)
            {
                data[i] = new string[ds.MetaData.Length];
                for (int j = 0; j < ds.MetaData.Length; j++)
                {
                    data[i][j] = this.Data[i][ds.MetaData[j].Index];
                }
            }

            if (randomizeData)
            {
                var rnd = new Random((int)DateTime.UtcNow.Ticks);
                var data1 = data.ToList<string[]>();
                var res1 = data1.OrderBy(row => rnd.Next());
                var res2 = res1.OrderBy(row => rnd.Next());
                data = res2.ToArray();
            }

            ds.Data = data;
            return ds;

        }


        public void InitMetaColumn(IEnumerable<string> mData)
        {
            try
            {
                var mcl = new List<MetaColumn>();
                //parse meta data
                int counter = 0;
                foreach (var c in mData)
                {
                    MetaColumn col = new MetaColumn();
                    //check if double point appear more than one time. In that case raise exception
                    if (c.Count(x => x == ':') > 1)
                        throw new Exception("Column data contains double point ':' which is reserved char. PLease remove double point from metadata.");

                    var strData = c.Substring(c.IndexOf(":") + 1);
                    var colValues = strData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    //
                    col.Name = colValues[0];

                    col.Id = counter;
                    col.Index = counter;
                    col.MissingValue = colValues[3];
                    col.Param = colValues[2];
                    //col.Scale = colValues[4];
                    col.Type = colValues[1];
                    col.Encoding = col.Type.Equals("category", StringComparison.OrdinalIgnoreCase) ? CategoryEncoding.OneHot.ToString() : CategoryEncoding.None.ToString();

                    counter++;
                    mcl.Add(col);
                }

                //
                MetaData = mcl.ToArray();
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        /// <summary>
        /// Initialize Projects with string data, with specific formating 
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="columDelimiter"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <param name="isFloatingPoint"></param>
        public static (string[] header, string[][] data) prepareData(string[] rowData, char[] columDelimiter, bool isFirstRowHeader, bool isFloatingPoint = true)
        {

            //Define the columns
            var colCount = rowData[0].Split(columDelimiter).Count();
            var rowCount = rowData.Length;
            int headerCount = 0;
            ///
            string[] header = null;
            if (isFirstRowHeader)
                headerCount++;

            string[][] data = new string[rowCount - headerCount][];

            //
            for (int i = 0; i < rowCount; i++)
            {
                var row = rowData[i].Split(columDelimiter);

                //column creation for each row
                if (i == 0 && isFirstRowHeader)
                    header = new string[colCount];
                else
                    data[i - headerCount] = new string[colCount];

                if (row.Length != colCount)
                {

                    data = null;
                    throw new Exception("Data is not consistent.");
                }
                //column enumeration
                for (int j = 0; j < colCount; j++)
                {
                    //value format
                    var value = "";
                    if (string.IsNullOrEmpty(row[j]))
                        value = "n/a";
                    else
                        value = row[j];

                    //
                    if (i == 0 && isFirstRowHeader)
                        header[j] = value;
                    else
                        data[i - headerCount][j] = value;


                }
            }

            header = removeInvalidCharFromHeader(header);
            return (header, data);
        }

        private static string[] removeInvalidCharFromHeader(string[] header)
        {

            if (header == null || header.Length < 1)
                return header;

            for (int i = 0; i < header.Length; i++)
            {
                var s = header[i];
                header[i] = Regex.Replace(s, "[^A-Za-z0-9_]", "_");
            }

            return header;
        }
    }

}
