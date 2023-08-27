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

using AnnDotNet.Core;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

namespace AnnDotNet.Core.Layers;

public record Embedding : Base
{
    public int OutputDim { get; set; }
    public double MaxNorm { get; set; }
    
    //1 - linear norm, 2 euklidian norma etc p parameter of the norm
    public double NormType { get; set; }

    public long? PaddingIdX { get; set; }

    public Embedding()
    {
        Type = LayerType.Embedding;
    }

}