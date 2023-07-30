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

using System.Collections.Generic;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using static TorchSharp.torch;

namespace AnnDotNet.Core.Interfaces;

public interface ISample
{
    List<ColumnInfo> Metadata { get; set; }
    DataParser Parser { get; set; }

    Task<(Tensor X, Tensor Y)> GenerateData();    

    Task<(Tensor X, Tensor Y)> GeneratePredictionData(int rowCount);

    List<ILayer> CreateNet();

}