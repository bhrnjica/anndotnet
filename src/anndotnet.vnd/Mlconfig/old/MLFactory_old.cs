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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using Daany;

using Anndotnet.Core;
using Anndotnet.Core.Data;

namespace Anndotnet.Vnd
{
    public class MLFactory_old
    {
        #region Private Fields
        //constant for handling files
        public static char[] m_Spearator = new char[] { '|' };
        public static char[] m_Spearator2 = new char[] { ' ', '\t' };
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


        public MLFactory_old()
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
        //public static MLFactory CreateMLFactory(Dictionary<string, string> dicMParameters)
        //{
        //    try
        //    {
        //        MLFactory f = new MLFactory();

        //        //extract features and label strings and create CNTK input variables 
        //        var strFeatures = dicMParameters["features"];
        //        var strLabels = dicMParameters["labels"];
        //        //var dynamicAxes = dicMParameters["network"].Contains("LSTM") && !dicMParameters["network"].Contains("CudaStacked");

        //        //create placeholders
        //        f.Placeholders(strFeatures, strLabels,TF_DataType.TF_FLOAT);

        //        return f;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        //private void Placeholders(string features, string labels, TF_DataType dTypes)
        //{
        //    try
        //    {
        //        //parse feature variables
        //        var strFeatures = features.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);
        //        var strLabels = labels.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);

        //        Inputs = createPlaceholders(strFeatures, dTypes);
        //        Outputs = createPlaceholders(strLabels, dTypes);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        //private List<Tensor> createPlaceholders(string[] strVariable, TF_DataType type)
        //{
        //    var lst = new List<Tensor>();
        //    for (int i = 0; i < strVariable.Length; i++)
        //    {
        //        var str = strVariable[i];
        //        var fVar = str.Split(MLFactory.m_Spearator2, StringSplitOptions.RemoveEmptyEntries);
        //        if (fVar.Length != 3)
        //            throw new Exception("One of variables were not formatted properly!");

        //        //Input variable can be defined on two ways, as vector(array) with dimension n, or as 1d matrix as (1,n)
        //        //in 1D convolution layer input varibale required to be defined like this. 
        //        //check for dynamic axes

        //        int[] shape = null;

        //        //multidimensional input variable
        //        if (fVar[1].Contains(m_ValueSpearator[0].ToString()))
        //           shape = fVar[1].Split(m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
        //        else//1D variable
        //            shape = new int[] {-1, int.Parse(fVar[1]) };

        //        //create variable
        //        var plc = new Placeholders();
        //        var v = plc.Create(shape, type);
        //        lst.Add(v);
        //    }

        //    return lst;
        //}

        public static (Tensor x, Tensor y) CreatePlaceholders(NDArray xData, NDArray yData)
        {
            //
            List<int> shapeX = new List<int>();
            List<int> shapeY = new List<int>();

            //
            shapeX.Add(-1);//first dimension
            shapeX.AddRange(xData.Shape.Dimensions.Skip(1));
            shapeY.Add(-1);//first dimentsion
            shapeY.AddRange(yData.Shape.Dimensions.Skip(1));

            //create variable
            var plc = new Placeholders();
            var x = plc.Create(shapeX.ToArray(), TF_DataType.TF_FLOAT);
            var y = plc.Create(shapeY.ToArray() , TF_DataType.TF_FLOAT);

            return (x, y);
        }

