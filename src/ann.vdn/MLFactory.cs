//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using AnnDotNET.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;

namespace ann.vdn
{
    public class MLFactory
    {
        #region Private Fields
        //constant for handling files
        public static char[] m_cntkSpearator = new char[] { '|' };
        public static char[] m_cntkSpearator2 = new char[] { ' ', '\t' };
        public static char[] m_ParameterNameSeparator = new char[] { ':' };
        public static char[] m_ValueSpearator = new char[] { ';' };
        public static readonly string m_NumFeaturesGroupName = "NumFeatures";
        public static readonly string m_MLDataFolder = "data";
        public static readonly string m_MLModelFolder = "models";
        public static readonly string m_MLTempModelFolder = "temp_models";
        public static readonly string m_MLLogFolder = "log";
        public static readonly string m_MLDataSetName = "mldataset";
        public static readonly string m_MLConfigFileExt = ".mlconfig";
        public static readonly string m_MLConfigSufix = "_mlconfig";
        public static readonly string m_SufixTrainingDataSet = "_train";
        public static readonly string m_SufixValidationDataSet = "_valid";
        public static readonly string m_SufixTestDataSet = "_test";
        public List<Tensor> Inputs;
        public List<Tensor> Outputs;
        #endregion


        public MLFactory()
        { }

