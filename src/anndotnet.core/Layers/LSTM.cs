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