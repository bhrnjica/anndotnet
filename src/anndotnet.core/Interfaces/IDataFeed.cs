﻿//////////////////////////////////////////////////////////////////////////////////////////
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
using Tensorflow;
using Tensorflow.NumPy;

namespace Anndotnet.Core.Interface
{
    public interface IDataFeed 
    {
        IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize);
        (Tensor xBatch, Tensor yBatch) GetFullBatch();
    }
}
