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
using ANNdotNET.Core;
using ANNdotNET.Lib.Ext;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace anndotnet.core.app
{
    public static class MachineLearning
    {
        /// <summary>
        /// Main method to perform training process
        /// </summary>
        /// <param name="strFilePath">ML configuration file</param>
        /// <param name="device">Device of computation (GPU/CPU)</param>
        /// <param name="token">Cancellation token for interrupting training process on user request.</param>
        /// <param name="trainingProgress">training progress object</param>
        /// <param name="customModel">custom neural network model if available</param>
        public static TrainResult Run(string modelPath, DeviceDescriptor device, CancellationToken token, TrainingProgress trainingProgress, CreateCustomModel customModel = null)
        {

            //LOad ML configuration file
            var dicMParameters = MLFactory.LoadMLConfiguration(modelPath);

            var fi = new FileInfo(modelPath);
            var folderPath = MLFactory.GetMLConfigFolder(fi.FullName);
            //add path of model folder
            dicMParameters.Add("root", folderPath);

            var retVal = MLFactory.PrepareNNData(dicMParameters, customModel, device);

            //create trainer 
            MLTrainerEx tr = new MLTrainerEx(retVal.f.StreamConfigurations, retVal.f.InputVariables, retVal.f.OutputVariables);

            //setup model checkpoint
            string modelCheckPoint = null;
            if (dicMParameters.ContainsKey("configid"))
            {
                modelCheckPoint = MLFactory.GetModelCheckPointPath(modelPath, dicMParameters["configid"].Trim(' '));
            }
            //setup history of training path
            //TODO
            //create trainer 
            var trainer = tr.CreateTrainer(retVal.nnModel, retVal.lrData, retVal.trData, modelCheckPoint,null);

            //perform training
            var result = tr.Train(trainer, retVal.nnModel, retVal.trData, retVal.mbs, device, token, trainingProgress, modelCheckPoint, null);

            return result;
        }

        /// <summary>
        /// Evaluate the model against dataset sored in the dataset file, and exports the result in csv format for further analysis
        /// </summary>
        /// <param name="mlF"> ml factory object contains members needed to evaluation process</param>
        /// <param name="mbs"> Minibatch source which provides helpers members needed for for evaluation</param>
        /// <param name="strDataSetPath"> file of dataset</param>
        /// <param name="modelPath"> models which will be evaluate</param>
        /// <param name="resultExportPath"> result file in which the result will be exported</param>
        /// <param name="device"> device for computation</param>
        public static void EvaluateModel(string configPath, string bestTrainedModelPath, DeviceDescriptor device)
        {

            //Load ML model configuration file
            var dicMParameters = MLFactory.LoadMLConfiguration(configPath);

            //get model daa paths
            var dicPath = MLFactory.GetMLConfigComponentPaths(dicMParameters["paths"]);

            //parse feature variables
            var projectValues = dicMParameters["training"].Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            var trainedModelRelativePath = MLFactory.GetParameterValue(projectValues, "TrainedModel");
            //Minibatch type
            var mbTypestr = MLFactory.GetParameterValue(projectValues, "Type");
            MinibatchType mbType = (MinibatchType)Enum.Parse(typeof(MinibatchType), mbTypestr, true);

            //add full path of model folder since model file doesn't contains any apsolute path
            var fi = new FileInfo(configPath);
            dicMParameters.Add("root", MLFactory.GetMLConfigFolder(fi.FullName));

            //prepare MLFactory 
            var f = MLFactory.CreateMLFactory(dicMParameters);

            //prepare data paths for mini-batch source
            var strTrainPath = $"{dicMParameters["root"]}\\{dicPath["Training"]}";
            var strValidPath = $"{dicMParameters["root"]}\\{dicPath["Validation"]}";
            var strResult = $"{dicMParameters["root"]}\\{dicPath["Result"]}";
           
            var bestModelFullPath = $"{dicMParameters["root"]}\\{bestTrainedModelPath}";
            //decide what data to evaluate 
            var dataPath = strValidPath;

            //load model
            var model = Function.Load(bestModelFullPath, device);

            //get data for evaluation by calling GetFullBatch
            var minibatchData = MinibatchSourceEx.GetFullBatch(mbType, dataPath, f.StreamConfigurations.ToArray(), device);
            //input map creation for model evaluation
            var inputMap = new Dictionary<Variable, Value>();
            foreach (var v in minibatchData)
            {
                var vv = model.Arguments.Where(x => x.Name == v.Key.m_name).FirstOrDefault();
                var streamInfo = v.Key;
                if (vv != null)
                    inputMap.Add(vv, minibatchData[streamInfo].data);

            }

            //output map 
            var predictedDataMap = new Dictionary<Variable, Value>();
            foreach (var outp in model.Outputs)
            {
                predictedDataMap.Add(outp, null);
            }

            //model evaluation
            model.Evaluate(inputMap, predictedDataMap, device);

            //retrieve actual and predicted values from model
            List<List<float>> actual = new List<List<float>>();
            List<List<float>> predict = new List<List<float>>();

            foreach (var output in model.Outputs)
            {
                //label stream
                var labelStream = minibatchData.Keys.Where(x => x.m_name == output.Name).First();

                //actual values
                List<List<float>> av = MLValue.GetValues(output, minibatchData[labelStream].data);
                //predicted values
                List<List<float>> pv = MLValue.GetValues(output, predictedDataMap[output]);

                for (int i = 0; i < av.Count; i++)
                {
                    //actual
                    var act = av[i];
                    if (actual.Count <= i)
                        actual.Add(new List<float>());
                    actual[i].AddRange(act);
                    //prediction
                    var prd = pv[i];
                    if (predict.Count <= i)
                        predict.Add(new List<float>());
                    predict[i].AddRange(prd);
                }
            }


            //export result
            MLValue.ValueToFile(actual, predict, strResult);

            //
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"*******************Model Evaluation**************");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Model Evaluation successfully exported result into file {strResult}!");
            Console.WriteLine(Environment.NewLine);
        }

    }
}
