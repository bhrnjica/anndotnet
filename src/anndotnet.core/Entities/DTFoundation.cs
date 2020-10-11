using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Daany.Ext;

namespace Anndotnet.Core
{
   
    public class ColumnInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MLColumnType MLType { get; set; }
        public Daany.ColType ValueColumnType { get; set; }
        public string ValueFormat { get; set; }
        public Daany.Aggregation MissingValue { get; set; }
        public CategoryEncoding Encoding { get; set; }
        public string[] ClassValues { get; set; }

    }

    public class DataParser
    {
        public string RowSeparator { get; set; } = "\r\n";
        public char ColumnSeparator { get; set; } = ';';
        public bool HasHeader { get; set; } = false;
        public int SkipLine { get; set; } = 0;
        public char[] MissingValueSymbol { get; set; }
        public char DescriptionSymbol { get; set; } = '!';
    }
        


    //Type of the column data
    //public enum ColumnType
    //{
    //    [Description("String")]
    //    Unknown = 0,
    //    [Description("None")]
    //    None,
    //    [Description("Numeric")]
    //    Numeric,//continuous column values
    //    [Description("Binary")]
    //    Binary,//binary column values (0,1)
    //    [Description("Category")]
    //    Category,//categorical column values    
    //}
    

    //Normalization  of the numerical column
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
    public enum MLColumnType
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
