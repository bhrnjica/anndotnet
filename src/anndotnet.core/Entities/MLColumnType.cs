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

public enum MLColumnType
{
    [Description("None")]
    None, // ignore columns in modeling
    [Description("Feature")]
    Feature, //-treat column as input parameter or feature
    [Description("Label")]
    Label, // - treat column as output value or label

}