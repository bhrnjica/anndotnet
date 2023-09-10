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
using Anndotnet.Core.Layers;


namespace Anndotnet.Core.Layers;

public record Dense : Base
{
    public int OutputDim { get; set; }
    public bool HasBias { get; set; }
    public Activation Activation { get; set; }

    public Dense()
    {
        Type = LayerType.Dense;
    }

}