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
using System.Linq;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

using AnnDotNet.Core.Layers;
using Daany.MathStuff.Stats;
using TorchSharp;

namespace Anndotnet.cmd.tool.Samples;

public class SlampTestSample :ISample
{

    public List<ColumnInfo> Metadata { get; set; }
    public DataParser       Parser   { get; set; }

    public SlampTestSample()
    {
        Parser = new DataParser();
        Parser.ColumnSeparator = ';';
        Parser.RawDataName = "mlconfigs/slumptest/slump.txt";
        Parser.HasHeader= false;
        Parser.SkipLines = 1;
        Parser.Header = new string[]
                        {
                            "Id", "Cement", "Slag", "Fly_ash","Water", "SP", "Coarse_Aggr", "Fine_Aggr", "SLUMP_cm_", "Flow_cm", "Strength"
                            //"Cement", "Slag", "Fly_ash", "Water", "SP", "Coarse_Aggr", "Strength"
                        };
    }

    DataFrame loadData()
    {
        //unzip the file
        var rawdata = DfExtensions.FromDataParser(Parser);

        //prepare the data
        var features = new string[] { "Cement", "Slag", "Fly_ash", "Water", "SP", "Coarse_Aggr" };
        var label = "Strength";

        var cols = features.Union(new string[] { label }).ToArray();
        var data = rawdata[cols];
        //change column type
        data.SetColumnType("Cement", ColType.F32 );
        data.SetColumnType("Slag", ColType.F32);
        data.SetColumnType("Fly_ash", ColType.F32);
        data.SetColumnType("Water", ColType.F32);
        data.SetColumnType("SP", ColType.F32);
        data.SetColumnType("Coarse_Aggr", ColType.F32);


        //parse the data and generate metadata.
        var mData = data.ParseMetadata(label);

        Metadata = mData;

        //fix missing values
        return data.HandlingMissingValue(mData);
    }

    public async Task<(torch.Tensor X, torch.Tensor Y)> GenerateData()
    {
        await Task.CompletedTask;

        var rawdata = loadData();

        (torch.Tensor X, torch.Tensor Y) = rawdata.TransformData(metadata: Metadata);

        return (X, Y);
    }

    public Task<(torch.Tensor X, torch.Tensor Y)> GeneratePredictionData(int rowCount)
    {
        throw new System.NotImplementedException();
    }

    public (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
    {
        var lParams = new LearningParameters()

                      {
                        EvaluationFunctions = new List<EvalFunction>()
                                              { 
                                                  EvalFunction.RMSE, 
                                                  EvalFunction.MSE
                                              },

                          LossFunction = LossFunction.MSE,
                          LearnerType = LearnerType.RMSProp,
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
                   new Dense{Type= LayerType.Dense, Name="FCLAyer01", OutputDim = 7, HasBias = true, Activation = Activation.None},
                   new Dense{Type= LayerType.Dense, Name="FCLAyer02", OutputDim = 1, HasBias = true, Activation = Activation.None},
               };
    }


}