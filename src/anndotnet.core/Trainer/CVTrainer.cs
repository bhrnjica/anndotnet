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
using Anndotnet.Core.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.NumPy;
namespace Anndotnet.Core.Trainers
{
    public class CVTrainer : ITrainer
    {
        NDArray X;
        NDArray Y;
        (DataFeed train, DataFeed valid)[] _cvData;
        int _kFold;
        public CVTrainer(NDArray x, NDArray y, int kFold = 5)
        {
            X = x;
            Y = y;
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
            for (int i=0; i<_kFold; i++)
              _cvData[i] = Split(trainSize, testSize, i);

        }

        public bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel)
        {
            //check for progress
            if (tParams.Progress == null)
            {
                var pt = new ProgressCVTraining();
                tParams.Progress = pt.Run;
            }

            //get placeholders 
            var x = session.graph.get_tensor_by_name("Input/X:0");
            var y = session.graph.get_tensor_by_name("Input/Y:0");

            //get optimizer
            var opt = session.graph.get_operation_by_name($"Train/Optimizer/{lParams.LearnerType}");

            //create list of loss and evaluation functions
            var funs = new List<Tensor>();
            var loss = session.graph.get_tensor_by_name($"Train/Loss/{lParams.LossFunction}:0");
            funs.Add(loss);
            //extract evaluation functions
            for (int i = 0; i < lParams.EvaluationFunctions.Count(); i++)
            {
                var efun = lParams.EvaluationFunctions[i];
                var eval = session.graph.get_tensor_by_name($"Train/Eval{i}/{efun.ToString()}:0");
                if (eval == null)
                    break;
                funs.Add(eval);
            }

            //
            using (session)
            {
                for (int f = 1; f <= _cvData.Length; f++)
                {
                    var feed = _cvData[f - 1];

                    // Training cycle
                    foreach (var epoch in range(1, tParams.Epochs))
                    {
                        // Loop over all batches
                        foreach (var (xTrain, yTrain) in feed.train.GetNextBatch(tParams.MinibatchSize))
                        {
                            // Run optimization op (backprop)
                            session.run(opt, (x, xTrain), (y, yTrain));
                        }

                        // Display logs per epoch step
                        if (epoch % tParams.ProgressStep == 0 || epoch == 1)
                        {
                            //get data
                            var (xTrain, yTrain) = feed.train.GetFullBatch();
                            var (xValid, yValid) = feed.valid.GetFullBatch();

                            //evaluate model
                            var resultsT = session.run(funs.ToArray(), (x, xTrain), (y, yTrain));
                            var resultsV = session.run(funs.ToArray(), (x, xValid), (y, yValid));
                            var evalFunctions = funs.Skip(1).Select(ev => ev.op.name.Substring(ev.op.name.LastIndexOf("/") + 1)).ToArray();

                            var progress = reportProgress(tParams, f, epoch, resultsT, resultsV, evalFunctions);
                            tParams.Progress(progress);

                            //processModel
                            processModel(session, progress);
                        }

                    }
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

        private static ProgressReport reportProgress(TrainingParameters tParams, int fold, int epoch,  NDArray[] resultsT, NDArray[] resultsV, string[] evalFuncs)
        {
            //report progress
            var pr = new ProgressReport()
            {
                ProgressType = ProgressType.Training,
                Epoch = epoch,
                Epochs = tParams.Epochs,
                KFold = tParams.KFold,
                Fold= fold,
                TrainLoss = resultsT.First(),
                ValidLoss = resultsV.First(),
            };

            if (epoch == tParams.Epochs)
                pr.ProgressType = ProgressType.Completed;
            if (epoch == 1)
                pr.ProgressType = ProgressType.Completed;

            //
            var evalsT = resultsT.Skip(1).ToArray();
            var evalsV = resultsV.Skip(1).ToArray();
            pr.TrainEval = new Dictionary<string, float>();
            pr.ValidEval = new Dictionary<string, float>();
            //
            for (int i = 0; i < evalsT.Length; i++)
            {
                pr.TrainEval.Add($"{evalFuncs[i]}", evalsT[i]);
                pr.ValidEval.Add($"{evalFuncs[i]}", evalsV[i]);
            }

            return pr;   
        }

    }
}
