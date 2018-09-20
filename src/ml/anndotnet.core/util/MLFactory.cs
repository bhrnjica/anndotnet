//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
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
using CNTK;
using NNetwork.Core;
using NNetwork.Core.Common;
using NNetwork.Core.Network;
using NNetwork.Core.Network.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ANNdotNET.Core
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
        List<Variable> m_Inputs;
        List<Variable> m_Outputs;
        List<StreamConfiguration> m_StreamConfig;
        #endregion

        #region Properties
        /// <summary>
        /// Streams are Variables stored in file
        /// </summary>
        public List<StreamConfiguration> StreamConfigurations
        {
            get
            {
                return m_StreamConfig;
            }
            set
            {
                m_StreamConfig = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trData"></param>
        /// <param name="bestModelFile"></param>
        /// <returns></returns>
        public static string ReplaceBestModel(TrainingParameters trData, string mlconfigPath, string bestModelFile)
        {
            try
            {
                var mlconfigFolder = MLFactory.GetMLConfigFolder(mlconfigPath);
                var oldPath = Path.Combine(mlconfigFolder,trData.LastBestModel);
                //delete history 
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                //set new value after delete old model
                trData.LastBestModel = bestModelFile;
                return bestModelFile;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        /// <summary>
        /// List of input variables for NN model. The variable can hold one or more features.
        /// Having more input variables is handy in situation when some features need performing action.
        /// e.g. all numeric features  need normalization but categorical not. 
        /// So one variable should be all numeric features, and second variables should be the rest of features. 
        /// </summary>
        public List<Variable> InputVariables
        {
            get
            {
                return m_Inputs;
            }
            set
            {
                m_Inputs = value;
            }
        }

        /// <summary>
        /// List of output variables for NN Model
        /// </summary>
        public List<Variable> OutputVariables
        {
            get
            {
                return m_Outputs;
            }
            set
            {
                m_Outputs = value;
            }
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Prepares all ml data to start training process
        /// </summary>
        /// <param name="dicMParameters">Where all parameters are stored.</param>
        /// <param name="customModel">In case custom nn model is implemented, this is function pointer. </param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static (MLFactory f, LearningParameters lrData, TrainingParameters trData, Function nnModel, MinibatchSourceEx mbs)
           PrepareNNData(Dictionary<string, string> dicMParameters, CreateCustomModel customModel, DeviceDescriptor device)
        {
            try
            {
                //create factory object
                MLFactory f = CreateMLFactory(dicMParameters);

                //create learning params
                var strLearning = dicMParameters["learning"];
                LearningParameters lrData = MLFactory.CreateLearningParameters(strLearning);

                //create training param
                var strTraining = dicMParameters["training"];
                TrainingParameters trData = MLFactory.CreateTrainingParameters(strTraining);

                //set model component locations
                var dicPath = MLFactory.GetMLConfigComponentPaths(dicMParameters["paths"]);
                //
                trData.ModelTempLocation = $"{dicMParameters["root"]}\\{dicPath["TempModels"]}";
                trData.ModelFinalLocation = $"{dicMParameters["root"]}\\{dicPath["Models"]}";
                var strTrainPath = $"{dicMParameters["root"]}\\{dicPath["Training"]}";
                var strValidPath = $"{dicMParameters["root"]}\\{dicPath["Validation"]}";

                //data normalization in case the option is enabled
                //check if network contains Normalization layer and assign value to normalization parameter 
                if (dicMParameters["network"].Contains("Normalization"))
                    trData.Normalization = new string[] { MLFactory.m_NumFeaturesGroupName };

                //perform data normalization according to the normalization parameter
                List<Variable> networkInput = NormalizeInputLayer(trData, f, strTrainPath, strValidPath, device);

                //create network parameters
                Function nnModel = CreateNetworkModel(dicMParameters["network"], networkInput, f.OutputVariables, customModel, device);

                //create minibatch spurce
                var mbs = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(), strTrainPath, strValidPath, MinibatchSource.InfinitelyRepeat, trData.RandomizeBatch);

                //return ml parameters
                return (f, lrData, trData, nnModel, mbs);
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
                var dynamicAxes = dicMParameters["network"].Contains("LSTM");
                //create config streams
                f.CreateIOVariables(strFeatures, strLabels, DataType.Float, dynamicAxes);

                return f;
            }
            catch (Exception)
            {

                throw;
            }

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
        private static Function CreateNetworkModel(string strNetwork, List<Variable> inputLayer, List<Variable> outputVars, CreateCustomModel customModel, DeviceDescriptor device)
        {
            Function nnModel = null;
            //for custom implementation should use this string format
            if (strNetwork.StartsWith("|Custom") || strNetwork.StartsWith("|Layer:Custom"))
            {
                //Todo: Implementation of reflection needs to be implemented.
                //Two ways of calling customModels
                //by reflection when delegate is null mostly from Desktop application
                if (customModel == null)
                {
                    throw new Exception("Custom Model is not implemented!");
                }
                else//when using Console tool when the delegate is not null
                    nnModel = customModel(inputLayer.Union(outputVars).ToList(), device);
            }
            else
            {
                var Layers = MLFactory.CreateNetworkParameters(strNetwork);
                nnModel = MLFactory.CreateNetwrok(Layers, inputLayer, outputVars.First(), device);
            }

            return nnModel;
        }

        /// <summary>
        /// Perform normalization against input features and creates Normalization Layer prior to neural network creation. 
        /// On this way data normalization is included
        /// in the network itself and not additional normalization is required
        /// </summary>
        /// <param name="trData"></param>
        /// <param name="f"></param>
        /// <param name="strTrainFile"></param>
        /// <param name="strValidFile"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<Variable> NormalizeInputLayer(TrainingParameters trData, MLFactory f, string strTrainFile, string strValidFile, DeviceDescriptor device)
        {
            var networkInput = new List<Variable>();
            if (trData.Normalization != null)
            {
                using (var mbs1 = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(), strTrainFile, strValidFile, MinibatchSource.FullDataSweep, trData.RandomizeBatch))
                {
                    //select variables which are marked for the normalization. Train Data contains this information
                    var vars = f.InputVariables.Where(x => trData.Normalization.Contains(x.Name)).ToList();

                    //This line check if existing variable contains specific sufix in the name. In this case throws exception 
                    // Such name is reserved for normalized variables only.
                    if (f.InputVariables.Where(x => x.Name.EndsWith(MinibatchSourceEx.m_NormalizedSufixName)).Count() > 0)
                        throw new Exception($"Name of Variable cannot ends with '{MinibatchSourceEx.m_NormalizedSufixName}'");

                    //create normalized layer and return it
                    var norVars = mbs1.NormalizeInput(vars, device);

                    //select al other variables which are not normalized 
                    var nonNorm = f.InputVariables.Where(x => !trData.Normalization.Contains(x.Name));

                    //join normalized and unnormalized input variables
                    // create them in the same order since the mini-batch follow this variable order
                    foreach (var iv in f.InputVariables)
                    {
                        var v = nonNorm.Where(x => x.Name.Equals(iv.Name)).FirstOrDefault();
                        //
                        if (v != null)//add non normalized variable
                            networkInput.Add(v);
                        else//add normalize variable
                        {
                            var vn = norVars.Where(x => x.Name.Equals(iv.Name + MinibatchSourceEx.m_NormalizedSufixName)).FirstOrDefault();
                            if (vn == null)
                                throw new Exception("Error in normalization group of features. Check Features in dataset.");
                            networkInput.Add(vn);
                        }
                    }

                }
            }
            else
                networkInput = f.InputVariables;

            return networkInput;
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
                var strParameters = strTrainingData.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);


                //training type
                var type = strParameters.Where(x => x.StartsWith("Type:")).Select(x => x.Substring("Type:".Length)).FirstOrDefault();
                //in case the parameter is not provided throw exception
                if (string.IsNullOrEmpty(type))
                    throw new Exception("Unsupported Training type, or the parameter is not provided!");
                //convert to enum
                trParam.Type = (MinibatchType)Enum.Parse(typeof(MinibatchType), type, true);


                //mbs
                var mbs = strParameters.Where(x => x.StartsWith("BatchSize:")).Select(x => x.Substring("BatchSize:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(mbs))
                    throw new Exception("Unsupported BatchSize!");
                //convert to float
                trParam.BatchSize = uint.Parse(mbs, CultureInfo.InvariantCulture);

                //epoch
                var epoch = strParameters.Where(x => x.StartsWith("Epochs:")).Select(x => x.Substring("Epochs:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(epoch))
                    throw new Exception("Unsupported BatchSize!");
                //convert to float
                trParam.Epochs = int.Parse(epoch, CultureInfo.InvariantCulture);

                //ProgressFrequency
                var progFreq = strParameters.Where(x => x.StartsWith("ProgressFrequency:")).Select(x => x.Substring("ProgressFrequency:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(progFreq))
                    throw new Exception("Unsupported ProgressFrequency!");
                //convert to float
                trParam.ProgressFrequency = int.Parse(progFreq, CultureInfo.InvariantCulture);

                //randomize
                var randomizeBatch = strParameters.Where(x => x.StartsWith("RandomizeBatch:")).Select(x => x.Substring("RandomizeBatch:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(randomizeBatch))
                    throw new Exception("Unsupported RandomizeBatch!");

                //convert to float
                trParam.RandomizeBatch = randomizeBatch.Trim(' ') == "1" ? true : false;

                //normalization data
                var normalization = strParameters.Where(x => x.StartsWith("Normalization:")).Select(x => x.Substring("Normalization:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(normalization) || normalization.Trim(' ') == "0")
                    trParam.Normalization = new string[] { "0" };
                else if (normalization.Trim(' ') == "1")
                    trParam.Normalization = new string[] { "1" };
                else
                {
                    trParam.Normalization = normalization.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }

                //save while training
                var saveWhileTraining = strParameters.Where(x => x.StartsWith("SaveWhileTraining:")).Select(x => x.Substring("SaveWhileTraining:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(saveWhileTraining))
                    throw new Exception("Required SaveWhileTrainig option!");
                trParam.SaveModelWhileTraining = saveWhileTraining.Trim(' ') == "1" ? true : false;

                //FullTrainingSetEval 
                var fullTrainingSetEval = strParameters.Where(x => x.StartsWith("FullTrainingSetEval:")).Select(x => x.Substring("FullTrainingSetEval:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(fullTrainingSetEval))
                {
                    trParam.FullTrainingSetEval = true;//by default full training set is enabled. Disable it in case of large dataset
                }
                else
                    trParam.FullTrainingSetEval = fullTrainingSetEval.Trim(' ') == "1" ? true : false;

                //continue training from previous model
                var continueTraining = strParameters.Where(x => x.StartsWith("ContinueTraining:")).Select(x => x.Substring("ContinueTraining:".Length)).FirstOrDefault();
                if (string.IsNullOrEmpty(continueTraining))
                    trParam.ContinueTraining = false;
                else
                    trParam.ContinueTraining = continueTraining.Trim(' ') == "1" ? true : false;

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
        /// Parser the array of string and returns string value for the parameters name. In case no parameters exist returns null
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="name">Parameter name without double points</param>
        /// <returns></returns>
        public static string GetParameterValue(string[] strData, string name)
        {
            var retVal = strData.Where(x => x.StartsWith($"{name}:")).Select(x => x.Substring($"{name}:".Length)).FirstOrDefault();
            if (string.IsNullOrEmpty(retVal))
                return null;

            retVal = retVal.Trim(' ');//trim space from bot sides if exists
            return retVal;
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
                var strParameters = strLearning.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

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
                trParam.EvaluationFunction = (EFunction)Enum.Parse(typeof(EFunction), eval, true);

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
                    var l = new NNLayer() { Id = 1, Name = "Custom Implemented Network", };
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
                    l.Name = $"{values[0]} Layer";
                    l.HDimension = int.Parse(values[1].Trim(' '));
                    l.CDimension = int.Parse(values[2].Trim(' '));
                    l.Value = int.Parse(values[3].Trim(' '));
                    l.Activation = (Activation)Enum.Parse(typeof(Activation), values[4], true);
                    l.SelfStabilization = values[5] == "1" ? true : false;
                    l.Peephole = values[6] == "1" ? true : false;
                    l.UseActivation = l.Type != LayerType.Embedding;
                    layers.Add(l);
                }

                return layers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Create cntk model function by providing parameters. The method is able for create:
        ///     - feedforward  with one hidden layer and any number of neurons
        ///     - deep neural network with any number of hidden layers and any number of neurons. Each hidden number has the same number of neurons
        ///     - LSTM NN with any number of hidden layers of LSTM , and any number of LSTM Cells in each layer. Also at the top of the network you can define
        ///             one dense layer and one dropout layer.
        /// </summary>
        /// <param name="nnParams"></param>
        /// <returns></returns>
        public static Function CreateNetwrok(List<NNLayer> layers, List<Variable> inputVars, Variable outpuVar, DeviceDescriptor device)
        {
            DataType type = DataType.Float;
            Variable inputLayer = null;
            if (inputVars.Count > 1)
            {
                var vv = new VariableVector();
                foreach (var v in inputVars)
                    vv.Add(v);
                //
                inputLayer = (Variable)CNTKLib.Splice(vv, new Axis(0));
            }
            else //define input layer
                inputLayer = inputVars.First();


            //Create network
            var net = inputLayer;
            var ff = new FeedForwaredNN(device, type);

            //set last layer name to label name
            layers.Last().Name = outpuVar.Name;

            //get last LSTM layer
            var lastLSTM = layers.Where(x => x.Type == LayerType.LSTM).LastOrDefault();

            //
            foreach (var layer in layers)
            {
                if (layer.Type == LayerType.Dense)
                {
                    net = ff.Dense(net, layer.HDimension, layer.Activation, layer.Name);
                }
                else if (layer.Type == LayerType.Drop)
                {
                    net = CNTKLib.Dropout(net, layer.Value / 100.0f);
                }
                else if (layer.Type == LayerType.Embedding)
                {
                    net = Embedding.Create(net, layer.HDimension, type, device, 1, layer.Name);
                }
                else if (layer.Type == LayerType.LSTM)
                {
                    var returnSequence = true;
                    if (layers.IndexOf(lastLSTM) == layers.IndexOf(layer))
                        returnSequence = false;
                    net = RNN.RecurrenceLSTM(net, layer.HDimension, layer.CDimension, type, device, returnSequence, layer.Activation,
                        layer.Peephole, layer.SelfStabilization, 1);
                }
            }

            //check if last layer is compatible with the output
            if (net.Shape.Dimensions.Last() != outpuVar.Shape.Dimensions.Last())
                ff.CreateOutputLayer(net, outpuVar, Activation.None);

            return net;
        }


        /// <summary>
        /// Creates variables for features and labels based on variable definition from the config file.
        /// </summary>
        /// <param name="features">string containing features streams</param>
        /// <param name="labels">string containing features streams</param>
        /// <returns>true if stream configuration list has been created successfully. Otherwise returns exception.</returns>
        public bool CreateIOVariables(string features, string labels, DataType type, bool labelWithDynamicAxes = false)
        {
            try
            {
                //parse feature variables
                var strFeatures = features.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
                var strLabels = labels.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

                //i/O vars
                m_Inputs = createVariables(strFeatures, type, false);
                m_Outputs = createVariables(strLabels, type, labelWithDynamicAxes).ToList();
                m_StreamConfig = createStreamConfiguration(m_Inputs.ToList(), m_Outputs);
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Private Methods
        /// <summary>
        /// create list of variables from string configuration 
        /// </summary>
        /// <param name="strVariable"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Variable> createVariables(string[] strVariable, DataType type, bool labelWithDynamicAxes)
        {
            var lst = new List<Variable>();
            for (int i = 0; i < strVariable.Length; i++)
            {
                var str = strVariable[i];
                var fVar = str.Split(MLFactory.m_cntkSpearator2, StringSplitOptions.RemoveEmptyEntries);
                if (fVar.Length != 3)
                    throw new Exception("One of variables were not formatted properly!");

                var axis = labelWithDynamicAxes ? new List<Axis>() { Axis.DefaultBatchAxis() } : null;
                var shape = new int[] { int.Parse(fVar[1]) };
                var v = Variable.InputVariable(shape, type, fVar[0], axis, fVar[2] == "1", false);
                lst.Add(v);
            }

            return lst;
        }

        /// <summary>
        /// create stream configuration based on defined variables
        /// </summary>
        /// <param name="lstFeaturesVars"></param>
        /// <param name="lstLabelVars"></param>
        /// <returns></returns>
        private static List<StreamConfiguration> createStreamConfiguration(List<Variable> lstFeaturesVars, List<Variable> lstLabelVars)
        {
            var retVal = new List<StreamConfiguration>();
            //
            foreach (var var in lstFeaturesVars)
            {
                var sc = new StreamConfiguration(var.Name, var.Shape.Dimensions.Last(), var.IsSparse, "", false);
                retVal.Add(sc);
            }
            foreach (var var in lstLabelVars)
            {
                var sc = new StreamConfiguration(var.Name, var.Shape.Dimensions.Last(), var.IsSparse, "", false);
                retVal.Add(sc);
            }

            return retVal;
        }
        #endregion

        #region Global Static Members
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


        /// <summary>
        /// Parses the mlconfig content and return dictionary of the model components 
        /// </summary>
        /// <param name="pathsString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMLConfigComponentPaths(string pathsString)
        {
            var dicValues = new Dictionary<string, string>();
            //parse feature variables
            var pathsValues = pathsString.Split(m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            //training path
            var val = MLFactory.GetParameterValue(pathsValues, "Training");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Training) is not defined!");
            dicValues.Add("Training", val);
            val = MLFactory.GetParameterValue(pathsValues, "Validation");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Validation) is not defined!");
            dicValues.Add("Validation", val);
            val = MLFactory.GetParameterValue(pathsValues, "Test");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Test) is not defined!");
            dicValues.Add("Test", val);
            val = MLFactory.GetParameterValue(pathsValues, "TempModels");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (TempModels) is not defined!");
            dicValues.Add("TempModels", val);
            val = MLFactory.GetParameterValue(pathsValues, "Models");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Models) is not defined!");
            dicValues.Add("Models", val);
            val = MLFactory.GetParameterValue(pathsValues, "Result");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Result) is not defined!");
            dicValues.Add("Result", val);
            val = MLFactory.GetParameterValue(pathsValues, "Logs");
            if (string.IsNullOrEmpty(val))
                throw new Exception("One of model paths (Log) is not defined!");
            dicValues.Add("Logs", val);

            return dicValues;
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
        /// Returns default ml dataset path 
        /// </summary>
        /// <param name="isTraining"></param>
        /// <returns></returns>
        public static string GetDefaultMLConfigDatSetPath(bool isTraining)
        {
            if (isTraining)
                return $"{MLFactory.m_MLDataFolder}\\{MLFactory.m_MLDataSetName}{MLFactory.m_SufixTrainingDataSet}";
            else
                return $"{MLFactory.m_MLDataFolder}\\{MLFactory.m_MLDataSetName}{MLFactory.m_SufixValidationDataSet}";
        }

        /// <summary>
        /// Deletes all files and folders within the model
        /// </summary>
        /// <param name="mlconfigFolder"></param>
        public static void DeleteAllFiles(string mlconfigFolder)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(mlconfigFolder);
                if (!di.Exists)
                    return;
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                //at the end delete the root  folder 
                di.Delete();
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Returns DEviceDescription from ProcessDevice enumeration
        /// </summary>
        /// <param name="pdevice"></param>
        /// <returns></returns>
        public static DeviceDescriptor GetDevice(ProcessDevice pdevice)
        {
            switch (pdevice)
            {
                case ProcessDevice.Default:
                    return DeviceDescriptor.UseDefaultDevice();
                case ProcessDevice.CPU:
                    return DeviceDescriptor.CPUDevice;
                case ProcessDevice.GPU:
                    return DeviceDescriptor.GPUDevice(0);
                default:
                    return DeviceDescriptor.UseDefaultDevice();
            }
        }

        #endregion
    }
}
