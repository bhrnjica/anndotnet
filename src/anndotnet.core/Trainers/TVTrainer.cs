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
using TorchSharp.Modules;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace Anndotnet.Core.Trainers;


public class TvTrainer : ITrainer
{
    //private readonly float _minLR = 0.000001f;
    private readonly Module<Tensor, Tensor> _model;
    private readonly Optimizer _optimizer;

    private readonly DataLoader _train;
    private readonly DataLoader _valid;

    private readonly TrainingParameters _tParams;
    private readonly LearningParameters _lParams;

    private readonly IProgressTraining _progress;
    private readonly Loss<Tensor, Tensor, Tensor> _loss;
    
    // Learning rate scheduler settings
    private readonly bool _useScheduler;
    private readonly SchedulerType _schedulerType;
    private readonly int _stepSize;
    private readonly double _gamma;
    private readonly double _minLR;
    private readonly int _patience;
    private double _bestMetric = double.MaxValue;
    private readonly double _initialLR;
    
    // Gradient clipping settings
    private readonly double? _gradClipNorm;
    private readonly double? _gradClipValue;

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
        
        // Initialize gradient clipping if specified
        _gradClipNorm = lParams.GradientClipNorm;
        _gradClipValue = lParams.GradientClipValue;
        
        // Initialize learning rate scheduler settings
        _useScheduler = lParams.UseScheduler;
        _schedulerType = lParams.SchedulerType;
        _stepSize = lParams.StepSize;
        _gamma = lParams.Gamma;
        _minLR = lParams.MinLR;
        _patience = lParams.Patience;
        _initialLR = lParams.LearningRate;
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
        
        // Initialize gradient clipping if specified
        _gradClipNorm = lParams.GradientClipNorm;
        _gradClipValue = lParams.GradientClipValue;
        
