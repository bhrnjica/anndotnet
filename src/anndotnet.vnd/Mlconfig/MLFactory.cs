//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using Anndotnet.Core;
using Anndotnet.Core.Data;
using Anndotnet.Core.Extensions;
using Anndotnet.Core.Interfaces;
using Anndotnet.Vnd.Layers;
using Anndotnet.Vnd.Util;
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

namespace Anndotnet.Vnd
{
    public class MLFactory
    {
        public MLConfig MLConfig { get; set; }

        public static string Root { get; set; }

        internal static (NDArray xData, NDArray yData) PrepareData(MLConfig mlConfig)
        {
            var dateFormat = mlConfig.Metadata.Where(x => x.ValueColumnType == ColType.DT).Select(x => x.ValueFormat).FirstOrDefault();
            //load data into DataFrame
            var df = Daany.DataFrame.FromCsv(filePath: $"{mlConfig.Parser.RawDataName}", 
                sep: mlConfig.Parser.ColumnSeparator, 
                                                names: mlConfig.
                                                Parser.Header, 
                                                colTypes: mlConfig.Metadata.Select(x=>x.ValueColumnType).ToArray(),
                                                missingValues: mlConfig.Parser.MissingValueSymbol,
                                                skipLines: mlConfig.Parser.SkipLines, dformat: dateFormat);

            //
            return df.TransformData(mlConfig.Metadata);
        }

        public static Tensor CreateNetwrok(List<ILayer> layers, Tensor inX, Tensor outY, int seed = 1234)
        {
            //
            ValueInitializer init = ValueInitializer.GlorotNormal;
            Tensor z = inX;
            var l = new TFLayers();
            //
            foreach (var layer in layers)
            {
                
                if (layer.Type == LayerType.Dense)
                {
                    var nnl = layer as FCLayer;
                    if (nnl == null)
                        throw new ArgumentNullException("FCLayer cannot be null!");
                    z = l.Dense(z, nnl.OutDim, init, layer.Name, seed: seed);
                }
                else if (layer.Type == LayerType.Drop)
                {
                    var nnl = layer as DropLayer;
                    if (nnl == null)
                        throw new ArgumentNullException("DropLayer cannot be null!");

                    z = l.Drop(z, nnl.DropPercentage / 100.0f, layer.Name, seed:seed);
                }
                else if (layer.Type == LayerType.Activation)
                {
                    var nnl = layer as ActLayer;
                    if (nnl == null)
                        throw new ArgumentNullException("ActivationLayer cannot be null!");

                    z = l.ActivationLayer(z, nnl.Activation, layer.Name);
                }
                //else if (layer.Type == LayerType.Embedding)
                //{
                //    net = Embedding.Create(net, layer.Param1, type, device, 1, layer.Name);
                //}
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
            return z;
        }

        public static (Tensor x, Tensor y) CreatePlaceholders(Shape shapeX, Shape shapeY)
        {

            Tensor x= null;
            Tensor y = null;

            // Placeholders for inputs (x) and outputs(y)
            var plc = new Placeholders();
            x = plc.Create(shapeX, "X", TF_DataType.TF_FLOAT);
            y = plc.Create(shapeY, "Y", TF_DataType.TF_FLOAT);

            return (x, y);
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
}
