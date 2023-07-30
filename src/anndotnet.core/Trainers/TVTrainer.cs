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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

using static TorchSharp.torch;


[assembly: InternalsVisibleTo("anndotnet.test")]
namespace AnnDotNet.Core.Trainers;

public class TVTrainer : ITrainer
{
    


    public TVTrainer(Tensor x, Tensor y, 
            IProgressTraining progress, 
            int percentageSplit = 20, 
            bool shuffle = false, 
            int seed= 1234 )
    {

        initTrainer(seed,shuffle);
    }

    private void initTrainer(int seed, bool shuffle = false)
    {
        //(_train, _valid) = Split(seed, shuffle);
    }

    private (DataFeed train, DataFeed validation) Split(int seed, bool shuffle = false)
    {
        throw new NotImplementedException();   

        //var testSize = (int)((_x.shape[0] * _percentageSplit) / 100);
        //var trainSize = (int)_x.shape[0] - testSize;

        ////generate indexes
        //var random = new Random(seed);
        //var lst = Enumerable.Range(0, (int)_x.shape[0]);
        //var trainIds = shuffle ? lst.OrderBy(t => random.Next()).ToArray().Take(trainSize) : lst.Take(trainSize);
        //var testIds = lst.Except(trainIds);

        ////create ndarrays
        //var trArray = np.array(trainIds.ToArray());
        //var teArray = np.array(testIds.ToArray());
        ////
        //var trainX = _x[trArray];
        //var testX = _x[teArray];
        //var trainY = _y[trArray];
        //var testY = _y[teArray];

        //return (new DataFeed(trainX, trainY), new DataFeed(testX, testY));

    }

    public bool Run()
    {
        throw new NotImplementedException();
    }

    protected void TrainMiniBatch()
    {
       throw new NotImplementedException();
    }

    protected void evaluate()
    {
       
    }
}