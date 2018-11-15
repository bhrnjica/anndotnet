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
using System.IO;
using System.Linq;
namespace DataProcessing.Core
{
    public class DataFrame
    {
        #region Ctor and Fields

        string[][] m_strData; //loaded string of data
        string[] m_strHeader;//columns header 
        /// <summary>
        /// Returns Header 
        /// </summary>
        public string[] HeaderAsString
        {
            get
            {

                if (m_strHeader == null || m_strHeader.Length == 0)
                {
                    m_strHeader = m_trainData.Select(x => x.Name).ToArray();
                }
                return m_strHeader;
            }
        }

        List<ColumnData> m_trainData;//columns for training 
        List<ColumnData> m_testData;//columns for testing

        public DataFrame()
        {

        }

        /// <summary>
        /// Create experiment based on Dataset
        /// </summary>
        /// <param name="dataset"></param>
        public DataFrame(ANNDataSet dataset)
        {
            //copy data
            m_strData = new string[dataset.Data.Count][];
            for (int i=0;i<dataset.Data.Count; i++)
            {
                m_strData[i]= new string[dataset.Data[i].Count];
                for (int j = 0; j < dataset.Data[i].Count; j++)
                {
                    m_strData[i][j] = dataset.Data[i][j];
                }

            }
            //
            Init(dataset.MetaData.ToList(), dataset.RowsToValidation, dataset.IsPrecentige);
        }

        public ANNDataSet GetDataSet()
        {
            var ds = new ANNDataSet();
            ds.Data = m_strData.Select(x=>x.ToList()).ToList();
            ds.MetaData = GetMetadata();
            ds.RowsToValidation = (m_testData == null || m_testData.Count == 0) ? 0 : m_testData[0].RowCount;
            return ds;
        }

        private MetaColumn[] GetMetadata()
        {
            var cols = new List < MetaColumn>();
            foreach(var c in m_trainData)
            {
                var m = new MetaColumn();
                m.Encoding = c.Encoding.Description();
                m.Id = c.Id;
                m.Index = c.Index;
               // m.IsIngored =c.is
                m.MissingValue = c.MissingValue.ToString();
                m.Name = c.Name;
                m.Param = c.GetVariableType();
                m.Scale = c.Scaling.ToString();
                m.Type = c.ColumnDataType.ToString();
                cols.Add(m);
            }

            return cols.ToArray();
        }

        /// <summary>
        /// Prepares the data to use in Modelling.It merges user defined column properties with imported data
        /// Preparation is conversion string array in to meaningful columns data
        /// </summary>
        /// <param name="colProp"></param>
        /// <param name="precentTraining"></param>
        public bool Init(List<MetaColumn> colProp, int testRows, bool isPrecentige)
        {

            //remove all row which is marked Ignore for missing value
            string[][] strData = ignoreRowsWithMissingValues(colProp);

            //row col count
            var colCount = colProp.Count;
            var rowCount = strData.Length;

            //
            int testCount = testRows;
            if (isPrecentige)
                testCount = (int)((double)(rowCount * testRows) / 100.0);
            int trainCount = rowCount - testCount;
            

            if (trainCount < testCount)
            {
                throw new Exception("Invalid number of testing data.");

            }

            //
            var dataTrn = strData.Skip(0).Take(trainCount).ToArray();
            var dataTst = strData.Skip(trainCount).Take(testCount).ToArray();


            if (dataTrn != null)
            {
                if (m_trainData == null)
                    m_trainData = new List<ColumnData>();
                else
                    m_trainData.Clear();


                //first go through all col
                for (int j = 0; j < colCount; j++)
                {
                    //skip column if type is string or parameter is ignored
                    if (colProp[j].Type.Equals(ParameterType.Ignored.Description()) || colProp[j].Param.Equals(ParameterType.Ignored.Description()))
                        continue;

                    var col = CreateColumn(colProp[j]);

                    //go through all rows for each col
                    col.RealValues = new string[trainCount];
                    for (int i = 0; i < dataTrn.Length; i++)
                        col.RealValues[i] = dataTrn[i][j];
                    //
                    col.InitializeData();

                    m_trainData.Add(col);


                }

            }
            //test data
            if (dataTst != null && dataTst.Length > 0)
            {
                if (m_testData == null)
                    m_testData = new List<ColumnData>();
                else
                    m_testData.Clear();

                //first go through all col
                for (int j = 0; j < colCount; j++)
                {
                    //skip column if type is string or parameter is ignored
                    if (colProp[j].Type == "String" || colProp[j].Param == "Ignore")
                        continue;

                    var col = CreateColumn(colProp[j]);

                    //go through all rows for each col
                    col.RealValues = new string[dataTst.Length];
                    for (int i = 0; i < dataTst.Length; i++)
                        col.RealValues[i] = dataTst[i][j];
                    //
                    var trainCol = m_trainData.Where(x => x.Name == col.Name).FirstOrDefault();
                    if (trainCol != null)
                    {
                        col.InitializeAsTestData(trainCol.Statistics);
                        m_testData.Add(col);
                    }
                }

            }
            else
                m_testData = null;

            return true;
        }

       

