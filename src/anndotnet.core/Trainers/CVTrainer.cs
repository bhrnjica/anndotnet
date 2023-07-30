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
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

namespace AnnDotNet.Core.Trainers;

public class CVTrainer : TVTrainer
{
   

    private void initTrainer()
    {
        //
        _cvData = new (DataFeed train, DataFeed valid)[_kFold];

        float percentage = 100.0f / _kFold;

        int testSize = (int)((_x.shape[0] * percentage) / 100);
        int trainSize = (int)_x.shape[0] - testSize;

        //create folds
        for (int i=0; i < _kFold; i++)
        {
          _cvData[i] = Split(trainSize, testSize, i);
        }

    }

    public new bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel)
    {
        //check for progress
        if (!ReferenceEquals(Progress, null))
        {
            tParams.Progress = Progress.Run;
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

                TrainMiniBatch(session, lParams, tParams, processModel, x, y, opt, fold, feed);
            }

        }
        return true;
    }

    

    internal (DataFeed train, DataFeed validation) Split(int trainSize, int testSize, int index)
    {
        //generate indexes
        var lst = Enumerable.Range(0, (int)_x.shape[0]);
        var trainIds = lst.Skip(index * testSize).Take(trainSize);
        var testIds = lst.Except(trainIds);

        //create ndarrays
        var trArray = np.array(trainIds.ToArray());
        var teArray = np.array(testIds.ToArray());
        //
        var trainX = _x[trArray];
        var testX = _x[teArray];
        var trainY = _y[trArray];
        var testY = _y[teArray];

        return (new DataFeed(trainX, trainY), new DataFeed(testX, testY));
    }
}
