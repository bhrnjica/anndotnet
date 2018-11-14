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
using System.ComponentModel;
using System.Text;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Class implements one column of Raw Data set.
    /// </summary>
    public class VariableDescriptor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public MLDataType Type { get; set; }

        public DataKind Kind { get; set; }

        public MissingValue MissingValue { get; set; }

        public string[] Classes { get; set; }
    }

    //Data type only numric and category
    public enum MLDataType
    {
        [Description("None")]
        None, // ignore columns in modeling
        [Description("Numeric")]
        Numeric, // column will be parsed as numeric float
        [Description("Category")]
        Category, //column will be parsed as One-Ho Vector
    }

    //Kind of data (none, features, label)
    public enum DataKind
    {
        [Description("None")]
        None, // ignore columns in modeling
        [Description("Feature")]
        Feature, //-treat column as input parameter or feature
        [Description("Label")]
        Label, // - treat column as output value or label

    }
    //Different way of handling missing value. 
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
}
