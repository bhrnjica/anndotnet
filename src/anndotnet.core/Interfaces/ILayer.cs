////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Entities;

namespace Anndotnet.Core.Interfaces;

/// <summary>
/// AnnDotNet layer interface
/// </summary>
public interface ILayer
{
    /// <summary>
    /// Layer name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Layer type e.g. Dense, Conv2D, etc.
    /// </summary>
    LayerType Type { get; set; }

}