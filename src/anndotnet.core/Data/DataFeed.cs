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
using Anndotnet.Core.Util;
using AnnDotNet.Core.Interfaces;
using Daany.MathStuff.Random;
using static TorchSharp.torch;
 

namespace AnnDotNet.Core.Data;

public class DataFeed : TorchSharp.torch.utils.data.Dataset, IDataFeed
{
    private readonly Tensor _x;
    private readonly Tensor _y;
    private readonly string _name;
        
    public DataFeed(string name, Tensor x, Tensor y )
    {
        _x = x;
        _y = y;
        _name = name;
    }

    public override long Count => _x.shape.First();

    public override Dictionary<string, Tensor> GetTensor(long index)
    {
        return new Dictionary<string, Tensor>()
        {
            { "X", _x[index] },
            { "y", _y[index] }
        };
    }

    public string Name => _name;

    public IEnumerable<(Tensor xBatch, Tensor yBatch)> GetNextBatch(int batchSize)
    {
        int batchPos = 0;

        while(true)
        {
            TensorIndex slice = GetSlice(batchPos, batchSize, (int)_x.shape[0]);

            var xBatch = GetNextBatch(_x, slice );
            var yBatch = GetNextBatch(_y, slice );    

            batchPos++;

            if (xBatch.numel() <= 0 || yBatch.numel() <= 0)
                break;

            yield return (xBatch, yBatch);


            if (batchSize == 0)
                break;
        }
            
    }


    private Tensor GetNextBatch(Tensor data, TensorIndex slice)
    {
        switch (data.ndim)
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
                return data[slice];//data[slice, ":"];
            }
            default:
                throw new InvalidEnumArgumentException("Dimension of the input data is not supported.");

        }
         
    }
        
    public (Tensor xBatch, Tensor yBatch) GetFullBatch()
    {
        return (_x, _y);
    }

    public long InputDimension => _x.shape[1];
    public long OutputDimension => _y.shape.Length > 1 ? _y.shape[1] : 1 ;


    private static TensorIndex GetSlice(int batchIndex, int batchSize, int rowCount)
    {
        long start = batchIndex * batchSize;

        long end = batchSize == 0 ? -1 : start + batchSize;

        if (start > rowCount)
        {
            start = rowCount;
        }

        if (end > rowCount)
        {
            end = rowCount;
        }

        var slice = TensorIndex.Slice(start, end);
        return slice;
    }

}
