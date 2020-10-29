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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NumSharp;
using Anndotnet.Core.Interface;

namespace Anndotnet.Core.Data
{
   
    public class DataFeed : IDataFeed
    {
        NDArray X { get; set; }
        NDArray Y { get; set; }
        
        public DataFeed(NDArray x, NDArray y)
        {
            X = x;
            Y = y;
        }

        
        public IEnumerable<(NDArray xBatch, NDArray yBatch)> GetNextBatch(int batchSize)
        {
            int batchPos = 0;
            while(true)
            {
                Slice slice = GetSlice(batchPos, batchSize);
                batchPos++;
                var xBatch = X[slice];
                var yBatch = Y[slice];
                if (xBatch.size == 0 || yBatch.size == 0)
                    break;

                yield return (xBatch, yBatch);
                //if batch-size is equal to zero
                if (batchSize == 0)
                    break;
            }
            
        }
        
        public (NDArray xBatch, NDArray yBatch) GetFullBatch()
        {
            return (X, Y);
        }


       
        private Slice GetSlice(int batchIndex, int batchSize)
        {
            int start = batchIndex * batchSize;
            int end = batchSize == 0 ? -1 : start + batchSize;           
            var slice = new Slice(start, end);
            return slice;
        }

        
    }
}