        /// <summary>
        /// Loads ML configuration files, with defined parameters for training and evaluation
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns> dictionary with keywords as key and its values</returns>
        public static Dictionary<string, string> LoadMLConfiguration(string filePath)
        {
            try
            {
                var str = File.ReadAllLines(filePath).Where(x => !x.StartsWith("!") && !string.IsNullOrEmpty(x));//load all configs but without comments
                var keys = str.Select(x => x.Substring(0, x.IndexOf(":")));
                var values = str.Select(x => x.Substring(x.IndexOf(":") + 1));
                //
                var confDic = new Dictionary<string, string>();
                for (int i = 0; i < values.Count(); i++)
                {
                    var k = keys.ElementAt(i);
                    if (string.IsNullOrEmpty(k) || k.Contains(" "))
                        continue;
                    confDic.Add(keys.ElementAt(i), values.ElementAt(i));
                }

                return confDic;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Save in-memory data into ml config file
        /// </summary>
        /// <param name="filePath">file path to save data</param>
        /// <param name="data"> in memory data in form of dictionary</param>
        /// <returns> </returns>
        public static bool SaveMLConfiguration(string filePath, Dictionary<string, string> data)
        {
            try
            {
                var strLines = new List<string>();
                if (File.Exists(filePath))
                    strLines = File.ReadAllLines(filePath).ToList();//load all configs but without comments

                foreach (var kk in data)
                {
                    int index = -1;
                    //find specific line and update it
                    for (int i = 0; i < strLines.Count; i++)
                    {
                        if (strLines[i].StartsWith(kk.Key))
                        {
                            index = i;
                            break;
                        }
                    }
                    //no specific line in the file
                    // add new line
                    if (index == -1)
                        strLines.Add($"{kk.Key}:{kk.Value}");
                    else//in case line exist just replace it with new values
                        strLines[index] = $"{kk.Key}:{kk.Value}";
                }
                //
                File.WriteAllLines(filePath, strLines);
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Create basic MLFactory object with input and output variables
        /// </summary>
        /// <param name="dicMParameters"></param>
        /// <returns></returns>
        public static MLFactory CreateMLFactory(Dictionary<string, string> dicMParameters)
        {
            try
            {
                MLFactory f = new MLFactory();

                //extract features and label strings and create CNTK input variables 
                var strFeatures = dicMParameters["features"];
                var strLabels = dicMParameters["labels"];
                //var dynamicAxes = dicMParameters["network"].Contains("LSTM") && !dicMParameters["network"].Contains("CudaStacked");
                
                //create placeholders
                f.Placeholders(strFeatures, strLabels,TF_DataType.TF_FLOAT);

                return f;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void Placeholders(string features, string labels, TF_DataType dTypes)
        {
            try
            {
                //parse feature variables
                var strFeatures = features.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
                var strLabels = labels.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

                Inputs = createPlaceholders(strFeatures, dTypes);
                Outputs = createPlaceholders(strLabels, dTypes);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private List<Tensor> createPlaceholders(string[] strVariable, TF_DataType type)
        {
            var lst = new List<Tensor>();
            for (int i = 0; i < strVariable.Length; i++)
            {
                var str = strVariable[i];
                var fVar = str.Split(MLFactory.m_cntkSpearator2, StringSplitOptions.RemoveEmptyEntries);
                if (fVar.Length != 3)
                    throw new Exception("One of variables were not formatted properly!");

                //Input variable can be defined on two ways, as vector(array) with dimension n, or as 1d matrix as (1,n)
                //in 1D convolution layer input varibale required to be defined like this. 
                //check for dynamic axes
                
                int[] shape = null;

                //multidimensional input variable
                if (fVar[1].Contains(m_ValueSpearator[0].ToString()))
                   shape = fVar[1].Split(m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                else//1D variable
                    shape = new int[] {-1, int.Parse(fVar[1]) };

                //create variable
                var plc = new Placeholders();
                var v = plc.Create(shape, type);
                lst.Add(v);
            }

            return lst;
        }

        public static List<string> GetOutputClasses(string strMetadata)
        {
            if (string.IsNullOrEmpty(strMetadata))
                return null;


            var columns = strMetadata.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            var labelColumn = columns.Where(x => x.Contains("Label")).FirstOrDefault();
            if (string.IsNullOrEmpty(labelColumn))
                return null;

            return GetColumnClasses(labelColumn);
        }

        public static List<string> GetColumnClasses(string columnData)
        {
            var lst = new List<string>();
            var paramName = columnData.Split(MLFactory.m_ParameterNameSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (paramName == null || paramName.Length != 2)
                return null;


            var par = paramName[1].Split(MLFactory.m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries);

            lst = par.Skip(4).Select(x => x.Trim(' ')).ToList();
            return lst;
        }

        /// <summary>
        /// Returns CheckPoint of the model.
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="configid"></param>
        /// <returns></returns>
        public static string GetModelCheckPointPath(string mlconfigPath, string configid)
        {
            var modelFolder = MLFactory.GetMLConfigFolder(mlconfigPath);
            var name = MLFactory.GetMLConfigFolderName(mlconfigPath);
            return $"{modelFolder}\\model_{configid}.checkpoint";
        }

        /// <summary>
        /// Returns training history path of the model.
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="configid"></param>
        /// <returns></returns>
        public static string GetTrainingHistoryPath(string mlconfigPath, string configid)
        {
            var modelFolder = MLFactory.GetMLConfigFolder(mlconfigPath);
            var name = MLFactory.GetMLConfigFolderName(mlconfigPath);
            return $"{modelFolder}\\{MLFactory.m_MLLogFolder}\\model_{configid}.history";
        }

        public static string GetMLConfigId(string mlconfigPath)
        {
            try
            {
                //Load ML configuration file
                var dicMParameters = MLFactory.LoadMLConfiguration(mlconfigPath);

                if (dicMParameters.ContainsKey("configid"))
                {
                    return dicMParameters["configid"].Trim(' ');
                }
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Returns path of mlconfig folder
        /// </summary>
        /// <param name="mlconfigFullPath"></param>
        /// <returns></returns>
        public static string GetMLConfigFolder(string mlconfigFullPath)
        {
            //remove last suffix from the path
            var folder = mlconfigFullPath.Substring(0, mlconfigFullPath.Length /*-MLFactory.m_MLConfigSufix.Length*/- MLFactory.m_MLConfigFileExt.Length);
            return folder;
        }

        /// <summary>
        /// Returns the Name of the mlconfig folder 
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <returns></returns>
        private static object GetMLConfigFolderName(string mlconfigPath)
        {
            var folderName = Path.GetFileNameWithoutExtension(mlconfigPath);
            //var name = fileName.Substring(0, fileName.Length /*-MLFactory.m_MLConfigSufix.Length*/-MLFactory.m_MLConfigFileExt.Length);
            return folderName;
        }

        /// <summary>
        /// Creates network models based on ml configuration string or to create custom model in case 
        /// custom model creation delegate function is defined
        /// </summary>
        /// <param name="strNetwork"></param>
        /// <param name="inputLayer"></param>
        /// <param name="outputVars"></param>
        /// <param name="customModel"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Tensor CreateNetworkModel(string strNetwork, List<Tensor> inputs, List<Tensor> outputs)
        {
            Tensor nnModel = null;
            //for custom implementation should use this string format
            if (!strNetwork.StartsWith("|Custom") && !strNetwork.StartsWith("|Layer:Custom"))
            {
                var Layers = MLFactory.CreateNetworkParameters(strNetwork);
                nnModel = MLFactory.CreateNetwrok(Layers,inputs.FirstOrDefault(), outputs.FirstOrDefault());
            }
            else
                throw new Exception("Not supported custom models.");
           

            return nnModel;
        }

        private static Tensor CreateNetwrok(List<NNLayer> layers, Tensor inX, Tensor outY)
        {
            //
            ValueInitializer init = ValueInitializer.GlorotNormal;
            Tensor z = inX;
            var l = new Layers();
            //
            foreach (var layer in layers)
            {
                if (layer.Type == LayerType.Dense)
                {
                    z =  l.Dense(z, layer.Param1, init, layer.Name);
                }
                else if (layer.Type == LayerType.Drop)
                {
                    z = l.Drop(z, layer.Param3 / 100.0f,layer.Name );
                }
                else if (layer.Type == LayerType.Activation)
                {
                    z = l.ActivationLayer(z, Activation.Softmax, layer.Name);
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



        /// <summary>
        /// Extract the string and create network parameters aranged in network layer
        /// </summary>
        /// <param name="strNetwork"></param>
        /// <returns></returns>
        public static List<NNLayer> CreateNetworkParameters(string strNetwork)
        {
            try
            {
                //
                var layers = new List<NNLayer>();
                //parse feature variables
                var strParameters = strNetwork.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

                //in case of custom network model
                if (strParameters.Length == 1 && (strParameters[0].Contains("Custom") || strParameters[0].Contains("custom")))
                {
                    var l = new NNLayer() { Id = 1, Type = LayerType.Custom, Name = "Custom Implemented Network", };
                    layers.Add(l);
                    return layers;
                }

                for (int i = 0; i < strParameters.Length; i++)
                {
                    //
                    var strLayerValues = strParameters[i];
                    var ind = strLayerValues.IndexOf(":");
                    var layer = strLayerValues.Substring(ind + 1);
                    var values = layer.Split(m_cntkSpearator2, StringSplitOptions.RemoveEmptyEntries);

                    //create layer
                    var l = new NNLayer();
                    l.Type = (LayerType)Enum.Parse(typeof(LayerType), values[0], true);
                    l.Name = $"{values[0]}_Layer";
                    l.Param1 = int.Parse(values[1].Trim(' '));
                    l.Param2 = int.Parse(values[2].Trim(' '));
                    l.Param3 = int.Parse(values[3].Trim(' '));
                    l.FParam = (Activation)Enum.Parse(typeof(Activation), values[4], true);
                    l.BParam1 = values[5] == "1" ? true : false;
                    l.BParam2 = values[6] == "1" ? true : false;
                    l.UseFParam = (l.Type != LayerType.Embedding && l.Type != LayerType.Scale);
                    layers.Add(l);
                }

                return layers;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
