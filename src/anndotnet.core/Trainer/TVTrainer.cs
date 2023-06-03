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
using System.Runtime.CompilerServices;
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.NumPy;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace Anndotnet.Core.Trainers
{
    public class TVTrainer : ITrainer
    {
        
        DataFeed _train;
        DataFeed _valid;
        NDArray X;
        NDArray Y;
        int _percentageSplit;


        public TVTrainer(NDArray x, NDArray y, int percentageSplit = 20, bool shuffle = false, int seed= 1234)
        {
            X = x;
            Y = y;
            _percentageSplit = percentageSplit;
            initTrainer(seed,shuffle);
        }

        private void initTrainer(int seed, bool shuffle = false)
        {
            //

            (_train, _valid) = Split(seed, shuffle);

            
        }

        internal (DataFeed train, DataFeed validation) Split(int seed, bool shuffle = false)
        {
            int testSize = (int)((X.shape[0] * _percentageSplit) / 100);
            int trainSize = (int)X.shape[0] - testSize;

            //generate indexes
            var random = new Random(seed);
            var lst = Enumerable.Range(0, (int)X.shape[0]);
            var trainIds = shuffle ? lst.OrderBy(t => random.Next()).ToArray().Take(trainSize) : lst.Take(trainSize);
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

        public bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel)
        {
            //check for progress
            if (tParams.Progress == null)
            {
                var pt = new ProgressTVTraining();
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
                // Training cycle
                foreach (var epoch in range(1, tParams.Epochs + 1))
                {

                    int batchCount = 1;
                    // Loop over all batches
                    foreach (var (x_in, y_in) in _train.GetNextBatch(tParams.MinibatchSize))
                    {
                        // Run optimization op (backprop)
                        session.run(opt, (x, x_in), (y, y_in));
                        batchCount++;
                    }
                    //Console.WriteLine($"BatchCount {batchCount}");

                    // Display logs per epoch step
                    if (epoch % tParams.ProgressStep == 0 || epoch == 1 || epoch == tParams.Epochs)
                    {
                        var (x_input, y_input) = _train.GetFullBatch();
                        var (x_inputV, y_inputV) = _valid.GetFullBatch();

                        //evaluate model
                        var resultsT = session.run(funs.ToArray(), (x, x_input), (y, y_input));
                        var resultsV = session.run(funs.ToArray(), (x, x_inputV), (y, y_inputV));
                        var evalFunctions = funs.Skip(1).Select(ev => ev.op.name.Substring(ev.op.name.LastIndexOf("/") + 1)).ToArray();


                        var progress = reportProgress(tParams, epoch, resultsT, resultsV, evalFunctions);
                        tParams.Progress(progress);

                        ////process model
                        //if (progress.ProgressType == ProgressType.Completed)
                        //{
                        //    var W = session.graph.get_tensor_by_name("ReLuLayer/Relu:0");
                        //    var b = session.graph.get_tensor_by_name("FCLAyer01/b:0");
                        //    Console.WriteLine($"Training:  W={session.run(W, (x, x_input), (y, y_input))} b={session.run(b, (x, x_input), (y, y_input))}");
                        //}
                        processModel(session, progress);
                    }
                }

                //session.close();
                session.Dispose();
                return true;
            }
        }

        private static ProgressReport reportProgress(TrainingParameters tParams, int epoch, NDArray[] resultsT, NDArray[] resultsV, string[] evalFuncs)
        {
            //report progress
            var pr = new ProgressReport()
            {
                ProgressType = ProgressType.Training,
                Epoch = epoch,
                Epochs = tParams.Epochs,
                TrainLoss = resultsT.First(),
                ValidLoss = resultsV.First(),
            };

            if (epoch == tParams.Epochs)
                pr.ProgressType = ProgressType.Completed;
            if(epoch == 1)
                pr.ProgressType = ProgressType.Initialization;
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