        private string[][] ignoreRowsWithMissingValues(List<MetaColumn> colProp)
        {
            List<int> ignoredIndex = new List<int>();

            //parse all string data remember row index for missingValues 
            for (int j = 0; j < colProp.Count; j++)
            {
                //check if current column ignores row with missing value
                if (colProp[j].MissingValue == "Ignore" && !colProp[j].IsIngored)
                {
                    for (int i = 0; i < m_strData.Length; i++)
                    {
                        bool retVal = ColumnData.isMissingValue(m_strData[i][j]);
                        if (retVal)
                            ignoredIndex.Add(i);
                    }
                }
            }

            //go through all rows and remove those with remembered index
            int ignoredRows = ignoredIndex.Distinct().ToList().Count;
            string[][] filteredData = new string[m_strData.Length - ignoredRows][];
            int index = 0;
            for (int i = 0; i < m_strData.Length; i++)
            {
                if (!ignoredIndex.Contains(i))
                {
                    filteredData[index] = m_strData[i];
                    index++;
                }
            }

            return filteredData;
        }

        /// <summary>
        /// Creates columns based of the columns properties argument
        /// </summary>
        /// <param name="colProp"></param>
        /// <returns></returns>
        private ColumnData CreateColumn(MetaColumn colProp)
        {
            //determine column type
            ColumnType colType;
            if (colProp.Type.Equals(ColumnType.Numeric.Description(), StringComparison.OrdinalIgnoreCase))
                colType = ColumnType.Numeric;
            else if (colProp.Type.Equals(ColumnType.Binary.Description(), StringComparison.OrdinalIgnoreCase))
                colType = ColumnType.Binary;
            else if (colProp.Type.Equals(ColumnType.Category.Description(), StringComparison.OrdinalIgnoreCase))
                colType = ColumnType.Category;
            else
                colType = ColumnType.Unknown;

            //determine encoding for category column type
            CategoryEncoding colEncoding;
            if (colProp.Encoding == null)
                colEncoding = CategoryEncoding.None;
            else if (colProp.Encoding.Equals(CategoryEncoding.Level.Description(),StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.Level.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.Level;
            else if (colProp.Encoding.Equals(CategoryEncoding.OneHot.Description(), StringComparison.OrdinalIgnoreCase)||
                colProp.Encoding.Equals(CategoryEncoding.OneHot.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.OneHot;
            else if (colProp.Encoding.Equals(CategoryEncoding.Dummy1.Description(), StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.Dummy1.Description(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.Dummy1;
            else if (colProp.Encoding.Equals(CategoryEncoding.Dummy2.Description(), StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.Dummy2.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.Dummy2;
            else if (colProp.Encoding.Equals(CategoryEncoding.Binary1.Description(), StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.Binary1.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.Binary1;
            else if (colProp.Encoding.Equals(CategoryEncoding.Binary2.Description(), StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.Binary2.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.Binary2;
            else if (colProp.Encoding.Equals(CategoryEncoding.None.Description(), StringComparison.OrdinalIgnoreCase) ||
                colProp.Encoding.Equals(CategoryEncoding.None.ToString(), StringComparison.OrdinalIgnoreCase))
                colEncoding = CategoryEncoding.None;
            else
                throw new Exception($"Unknown encoding !");


            //create column data type
            var isOutput = colProp.Param.Equals(ParameterType.Output.Description(), StringComparison.OrdinalIgnoreCase) || 
                colProp.Param.Equals(VariableType.Label.Description(), StringComparison.OrdinalIgnoreCase);

            ColumnData col = new ColumnData(isOutput, colEncoding);
            if (colProp.Scale == null)
                col.SetNormalization(Scaling.None);
            else if (colProp.Scale.Equals(Scaling.MinMax.Description(), StringComparison.OrdinalIgnoreCase))
                col.SetNormalization(Scaling.MinMax);
            else if (colProp.Scale.Equals(Scaling.Gauss.Description(), StringComparison.OrdinalIgnoreCase))
                col.SetNormalization(Scaling.Gauss);
            else if (colProp.Scale.Equals(Scaling.None.Description(), StringComparison.OrdinalIgnoreCase))
                col.SetNormalization(Scaling.None);
            else
                throw new Exception($"Unknown scaling for column '{col.Name}'");


            //set missing value action
            if (colProp.MissingValue.Equals(MissingValue.Ignore.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Ignore;
            else if (colProp.MissingValue.Equals(MissingValue.Average.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Average;
            else if (colProp.MissingValue.Equals(MissingValue.Random.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Random;
            else if (colProp.MissingValue.Equals(MissingValue.Mode.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Mode;
            else if (colProp.MissingValue.Equals(MissingValue.Max.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Max;
            else if (colProp.MissingValue.Equals(MissingValue.Min.Description(), StringComparison.OrdinalIgnoreCase))
                col.MissingValue = MissingValue.Min;
            else
                throw new Exception($"Unknown missing value for column '{col.Name}'");

            //set column name and type
            col.Name = colProp.Name;
            col.ColumnDataType = colType;
            col.Encoding = colEncoding;
            col.Id = colProp.Id;
            col.Index = colProp.Index;
            return col;
        }

        /// <summary>
        /// Returns number of rows in the experiment from training or testing data
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public int GetRowCount(bool testData = false)
        {
            if (!testData)
            {
                if (m_trainData != null)
                    return m_trainData.FirstOrDefault().RowCount;
            }
            else
            {
                if (m_testData != null)
                    return m_testData.FirstOrDefault().RowCount;
            }
            return 0;
            throw new Exception(testData ? "Test Data is null." : "Train Data is null.");
        }
       

        /// <summary>
        /// Returns all columns from experiment
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public List<ColumnData> GetColumns(bool testData = false)
        {
            var data = GetData(testData);
            return data;
        }

        /// <summary>
        /// Returns Count of input columns
        /// </summary>
        /// <returns></returns>
        public int GetColumnInputCount()
        {
            var data = GetData(false);
            return data.Where(x => !x.IsOutput).Count();
        }

        /// <summary>
        /// Returns Count of output columns
        /// </summary>
        /// <returns></returns>
        public int GetColumnOutputCount()
        {
            var data = GetData(false);
            return data.Where(x => x.IsOutput).Count();
        }

        

        /// <summary>
        /// Get 2D array of values from all columns 
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public double[][] GetColumnAllValues(bool testData)
        {
            var data = GetData(testData);

            int numRow = data[0].RowCount;
            int numCol = data.Count;

            double[][] val = new double[numRow][];

            for (int i = 0; i < numRow; i++)
            {
                val[i] = GetRowDataNumeric(i, testData);
            }

            return val;
        }
        /// <summary>
        /// Check if test data is available
        /// </summary>
        /// <returns></returns>
        public bool IsTestDataExist()
        {
            if (m_testData != null && m_testData.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retrieve single row data in numeric format regardless of input output vars.
        /// Categorical types returns in their numeric representation
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="testData">returns from testing or training data set</param>
        /// <returns></returns>
        private double[] GetRowDataNumeric(int rowIndex, bool testData = false)
        {
            var data = GetData(testData);

            double[] str = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                var col = data[i];
                var value = col.GetNumericValue(rowIndex);
                if (value == null)
                    throw new Exception("Experimental data is not numeric and cannot be retrieved.");

                str[i] = value.Value;
            }

            return str;
        }



        /// <summary>
        /// Returns values for output c
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public double[][] GetColumnOutputValues(bool testData)
        {
            //can be more output columns t
            var outCols = GetColumnsFromOutput(testData);
            if (outCols == null)
                return null;
            double[][] values = new double[outCols.Count][];

            for (int j = 0; j < outCols.Count; j++)
            {
                var col = outCols[j];
                values[j] = new double[col.RealValues.Length];
                for (int i = 0; i < col.RealValues.Length; i++)
                    values[j][i] = col.GetNumericValue(i).Value;
            }


            return values;
        }

        

        /// <summary>
        /// Helper for getting right columns from the experiment
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        private List<ColumnData> GetData(bool testData)
        {
            if (testData)
                return m_testData;
            else
                return m_trainData;
        }

        /// <summary>
        /// Return the type of the output column
        /// </summary>
        /// <returns></returns>
        public ColumnType GetOutputColumnType()
        {
            var cols = GetColumnsFromOutput();
            if (cols == null || cols.Count == 0)
                throw new Exception("Experiment must have at least one output column.");

            return cols.LastOrDefault().ColumnDataType;
        }


        public CategoryEncoding GetOutputColumnEncoding()
        {
            var cols = GetColumnsFromOutput();
            if (cols == null || cols.Count == 0)
                throw new Exception("Experiment must have at least one output column.");

            return cols.LastOrDefault().Encoding;
        }


        /// <summary>
        /// Return real output row from normalized output row
        /// </summary>
        /// <param name="encodedOutputRow"></param>
        /// <returns></returns>
        public double[] GetDecodedOutputRow(double[] encodedOutputRow)
        {
            if (encodedOutputRow == null /*|| encodedOutputRow.Length != GetColumnOutputCount_FromNormalizedValue()*/)
                throw new Exception("Column number does not match. normalizedInputRow has different elements than number of input columns in the experiment.");

            //
            var outputCols = GetColumnsFromOutput();
            var retVal = new double[outputCols.Count];

            //
            int rowIndex = 0;
            for (int i = 0; i < outputCols.Count; i++)
            {
                var col = outputCols[i];
                if (col.ColumnDataType == ColumnType.Numeric)
                {
                    retVal[i] = col.GetUnscaledValue(encodedOutputRow[rowIndex]);
                    rowIndex++;
                }
                else if (col.ColumnDataType == ColumnType.Category)
                {
                    retVal = encodedOutputRow;
                    //double[] row = new double[col.Statistics.Categories.Count];
                    //for (int j = 0; j < col.Statistics.Categories.Count; j++)
                    //{
                    //    row[j] = normalizedOutputRow[rowIndex];
                    //    rowIndex++;
                    //}
                    //retVal[i] = col.GetNumericFromNormalized_Category(row);
                }
                else if (col.ColumnDataType == ColumnType.Binary)
                {
                    retVal[i] = col.GetDecodedBinaryValue(encodedOutputRow[rowIndex]);
                    rowIndex++;
                }
                else
                    throw new Exception("The column type is unknown.");

            }

            return retVal;
        }

        /// <summary>
        /// Number of all column after Category columns perform encoding
        /// </summary>
        /// <returns></returns>
        public int GetEncodedColumnInputCount()
        {
            var data = GetData(false);
            var inCols = data.Where(x => !x.IsOutput).ToList();

            int counter = 0;
            for (int i = 0; i < inCols.Count; i++)
            {
                if (inCols[i].ColumnDataType == ColumnType.Category)
                    counter += inCols[i].GetEncodedCount();
                else
                    counter++;
            }

            return counter;
        }

        /// <summary>
        /// Calculate the decoded columns of the output column 
        /// </summary>
        /// <returns></returns>
        public int GetEncodedColumnOutputCount()
        {
            var data = GetData(false);
            var outCols = data.Where(x => x.IsOutput).FirstOrDefault();
            var count = 0;
            if (outCols.ColumnDataType == ColumnType.Category)
                count = outCols.GetEncodedCount();
            else
                count=1;

            return count;
        }

        /// <summary>
        /// Returns list of output columns
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public List<ColumnData> GetColumnsFromOutput(bool testData = false)
        {
            var data = GetData(testData);
            if (data != null)
                return data.Where(x => x.IsOutput).ToList();
            return null;
        }

        /// <summary>
        /// Returns list of input columns
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        public List<ColumnData> GetColumnsFromInput(bool testData = false)
        {
            var data = GetData(testData);

            return data.Where(x => !x.IsOutput).ToList();
        }
       
        /// <summary>
        ///  Retrieve input row data. It is always double value
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="testData">returns from testing or training data set</param>
        /// <returns></returns>
        public double[] GetRowFromInput(int rowIndex, bool testData = false)
        {
            var outputCols = GetColumnsFromInput(testData);
            double[] values = new double[outputCols.Count];
            for (int i = 0; i < outputCols.Count; i++)
            {
                var col = outputCols[i];
                var value = col.GetNumericValue(rowIndex);
                if (value == null)
                    throw new Exception("Experimental data is not numeric and cannot be retrieved.");

                values[i] = value.Value;
            }

            return values;
        }
        
        /// <summary>
        /// Retrieve output row data. It is always double value
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="testData">returns from testing or training data set</param>
        /// <returns></returns>
        public double[] GetRowFromOutput(int rowIndex, bool testData = false)
        {
            var outputCols = GetColumnsFromOutput(testData);
            double[] values = new double[outputCols.Count];
            for (int i = 0; i < outputCols.Count; i++)
            {
                var col = outputCols[i];
                var value = col.GetNumericValue(rowIndex);
                if (value == null)
                    throw new Exception("Experimental data is not numeric and cannot be retrieved.");

                values[i] = value.Value;
            }

            return values;
        }
       

       

        

        public (double[][] train, double[][] test) GetInputData()
        {
            var training = GetDataForGP();
            var testing = GetDataForGP(true);
            ////
            //var inputs = new ExperimentData(training, testing, constants);
            //inputs.SetExperiment(this);
            //
            return (training,testing);
        }

        private object GetHeader()
        {
            var cols = GetColumns();

            return null;
        }

        public double[][] GetDataForGP(bool testData = false)
        {
            if (testData && m_testData == null)
                return null;

            var rowCount = GetRowCount(testData);
            var colCount = GetEncodedColumnInputCount();

            double[][] data = new double[rowCount][];

            for (int i = 0; i < rowCount; i++)
            {
                data[i] = new double[colCount + 1];//+1 for output column
                var intCol = GetEncodedInput(i, testData);

                for (int j = 0; j < colCount; j++)
                    data[i][j] = intCol[j];

                var output = GetEncodedOutput(i, testData);
                //
                double val = 0;
                if (output.Length > 1)
                {
                    for (int ind = 0; ind < output.Length; ind++)
                    {
                        if (output[ind] == 1)
                        {
                            val = ind;
                            break;
                        }
                    }

                }
                else
                    val = output[0];
                //
                data[i][colCount] = val;

            }

            return data;
        }

        /// <summary>
        /// Retrieve normalized row data. It is always double value
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="testData">returns from testing or training data set</param>
        /// <returns></returns>
        public double[] GetEncodedOutput(int rowIndex, bool testData, bool isScaled=true)
        {
            var inCols = GetColumnsFromOutput(testData);
            var retVal = new List<double>();

            for (int i = 0; i < inCols.Count; i++)
            {
                var col = inCols[i];
                var value = col.GetEncodedValue(rowIndex);
                if (value == null)
                    throw new Exception("Experimental data is not numeric and cannot be retrieved.");

                if (!isScaled && col.ColumnDataType == ColumnType.Numeric)
                    value[0] = col.GetUnscaledValue(value[0], col.Statistics);

                retVal.AddRange(value);
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Retrieve normalized row data. It is always double value
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="testData">returns from testing or training data set</param>
        /// <param name="isScaled">during export we need unscaled numeric column values</param>
        /// <returns></returns>
        public double[] GetEncodedInput(int rowIndex, bool testData, bool isScaled = true)
        {

            var inCols = GetColumnsFromInput(testData);
            var retVal = new List<double>();

            for (int i = 0; i < inCols.Count; i++)
            {
                var col = inCols[i];
                var value = col.GetEncodedValue(rowIndex);

                if (value == null)
                    throw new Exception("Experimental data is not numeric and cannot be retrieved.");

                if (!isScaled && col.ColumnDataType == ColumnType.Numeric)
                    value[0] = col.GetUnscaledValue(value[0], col.Statistics);

               

                retVal.AddRange(value);
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// persist preprocessed data to file which can be imported into ML algorithm.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isTestData"></param>
        /// <param name="cntk"></param>
        public void WriteToFile(string fileName, bool isTestData, char delimiter, bool cntkFormat)
        {
            //make a training dataset
            FileStream file = File.Open(fileName, FileMode.OpenOrCreate);
            file.Flush();
            using (StreamWriter sw = new StreamWriter(file))
            {
                for (int i = 0; i < GetRowCount(isTestData); i++)
                {
                    var features = GetEncodedInput(i, isTestData);
                    var label = GetEncodedOutput(i, isTestData);
                    
                    //if CNTK format is enabled
                    if (cntkFormat)
                    {
                        //save to file
                        var f1 = string.Join(" ", features);
                        var f2 = string.Join(" ", label);

                        f1 = "|features " + f1;
                        f2 = "|labels " + f2;
                        //join
                        sw.WriteLine(f1 + "\t" + f2);
                    }
                    else
                    {
                        //save to file
                        var f1 = string.Join(delimiter.ToString(), features);
                        var f2 = string.Join(delimiter.ToString(), label);
                        //join
                        sw.WriteLine(f1 + delimiter + f2);
                    }
                }
            }
        }

        #endregion


    }
}
