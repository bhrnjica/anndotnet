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
using System.Threading.Tasks;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Interfaces;
using Anndotnet.Core.Model;

using AnnDotNet.Core.Util;

using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.optim;
using static TorchSharp.torch.nn;



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

    public static AnnModel CreateNetwork(List<ILayer> layers, int inputDim, int outputDim, string name, int seed = 1234)
    {

        return new AnnModel(name, layers, inputDim, outputDim);
    }

    public static (Tensor x, Tensor y) CreatePlaceholders(long[] shapeX, long[] shapeY)
    {

        Tensor x= null;
        Tensor y = null;

        // Placeholders for inputs (x) and outputs(y)
        //var plc = new Placeholders();
        //x = plc.CreatePlaceholder(shapeX, "X", TF_DataType.TF_FLOAT);
        //y = plc.CreatePlaceholder(shapeY, "Y", TF_DataType.TF_FLOAT);

        return (x, y);
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
        var mlConfig = new MlConfig(guid);
        return mlConfig;
    }
}