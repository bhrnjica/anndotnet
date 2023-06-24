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
using Anndotnet.Core.Data;
using Anndotnet.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.NumPy;
namespace Anndotnet.Core.Trainers;

public class CVTrainer : TVTrainer
{
    (DataFeed train, DataFeed valid)[] _cvData;
    int _kFold;
    public CVTrainer(NDArray x, NDArray y, IProgressTraining progress, int percentageSplit = 20,
                            bool shuffle = false, int seed = 1234, int kFold = 5)
                            :base(x,y,progress,percentageSplit,shuffle, seed)
    {
        
        _kFold = kFold;

        initTrainer();
    }

    private void initTrainer()
    {
        //
        _cvData = new (DataFeed train, DataFeed valid)[_kFold];

        float percentage = 100.0f / _kFold;

        int testSize = (int)((X.shape[0] * percentage) / 100);
        int trainSize = (int)X.shape[0] - testSize;

        //create folds
        for (int i=0; i < _kFold; i++)
        {
          _cvData[i] = Split(trainSize, testSize, i);
        }

    }

    public bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel)
    {
        //check for progress
        if (!ReferenceEquals(_progress, null))
        {
            tParams.Progress = _progress.Run;
        }

        //set KFold
        tParams.KFold = _kFold; 

        //get placeholders 
        var x = session.graph.get_tensor_by_name("Input/X:0");
        var y = session.graph.get_tensor_by_name("Input/Y:0");

        //get optimizer
        var opt = session.graph.get_operation_by_name($"Train/Optimizer/{lParams.LearnerType}");

        //
        using (session)
        {
            for (int fold = 1; fold <= _cvData.Length; fold++)
            {
                var feed = _cvData[fold - 1];

                TrainminiBatch(session, lParams, tParams, processModel, x, y, opt, fold, feed);
            }

        }
        return true;
    }

    

    internal (DataFeed train, DataFeed validation) Split(int trainSize, int testSize, int index)
    {
        //generate indexes
        var lst = Enumerable.Range(0, (int)X.shape[0]);
        var trainIds = lst.Skip(index * testSize).Take(trainSize);
        var testIds = lst.Except(trainIds);

        //create ndarrays
        var trArray = np.array(trainIds.ToArray());
        var teArray = np.array(testIds.ToArray());
        //
        var trainX = X[trArray];
        var testX = X[teArray];
        var trainY = Y[trArray];
        var testY = Y[teArray];

        return (new DataFeed(trainX, trainY), new DataFeed(testX, testY));
    }
}
