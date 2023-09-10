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