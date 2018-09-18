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

using GPdotNet.MathStuff;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace DataProcessing.Core
{

    /// <summary>
    /// Represent the variable of the experiment.
    /// </summary>
    public class ColumnData
    {
        #region Fields
        int m_Id;//unique number 
        int m_Index;//position of the column  collection
        public static readonly string[] m_missingSymbols = { "n/a", " ", "?", ""};     
        Scaling                         m_ScalingType;//normalization type of column values
        ColumnType                      m_ColType;//type of the column
        CategoryEncoding                m_Encoding;// in case of category column which category encoding will used
        ParameterType                   m_ParamType; //type of the ML component Predictor or Label
        Statistics                      m_Statistics;//statistic of the column
        MissingValue                 m_MissingValue; //MissingValue in row 
        string[]                        m_RealValues;//real  column value extracted from the file in string format
        double[]                        m_NumericValues;//if the column  is numeric it holds numeric representation of the real value
        double[][]                      m_EncodedValues;// before apply to the solver column has to be normalized

        public ColumnData(bool isOutput = false, CategoryEncoding encoding = CategoryEncoding.None)
        {
            m_Encoding = encoding;
            if (isOutput)
                m_ParamType = ParameterType.Output;
            else
                m_ParamType = ParameterType.Input;
            m_ColType = ColumnType.Numeric;
        }

        #endregion

        #region Properties
        public Scaling          Scaling { get { return m_ScalingType; } }
        internal bool           IsTest{get;private set;} // if it is test data, normalization must be performed by external stats parameters
        public bool             IsOutput { get { return m_ParamType == ParameterType.Output; } }
        internal string[]       RealValues { get { return m_RealValues; } set { m_RealValues = value; } }
        public string           Name { get; set; }//Name of the column in experiment
        internal int            RowCount { get { return m_RealValues==null ? 0 : m_RealValues.Length; } }
        internal int Id { get { return m_Id; } set { m_Id = value; } }
        internal int Index { get { return m_Index; } set { m_Index = value; } }
        public ColumnType   ColumnDataType { get { return m_ColType; } set { m_ColType = value; } }
        public ParameterType VariableType { get { return m_ParamType; } set { m_ParamType = value; } }

        public CategoryEncoding Encoding { get { return m_Encoding; } set { m_Encoding = value; } }
        public Statistics       Statistics { get { return m_Statistics; } }

        internal MissingValue MissingValue { get { return m_MissingValue; } set { m_MissingValue = value; } }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Set values for the Column in string format, by passing the normalization type
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="normalization"></param>
        internal void SetData(string[] cols, Scaling normalization= Scaling.MinMax)
        {
            m_ScalingType = normalization;
            m_RealValues = new string[cols.Length];
            Array.Copy(cols, this.m_RealValues, cols.Length);
        }

        /// <summary>
        /// returns n th row value of the origin string data of the column
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public string GetData(int rowIndex)
        {
            if (m_RealValues.Length > rowIndex && rowIndex >= 0)
                return m_RealValues[rowIndex];
            else
                return null;
        }
       /// <summary>
       /// Returns the numeric values of the column for n th row.
       /// </summary>
       /// <param name="rowIndex"></param>
       /// <returns></returns>
        public double? GetNumericValue(int rowIndex)
        {
            if (m_NumericValues.Length > rowIndex && rowIndex >= 0)
                return m_NumericValues[rowIndex];
            else
                return null;
        }

        /// <summary>
        /// Return array of normalized value for specific row
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public double[] GetEncodedValue(int rowIndex)
        {
            var count = m_EncodedValues[rowIndex].Length;
            var retval = new double[count];
            Array.Copy(m_EncodedValues[rowIndex], retval, count);
            return retval;
        }

        internal string GetDecodedValue(double[] normalizedValue, Statistics normParams = null)
        {
            switch (m_ColType)
            {
                case ColumnType.Unknown:
                    throw new Exception("Cannot resolve column type.");

                case ColumnType.Numeric:
                    {
                        double val = GetUnscaledValue(normalizedValue[0], normParams);
                        return val.ToString(CultureInfo.InvariantCulture);
                    }
                case ColumnType.Binary:
                    {
                        string val = GetBinaryFromNormalized(normalizedValue[0]);
                        return val.ToString();//CultureInfo.InvariantCulture
                    }
                case ColumnType.Category:
                    {
                        string val = GetCategoricalFromNormalized(normalizedValue);
                        return val;
                    }
                default:
                    throw new Exception("Cannot resolve column type.");

            }
        }

        /// <summary>
        /// Returns the header for encoded columns
        /// </summary>
        /// <returns></returns>
        public string EncodedHeader(int index)
        {
            if (ColumnDataType== ColumnType.Category && CategoryEncoding.Level == Encoding)
            {
                var str = "";
                for (int i=0; i< m_Statistics.Categories.Count; i++)
                {
                    var cl=m_Statistics.Categories[i];
                    if(i+1== m_Statistics.Categories.Count)
                        str += $"{cl}={i}";
                    else
                        str += $"{cl}={i}; ";

                }
                return $"{Name}({str})";
            }
            else if (ColumnDataType == ColumnType.Category && CategoryEncoding.OneHot == Encoding)
            {
                return $"{Name}({m_Statistics.Categories[index]})";
            }
            else if (ColumnDataType == ColumnType.Category && ( CategoryEncoding.Dummy1 == Encoding || CategoryEncoding.Dummy2 == Encoding))
            {
                return $"{Name}({m_Statistics.Categories[0]} vs {m_Statistics.Categories[index+1]})";
            }
            else if (ColumnDataType == ColumnType.Binary)
            {
                if(Encoding == CategoryEncoding.Binary1)
                    return $"{Name}({m_Statistics.Categories[0]}=0; {m_Statistics.Categories[1]}=1)";
                else// (Encoding == CategoryEncoding.Binary2)
                    return $"{Name}({m_Statistics.Categories[0]}=-1; {m_Statistics.Categories[1]}=1)";
            }
            else if (ColumnDataType == ColumnType.Numeric)
            {
                return $"{Name}";
            }
            else
                throw new Exception("Unknown column encoding!");
        }

        public int GetEncodedColumCount()
        {
            if (m_EncodedValues == null || m_EncodedValues.Length == 0)
                throw new Exception("ColumnData value cannot be empty!");
            return m_EncodedValues[0].Length;
        }

        /// <summary>
        /// Calculate real values from normalized values. Normalized data are normalized by training data statistics.
        /// </summary>
        /// <param name="scaledValue"></param>
        /// <param name="normParams">parameters in case of testing data</param>
        /// <returns></returns>
        internal double GetUnscaledValue(double scaledValue, Statistics normParams=null)
        {
            if (ColumnDataType != ColumnType.Numeric)
                throw new Exception("Column type is not Numeric.");

            //check if train or test data should be de normalized
            Statistics stat=null;
            if (normParams != null)
                stat = normParams;
            else
                stat = m_Statistics;

            //perform unscaled
            double retVal=0;
            if(m_ScalingType== Scaling.Gauss)
            {
                if (stat.StdDev == 0)
                    retVal = scaledValue;
                else
                    retVal = scaledValue * stat.StdDev + stat.Mean;
            }          
            else if(m_ScalingType == Scaling.MinMax)
            {
                //in case the column is constant
                if (stat.Max == stat.Min)
                {
                    if (stat.Max > 1 || stat.Max < -1)//nonzero column
                        retVal = scaledValue / stat.Max;
                    else
                        retVal = scaledValue;
                }
                else
                {
                    retVal = scaledValue * (stat.Max - stat.Min) + stat.Min;
                }
            }              
            else if (m_ScalingType == Scaling.None)
                retVal = scaledValue;
            else
                throw new Exception("Unknown normalization data type.");

           return retVal;
        }

        internal double GetDecodedCategoryValue(double[] encodedValues)
        {
            if (ColumnDataType != ColumnType.Category)
                throw new Exception("Column type is not Categorical.");

            if (encodedValues != null && encodedValues.Length != Statistics.Categories.Count)
                throw new Exception("Inconsistent number of category");

            double maxVal = encodedValues[0];
            int index = 0;
            for(int i=0;i<encodedValues.Length; i++)
            {
                if(maxVal< encodedValues[i])
                {
                    maxVal = encodedValues[i];
                    index = i;
                }
                   
            }

            return index;//because numeric value of category column is index of categoryName
        }

        internal double GetDecodedBinaryValue(double encodedValue)
        {
            if (ColumnDataType != ColumnType.Binary)
                throw new Exception("Column type is not Binary.");

            if (encodedValue < 0.5)
                return 0;
            else
                return 1.0;
        }

        internal void InitializeAsTestData(Statistics stat)
        {
            IsTest = true;
            InitializeData(stat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns row count number</returns>
        internal void InitializeData(Statistics stat = null)
        {
            if (m_RealValues != null && m_RealValues.Length > 0)
            {
                //m_ColType = GetColumType(m_RealValues[0]);
                switch (m_ColType)
                {
                    case ColumnType.Unknown:
                        throw new Exception("Cannot resolve column type.");

                    case ColumnType.Numeric:
                        RealDataToNumeric(stat);
                        break;
                    case ColumnType.Binary:
                        RealDataToBinary(stat);
                        break;
                    case ColumnType.Category:
                        RealDataToCategoric(stat);
                        break;
                    default:
                        throw new Exception("Cannot resolve column type.");

                }
            }

            //
            return;
        }

        internal void SetNormalization(Scaling normalizationType)
        {
            m_ScalingType = normalizationType;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// calculates statistic for the column
        /// </summary>
        private void CalculateStats()
        {
            if (m_RealValues == null)
                throw new Exception("Column is empty.");

            if (m_Statistics == null)
                m_Statistics = new Statistics();

            m_Statistics.Mean = m_NumericValues.Where(x => !double.IsNaN(x)).Average();
            m_Statistics.Max = m_NumericValues.Where(x => !double.IsNaN(x)).Max();
            m_Statistics.Min = m_NumericValues.Where(x => !double.IsNaN(x)).Min();
            m_Statistics.Mode = m_NumericValues.Where(x => !double.IsNaN(x)).ToArray().ModeOf();
            m_Statistics.Random = m_NumericValues.Where(x => !double.IsNaN(x)).ToArray().RandomOf();

            //replace missing value
            replaceMissingValue();

            //calculate median= middle value from the array
            var median = m_NumericValues.MedianOf();
            m_Statistics.Median = median;

            //range value
            m_Statistics.Range = m_Statistics.Max - m_Statistics.Min;

            //Standard deviation
            m_Statistics.StdDev = m_NumericValues.Stdev();
        }

        private void replaceMissingValue()
        {
            //replace missingValues
            for (int i = 0; i < m_NumericValues.Length; i++)
            {
                if (double.IsNaN(m_NumericValues[i]))
                {
                    if (m_MissingValue == MissingValue.Average)
                        m_NumericValues[i] = Statistics.Mean;
                    else if (m_MissingValue == MissingValue.Mode)
                        m_NumericValues[i] = Statistics.Mode;
                    else if (m_MissingValue == MissingValue.Random)
                        m_NumericValues[i] = Statistics.Random;
                    else if (m_MissingValue == MissingValue.Max)
                        m_NumericValues[i] = Statistics.Max;
                    else if (m_MissingValue == MissingValue.Min)
                        m_NumericValues[i] = Statistics.Min;
                    else
                        throw new Exception("Missing value for column=" + Name + " is not defined.");

                    setRealValueFromNumeric(i, m_NumericValues[i]);
                }
            }
        }

        private void setRealValueFromNumeric(int index, double value)
        {
            switch (m_ColType)
            {
                case ColumnType.Unknown:
                    throw new Exception("Column type is not known.");
                case ColumnType.Numeric:
                    m_RealValues[index] = value.ToString();
                    break;
                case ColumnType.Binary:
                    if(value<0.5)
                        m_RealValues[index]= Statistics.Categories[0];
                    else
                        m_RealValues[index] = Statistics.Categories[1];
                    break;
                case ColumnType.Category:
                    int cat=(int)value;
                    if(cat>Statistics.Categories.Count-1)
                        cat=Statistics.Categories.Count-1;
                    m_RealValues[index] = Statistics.Categories[cat];
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// based on the normalization parameters and values returns the corresponded category
        /// Converts normalized value in to categorical values
        /// </summary>
        /// <param name="normalizedValue"></param>
        /// <param name="normParams"></param>
        /// <returns></returns>
        private string GetCategoricalFromNormalized(double[] normalizedValue, Statistics normParams = null)
        {
           //
            Statistics stat = null;
            if (normParams != null)
                stat = normParams;
            else
                stat = m_Statistics;
            double maxVal = normalizedValue[0];
            int index=0;
            for (int i = 0; i < normalizedValue.Length; i++ )
            {
                if(normalizedValue[i]>maxVal)
                {
                    maxVal = normalizedValue[i];
                    index = i;
                }
                  
            }
            return stat.Categories[index];
        }

        /// <summary>
        /// Converts numeric value (which represent index of the category) in to Category values
        /// </summary>
        /// <param name="numeric"></param>
        /// <param name="normParams"></param>
        /// <returns></returns>
        public string GetCategoryFromNumeric(double numeric, Statistics normParams)
        {
            string catValue = "";
            Statistics stat = null;
            if (normParams == null)
                stat = m_Statistics;
            else
                stat = normParams;

            for (int i = 0; i < stat.Categories.Count; i++)
            {
                if (i <= numeric && numeric < i + 1)
                {
                    catValue = stat.Categories[i];
                    break;
                }
            }

            return catValue;
        }

        /// <summary>
        /// Converts numeric value (which represent index of the category) in to Binary Category values
        /// </summary>
        /// <param name="numeric"></param>
        /// <param name="normParams"></param>
        /// <returns></returns>
        public string GetBinaryClassFromNumeric(double numeric, Statistics normParams)
        {
             
            Statistics stat = null;
            if (normParams == null)
                stat = m_Statistics;
            else
                stat = normParams;

            if(numeric<0.5)
                return stat.Categories[0];
            else
                return stat.Categories[1];

        }

        /// <summary>
        /// Converts normalized value (0 -1) in to binary representation of te column value
        /// </summary>
        /// <param name="normalizedValue"></param>
        /// <param name="normParams"></param>
        /// <returns></returns>
        private string GetBinaryFromNormalized(double normalizedValue, Statistics normParams = null)
        {
            var numeric = GetUnscaledValue(normalizedValue, normParams);
            string catValue = GetCategoryFromNumeric(numeric, normParams);
            return catValue;
        }

        /// <summary>
        /// Code real values in to numeric data typa
        /// </summary>
        private void RealDataToNumeric(Statistics stat = null)
        {
            //Create numeric data
            m_NumericValues = new double[m_RealValues.Length];
            for (int i = 0; i < m_RealValues.Length; i++)
            {
                string str = m_RealValues[i];
                double v;
                if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out v))
                    m_NumericValues[i] = v;
                else if(isMissingValue(str))
                    m_NumericValues[i] = double.NaN;
                else
                    throw new Exception("The Values of " + Name + " column cannot be converted to numeric value. Try to change the type of the column.");
            }
            //calculate stats
            if (stat == null)
                CalculateStats();
            else//this is testing data we used stat from training in order to get correct normalization
            {
                m_Statistics = stat;
                replaceMissingValue();
            }
                

            //normalize values
            m_EncodedValues = new double[m_RealValues.Length][];
            for (int i = 0; i < m_RealValues.Length; i++)
            {
                m_EncodedValues[i]= new double[1];

                if (m_ScalingType == Scaling.Gauss)
                {
                    if (m_Statistics.StdDev == 0)
                        m_EncodedValues[i][0] = m_NumericValues[i];
                    else
                        m_EncodedValues[i][0] = Math.Round((m_NumericValues[i] - m_Statistics.Mean) / m_Statistics.StdDev, 5);
                }
                else if (m_ScalingType == Scaling.MinMax)
                {
                    if (m_Statistics.Max - m_Statistics.Min==0)
                    {
                        if (m_Statistics.Max > 1 || m_Statistics.Max < -1)//nonzero column
                            m_EncodedValues[i][0] = m_NumericValues[i] * m_Statistics.Max;
                        else
                            m_EncodedValues[i][0] = m_NumericValues[i];
                    }
                    else
                      m_EncodedValues[i][0] = Math.Round((m_NumericValues[i] - m_Statistics.Min) / (m_Statistics.Max - m_Statistics.Min),5);
                }                  
                else if (m_ScalingType == Scaling.None)
                    m_EncodedValues[i][0] = m_NumericValues[i];
                else
                    throw new Exception("Unknown normalization data type.");
            }
        }

        internal int GetEncodedCount()
        {
            if(m_EncodedValues!=null && m_EncodedValues.Length>0 && m_EncodedValues[0].Length>0)
                return m_EncodedValues[0].Length;
            throw new Exception($"Encoding is not defined for the column '{Name}'");
        }

        /// <summary>
        /// Converts real values in to numeric and normalized values, based on the statistic parameters
        /// </summary>
        /// <param name="stat"></param>
        private void RealDataToBinary(Statistics stat = null)
        {
            //Create numeric and normalized  data
            m_NumericValues = new double[m_RealValues.Length];
            m_EncodedValues = new double[m_RealValues.Length][];
            //define binary column
            //first value should be 0 and second 1
            List<string> classes = null;
            if (stat == null)
            {
                classes = m_RealValues.Where(x => !m_missingSymbols.Contains(x)).Distinct().OrderBy(x => x).ToList();
                if (m_Statistics == null)
                    m_Statistics = new Statistics();
                m_Statistics.Categories = classes;
            }
            else//usually this is data for testing and validation
                classes = stat.Categories;

            if (classes.Count != 2)
                throw new Exception("Binary column type should has only 2 classes.");


            for (int i = 0; i < m_RealValues.Length; i++)
            {
                var val = m_RealValues[i];
                m_EncodedValues[i]= new double[1];

                var c = classes.Where(x => x == val).FirstOrDefault();
                if (c != null)
                {
                    //in case of binary column type real and normalized value are the same
                    m_NumericValues[i] = classes.IndexOf(c);
                    m_EncodedValues[i][0]= m_NumericValues[i];
                }
                else if(isMissingValue(val))
                    m_NumericValues[i] = double.NaN;//missing value
                else
                    throw new Exception("Data in " + Name + " column cannot be null.");
            }

            //calculate stats
            if (stat == null)
                CalculateStats();
            else//this is testing data we used stat from training in order to get correct normalization
            {
                m_Statistics = stat;
                replaceMissingValue();
            }

            //
            EncodeCategoricColumn();
        }

        public static bool isMissingValue(string val)
        {
            //return string.IsNullOrEmpty(val) || val.Trim() == "n/a" || val.Trim() == "-";
            return m_missingSymbols.Contains(val);
        }

        /// <summary>
        /// Converts real categories in to numeric and normalized values. In case of normalized value it apply 1 of N category rule.
        /// </summary>
        /// <param name="stat"></param>
        private void RealDataToCategoric(Statistics stat = null)
        {
            //Create numeric data
            m_NumericValues = new double[m_RealValues.Length];

            //define classes
            //for each classes we assign natural value
            //1. class - 0
            //2. class - 1
            //3. class - 2
            //...
            List<string> classes = null;
            if (stat == null || stat.Categories==null)
            {
                classes = m_RealValues.Where(x=> !m_missingSymbols.Contains(x)).Distinct().OrderBy(x => x).ToList();
                if (m_Statistics == null)
                    m_Statistics = new Statistics();
                m_Statistics.Categories = classes;
            }
            else//usually this is data for testing and validation
                classes = stat.Categories;


            for (int i = 0; i < m_RealValues.Length; i++)
            {
                var val = m_RealValues[i];

                var c = classes.Where(x => x == val).FirstOrDefault();
                if (c != null)
                {
                    m_NumericValues[i] = classes.IndexOf(c);
                }
                else if(m_missingSymbols.Contains(val) && m_MissingValue!= MissingValue.Ignore)
                    m_NumericValues[i] = double.NaN;//missing value
                else
                    throw new Exception("Data in " + Name + " column cannot be null.");
            }

            //calculate stats
            if (stat == null)
                CalculateStats();
            else//this is testing data we used stat from training in order to get correct normalization
            {
                m_Statistics = stat;
                replaceMissingValue();
            }

            //
            EncodeCategoricColumn();
            
        }

        /// <summary>
        /// https://stats.idre.ucla.edu/r/library/r-library-contrast-coding-systems-for-categorical-variables/
        /// 
        /// COnverts numeric values (index of category collection in to 1 of N (0,1) values)
        /// The method creates array which has length of Category count.
        /// Example: Red, Green, Blue - 3 categories  - real values
        ///             0,  1,  2    - 3 numbers     - numeric encoding
        ///             
        /// Normalized values for Blues category: One vs All encoding
        ///          Blue  =  (0,0,1)  - three values which sum is 1,
        ///          Red   =  (1,0,0)
        ///          Green =  (0,1,0)
        /// Normalized values for Blues category: dummy encoding -N-1(0)
        ///          Blue  =  (0,0)  - three values which sum is 1,
        ///          Red   =  (1,0)
        ///          Green =  (1,1)    
        /// Normalized values for Blues category: dummy encoding -N-1(-1)
        ///          Blue  =  (-1,-1)  - three values which sum is 1,
        ///          Red   =  (1,0)
        ///          Green =  (1,1)   
        /// </summary>
        private void EncodeCategoricColumn()
        {
            //normalize values
            m_EncodedValues = new double[m_RealValues.Length][];
            //
            for (int i = 0; i < m_RealValues.Length; i++)
            {
                switch (Encoding)
                {
                    case CategoryEncoding.None:
                        //throw new Exception($"Missing category encoding for columnName={Name}");
                    case CategoryEncoding.Level:
                        {
                            m_EncodedValues[i] = new double[1];
                            m_EncodedValues[i][0] = m_NumericValues[i];
                        }
                        break;
                    case CategoryEncoding.Binary1:
                        {
                            m_EncodedValues[i] = new double[1];
                            m_EncodedValues[i][0] = m_NumericValues[i];
                        }
                        break;
                    case CategoryEncoding.Binary2:
                        {
                            m_EncodedValues[i] = new double[1];
                            m_EncodedValues[i][0] = m_NumericValues[i]==0?-1:1;
                        }
                        break;
                    case CategoryEncoding.OneHot:
                        {
                            m_EncodedValues[i] = new double[m_Statistics.Categories.Count];

                            for (int j = 0; j < m_Statistics.Categories.Count; j++)
                            {
                                if (j == m_NumericValues[i])
                                    m_EncodedValues[i][j] = 1;
                                else
                                    m_EncodedValues[i][j] = 0;
                            }
                        }
                        break;
                    case CategoryEncoding.Dummy1:
                        {
                            m_EncodedValues[i] = new double[m_Statistics.Categories.Count-1];

                            for (int j = 0; j < m_EncodedValues[i].Length; j++)
                            {
                                if (0 == m_NumericValues[i])
                                {
                                    m_EncodedValues[i][j] = 0;
                                }
                                else
                                {
                                    int index = (int)m_NumericValues[i]-1;
                                    m_EncodedValues[i][index] = 1;
                                }
                                   
                            }
                        }
                        break;
                    case CategoryEncoding.Dummy2:
                        {
                            m_EncodedValues[i] = new double[m_Statistics.Categories.Count - 1];

                            for (int j = 0; j < m_EncodedValues[i].Length; j++)
                            {
                                if (m_EncodedValues[i].Length == m_NumericValues[i])
                                {
                                    m_EncodedValues[i][j] = -1;
                                }
                                else
                                {
                                    int index = (int)m_NumericValues[i];
                                    m_EncodedValues[i][index] = 1;
                                }

                            }
                        }
                        break;
                    default:
                        break;
                }
               
            }
        }

        public string GetColumnType()
        {
            return m_ColType.Description();
            
        }

        public string GetMissingValue()
        {
            return m_MissingValue.Description();

           
        }

        public string GetVariableType()
        {
            return VariableType.Description();
           
        }

        public string GetNormalization()
        {
            return Scaling.Description();
        }
        #endregion
    }

    
}
