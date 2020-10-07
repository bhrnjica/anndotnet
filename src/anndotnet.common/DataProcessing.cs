using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace anndotnet.common
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

    //NOrmalization  of the numerical column
    public enum Scaling
    {
        [Description("None")]
        None,
        [Description("MinMax")]
        MinMax,
        [Description("Gauss")]
        Gauss,

    }

    //Variable type
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
                    Type.IndexOf("none", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Type.IndexOf("ignored", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                    
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
}
