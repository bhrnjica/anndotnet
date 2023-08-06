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
using Anndotnet.Core.Util;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using Daany.MathStuff.Random;
using TorchSharp.Modules;
using static System.Net.Mime.MediaTypeNames;
using static TorchSharp.torch.optim.lr_scheduler;
using static TorchSharp.torch.optim;
using static TorchSharp.torch.utils.data;
using TorchSharp;


[assembly: InternalsVisibleTo("anndotnet.test")]
namespace AnnDotNet.Core.Trainers;

public class TVTrainer : ITrainer
{
    private readonly Module<Tensor, Tensor> _model;
    private readonly DataLoader _train;
    private readonly DataLoader _valid;
    private readonly TrainingParameters _tParams;
    private readonly LearningParameters _lParams;
    private readonly IProgressTraining _progress;


    public TVTrainer(Module<Tensor, Tensor> model, DataFeed trainData, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed= 1234 )
    {
        _model = model;
        
        _tParams = tParams;
        _lParams = lParams;
        _progress = progress;

        (_train, _valid) = Split(trainData, seed);

    }


    public virtual async Task<bool> RunAsync()
    {
        var optimizer = MlFactory.CreateOptimizer(_model, _lParams);

        //early stopping
        var scheduler = torch.optim.lr_scheduler.StepLR(optimizer, 1, 0.75);
        var loss = MlFactory.CreateLoss(_lParams.LossFunction);

        for (var epoch = 1; epoch <= _tParams.Epochs; epoch++)
        {

            using (var d = torch.NewDisposeScope())
            {
                TrainMinibatch(_model, optimizer, loss, _train, epoch, (int)_train.Count);

                //Train(model, optimizer, torch.nn.NLLLoss(reduction: Reduction.Mean), train, epoch, train_data.Count);
                //Test(model, torch.nn.NLLLoss(reduction: torch.nn.Reduction.Sum), test, test_data.Count);

                Console.WriteLine($"End-of-epoch memory use: {GC.GetTotalMemory(false)}");
                scheduler.step();
            }
        }

        await Task.CompletedTask;

        return true;
    }

    private void TrainMinibatch(Module<Tensor, Tensor> model, Optimizer optimizer, Loss<torch.Tensor, torch.Tensor, torch.Tensor> loss, DataLoader trainData, int epoch, int batchCount)
    {
        
        model.train();

        long total = 0;

        Console.WriteLine($"Epoch: {epoch}...");

        using (var d = torch.NewDisposeScope())
        {

            foreach (var data in trainData)
            {
                optimizer.zero_grad();

                var target = data["y"];
                var prediction = model.call(data["X"]);
                var output = loss.call(prediction, target);

                output.backward();

                optimizer.step();

                total++;

                if (total == batchCount)
                {
                    Console.WriteLine($"\rTrain: epoch {epoch} [{total} / {trainData.Count}] Loss: {output.ToSingle():F4}");
                }

                d.DisposeEverything();
            }
        }
    }


    internal (DataLoader train, DataLoader validation) Split(DataFeed data, int seed = 1234)
    {
        var trainSize = (long)(data.Count * _tParams.SplitPercentage/100.0);
        var evalSize = data.Count - trainSize;

        var lst = LongEnumerable.Range(0, trainSize + evalSize).ToArray();

        var trainIds = _tParams.ShuffleWhenSplit ? TSRandom.Rand<long>(lst, (int)trainSize, seed).ToList() : lst.Take((int)trainSize).ToList();
        var testIds = lst.Except(trainIds).ToList();

        var train = new DataLoader(data, _tParams.MiniBatchSize, trainIds);
        var valid = new DataLoader(data, _tParams.MiniBatchSize, testIds);
        
        return (train, valid);
    }


}