        /// <summary>
        /// Creates TrainingParameter object form string
        /// </summary>
        /// <param name="strTrainingData"></param>
        /// <returns></returns>
        public static TrainingParameters CreateTrainingParameters(string strTrainingData)
        {
            try
            {
                //
                var trParam = new TrainingParameters();
                //parse feature variables
                var strParameters = strTrainingData.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);


                //training type
                var type = strParameters.Where(x => x.StartsWith("Type:")).Select(x => x.Substring("Type:".Length)).FirstOrDefault();
                //in case the parameter is not provided throw exception
                if (string.IsNullOrEmpty(type))
                    throw new Exception("Unsupported Training type, or the parameter is not provided!");

                type = type.Trim();
                if(type == "Default")//backward compatibility
                    trParam.TrainingType = TrainingType.TVTraining;
                else//convert to enum
                trParam.TrainingType = (TrainingType)Enum.Parse(typeof(TrainingType), type, true);


                //mbs
                var mbs = strParameters.Where(x => x.StartsWith("BatchSize:")).Select(x => x.Substring("BatchSize:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(mbs))
                    throw new Exception("Unsupported BatchSize!");
                //convert to float
                trParam.MinibatchSize = int.Parse(mbs, CultureInfo.InvariantCulture);

                //split
                var psplit = strParameters.Where(x => x.StartsWith("Split:")).Select(x => x.Substring("Split:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(psplit))
                    throw new Exception("Unsupported percentage split!");
                //convert to float
                trParam.SplitPercentage = int.Parse(psplit, CultureInfo.InvariantCulture);

                //kfold
                var kfold = strParameters.Where(x => x.StartsWith("KFold:")).Select(x => x.Substring("KFold:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(kfold))
                    throw new Exception("Unsupported kFold split!");
                //convert to float
                trParam.KFold = int.Parse(kfold, CultureInfo.InvariantCulture);

                //epochs
                var epochs = strParameters.Where(x => x.StartsWith("Epochs:")).Select(x => x.Substring("Epochs:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(epochs))
                    throw new Exception("Unsupported Epochs!");
                //convert to float
                trParam.Epochs = int.Parse(epochs, CultureInfo.InvariantCulture);

                //ProgressFrequency
                var progFreq = strParameters.Where(x => x.StartsWith("ProgressFrequency:")).Select(x => x.Substring("ProgressFrequency:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(progFreq))
                    throw new Exception("Unsupported ProgressFrequency!");
                //convert to float
                trParam.ProgressStep = int.Parse(progFreq, CultureInfo.InvariantCulture);

                //randomize
                //var randomizeBatch = strParameters.Where(x => x.StartsWith("RandomizeBatch:")).Select(x => x.Substring("RandomizeBatch:".Length)).FirstOrDefault();
                //if (string.IsNullOrEmpty(randomizeBatch))
                //    throw new Exception("Unsupported RandomizeBatch!");

                ////convert to float
                //trParam.RandomizeBatch = randomizeBatch.Trim(' ') == "1" ? true : false;

                //normalization data
                //var normalization = strParameters.Where(x => x.StartsWith("Normalization:")).Select(x => x.Substring("Normalization:".Length)).FirstOrDefault();
                //if (string.IsNullOrEmpty(normalization) || normalization.Trim(' ') == "0")
                //    trParam.Normalization = new string[] { "0" };
                //else if (normalization.Trim(' ') == "1")
                //    trParam.Normalization = new string[] { "1" };
                //else
                //{
                //    trParam.Normalization = normalization.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //}

                //save while training
                //var saveWhileTraining = strParameters.Where(x => x.StartsWith("SaveWhileTraining:")).Select(x => x.Substring("SaveWhileTraining:".Length)).FirstOrDefault();
                //if (string.IsNullOrEmpty(saveWhileTraining))
                //    throw new Exception("Required SaveWhileTrainig option!");
                //trParam.SaveModelWhileTraining = saveWhileTraining.Trim(' ') == "1" ? true : false;

                ////FullTrainingSetEval 
                //var fullTrainingSetEval = strParameters.Where(x => x.StartsWith("FullTrainingSetEval:")).Select(x => x.Substring("FullTrainingSetEval:".Length)).FirstOrDefault();
                //if (string.IsNullOrEmpty(fullTrainingSetEval))
                //{
                //    trParam.FullTrainingSetEval = true;//by default full training set is enabled. Disable it in case of large dataset
                //}
                //else
                //    trParam.FullTrainingSetEval = fullTrainingSetEval.Trim(' ') == "1" ? true : false;

                //continue training from previous model
                //var continueTraining = strParameters.Where(x => x.StartsWith("ContinueTraining:")).Select(x => x.Substring("ContinueTraining:".Length)).FirstOrDefault();
                //if (string.IsNullOrEmpty(continueTraining))
                //    trParam.ContinueTraining = false;
                //else
                //    trParam.ContinueTraining = continueTraining.Trim(' ') == "1" ? true : false;

                //best found model
                var trainedModel = strParameters.Where(x => x.StartsWith("TrainedModel:")).Select(x => x.Substring("TrainedModel:".Length)).FirstOrDefault();
                trParam.LastBestModel = trainedModel;

                return trParam;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create LearningParameters object from string.
        /// </summary>
        /// <param name="strLearning"></param>
        /// <returns></returns>
        public static LearningParameters CreateLearningParameters(string strLearning)
        {
            try
            {
                //
                var trParam = new LearningParameters();
                //parse feature variables
                var strParameters = strLearning.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);

                //learner type
                var type = strParameters.Where(x => x.StartsWith("Type:")).Select(x => x.Substring("Type:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(type))
                    throw new Exception("Unsupported Learning type!");
                //convert to enum
                trParam.LearnerType = (LearnerType)Enum.Parse(typeof(LearnerType), type, true);

                //loss function
                var loss = strParameters.Where(x => x.StartsWith("Loss:")).Select(x => x.Substring("Loss:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(loss))
                    throw new Exception("Unsupported Loss function!");
                //convert to enum
                trParam.LossFunction = (EFunction)Enum.Parse(typeof(EFunction), loss, true);

                //eval function
                var eval = strParameters.Where(x => x.StartsWith("Eval:")).Select(x => x.Substring("Eval:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(eval))
                    throw new Exception("Unsupported Evaluation function!");
                //convert to enum
                trParam.EvaluationFunctions = (EFunction)Enum.Parse(typeof(EFunction), eval, true);

                //lr function
                var lr = strParameters.Where(x => x.StartsWith("LRate:")).Select(x => x.Substring("LRate:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(lr))
                    throw new Exception("Unsupported Learning Rate function!");
                //convert to float
                trParam.LearningRate = double.Parse(lr, CultureInfo.InvariantCulture);

                //momentum function
                var momentum = strParameters.Where(x => x.StartsWith("Momentum:")).Select(x => x.Substring("momentum:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(momentum))
                    throw new Exception("Unsupported Momentum parameter!");
                //convert to float
                trParam.Momentum = double.Parse(momentum, CultureInfo.InvariantCulture);

                // L1 
                var l1 = strParameters.Where(x => x.StartsWith("L1:")).Select(x => x.Substring("L1:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(l1))
                    trParam.L1Regularizer = 0;
                else //convert to float
                    trParam.L1Regularizer = double.Parse(l1, CultureInfo.InvariantCulture);

                // L2 
                var l2 = strParameters.Where(x => x.StartsWith("L2:")).Select(x => x.Substring("L2:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(l2))
                    trParam.L2Regularizer = 0;
                else //convert to float
                    trParam.L2Regularizer = double.Parse(l2, CultureInfo.InvariantCulture);
                // 
                return trParam;
            }
            catch (Exception)
            {

                throw;
            }
        }

        
        /// <summary>
        /// Creates network models based on list of network layers
        /// </summary>
        /// <param name="strNetwork"></param>
        /// <param name="inputLayer"></param>
        /// <param name="outputVars"></param>
        /// <param name="customModel"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Tensor CreateNetworkModel(string strNetwork, Tensor x, Tensor y)
        {
            Tensor nnModel = null;
            //for custom implementation should use this string format
            if (!strNetwork.StartsWith("|Custom") && !strNetwork.StartsWith("|Layer:Custom"))
            {
                var Layers = MLFactory_old.CreateNetworkParameters(strNetwork);

                nnModel = MLFactory_old.CreateNetwrok(Layers,x, y);
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
        /// Extract the string and create network parameters arranged in network layer
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
                var strParameters = strNetwork.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);

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
                    var values = layer.Split(m_Spearator2, StringSplitOptions.RemoveEmptyEntries);

                    //create layer
                    var l = new NNLayer();
                    var type =  (LayerType)Enum.Parse(typeof(LayerType), values[0], true);
                    if(type != LayerType.Activation)
                    {
                        l.Type = type;
                        l.Name = $"{values[0]}_Layer";
                        l.Param1 = int.Parse(values[1].Trim(' '));
                        l.Param2 = int.Parse(values[2].Trim(' '));
                        l.Param3 = int.Parse(values[3].Trim(' '));
                        l.FParam = 0;
                        l.BParam1 = values[5] == "1" ? true : false;
                        l.BParam2 = values[6] == "1" ? true : false;
                        l.UseFParam = (l.Type != LayerType.Embedding && l.Type != LayerType.Scale);
                        layers.Add(l);

                    }
                    //
                    var act = (Activation)Enum.Parse(typeof(Activation), values[4], true);

                    //add activation as separated layer
                    if(act != Activation.None)
                    {
                        var ll = new NNLayer();
                        ll.Name = $"Activation_Layer";
                        ll.Type = LayerType.Activation;
                        ll.FParam = act;
                        layers.Add(ll);
                    }
                }

                return layers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Prepares data based on meta-data and raw file name.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static (NDArray xData, NDArray yData) PrepareData(string metadata, string paths)
        {
            var filePath = paths.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Substring("Training:".Length)).FirstOrDefault();
            var strMetaParameters = metadata.Split(m_Spearator, StringSplitOptions.RemoveEmptyEntries);



            var collst = new List<ColumnInfo>();

            var cols = new List<string>();
            var features = new List<string>();
            var labels = new List<string>();
            var types = new List<Daany.ColType>();
            var missingValue = new Dictionary<string, string>();

            //
            for(int i=0; i< strMetaParameters.Length; i++)
            {
                var c = strMetaParameters[i];
                var colName = c.Substring($"Column_{(i+1).ToString("00")}:".Length-1).Split(';');
                
                if (colName[1] == "Numeric")
                {
                    cols.Add(colName[0]);
                    types.Add(Daany.ColType.F32);
                    
                }  
                else if(colName[1] == "Category")
                {
                    cols.Add(colName[0]);
                    types.Add(Daany.ColType.IN);
                }

                if (colName[2] == "Feature")
                {
                    features.Add(colName[0]);
                    missingValue.Add(colName[0], colName[3]);

                }
                else if (colName[2] == "Label")
                {
                    labels.Add(colName[0]);
                    missingValue.Add(colName[0], colName[3]);
                }
                   
            }

            var df = Daany.DataFrame.FromCsv(filePath: filePath, sep: ';', names: cols.ToArray(), colTypes: types.ToArray());
            //handling missing value
            

            DataFrame dff = handlingMissingValue(df, missingValue);

            return (null, null);//df.TransformData(features.ToArray(), labels.FirstOrDefault());

        }

        private static DataFrame handlingMissingValue(DataFrame df, Dictionary<string, string> missingValue)
        {
            var missing = df.MissingValues();
            foreach (var mv in missing)
            {
                var value = missingValue[mv.Key];
                if (value.Equals("ignore",StringComparison.OrdinalIgnoreCase))
                    df.DropNA(mv.Key);
                else if (value.Equals("average", StringComparison.OrdinalIgnoreCase))
                    df.FillNA(mv.Key, Aggregation.Avg);
                else if (value.Equals("mode", StringComparison.OrdinalIgnoreCase))
                    df.FillNA(mv.Key, Aggregation.Mode);
                else if (value.Equals("random", StringComparison.OrdinalIgnoreCase))
                    df.FillNA(mv.Key, Aggregation.Random);
                else if (value.Equals("max", StringComparison.OrdinalIgnoreCase))
                    df.FillNA(mv.Key, Aggregation.Max);
                else if (value.Equals("min", StringComparison.OrdinalIgnoreCase))
                    df.FillNA(mv.Key, Aggregation.Min);
            }

            return df;
        }

       
    }
}
