////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////
using Anndotnet.Core.Data;
using TorchSharp.Modules;

namespace Anndotnet.Core.Interfaces;

public interface IDataSplitter
{

    (DataLoader train, DataLoader validation) Split(DataFeed data, int testPercentage, bool shuffle, int batchSize, int seed = 1234);
}