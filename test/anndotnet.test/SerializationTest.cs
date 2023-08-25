using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnnDotNet.Core;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Layers;
using AnnDotNet.Core.Util;
using Daany;
using TorchSharp.Modules;
using Xunit;

using static TorchSharp.torch;
using Dropout = AnnDotNet.Core.Layers.Dropout;


namespace AnnDotNet.test;

public class SerializationTests
{
    private readonly string ml_file = """
        {
          "Id": "c15b26ba-2876-4cdd-b97a-02b3fa83944b",
          "Parser": {
            "RowSeparator": "\r\n",
            "ColumnSeparator": ";",
            "HasHeader": false,
            "SkipLines": 0,
            "MissingValueSymbol": null,
            "DescriptionSymbol": "!",
            "RawDataName": "mlconfigs/iris/iris_raw.txt",
            "Header": [
              "sepal_length",
              "sepal_width",
              "petal_length",
              "petal_width",
              "species"
            ],
            "DateFormat": null
          },
           "Metadata": [
            {
              "Id": 0,
              "Name": "sepal_length",
              "MLType": "feature",
              "ValueColumnType": "f32",
              "ValueFormat": null,
              "MissingValue": "none",
              "Transformer": {
                "DataNormalization": "standardizer",
                "ClassValues": null,
                "NormalizationValues": [
                  5.8433332,
                  0.8280661
                ]
              }
            },
            {
              "Id": 1,
              "Name": "sepal_width",
              "MLType": "feature",
              "ValueColumnType": "f32",
              "ValueFormat": null,
              "MissingValue": "none",
              "Transformer": {
                "DataNormalization": "standardizer",
                "ClassValues": null,
                "NormalizationValues": [
                  3.0573332,
                  0.4358663
                ]
              }
            },
            {
              "Id": 2,
              "Name": "petal_length",
              "MLType": "feature",
              "ValueColumnType": "f32",
              "ValueFormat": null,
              "MissingValue": "none",
              "Transformer": {
                "DataNormalization": "standardizer",
                "ClassValues": null,
                "NormalizationValues": [
                  3.758,
                  1.7652982
                ]
              }
            },
            {
              "Id": 3,
              "Name": "petal_width",
              "MLType": "feature",
              "ValueColumnType": "f32",
              "ValueFormat": null,
              "MissingValue": "none",
              "Transformer": {
                "DataNormalization": "standardizer",
                "ClassValues": null,
                "NormalizationValues": [
                  1.1993333,
                  0.76223767
                ]
              }
            },
            {
              "Id": 4,
              "Name": "species",
              "MLType": "label",
              "ValueColumnType": "in",
              "ValueFormat": null,
              "MissingValue": "none",
              "Transformer": {
                "DataNormalization": "oneHot",
                "ClassValues": [
                  "setosa",
                  "versicolor",
                  "virginica"
                ],
                "NormalizationValues": null
              }
            }
          ],
          "Network": [
            {
              "Type": "Dense",
              "HasBias": true,
              "OutputDim": 7,
              "Activation": "ReLU",
              "Name": "Dense01"
            },
            {
              "Type": "Dropout",
              "Rate": 0.5,
              "Name": "Dropout01"
            },
            {
              "Type": "Dense",
              "HasBias": true,
              "OutputDim": 3,
              "Activation": "Softmax",
              "Name": "Dense02"
            }
          ],
          "LearningParameters": {
            "LearnerType": "momentumSGD",
            "LossFunction": "nllLoss",
            "EvaluationFunctions": [
              "cAcc",
              "cErr"
            ],
            "LearningRate": 0.01,
            "UsePolyDecay": false,
            "StartLRate": 0,
            "EndLRate": 0,
            "DecayPower": 0,
            "DecaySteps": 0,
            "Momentum": 0,
            "L1Regularizer": 0,
            "L2Regularizer": 0,
            "Alpha": 0,
            "Eps": 0,
            "Beta1": 0,
            "Beta2": 0,
            "WeightDecay": 0,
            "Rho": 0
          },
          "TrainingParameters": {
            "TrainingType": "tvTraining",
            "EarlyStopping": "none",
            "Retrain": true,
            "Epochs": 500,
            "ProgressStep": 10,
            "MiniBatchSize": 100,
            "KFold": 5,
            "SplitPercentage": 20,
            "LastBestModel": null,
            "ShuffleWhenSplit": false,
            "ShuffleWhenTraining": false
          },
          "Paths": {
            "MLConfig": "iris.mlconfig",
            "Root": "mlconfigs/iris",
            "Models": "models",
            "BestModel": "638227190040989949.ckp"
          },
          "Name": "empty"
        }
        """;
    [Fact]
    public void Test_Create_factory_from_file()
    {
        var mlConfig = MlFactory.LoadFromString(ml_file);

        Assert.NotNull(mlConfig);

        Assert.True(mlConfig.Network.Count==3);

        Assert.True(mlConfig.Metadata.Count==5);

        Assert.Equal("NLLLoss", mlConfig.LearningParameters.LossFunction.ToString());

        Assert.True(mlConfig.TrainingParameters.Epochs==500);   

        Assert.True(mlConfig.Paths["Root"]=="mlconfigs/iris");

        Assert.True(mlConfig.Paths["Models"]=="models");   

    }

    




