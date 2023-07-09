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

using AnnDotNet.Core;
using AnnDotNet.Core.Extensions;
using Daany;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Vnd.Layers;
using Tensorflow.NumPy;

namespace AnnDotNet.Vnd.Samples;

public  class TitanicSample : ISample
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

        mData[5].MissingValue = Aggregation.Random;//survived
        mData[5].Transformer.DataNormalization = ColumnTransformer.Dummy;
        mData[5].MLType = MLColumnType.Label;    
        Metadata = mData;

        //fix missing values
        return rawdata.HandlingMissingValue(mData);
    }


    public async Task<(NDArray X, NDArray Y)> GenerateData()
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        (NDArray X, NDArray Y) = rawdata.TransformData(metadata: Metadata);

        return (X, Y);

    }

    public async Task<(NDArray X, NDArray Y)> GeneratePredictionData(int rowCount)
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        var data = rawdata.TakeRandom(rowCount);

        (NDArray x, NDArray y) = data.TransformData(Metadata);

        return (x, y);
   }

    public  (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
    {
        var lParams = new LearningParameters()

        {
            EvaluationFunctions = new List<Metrics>()
            {
                Metrics.CAcc,
            },

            LossFunction = Metrics.BCE,
            LearnerType = LearnerType.SGD,
            LearningRate = 0.001f
        };

        var tParams = new TrainingParameters()
        {
            Retrain= false,
            Epochs = 1000,
            MiniBatchSize = 50,
        };

        return (tParams, lParams);
    }

    public  List<ILayer>  CreateNet()
    {
        return new List<ILayer>()
        {
            new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 7 },
            new ActLayer(){Type= LayerType.Activation, Name="ReLu", Activation=Activation.ReLU},
            new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 1 },
            new ActLayer(){Type= LayerType.Activation, Name="Sigmoid", Activation=Activation.Sigmoid},
        };
    }
       
    
}