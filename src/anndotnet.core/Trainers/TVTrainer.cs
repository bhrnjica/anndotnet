////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Util;
using Daany.MathStuff.Random;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Anndotnet.Core.Data;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Mlconfig;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace Anndotnet.Core.Trainers;


public class TvTrainer : ITrainer
{
    private readonly float _minLR = 0.000001f;
    private readonly Module<Tensor, Tensor> _model;
    private readonly Optimizer _optimizer;

    private readonly DataLoader _train;
    private readonly DataLoader _valid;

    private readonly TrainingParameters _tParams;
    private readonly LearningParameters _lParams;

    private readonly IProgressTraining _progress;
    private readonly Loss<Tensor, Tensor, Tensor> _loss;

    public TvTrainer(Module<Tensor, Tensor> model, Optimizer optimizer, DataLoader train, DataLoader valid, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed = 1234)
    {
        _model = model;
        _optimizer = optimizer;
        _tParams = tParams;
        _lParams = lParams;
        _progress = progress;
        _train = train;
        _valid = valid;
        _loss = MlFactory.CreateLoss(_lParams.LossFunction);

    }

    public TvTrainer(Module<Tensor, Tensor> model, DataLoader train, DataLoader valid, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed = 1234)
    {
        _model = model;
        _optimizer = MlFactory.CreateOptimizer(_model,lParams);
        _tParams = tParams;
        _lParams = lParams;
        _progress = progress;
        _train = train;
        _valid = valid;
        _loss = MlFactory.CreateLoss(_lParams.LossFunction);

    }

    public TvTrainer(Module<Tensor, Tensor> model, DataFeed trainData, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed= 1234 )
    {
        _model = model;
        _optimizer = MlFactory.CreateOptimizer(model, lParams);
        _tParams = tParams;
        _lParams = lParams;
        _progress = progress;

        (_train, _valid) = Split(trainData, seed);

        _loss = MlFactory.CreateLoss(_lParams.LossFunction);
       
    }

    internal (DataLoader train, DataLoader validation) Split(DataFeed data, int seed = 1234)
    {
        var trainSize = (long)(data.Count * _tParams.SplitPercentage / 100.0);
        var evalSize = data.Count - trainSize;

        var lst = LongEnumerable.Range(0, trainSize + evalSize).ToArray();

        var trainIds = _tParams.ShuffleWhenSplit ? TSRandom.Rand<long>(lst, (int)trainSize, seed).ToList() : lst.Take((int)trainSize).ToList();
        var testIds = lst.Except(trainIds).ToList();

        var train = new DataLoader(data, _tParams.MiniBatchSize, trainIds);
        var valid = new DataLoader(data, _tParams.MiniBatchSize, testIds);

        return (train, valid);
    }

    public async Task<bool> RunAsync()
    {
        //early stopping
        //var scheduler = torch.optim.lr_scheduler.LinearLR(optimizer,last_epoch:_tParams.Epochs, verbose:true);

        for (var epoch = 1; epoch <= _tParams.Epochs; epoch++)
        {
            using var d = torch.NewDisposeScope();

            var (trainLoss, trainMetrics) = TrainMiniBatch(_train, epoch);

            var (evalLoss, validMetrics) = EvaluateEpoch(_valid);

            //scheduler.step(trainLoss);
            
            ProgressReport report = new ProgressReport
            {
                Epochs = _tParams.Epochs,
                Epoch = epoch,
                ProgressType = ProgressType.Training,
                TrainLoss = trainLoss,
                ValidLoss = evalLoss,
                TrainEval = trainMetrics, 
                ValidEval = validMetrics
            };

            if (epoch % _tParams.ProgressStep == 0)
            {
                _progress.Run(report);
            }
            
        }

        await Task.CompletedTask;

        return true;
    }

    private (float loss, Dictionary<string, float> metrics) TrainMiniBatch(DataLoader trainData, int epoch)
    {
        _model.train();
        
        float trainingLoss = 0;

        using (var d = torch.NewDisposeScope())
        {
            Tensor totPredicted = null;
            Tensor totTarget = null;

            foreach (var data in trainData)
            {
                _optimizer.zero_grad();

                var predicted = _model.forward(data["X"]);

                var target = TargetTransform(data["y"], _lParams.LossFunction);

                var loss = CalculateLoss(predicted, target);

                loss.backward();

                _optimizer.step();

                trainingLoss += loss.ToSingle();

                AccumulateResults(predicted, ref totPredicted, target, ref totTarget);
            }

            var result = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            return (trainingLoss, result);
        }

    }

    private (float eval_loss, Dictionary<string, float> metrics) EvaluateEpoch(DataLoader evalData) 
    {
        _model.eval();
        
        using (var d = torch.NewDisposeScope())
        {
            Tensor totPredicted =null;
            Tensor totTarget = null;
            float validLoss = 0;

            foreach (var data in evalData)
            {
                var predicted = _model.forward(data["X"]);
 
                var target = TargetTransform(data["y"], _lParams.LossFunction);
                 
                var loss = CalculateLoss(predicted, target);

                validLoss += loss.ToSingle();

                AccumulateResults(predicted, ref totPredicted, target, ref totTarget);
            }

            var metrics = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            return ( validLoss, metrics);
        }
    }

    internal static Tensor TargetTransform(Tensor targetData, LossFunction loss)
    {
        //this is MCC problem with SoftMax or LogSoftMax
        if (loss == LossFunction.NLLLoss)
        {
            //target data is multidimensional one hot encoding tensor
            return targetData.flatten().to_type(ScalarType.Int64); //.argmax(dim: 1);
        }
        else if (loss == LossFunction.BCE)
        {
            return targetData.to_type(ScalarType.Float32);
        }

        return targetData;
    }

    internal static void AccumulateResults(Tensor predicted,ref Tensor totPredicted, Tensor target, ref Tensor totTarget)
    {
        //if (predicted.shape.Length == 2 && predicted.shape[1] == 1)
        //{
        //    predicted = predicted.flatten();
        //}

        //if (target.shape.Length == 2 && target.shape[1] == 1)
        //{
        //    target = target.flatten();
        //}

        if (ReferenceEquals(totPredicted, null))
        {
            totPredicted = torch.clone(predicted);
            totTarget = torch.clone(target); 
        }
        else
        {
            totPredicted = torch.cat(new List<Tensor> { totPredicted, predicted }, 0);
            totTarget = torch.cat(new List<Tensor> { totTarget, target },          0);
        }

        return;
    }

    private Dictionary<string, float> CalculateMetrics(List<EvalFunction> evalFunctions, Tensor predicted, Tensor target)
    {
        var metrics = new Dictionary<string, float>();

        foreach (var eval in evalFunctions)
        {
           var keyValue = TorchMetrics.Evaluate(eval, predicted, target);
           metrics.Add(keyValue.Key, keyValue.Value);
        }

        return metrics;
    }
    private Tensor CalculateLoss(Tensor predicted, Tensor actual)
    {
        Tensor retVal = _loss.forward(predicted, actual);

        return retVal;

    }
}
