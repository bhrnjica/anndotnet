///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//            Copyright 2017-2021 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                           //
//             For feedback:https://github.com/bhrnjica/anndotnet/issues     //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel;
using AnnDotNet.Core.Interfaces;
using Tensorflow;
using Tensorflow.NumPy;

namespace AnnDotNet.Core.Data;

public class DataFeed : IDataFeed
{
    private readonly NDArray _x;
    private readonly NDArray _y;
        
    public DataFeed(NDArray x, NDArray y)
    {
        _x = x;
        _y = y;
    }

        
    public IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize)
    {
        int batchPos = 0;
        while(true)
        {
            Slice slice = GetSlice(batchPos, batchSize, (int)_x.shape[0] );

            var xBatch = getNextBatch(_x, slice );
            var yBatch = getNextBatch(_y, slice );    

            batchPos++;

            if (xBatch.size == 0 || yBatch.size == 0)
                break;

            yield return (xBatch, yBatch);


            if (batchSize == 0)
                break;
        }
            
    }


    private NDArray getNextBatch(NDArray data, Slice slice)
    {
        switch (data.rank)
        {
            case 0:
            {
                throw new InvalidEnumArgumentException("Input data is not valid.");
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
                throw new InvalidEnumArgumentException("Dimension of the input data is not supported.");

        }
         
    }
        
    public (Tensor xBatch, Tensor yBatch) GetFullBatch()
    {
        return (_x, _y);
    }


    private static Slice GetSlice(int batchIndex, int batchSize, int dataSize)
    {
        int start = batchIndex * batchSize;

        int end = batchSize == 0 ? -1 : start + batchSize;

        if (start > dataSize)
        {
            start = dataSize;
        }

        if (end > dataSize)
        {
            end = dataSize;
        }

        var slice = new Slice(start, end);
        return slice;
    }
}