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

using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Vnd.Layers;
using AnnDotNet.Vnd.Util;
using Daany;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static TorchSharp.torch.nn;
using static TorchSharp.torch;
using TorchSharp;

namespace AnnDotNet.Vnd.Mlconfig;

public class MlFactory: IFactory
{
    public MlConfig MLConfig { get; set; }

    internal static (Tensor xData, Tensor yData) PrepareData(MlConfig mlConfig)
    {
        var dateFormat = mlConfig.Metadata.Where(x => x.ValueColumnType == ColType.DT)
                                                .Select(x => x.ValueFormat).FirstOrDefault();
            
        //load data into DataFrame
        var df = DataFrame.FromCsv(filePath: $"{mlConfig.Parser.RawDataName}",sep: mlConfig.Parser.ColumnSeparator, 
            names: mlConfig.Parser.Header, colTypes: mlConfig.Metadata.Select(x=>x.ValueColumnType).ToArray(),
            missingValues: mlConfig.Parser.MissingValueSymbol, skipLines: mlConfig.Parser.SkipLines, dformat: dateFormat);

        return df.TransformData(mlConfig.Metadata);
    }

   
    public static (Tensor x, Tensor y) CreatePlaceholders(Shape shapeX, Shape shapeY)
    {

        Tensor x= null;
        Tensor y = null;

        // Placeholders for inputs (x) and outputs(y)
        var plc = new Placeholders();
        x = plc.CreatePlaceholder(shapeX, "X", TF_DataType.TF_FLOAT);
        y = plc.CreatePlaceholder(shapeY, "Y", TF_DataType.TF_FLOAT);

        return (x, y);
    }

    public static Tensor CreatePlaceholder(Shape shape, string name)
    {

        // Placeholders for inputs (x) and outputs(y)
        var plc = new Placeholders();
        var t = plc.CreatePlaceholder(shape, name, TF_DataType.TF_FLOAT);

        return t;
    }

    public static async Task<bool> Save(MlConfig mlConfig, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        //add enum string converters
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new JsonLayerConverter());
        //
        using (FileStream fs = File.Create(filePath))
        {
            //mlConfig.Paths.Remove("MLConfig");
            //mlConfig.Paths.Remove("MainFolder");
            await JsonSerializer.SerializeAsync<MlConfig>(fs, mlConfig,options: options);
        }

        return true;
    }

   
    public static async Task<MlConfig> Load(string filePath)
    {
        MlConfig mlConfig = null;
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        //add enum string converters
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new JsonLayerConverter());

        //
        using (FileStream fs = File.OpenRead(filePath))
        {
            mlConfig =  await JsonSerializer.DeserializeAsync<MlConfig>(fs, options: options);
        }


        //Set the current directory.
        if (mlConfig.Paths == null)
            mlConfig.Paths = new Dictionary<string, string>();

        var dir = Path.GetDirectoryName(filePath);
        var di = new DirectoryInfo(filePath);

        //define mainfolder
        if(!mlConfig.Paths.ContainsKey("MainFolder"))
            mlConfig.Paths.Add("MainFolder", di.Parent.FullName);
        else
            mlConfig.Paths["MainFolder"]= di.Parent.FullName;

        //save ml config file path
        if (!mlConfig.Paths.ContainsKey("MLConfig"))
            mlConfig.Paths.Add("MLConfig", new FileInfo(filePath).FullName);
        else
            mlConfig.Paths["MLConfig"] =new FileInfo(filePath).FullName;


        return mlConfig;
    }

}

public interface IFactory
{

}