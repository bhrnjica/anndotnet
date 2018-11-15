using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataProcessing.Core
{
    /// <summary>
    /// Class holds full information about data set including 
    /// metadata and values in original format
    /// </summary>
    public class ANNDataSet
    {
        public MetaColumn[] MetaData { get; set; }
        //public string[][] Data { get; set; }
        public List<List<string>> Data { get; set; }
        public int RowsToValidation { get; set; }
        public int RowsToTest { get; set; }
        public bool IsPrecentige { get; set; }
        public bool RandomizeData { get; set; }

        public ANNDataSet GetDataSet(bool randomizeData)
        {
            var ds = new ANNDataSet();
            var ls = new List<MetaColumn>();
            for (int i = 0; i < MetaData.Length; i++)
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
            ds.RowsToValidation = RowsToValidation;
            ds.RowsToTest = RowsToTest;

            //
            List<List<string>> data = new List<List<string>>();
            for (int i = 0; i < this.Data.Count; i++)
            {
                //data[i] = new string[ds.MetaData.Length];
                var rowData = new List<string>();
                for (int j = 0; j < ds.MetaData.Length; j++)
                {
                    rowData.Add(this.Data[i][ds.MetaData[j].Index]);
                }
                data.Add(rowData);
            }

            if (randomizeData)
            {
                var rnd = new Random((int)DateTime.UtcNow.Ticks);
                var data1 = data;
                var res1 = data1.OrderBy(row => rnd.Next());
                var res2 = res1.OrderBy(row => rnd.Next());
                data = res2.ToList();
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
            if (header == null)
                return (null, data);

            var head = removeInvalidCharFromHeader(header.ToList());
            return (head.ToArray(), data);
        }

        /// <summary>
        /// Transforms the string of time series into data frame string 
        /// </summary>
        /// <param name="tdata"></param>
        /// <param name="lagTime"></param>
        /// <returns></returns>
        public static (string[] header, string[][] data) prepareTimeSeriesData(string[] tdata, int lagTime, char[] delimiters, bool isHeader)
        {

            //split data on for feature and label datasets
            var header = new List<string>();
            var data = new List<string[]>();

            //define header if specified
            if (isHeader)
            {
                //
                var cols = tdata[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < cols.Length; j++)
                {
                    if (j + 1 < cols.Length)
                    {
                        //add regular header
                        header.Add(cols[j]);
                    }
                    else
                    {
                        //add lagged features header
                        for (int i = 0; i < lagTime; i++)
                            header.Add($"{cols[j]}-{lagTime - i}");

                        //add last column header
                        header.Add($"{cols[j]}");
                    }

                }
            }
            //
            int l = isHeader ? 1 : 0;
            var lagValues = new Queue<string>();
            for (; l < tdata.Length; l++)
            {

                var col = tdata[l].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                //fill lagged features
                if (lagValues.Count > lagTime)
                    lagValues.Dequeue();
                lagValues.Enqueue(col.Last());

                //until lagged features are not defined don't add data to data-frame
                var row = new List<string>();
                for (int j = 0; j < col.Length && lagValues.Count > lagTime; j++)
                {

                    if (j + 1 < col.Length)
                    {
                        //add regular header
                        row.Add(col[j]);
                    }
                    else
                    {
                        //add lagged features
                        for (int f = 0; f < lagTime; f++)
                        {
                            row.Add(lagValues.ElementAt(f));
                        }
                        //add label
                        row.Add(col[j]);
                    }

                }

                if (lagValues.Count > lagTime)
                    data.Add(row.ToArray());

            }

            //
            return (header.ToArray(), data.ToArray());

        }

        /// <summary>
        /// Initialize Projects with string data, with specific formating 
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="columDelimiter"></param>
        /// <param name="isFirstRowHeader"></param>
        /// <param name="isFloatingPoint"></param>
        public static (List<string> header, List<List<string>> data) prepareDataFromFile(string filePath, char[] columDelimiter, bool isFirstRowHeader, DataLoadingProgress progress, bool isFloatingPoint = true)
        {
            var data = new List<List<string>>();
            var header = new List<string>();

            var dd = File.ReadAllLines(filePath);
            int totalValue = dd.Length;
            string fileText = "";
            for (int i = 0; i < dd.Length; i++)
            {
                fileText = dd[i];
                if (fileText.StartsWith("!"))
                    continue;

                var row = fileText.Split(columDelimiter);
                //column enumeration
                var strRow = new List<string>();
                for (int j = 0; j < row.Length; j++)
                {
                    //value format
                    var value = "";
                    if (string.IsNullOrEmpty(row[j]))
                        value = "n/a";
                    else
                        value = row[j];

                    //
                    if (i == 0 && isFirstRowHeader)
                    {
                        header.Add(value);
                    }
                    else
                        strRow.Add(value);
                }
                data.Add(strRow);

                if (progress != null && (i == 0 || i % 1000 == 0))
                    progress(i, totalValue);
            }
            header = removeInvalidCharFromHeader(header);
            if (progress != null)
                progress(totalValue, totalValue);

            return (header, data);
        }

        private static List<string> removeInvalidCharFromHeader(List<string> header)
        {

            if (header == null || header.Count < 1)
                return header;

            for (int i = 0; i < header.Count; i++)
            {
                var s = header[i];
                header[i] = Regex.Replace(s, "[^A-Za-z0-9_]", "_");
            }

            return header;
        }
    }
}
