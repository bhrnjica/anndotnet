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

namespace Anndotnet.Core.Layers;

public record Embedding : Base
{
    public int OutputDim { get; set; }
    public double MaxNorm { get; set; }
    
    //1 - linear norm, 2 euclidean norma etc p parameter of the norm
    public double NormType { get; set; }

    public long? PaddingIdX { get; set; }

    public Embedding()
    {
        Type = LayerType.Embedding;
    }

}