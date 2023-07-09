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
using Tensorflow;

namespace AnnDotNet.Core.Interfaces;

/// <summary>
/// Interface for data feed used in training process.
/// </summary>
public interface IDataFeed 
{
    /// <summary>
    /// Get next batch of data for training process.
    /// </summary>
    /// <param name="batchSize">Tehe size of the batch g.e. row number, image number</param>
    /// <returns>The feed that containing of the input and outpu/labeled data</returns>
    IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize);

    /// <summary>
    /// returns the whole data set.
    /// </summary>
    /// <returns>Returns the whole data set converted</returns>
    (Tensor xBatch, Tensor yBatch) GetFullBatch();
}