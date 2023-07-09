///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//            Copyright 2017-2021 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                           //
//             For feedback:https://github.com/bhrnjica/anndotnet/issues     //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;

namespace AnnDotNet.Core.Entities;

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