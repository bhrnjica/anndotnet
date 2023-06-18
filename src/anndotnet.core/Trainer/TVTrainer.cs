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
using System.Runtime.CompilerServices;
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.NumPy;
using System.Data;
using XPlot.Plotly;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace Anndotnet.Core.Trainers
{
    public class TVTrainer : ITrainer
    {
        protected IProgressTraining _progress;
        DataFeed _train;
        DataFeed _valid;
        protected NDArray X;
        protected NDArray Y;
        int _percentageSplit;


        public TVTrainer(NDArray x, NDArray y, IProgressTraining progress, int percentageSplit = 20,
                            bool shuffle = false, int seed= 1234 )
        {
            X = x;
            Y = y;
            _percentageSplit = percentageSplit;
            _progress = progress; 
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
            if(!ReferenceEquals(_progress, null))
            {
                tParams.Progress = _progress.Run;
            }
            
            //get placeholders 
            var x = session.graph.get_tensor_by_name("Input/X:0");
            var y = session.graph.get_tensor_by_name("Input/Y:0");

            //get optimizer
            var opt = session.graph.get_operation_by_name($"Train/Optimizer/{lParams.LearnerType}");

            //
            using (session)
            {
                TrainminiBatch(session, lParams, tParams, processModel, x, y, opt, 0, (_train, _valid));

                return true;
            }
        }

        protected void TrainminiBatch(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel, Tensor x, Tensor y, Operation opt, int fold, (DataFeed train, DataFeed valid) feed)
        {
            // Training cycle
            foreach (var epoch in Enumerable.Range(1, tParams.Epochs))
            {
                // Loop over all batches
                foreach (var (xTrain, yTrain) in feed.train.GetNextBatch(tParams.MinibatchSize))
                {
                    // Run optimization op (backprop)
                    session.run(opt, (x, xTrain), (y, yTrain));
                }


                evaluate(fold, epoch, tParams, lParams, session, processModel);
            }
        }

        protected void evaluate(int fold, int epoch, TrainingParameters tParams, LearningParameters lParams, Session session, Func<Session, ProgressReport, Session> processModel)
        {
            //get placeholders 
            var x = session.graph.get_tensor_by_name("Input/X:0");
            var y = session.graph.get_tensor_by_name("Input/Y:0");

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
                {
                    throw new Exception($"Evaluation function {efun.ToString()} is not found in the graph.");       
                }

                funs.Add(eval);
            }

            // Display logs per epoch step
            if (epoch % tParams.ProgressStep == 0 || epoch == 1 || epoch == tParams.Epochs)
            {
                var (x_input, y_input) = _train.GetFullBatch();
                var (x_inputV, y_inputV) = _valid.GetFullBatch();

                //evaluate model
                var resultsT = session.run(funs.ToArray(), (x, x_input), (y, y_input));
                var resultsV = session.run(funs.ToArray(), (x, x_inputV), (y, y_inputV));

                var evalFunctions = funs.Skip(1).Select(ev => ev.op.name.Substring(ev.op.name.LastIndexOf("/") + 1)).ToArray();

                //report about training process
                var progress = CreateProgressReport(tParams, fold, epoch, resultsT, resultsV, evalFunctions);

                //report about training process
                tParams.Progress(progress);

                processModel(session, progress);
            }
        }

        public ProgressReport CreateProgressReport(TrainingParameters tParams,int fold, int epoch, NDArray[] resultsT, NDArray[] resultsV, string[] evalFuncs)
        {
            //report progress
            var pr = new ProgressReport()
            {
                ProgressType = ((fold==0 || fold== tParams.KFold) && epoch == tParams.Epochs) ? 
                                                                        ProgressType.Completed :
                                                                        ProgressType.Training,
                Fold = fold,
                KFold = tParams.KFold,
                Epoch = epoch,
                Epochs = tParams.Epochs,
                TrainLoss = resultsT.First(),
                ValidLoss = resultsV.First(),
            };

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
