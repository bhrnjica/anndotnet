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

public class TitanicSample : ISample
{
    public List<ColumnInfo> Metadata { get; set; }
    public DataParser Parser { get; set; }

    public TitanicSample()
    {
        Parser = new DataParser();
        Parser.ColumnSeparator = ',';
        Parser.RawDataName = "mlconfigs/titanic/titanic_raw.txt";
        Parser.MissingValueSymbol = new char[] { '?' };

        //prepare the data
        Parser.Header = new string[] { "sex", "age", "sibsp", "parch", "pclass", "survived" };
    }

    DataFrame loaddata()
    {
        var colTypes = new ColType[] { ColType.IN, ColType.F32, ColType.F32, ColType.F32, ColType.IN, ColType.IN };

        //load titanic data into Daany.DataFrame
        var rawdata = DataFrame.FromCsv(filePath: Parser.RawDataName,
                                                    sep: Parser.ColumnSeparator,
                                            missingValues: Parser.MissingValueSymbol,
                                                    colTypes: colTypes,
                                                    names: Parser.Header);

        // 
        var mData = rawdata.ParseMetadata("survived");

        //define handling missing value
        mData[0].MissingValue = Aggregation.Random;//sex
        mData[1].MissingValue = Aggregation.Avg;//age
        mData[1].Transformer.DataNormalization = ColumnTransformer.MinMax;

        mData[2].MissingValue = Aggregation.Avg;//sibsp
        mData[3].MissingValue = Aggregation.Min; //parch

        mData[4].MissingValue = Aggregation.Mode;//pclass


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

    public async Task<(torch.Tensor X, torch.Tensor Y)> GeneratePredictionData(int rowCount)
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        var data = rawdata.TakeRandom(rowCount);

        (torch.Tensor x, torch.Tensor y) = data.TransformData(Metadata);

        return (x, y);
    }

    public (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
    {
        var lParams = new LearningParameters()

        {
            EvaluationFunctions = new List<EvalFunction>()
            {
                EvalFunction.BAcc,
            },

            LossFunction = LossFunction.BCE,
            LearnerType = LearnerType.SGD,
            LearningRate = 0.001f
        };

        var tParams = new TrainingParameters()
        {
            TrainingType = TrainingType.TvTraining,
            EarlyStopping = EarlyStopping.None,
            Epochs = 1000,
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
           new Dense{Type= LayerType.Dense, Name="FCLAyer01", OutputDim = 7, HasBias = true, Activation = Activation.ReLU},
           new Dense{Type= LayerType.Dense, Name="FCLAyer02", OutputDim = 1, HasBias = true, Activation = Activation.Sigmoid},
       };
    }


}