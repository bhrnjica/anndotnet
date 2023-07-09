///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using AnnDotNet.Core.Entities;

namespace AnnDotNet.Core.Interfaces;

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