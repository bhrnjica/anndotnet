////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////
using System.ComponentModel;

namespace Anndotnet.Core.Entities;

public enum MissingValue
{
    [Description("None")]
    None,//remove a row from the experiment
    [Description("Ignore")]
    Ignore,//remove a row from the experiment
    [Description("Average")]
    Average,//recalculate a column and put average value in all missing rows
    [Description("Mode")]
    Mode,//recalculate a column and put most frequent value in all missing rows
    [Description("Random")]
    Random,//recalculate a column and put most random value in all missing rows
    [Description("Max")]
    Max,//recalculate a column and put Max value in all missing rows
    [Description("Min")]
    Min //recalculate a column and put Min value in all missing rows
}