using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anndotnet.Core.Interface
{
    public interface IDataFeed 
    {
        IEnumerable<(NDArray xBatch, NDArray yBatch)> GetNextBatch(int batchSize);
        (NDArray xBatch, NDArray yBatch) GetFullBatch();
    }
}
