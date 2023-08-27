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

public record Lstm : Base
{
    public int OutputDim { get; set; }

    public long InputSize { get; set; }
    public long HiddenSize { get; set; }
    public long Layers { get; set; }
    public bool HasBias { get; set; }
    public bool BatchFirst { get; set; }
    public double DropRate { get; set; }
    public bool Bidirectional { get; set; }

    public Lstm()
    {
        Type = LayerType.Embedding;
    }

}