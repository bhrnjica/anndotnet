////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Threading.Tasks;
using Anndotnet.Core.Entities;
using static TorchSharp.torch;

namespace Anndotnet.Core.Interfaces;

public interface ISample
{
    List<ColumnInfo> Metadata { get; set; }
    DataParser Parser { get; set; }

    Task<(Tensor X, Tensor Y)> GenerateData();    

    Task<(Tensor X, Tensor Y)> GeneratePredictionData(int rowCount);

    List<ILayer> CreateNet();

}