        // Initialize learning rate scheduler settings
        _useScheduler = lParams.UseScheduler;
        _schedulerType = lParams.SchedulerType;
        _stepSize = lParams.StepSize;
        _gamma = lParams.Gamma;
        _minLR = lParams.MinLR;
        _patience = lParams.Patience;
        _initialLR = lParams.LearningRate;
    }

    public TvTrainer(Module<Tensor, Tensor> model, DataFeed trainData, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed= 1234 )
    {
        _model = model;
        _optimizer = model != null ? MlFactory.CreateOptimizer(model, lParams) : null;
        _tParams = tParams;
        _lParams = lParams;
        _progress = progress;

        (_train, _valid) = Split(trainData, seed);

        _loss = MlFactory.CreateLoss(_lParams.LossFunction);
        
        // Initialize gradient clipping if specified
        _gradClipNorm = lParams.GradientClipNorm;
        _gradClipValue = lParams.GradientClipValue;
        
        // Initialize learning rate scheduler settings
        _useScheduler = lParams.UseScheduler;
        _schedulerType = lParams.SchedulerType;
        _stepSize = lParams.StepSize;
        _gamma = lParams.Gamma;
        _minLR = lParams.MinLR;
        _patience = lParams.Patience;
        _initialLR = lParams.LearningRate;
    }


    internal (DataLoader train, DataLoader validation) Split(DataFeed data, int seed = 1234)
    {
        var trainSize = (long)(data.Count * _tParams.SplitPercentage / 100.0);
        var evalSize = data.Count - trainSize;

        var lst = LongEnumerable.Range(0, trainSize + evalSize).ToArray();

        var trainIds = _tParams.ShuffleWhenSplit ? TSRandom.Rand<long>(lst, (int)trainSize, seed).ToList() : lst.Take((int)trainSize).ToList();
        var testIds = lst.Except(trainIds).ToList();

        var train = DataLoader(data, _tParams.MiniBatchSize, trainIds);
        var valid = DataLoader(data, _tParams.MiniBatchSize, testIds);

        return (train, valid);
    }

    public async Task<bool> RunAsync()
    {
        float bestValidLoss = float.MaxValue;
        int epochsWithoutImprovement = 0;
        
        for (var epoch = 1; epoch <= _tParams.Epochs; epoch++)
        {
            using var d = torch.NewDisposeScope();

            var (trainLoss, trainMetrics) = TrainMiniBatch(_train, epoch);

            var (evalLoss, validMetrics) = EvaluateEpoch(_valid);

            // Update learning rate scheduler
            if (_useScheduler && _optimizer != null)
            {
                switch (_schedulerType)
                {
                    case SchedulerType.StepLR:
                        LearningRateSchedulers.StepLR(_optimizer, epoch, _stepSize, _gamma);
                        break;
                    case SchedulerType.ExponentialLR:
                        LearningRateSchedulers.ExponentialLR(_optimizer, epoch, _gamma);
                        break;
                    case SchedulerType.CosineAnnealingLR:
                        LearningRateSchedulers.CosineAnnealingLR(_optimizer, epoch, _tParams.Epochs, _minLR, _initialLR);
                        break;
                    case SchedulerType.ReduceLROnPlateau:
                        var (newBest, lrReduced) = LearningRateSchedulers.ReduceLROnPlateau(_optimizer, evalLoss, _bestMetric, _patience, _gamma, 1e-4, _minLR);
                        _bestMetric = newBest;
                        if (lrReduced)
                        {
                            Console.WriteLine($"Learning rate reduced at epoch {epoch}");
                        }
                        break;
                    case SchedulerType.LinearLR:
                        LearningRateSchedulers.LinearLR(_optimizer, epoch, _tParams.Epochs, 1.0/3, 1.0, _initialLR);
                        break;
                }
            }
            
            // Early stopping logic
            if (_tParams.EarlyStopping == EarlyStopping.ValidationLoss)
            {
                if (evalLoss < bestValidLoss)
                {
                    bestValidLoss = evalLoss;
                    epochsWithoutImprovement = 0;
                    
                    // Save best model checkpoint
                    if (!string.IsNullOrEmpty(_tParams.CheckpointPath))
                    {
                        var checkpointPath = Path.Combine(_tParams.CheckpointPath, "best_model");
                        ModelUtils.SaveCheckpoint(_model, _optimizer, epoch, evalLoss, validMetrics, checkpointPath);
                    }
                }
                else
                {
                    epochsWithoutImprovement++;
                    if (epochsWithoutImprovement >= _tParams.EarlyStoppingPatience)
                    {
                        Console.WriteLine($"Early stopping triggered after {epoch} epochs");
                        break;
                    }
                }
            }
            
            var currentLR = _optimizer != null ? LearningRateSchedulers.GetCurrentLearningRate(_optimizer) : 0;
            
            ProgressReport report = new ProgressReport
            {
                Epochs = _tParams.Epochs,
                Epoch = epoch,
                ProgressType = ProgressType.Training,
                TrainLoss = trainLoss,
                ValidLoss = evalLoss,
                TrainEval = trainMetrics, 
                ValidEval = validMetrics,
                LearningRate = (float)currentLR
            };

            if (epoch % _tParams.ProgressStep == 0)
            {
                _progress.Run(report);
            }
            
            // Save periodic checkpoint
            if (_tParams.CheckpointFrequency > 0 && epoch % _tParams.CheckpointFrequency == 0 
                && !string.IsNullOrEmpty(_tParams.CheckpointPath))
            {
                var checkpointPath = Path.Combine(_tParams.CheckpointPath, $"epoch_{epoch}");
                ModelUtils.SaveCheckpoint(_model, _optimizer, epoch, evalLoss, validMetrics, checkpointPath);
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

                // Apply gradient clipping if specified
                if (_gradClipNorm.HasValue)
                {
                    ModelUtils.ClipGradientNorm(_model, _gradClipNorm.Value);
                }
                else if (_gradClipValue.HasValue)
                {
                    ModelUtils.ClipGradientValue(_model, _gradClipValue.Value);
                }

                _optimizer.step();

                trainingLoss += loss.ToSingle();

                AccumulateResults(predicted, ref totPredicted, target, ref totTarget);
            }

            var result = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            
            // Clean up accumulated tensors
            totPredicted?.Dispose();
            totTarget?.Dispose();
            
            return (trainingLoss, result);
        }
    }

    private (float eval_loss, Dictionary<string, float> metrics) EvaluateEpoch(DataLoader evalData) 
    {
        _model.eval();
        
        using (var d = torch.NewDisposeScope())
        {
            Tensor totPredicted = null;
            Tensor totTarget = null;
            float validLoss = 0;

            using (torch.no_grad())  // Disable gradients for evaluation
            {
                foreach (var data in evalData)
                {
                    var predicted = _model.forward(data["X"]);
 
                    var target = TargetTransform(data["y"], _lParams.LossFunction);
                 
                    var loss = CalculateLoss(predicted, target);

                    validLoss += loss.ToSingle();

                    AccumulateResults(predicted, ref totPredicted, target, ref totTarget);
                }
            }

            var metrics = CalculateMetrics(_lParams.EvaluationFunctions, totPredicted, totTarget);
            
            // Clean up accumulated tensors
            totPredicted?.Dispose();
            totTarget?.Dispose();
            
            return (validLoss, metrics);
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

    internal static void AccumulateResults(Tensor predicted, ref Tensor totPredicted, Tensor target, ref Tensor totTarget)
    {
        if (totPredicted is null)
        {
            totPredicted = torch.clone(predicted).detach();
            totTarget = torch.clone(target).detach(); 
        }
        else
        {
            var newPredicted = torch.cat(new List<Tensor> { totPredicted, predicted.detach() }, 0);
            var newTarget = torch.cat(new List<Tensor> { totTarget!, target.detach() }, 0);
            
            // Dispose old tensors to free memory
            totPredicted.Dispose();
            totTarget.Dispose();
            
            totPredicted = newPredicted;
            totTarget = newTarget;
        }
    }

    private Dictionary<string, float> CalculateMetrics(List<EvalFunction> evalFunctions, Tensor predicted, Tensor target)
    {
        // Use the improved batch calculation to reduce GPU-CPU transfers
        var batchMetrics = TorchMetrics.CalculateMetricsBatch(evalFunctions, predicted, target);
        
        var metrics = new Dictionary<string, float>();
        foreach (var kvp in batchMetrics)
        {
            metrics.Add(kvp.Key.ToString(), kvp.Value);
        }

        return metrics;
    }
    private Tensor CalculateLoss(Tensor predicted, Tensor actual)
    {
        Tensor retVal = _loss.forward(predicted, actual);

        return retVal;

    }
}
