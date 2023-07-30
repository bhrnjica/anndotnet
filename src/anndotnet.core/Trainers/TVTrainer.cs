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
using Tensorflow;
using static Tensorflow.Binding;
using Tensorflow.NumPy;
using System.Data;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;


[assembly: InternalsVisibleTo("anndotnet.test")]
namespace AnnDotNet.Core.Trainers;

public class TVTrainer : ITrainer
{
    protected IProgressTraining Progress;
    DataFeed _train;
    DataFeed _valid;
    protected readonly NDArray _x;
    protected readonly NDArray _y;
    private readonly int _percentageSplit;


    public TVTrainer(NDArray x, NDArray y, IProgressTraining progress, int percentageSplit = 20,
        bool shuffle = false, int seed= 1234 )
    {
        _x = x;
        _y = y;
        _percentageSplit = percentageSplit;
        Progress = progress; 
        initTrainer(seed,shuffle);
    }

    private void initTrainer(int seed, bool shuffle = false)
    {
        (_train, _valid) = Split(seed, shuffle);
    }

    private (DataFeed train, DataFeed validation) Split(int seed, bool shuffle = false)
    {
        var testSize = (int)((_x.shape[0] * _percentageSplit) / 100);
        var trainSize = (int)_x.shape[0] - testSize;

        //generate indexes
        var random = new Random(seed);
        var lst = Enumerable.Range(0, (int)_x.shape[0]);
        var trainIds = shuffle ? lst.OrderBy(t => random.Next()).ToArray().Take(trainSize) : lst.Take(trainSize);
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

    public bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, 
                            Func<Session, ProgressReport, Session> processModel)
    {
        //check for progress
        if(!ReferenceEquals(Progress, null))
        {
            tParams.Progress = Progress.Run;
        }
            
        //get placeholders 
        var x = session.graph.get_tensor_by_name($"{TfScopes.Input}/X:0");
        var y = session.graph.get_tensor_by_name($"{TfScopes.Input}/Y:0");

        //get optimizer
        var opt = session.graph.get_operation_by_name($"{TfScopes.Train}/{TfScopes.Optimizer}/{lParams.LearnerType}");

        //
        using (session)
        {
            TrainMiniBatch(session, lParams, tParams, processModel, x, y, opt, 0, (_train, _valid));

            return true;
        }
    }

    protected void TrainMiniBatch(Session session, LearningParameters lParams, TrainingParameters tParams, 
        Func<Session, ProgressReport, Session> processModel, Tensor x, Tensor y, Operation opt, 
        int fold, (DataFeed train, DataFeed valid) feed)
    {
        var funs = extractFunctions(lParams, session);

        // Training cycle
        foreach (var epoch in Enumerable.Range(1, tParams.Epochs))
        {
            // Loop over all batches
            foreach (var (xTrain, yTrain) in feed.train.GetNextBatch(tParams.MiniBatchSize))
            {
                // Run optimization op (back-prop)
                session.run(opt, (x, xTrain), (y, yTrain));
            }

            evaluate(x, y, fold, epoch, funs, tParams, lParams, session, processModel);
        }
    }

    protected void evaluate(Tensor x, Tensor y, int fold, int epoch, List<Tensor> funs, TrainingParameters tParams, LearningParameters lParams, Session session, Func<Session, ProgressReport, Session> processModel)
    {
        // Display logs per epoch step
        if (epoch % tParams.ProgressStep == 0 || epoch == 1 || epoch == tParams.Epochs)
        {
            var (xInput, yInput) = _train.GetFullBatch();
            var (xInputV, yInputV) = _valid.GetFullBatch();

            //evaluate model
            var resultsT = session.run(funs.ToArray(), (x, xInput), (y, yInput));
            var resultsV = session.run(funs.ToArray(), (x, xInputV), (y, yInputV));

            var evalFunctions = funs.Skip(1).Select(ev => ev.op.name.Substring(ev.op.name.LastIndexOf("/") + 1)).ToArray();

            //report about training process
            var progress = CreateProgressReport(tParams, fold, epoch, resultsT, resultsV, evalFunctions);

            //report about training process
            //tParams.Progress;
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
        var evalT = resultsT.Skip(1).ToArray();
        var evalV = resultsV.Skip(1).ToArray();

        pr.TrainEval = new Dictionary<string, float>();
        pr.ValidEval = new Dictionary<string, float>();

        //
        for (var i = 0; i < evalT.Length; i++)
        {
            pr.TrainEval.Add($"{evalFuncs[i]}", evalT[i]);
            pr.ValidEval.Add($"{evalFuncs[i]}", evalV[i]);
        }

        return pr;
    }

    private static List<Tensor> extractFunctions(LearningParameters lParams, Session session)
    {
        //create list of loss and evaluation functions
        var funs = new List<Tensor>();

        //get loss function
        var loss = session.graph.get_tensor_by_name($"{TfScopes.Train}/{TfScopes.Loss}/{lParams.LossFunction}:0");
        funs.Add(loss);

        //extract evaluation functions
        for (int i = 0; i < lParams.EvaluationFunctions.Count(); i++)
        {
            var eFun = lParams.EvaluationFunctions[i];
            var eVal = session.graph.get_tensor_by_name($"{TfScopes.Train}/{TfScopes.Evaluation}{i}/{eFun.ToString()}:0");

            if (eVal == null)
            {
                throw new Exception($"Evaluation function {eFun.ToString()} is not found in the graph.");
            }

            funs.Add(eVal);
        }

        return funs;
    }
}