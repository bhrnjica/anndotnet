////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Interfaces;
using TorchSharp;
using Xunit;

using static TorchSharp.torch;
using static TorchSharp.torch.optim;
using static TorchSharp.torch.nn;
using static TorchSharp.torchvision;


namespace AnnDotNet.test;

public class IrisTrain_Test
{
    public IrisTrain_Test()
    {
        
    }

    [Fact]
    public void Train_Iris_Test01()
    {
        var irisData = TestDataProvider.PrepareIrisData();
        var irisDataFeed = new DataFeed("IrisData",irisData.Item1,irisData.Item2);

        var model = new IrisModel("model", 4, torch.CPU);

        var optimizer = torch.optim.Adam(model.parameters());

        var scheduler = torch.optim.lr_scheduler.StepLR(optimizer, 1, 0.75);

        int maxEpochs = 100;

        for (var epoch = 1; epoch <= maxEpochs; epoch++)
        {

            using (var d = torch.NewDisposeScope())
            {

                Train(model, optimizer, torch.nn.CrossEntropyLoss(), irisDataFeed, epoch, irisDataFeed.Count);
                Test(model, torch.nn.CrossEntropyLoss(reduction: torch.nn.Reduction.Sum), irisDataFeed, irisDataFeed.Count);

                Console.WriteLine($"End-of-epoch memory use: {GC.GetTotalMemory(false)}");

                scheduler.step();
            }
        }

        Console.WriteLine("Saving model to '{0}'", irisDataFeed.Name + ".model.bin");
        model.save(irisDataFeed.Name + ".model.bin");


    }

    private static void Train(IrisModel model, Optimizer optimizer,Loss<Tensor, Tensor, Tensor> loss, IDataFeed dataFeed, int epoch, long size)
    {
        model.train();

        int batchId = 1;
        long total = 0;

        Console.WriteLine($"Epoch: {epoch}...");

        using (var d = torch.NewDisposeScope())
        {

            foreach (var data in dataFeed.GetNextBatch(size))
            {
                optimizer.zero_grad();

                var target = data.yBatch;
                var prediction = model.call(data.xBatch);
                var output = loss.call(prediction, target);

                output.backward();

                optimizer.step();

                total += target.shape[0];

                if (/*batchId % _logInterval == 0 ||*/ total == size)
                {
                    Console.WriteLine($"\rTrain: epoch {epoch} [{total} / {size}] Loss: {output.ToSingle():F4}");
                }

                batchId++;

                d.DisposeEverything();
            }
        }
    }

    private static void Test(IrisModel model, Loss<torch.Tensor, torch.Tensor, torch.Tensor> loss, IDataFeed dataLoader,int size)
    {
        model.eval();

        double testLoss = 0;
        int correct = 0;

        using (var d = torch.NewDisposeScope())
        {

            foreach (var data in dataLoader.GetNextBatch(size))
            {
                var prediction = model.call(data.xBatch);
                var output = loss.call(prediction, data.yBatch);
                testLoss += output.ToSingle();

                var pred = prediction.argmax(1);
                var actual = data.yBatch.argmax(1);
                correct += pred.eq(actual).sum().ToInt32();

                d.DisposeEverything();
            }
        }

        Console.WriteLine($"Size: {size}, Total: {size}");

        Console.WriteLine($"\rTest set: Average loss {(testLoss / size):F4} | Accuracy {((double)correct / size):P2}");
    }


    internal class IrisModel : Module<Tensor, Tensor>
    {
        List<Module<Tensor, Tensor>> model = new List<Module<Tensor, Tensor>>();

        private Module<Tensor, Tensor> fc1 = Linear(4, 7);
        private Module<Tensor, Tensor> relu1 = ReLU();
        private Module<Tensor, Tensor> fc2 = Linear(7, 3);
        private Module<Tensor, Tensor> relu2 = ReLU();
        private Module<Tensor, Tensor> softMax = Softmax(1);


        public IrisModel(string name,int inputDim, torch.Device device = null) : base(name)
        {
            RegisterComponents();

            if (device != null && device.type == DeviceType.CUDA)
            {
                this.to(device);
            }

            InitModel(inputDim);
        }

        private void InitModel(int inputDim)
        {
            model.Add(Linear(inputDim, 7));
            model.Add(ReLU());
            model.Add(Linear(7, 3));
            model.Add(ReLU());

            model.Add(Softmax(1));
        }

        public override Tensor forward(Tensor input)
        {
            var layer= model[0].forward(input);

            for (var i=1; i<model.Count; i++)
            {
                layer = model[i].forward(layer);
            }
            return layer;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var m in model)
                {
                    m.Dispose();
                }
                
                ClearModules();
            }
            base.Dispose(disposing);
        }
    }

}