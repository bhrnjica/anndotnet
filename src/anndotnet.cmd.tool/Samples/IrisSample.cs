﻿///////////////////////////////////////////////////////////////////////////////
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
using AnnDotNet.Core;
using Daany;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Interfaces;

using AnnDotNet.Core.Layers;
using TorchSharp;

namespace Anndotnet.cmd.tool.Samples;

public class IrisSample : ISample
{
    public List<ColumnInfo> Metadata { get; set; }
    public DataParser Parser { get; set; }

    public IrisSample()
    {
        Parser = new DataParser();
        Parser.ColumnSeparator = ';';
        Parser.RawDataName = "mlconfigs/iris/iris_raw.txt";

        Parser.Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species" };
    }

    DataFrame loaddata()
    {
        //unzip the file
        var rawdata = DfExtensions.FromDataParser(Parser);
        var mData = rawdata.ParseMetadata("species");
        Metadata = mData;

        //fix missing values
        return rawdata.HandlingMissingValue(mData);
    }

    public async Task<(torch.Tensor X, torch.Tensor Y)> GenerateData()
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        (torch.Tensor X, torch.Tensor Y) = rawdata.TransformData(metadata: Metadata);

        return (X, Y);
    }

    public async Task<(torch.Tensor X, torch.Tensor Y)> GeneratePredictionData(int count)
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        var data = rawdata.TakeRandom(count);

        (torch.Tensor x, torch.Tensor y) = data.TransformData(Metadata);

        return (x, y);
    }
    public (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
    {
        var lParams = new LearningParameters()

        {
            EvaluationFunctions = new List<EvalFunction>()
                { EvalFunction.CAcc,EvalFunction.CErr },

            LossFunction = LossFunction.NLLLoss,
            LearnerType = LearnerType.MomentumSGD,
            LearningRate = 0.01f,
            Momentum = 0.9f,

        };

        var tParams = new TrainingParameters()
        {
            TrainingType = TrainingType.TvTraining,
            EarlyStopping = EarlyStopping.None,
            Epochs = 500,
            ProgressStep = 1,
            MiniBatchSize = 70,
            KFold = 5,
            SplitPercentage = 80,
            ShuffleWhenTraining = false,
            ShuffleWhenSplit = true,
            Retrain = true,
        };

        return (tParams, lParams);
    }

    public List<ILayer> CreateNet()
    {

        return new List<ILayer>()
        {
            new Dense{Type= LayerType.Dense, Name="FCLAyer01", OutputDim = 5, HasBias = true, Activation = Activation.TanH},
            new Dense{Type= LayerType.Dense, Name="FCLAyer02", OutputDim = 3, HasBias = true, Activation = Activation.Softmax},
        };
    }


}