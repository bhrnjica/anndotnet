using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnnDotNET.Common
{
   
    public class DataFeed : IDataFeed
    {
        NDArray X { get; set; }
        NDArray Y { get; set; }

        public int BatchSize { get; set; }
        int position = -1;

        public object Current
        {
            get 
            {
                Slice slice = GetSlice(position);
                return (X[slice], Y[slice]);
            }
        }

        public DataFeed(NDArray x, NDArray y, int batchsize)
        {
            X = x;
            Y = y;
            BatchSize = batchsize <=0 ? 0 :batchsize;
        }

        
        public IEnumerable<(NDArray xBatch, NDArray yBatch)> GetNextBatch()
        {
            int batchPos = 0;
            while(true)
            {
                Slice slice = GetSlice(batchPos);
                batchPos++;
                var xBatch = X[slice];
                var yBatch = Y[slice];
                if (xBatch.size == 0 || yBatch.size == 0)
                    break;

                yield return (xBatch, yBatch);
                //if batch-size is equal to zero
                if (BatchSize == 0)
                    break;
            }
            
        }
        
        public (NDArray xBatch, NDArray yBatch) GetFullBatch()
        {
            return (X, Y);
        }


        public (DataFeed train, DataFeed validation) Split(int percentage, bool shuffle = false, int seed = 1)
        {
            int testSize = (X.shape[0] * percentage) / 100;
            int trainSize = X.shape[0] - testSize;

            //generate indexes
            var random = new Random(seed);
            var lst = Enumerable.Range(0, X.shape[0]);
            var trainIds = shuffle ?  lst.OrderBy(t => random.Next()).ToArray().Take(trainSize) : lst.Take(trainSize);
            var testIds = lst.Except(trainIds);

            //create ndarrays
            var trArray = np.array(trainIds);
            var teArray = np.array(testIds);
            //
            var trainX = X[trArray];
            var testX = X[teArray];
            var trainY = Y[trArray];
            var testY = Y[teArray];

            return (new DataFeed(trainX, trainY, BatchSize), new DataFeed(testX, testY, BatchSize));

        }
        private Slice GetSlice(int batchIndex)
        {
            int start = batchIndex * BatchSize;
            int end = BatchSize == 0 ? -1 : start + BatchSize;           
            var slice = new Slice(start, end);
            return slice;
        }

        
    }
}
