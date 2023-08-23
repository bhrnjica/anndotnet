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

using Anndotnet.Core.Util;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using Daany.MathStuff.Random;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace AnnDotNet.Core.Trainers;


public class TvTrainer : ITrainer, IEvaluator
{
    private readonly float _minLR = 0.000001f;
    private readonly Module<Tensor, Tensor> _model;
    private readonly DataLoader _train;
    private readonly DataLoader _valid;
    private readonly TrainingParameters _tParams;
    private readonly LearningParameters _lParams;
    private readonly IProgressTraining _progress;
    private readonly Loss<Tensor, Tensor, Tensor> _loss;

    public TvTrainer(Module<Tensor, Tensor> model, DataLoader train, DataLoader valid, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed = 1234)
    {
        _model = model;

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

    public virtual async Task<bool> RunAsync()
    {
        var optimizer = MlFactory.CreateOptimizer(_model, _lParams);

        //early stopping
        //var scheduler = torch.optim.lr_scheduler.LinearLR(optimizer,last_epoch:_tParams.Epochs, verbose:true);

        for (var epoch = 1; epoch <= _tParams.Epochs; epoch++)
        {
            using var d = torch.NewDisposeScope();

            var (trainLoss, trainMetrics) = TrainMiniBatch(optimizer,  _train, epoch);

            var (evalLoss, validMetrics) = EvaluateModel<float>( optimizer,  _valid);

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

    private (float loss, Dictionary<string, float> metrics) TrainMiniBatch(Optimizer optimizer, DataLoader trainData, int epoch)
    {
        _model.train();
        
        float trainingLoss = 0;

        using (var d = torch.NewDisposeScope())
        {
            Tensor totPredicted = null;
            Tensor totTarget = null;

            foreach (var data in trainData)
            {
                optimizer.zero_grad();

                var predicted = _model.forward(data["X"]);

                var target = data["y"];
                //this is MCC problem with SoftMax or LogSoftMax
                if (_lParams.LossFunction == LossFunction.NLLLoss)
                {
                    //target data is multidimensional one hot encoding tensor
                    target = target.argmax(dim: 1);
                }

                var loss = CalculateLoss(predicted, target);

                loss.backward();

                optimizer.step();

                trainingLoss += loss.ToSingle();

                if (ReferenceEquals(totPredicted, null))
                {
                    totPredicted = predicted;
                    totTarget = target;
                }
                else
                {
                    totPredicted = torch.cat(new List<Tensor>{totPredicted, predicted}, 0);
                    totTarget = torch.cat(new List<Tensor> { totTarget, target }, 0);
                }

              //  d.DisposeEverything();
            }
            var result = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            return (trainingLoss, result);
        }

    }

    private (float eval_loss, Dictionary<string, float> metrics) EvaluateModel<T>( Optimizer optimizer, DataLoader evalData) where T : unmanaged
    {
        _model.eval();
        
        using (var d = torch.NewDisposeScope())
        {
            Tensor totPredicted = null;
            Tensor totTarget = null;
            float validLoss = 0;

            foreach (var data in evalData)
            {
                var predicted = _model.forward(data["X"]);
                var target = data["y"];

                //this is MCC problem with SoftMax or LogSoftMax
                if (_lParams.LossFunction == LossFunction.NLLLoss)
                {
                    //target data is multidimensional one hot encoding tensor
                    target = target.argmax(dim: 1);
                }

                var loss = CalculateLoss(predicted, target);

                validLoss += loss.ToSingle();


                if (ReferenceEquals(totPredicted, null))
                {
                    totPredicted = predicted;
                    totTarget = target;
                }
                else
                {
                    totPredicted = torch.cat(new List<Tensor> { totPredicted, predicted }, 0);
                    totTarget = torch.cat(new List<Tensor> { totTarget, target }, 0);
                }
            }

            var metrics = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            return ( validLoss, metrics);
        }
    }
    
    public Dictionary<string, float> CalculateMetrics(List<EvalFunction> evalFunctions, Tensor predicted, Tensor target)
    {
        var metrics = new Dictionary<string, float>();

        foreach (var eval in evalFunctions)
        {
           var keyValue = TorchMetrics.Evaluate(eval, predicted, target);
           metrics.Add(keyValue.Key, keyValue.Value);
        }

        return metrics;
    }
    public Tensor CalculateLoss(Tensor predicted, Tensor actual)
    {
        Tensor retVal = _loss.forward(predicted, actual);

        return retVal;

    }
}
