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
using static TorchSharp.torch;

namespace AnnDotNet.Core.Interfaces;

/// <summary>
/// Interface for data feed used in training process.
/// </summary>
public interface IDataFeed 
{
    /// <summary>
    /// Get next batch of data for training process.
    /// </summary>
    /// <param name="batchSize">The size of the batch g.e. row number, image number</param>
    /// <returns>The feed that containing of the input and output/labeled data</returns>
    IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize);

    /// <summary>
    /// Returns whole data set.
    /// </summary>
    /// <returns>Returns whole data set</returns>
    (Tensor xBatch, Tensor yBatch) GetFullBatch();

    long InputDimension { get;}
    long OutputDimension { get;}
}