    [Fact]
    public void Test_Save_MlConfig_To_String()
    {
        MlConfig mlConfig = MlFactory.CreatEmptyMlConfig("c15b26ba-2876-4cdd-b97a-02b3fa83944b");

        mlConfig.Parser = new DataParser()
        {
            RowSeparator = "\r\n",
            ColumnSeparator = ';',
            HasHeader = false,
            SkipLines = 0,
            MissingValueSymbol = null,
            DescriptionSymbol = '!',
            RawDataName = "mlconfigs/iris/iris_raw.txt",
            Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species" }
        };

        mlConfig.Metadata.Add(new ColumnInfo()
        {
            Id = 0, 
            Name = "sepal_length", 
            MLType = MLColumnType.Feature, 
            ValueColumnType = Daany.ColType.F32, 
            MissingValue = Aggregation.None, 
            ValueFormat = null, 
            Transformer = new DataTransformer(Daany.ColumnTransformer.Standardizer,
                                    null, 
                                    new float[] { 5.8433332f, 0.8280661f })
        });
        mlConfig.Metadata.Add(new ColumnInfo()
        {
            Id = 1, 
            Name = "sepal_width", 
            MLType = MLColumnType.Feature, 
            ValueColumnType = Daany.ColType.F32, 
            MissingValue = Aggregation.None, 
            ValueFormat = null, 
            Transformer = new DataTransformer(Daany.ColumnTransformer.Standardizer, 
                                    null,
                                    new float[] { 3.0573332f, 0.4358663f })
        });
        mlConfig.Metadata.Add(new ColumnInfo()
        {
            Id = 2,
            Name = "petal_length",
            MLType = MLColumnType.Feature,
            ValueColumnType = Daany.ColType.F32,
            MissingValue = Aggregation.None,
            ValueFormat = null,
            Transformer = new DataTransformer(Daany.ColumnTransformer.Standardizer,
                                    null, new float[] { 3.758f, 1.7652982f })
        });
        mlConfig.Metadata.Add(new ColumnInfo()
        {
            Id = 3,
            Name = "petal_width",
            MLType = MLColumnType.Feature,
            ValueColumnType = Daany.ColType.F32,
            MissingValue = Aggregation.None,
            ValueFormat = null,
            Transformer = new DataTransformer(Daany.ColumnTransformer.Standardizer, null,
                               new float[] { 1.1993333f, 0.76223767f })
        });
        mlConfig.Metadata.Add(new ColumnInfo()
        {
            Id = 4, 
            Name = "species", 
            MLType = MLColumnType.Label, 
            ValueColumnType = Daany.ColType.IN,
            MissingValue = Aggregation.None,
            ValueFormat = null,
            Transformer = new DataTransformer(ColumnTransformer.OneHot, new string[3] { "setosa", "versicolor", "virginica" }, null)
        });

        //network
        mlConfig.Network.Add(new Dense()
        {
            OutputDim = 7, Name = "Dense01", Activation = Activation.ReLU, HasBias = true
        });
        mlConfig.Network.Add(new Dropout()
        {
           Name = "Dropout01", Rate = 0.5f,
        });
        mlConfig.Network.Add(new Dense()
            {
                OutputDim = 3,Name = "Dense02", Activation = Activation.Softmax, HasBias = true
            });


        mlConfig.LearningParameters = new LearningParameters()
        {
            LearnerType = LearnerType.MomentumSGD,
            LossFunction = LossFunction.NLLLoss,
            EvaluationFunctions = new List<EvalFunction>() { EvalFunction.CAcc,EvalFunction.CErr },
            LearningRate = 0.01f,
            UsePolyDecay = false,
            StartLRate = 0,
            EndLRate = 0,
            DecayPower = 0,
            DecaySteps = 0,
            Momentum = 0,
            L1Regularizer = 0,
            L2Regularizer = 0,
            Alpha = 0.0,
            Eps = 0.0,
            Beta1 = 0.0,
            Beta2 = 0.0,
            WeightDecay = 0.0,
            Rho = 0.0
        }; 
        
        mlConfig.TrainingParameters = new TrainingParameters()
        {
            TrainingType = TrainingType.TvTraining,
            EarlyStopping = EarlyStopping.None,
            Retrain = true,
            Epochs = 500,
            ProgressStep = 10,
            MiniBatchSize = 100,
            KFold = 5,
            SplitPercentage = 20,
            LastBestModel = null,
            ShuffleWhenSplit = false,
            ShuffleWhenTraining = false
        };

        mlConfig.Paths = new Dictionary<string, string>()
        {
            { "MLConfig", "iris.mlconfig" },
            { "Root", "mlconfigs/iris" },
            { "Models", "models" },
            { "BestModel", "638227190040989949.ckp" }
        };


        var strJson = MlFactory.SaveToString(mlConfig);

        var str1 = strJson.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var str2 = ml_file.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < str2.Length; i++)
        {
            Assert.Equal(str1[i].Trim(), str2[i].Trim());
        }
        

    }
}