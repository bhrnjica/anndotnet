//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using Tensorflow.NumPy;

namespace Anndotnet.Core.Interface
{
    public interface IDataFeed 
    {
        IEnumerable<(NDArray xBatch, NDArray yBatch)> GetNextBatch(int batchSize);
        (NDArray xBatch, NDArray yBatch) GetFullBatch();
    }
}
