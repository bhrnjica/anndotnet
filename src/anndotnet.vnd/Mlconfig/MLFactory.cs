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
using Tensorflow;
using Tensorflow.NumPy;

namespace AnnDotNet.Vnd.Mlconfig;

public class MLFactory
{
    public MLConfig MLConfig { get; set; }

    internal static (NDArray xData, NDArray yData) PrepareData(MLConfig mlConfig)
    {
        var dateFormat = mlConfig.Metadata.Where(x => x.ValueColumnType == ColType.DT)
                                                .Select(x => x.ValueFormat).FirstOrDefault();
            
        //load data into DataFrame
        var df = DataFrame.FromCsv(filePath: $"{mlConfig.Parser.RawDataName}",sep: mlConfig.Parser.ColumnSeparator, 
            names: mlConfig.Parser.Header, colTypes: mlConfig.Metadata.Select(x=>x.ValueColumnType).ToArray(),
            missingValues: mlConfig.Parser.MissingValueSymbol, skipLines: mlConfig.Parser.SkipLines, dformat: dateFormat);

        return df.TransformData(mlConfig.Metadata);
    }

    public static Tensor CreateNetwork(List<ILayer> layers, Tensor inX, Tensor outY, int seed = 1234)
    {

        var init = ValueInitializer.GlorotNormal;

        Tensor z = inX;

        var l = new TFLayers();

        foreach (var layer in layers)
        {
                
            if (layer.Type == LayerType.Dense)
            {
                var nnl = layer as FCLayer ?? throw new ArgumentNullException("FCLayer cannot be null!");

                z = l.Dense(z, nnl.OutDim, init, layer.Name, seed: seed);
            }
            else if (layer.Type == LayerType.Drop)
            {
                var nnl = layer as DropLayer ?? throw new ArgumentNullException("DropLayer cannot be null!");
                z = l.Drop(z, nnl.DropPercentage / 100.0f, layer.Name, seed:seed);
            }
            else if (layer.Type == LayerType.Activation)
            {
                var nnl = layer as ActLayer ?? throw new ArgumentNullException("ActivationLayer cannot be null!");
                z = l.ActivationLayer(z, nnl.Activation, layer.Name);
            }
            else if (layer.Type == LayerType.Embedding)
            {
                var nnl = layer as EmbeddingLayer ?? throw new ArgumentNullException("EmbeddingLayer cannot be null!"); 
                z = l.Embedding(z, nnl.OutDim, init, layer.Name, seed:seed);
                
                //z = Embedding.Create(z, layer.Param1, type, device, 1, layer.Name);
            }
            //else if (layer.Type == LayerType.LSTM)
            //{
            //    var returnSequence = true;
            //    if (layers.IndexOf(lastLSTM) == layers.IndexOf(layer))
            //        returnSequence = false;
            //    net = RNN.RecurrenceLSTM(net, layer.Param1, layer.Param2, type, device, returnSequence, layer.FParam,
            //        layer.BParam2, layer.BParam1, 1);
            //}
            //else if (layer.Type == LayerType.NALU)
            //{
            //    var nalu = new NALU(net, layer.Param1, type, device, 1, layer.Name);
            //    net = nalu.H;
            //}
            //else if (layer.Type == LayerType.Conv1D)
            //{
            //    var cn = new Convolution();
            //    net = cn.Conv1D(net, layer.Param1, layer.Param2, type, device,
            //        layer.BParam1, layer.BParam2, layer.Name, 1);
            //}
            //else if (layer.Type == LayerType.Conv2D)
            //{
            //    var cn = new Convolution();
            //    net = cn.Conv2D(net, layer.Param1, new int[] { layer.Param2, layer.Param2 }, type, device,
            //        layer.BParam1/*padding*/, layer.BParam2/*bias*/, layer.Name, 1);
            //}
            //else if (layer.Type == LayerType.Pooling1D)
            //{
            //    var cn = new Convolution();
            //    var pType = PoolingType.Max;
            //    if (layer.FParam == Activation.Avg)
            //        pType = PoolingType.Average;

            //    //
            //    net = cn.Pooling1D(net, layer.Param1, type, pType, device, layer.Name, 1);
            //}
            //else if (layer.Type == LayerType.Pooling2D)
            //{
            //    var cn = new Convolution();
            //    var pType = PoolingType.Max;
            //    if (layer.FParam == Activation.Avg)
            //        pType = PoolingType.Average;

            //    //
            //    net = cn.Pooling2D(net, new int[] { layer.Param1, layer.Param1 }, layer.Param2,
            //        type, pType, device, layer.Name, 1);
            //}
            //else if (layer.Type == LayerType.CudaStackedLSTM)
            //{
            //    net = RNN.RecurreceCudaStackedLSTM(net, layer.Param1, layer.Param2, layer.BParam2, device);
            //}
            //else if (layer.Type == LayerType.CudaStackedGRU)
            //{
            //    net = RNN.RecurreceCudaStackedGRU(net, layer.Param1, layer.Param2, layer.BParam2, device);
            //}

        }

        //add last layer as Output 
        z = l.Output(z, outY);
        return z;
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

    public async static Task<bool> Save(MLConfig mlConfig, string filePath)
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
            await JsonSerializer.SerializeAsync<MLConfig>(fs, mlConfig,options: options);
        }

        return true;
    }

   
    public async static Task<MLConfig> Load(string filePath)
    {
        MLConfig mlConfig = null;
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
            mlConfig =  await JsonSerializer.DeserializeAsync<MLConfig>(fs, options: options);
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