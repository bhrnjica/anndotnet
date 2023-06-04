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
using Anndotnet.Core.Interface;
using System.Collections.Generic;
using Tensorflow;
using Tensorflow.NumPy;

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

        
        public IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize)
        {
            int batchPos = 0;
            while(true)
            {
                Slice slice = GetSlice(batchPos, batchSize, (int)X.shape[0] );

                var xBatch = GetNextBatch(X, slice );
                var yBatch = GetNextBatch(Y, slice );    

                batchPos++;

                if (xBatch.size == 0 || yBatch.size == 0)
                    break;

                yield return (xBatch, yBatch);


                if (batchSize == 0)
                    break;
            }
            
        }


        public NDArray GetNextBatch(NDArray data, Slice slice)
        {
            switch (data.rank)
            {
                case 0:
                    {
                        throw ExceptionHelper.InvalidArgument("Input data is not valid.");
                    }
                case 1:
                    {
                        return data[slice];
                    }
                case 2:
                    {
                        return data[slice, ":"];
                    }
                default:
                    throw ExceptionHelper.InvalidArgument("Dimension of the input data is not supported.");

            }
         
        }
        
        public (Tensor xBatch, Tensor yBatch) GetFullBatch()
        {
            return (X, Y);
        }


        private Slice GetSlice(int batchIndex, int batchSize, int datasize)
        {
            int start = batchIndex * batchSize;

            int end = batchSize == 0 ? -1 : start + batchSize;

            if (start > datasize)
            {
                start = datasize;
            }

            if (end > datasize)
            {
                end = datasize;
            }

            var slice = new Slice(start, end);
            return slice;

        }

        
    }
}
