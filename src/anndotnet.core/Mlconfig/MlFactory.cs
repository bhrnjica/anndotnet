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


using Daany;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Interfaces;
using Anndotnet.Core.Model;

using AnnDotNet.Core.Util;

namespace AnnDotNet.Core;

public class MlFactory
{
    public MlConfig MLConfig { get; set; }

    internal static (Tensor xData, torch.Tensor yData) PrepareData(MlConfig mlConfig)
    {
        var dateFormat = mlConfig.Metadata.Where(x => x.ValueColumnType == ColType.DT)
                                                .Select(x => x.ValueFormat).FirstOrDefault();
            
        //load data into DataFrame
        var df = DataFrame.FromCsv(filePath: $"{mlConfig.Parser.RawDataName}",sep: mlConfig.Parser.ColumnSeparator, 
            names: mlConfig.Parser.Header, colTypes: mlConfig.Metadata.Select(x=>x.ValueColumnType).ToArray(),
            missingValues: mlConfig.Parser.MissingValueSymbol, skipLines: mlConfig.Parser.SkipLines, dformat: dateFormat);

        return df.TransformData(mlConfig.Metadata);
    }

    public static AnnModel CreateNetwork(string name, List<ILayer> layers, int inputDim, int outputDim, Device device, int seed = 1234)
    {
        torch.random.manual_seed(seed);
        return new AnnModel(name, layers, inputDim, outputDim, device);
    }

    
    public static async Task<bool> SaveToFile(MlConfig mlConfig, string filePath)
    {
        var options = JsonSerializerOptions();
        //
        using (FileStream fs = File.Create(filePath))
        {
            //mlConfig.Paths.Remove("MLConfig");
            //mlConfig.Paths.Remove("MainFolder");
            await JsonSerializer.SerializeAsync<MlConfig>(fs, mlConfig,options: options);
        }

        return true;
    }

    public static string SaveToString(MlConfig mlConfig)
    {
        var options = JsonSerializerOptions();

        string jsonString = JsonSerializer.Serialize(mlConfig, options);

        return jsonString;
       
    }

    public static MlConfig LoadFromString(string jsonContent)
    {
        MlConfig mlConfig = null;

        var options = JsonSerializerOptions();

        mlConfig = JsonSerializer.Deserialize<MlConfig>(jsonContent,options);

        return mlConfig;
    }

    public static async Task<MlConfig> LoadfromFileAsync(string filePath)
    {
        MlConfig mlConfig = null;
        var options = JsonSerializerOptions();

        //
        using (FileStream fs = File.OpenRead(filePath))
        {
            mlConfig =  await JsonSerializer.DeserializeAsync<MlConfig>(fs, options: options);
        }


        //Set the current directory.
        //if (mlConfig.Paths == null)
        //    mlConfig.Paths = new Dictionary<string, string>();

        //var dir = Path.GetDirectoryName(filePath);
        //var di = new DirectoryInfo(filePath);

        ////define mainfolder
        //if(!mlConfig.Paths.ContainsKey("MainFolder"))
        //    mlConfig.Paths.Add("MainFolder", di.Parent.FullName);
        //else
        //    mlConfig.Paths["MainFolder"]= di.Parent.FullName;

        ////save ml config file path
        //if (!mlConfig.Paths.ContainsKey("MLConfig"))
        //    mlConfig.Paths.Add("MLConfig", new FileInfo(filePath).FullName);
        //else
        //    mlConfig.Paths["MLConfig"] =new FileInfo(filePath).FullName;


        return mlConfig;
    }

    private static JsonSerializerOptions JsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        //add enum string converters
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new LayerConverter());
        return options;
    }

    public static MlConfig CreatEmptyMlConfig(string guid)
    {
        var mlConfig = new MlConfig(guid, "empty");
        return mlConfig;
    }

    public static torch.optim.Optimizer CreateOptimizer(Module<Tensor, Tensor> model, LearningParameters lParams)
    {
        switch (lParams.LearnerType)
        {
            case LearnerType.SGD:
                return torch.optim.SGD(model.parameters(), lParams.LearningRate);
            case LearnerType.MomentumSGD:
                return torch.optim.SGD(model.parameters(), lParams.LearningRate, lParams.Momentum, 0D,lParams.WeightDecay);
            case LearnerType.RMSProp:
                return torch.optim.RMSProp(model.parameters(), lParams.LearningRate, lParams.Alpha, lParams.Eps,
                    lParams.DecayPower, lParams.Momentum);
            case LearnerType.FSAdaGrad:
            case LearnerType.AdaGrad:
                return torch.optim.Adagrad(model.parameters(), lParams.LearningRate, lParams.Eps, lParams.DecayPower);
            case LearnerType.Adam:
                return torch.optim.Adam(model.parameters(), lParams.LearningRate, lParams.Beta1, lParams.Beta2,
                                       lParams.Eps, lParams.WeightDecay);
            case LearnerType.AdaDelta:
                return torch.optim.Adadelta(model.parameters(), lParams.LearningRate, lParams.Rho, lParams.Eps,
                                                               lParams.WeightDecay);
            default:
                return torch.optim.SGD(model.parameters(), lParams.LearningRate);
        }
    }

    public static Loss<Tensor,Tensor,Tensor> CreateLoss(LossFunction lossFunction)
    {
        switch (lossFunction)
        {
            case LossFunction.AE:
                return torch.nn.L1Loss();
            case LossFunction.MSE: 
                return torch.nn.MSELoss();
            case LossFunction.BCE:  
                return torch.nn.BCELoss();
            case LossFunction.CCE:
                return torch.nn.CrossEntropyLoss();
            case LossFunction.NLLLoss:
                return torch.nn.NLLLoss();
            default:
                return torch.nn.MSELoss();
        }
    }


    public static float Evaluate(Tensor actual, Tensor predicted, EvalFunction fun)
    {
        //var evaluator = new Evaluator();

        var actualData = actual.data<float>();
        var predictedData = predicted.data<float>();
        switch (fun)    
        {
            case EvalFunction.CAcc:
                return 0;
        }

        return 0;
    }
}

