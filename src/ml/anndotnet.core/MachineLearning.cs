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
        public static DeviceDescriptor Device = DeviceDescriptor.UseDefaultDevice();

        /// <summary>
        /// Main method to perform training process
        /// </summary>
        /// <param name="strFilePath">ML configuration file</param>
        /// <param name="device">Device of computation (GPU/CPU)</param>
        /// <param name="token">Cancellation token for interrupting training process on user request.</param>
        /// <param name="trainingProgress">training progress object</param>
        /// <param name="customModel">custom neural network model if available</param>
        public static TrainResult Train(string mlconfigPath, TrainingProgress trainingProgress, CancellationToken token, CreateCustomModel customModel = null)
        {

            //LOad ML configuration file
            var dicMParameters = MLFactory.LoadMLConfiguration(mlconfigPath);

            var fi = new FileInfo(mlconfigPath);
            var folderPath = MLFactory.GetMLConfigFolder(fi.FullName);
            //add path of model folder
            dicMParameters.Add("root", folderPath);

            var retVal = MLFactory.PrepareNNData(dicMParameters, customModel, Device);

            //create trainer 
            var tr = new MLTrainer(retVal.f.StreamConfigurations, retVal.f.InputVariables, retVal.f.OutputVariables);

            //setup model checkpoint
            string modelCheckPoint = null;
            if (dicMParameters.ContainsKey("configid"))
            {
                modelCheckPoint = MLFactory.GetModelCheckPointPath(mlconfigPath, dicMParameters["configid"].Trim(' '));
            }

            //setup model checkpoint
            string historyPath = null;
            if (dicMParameters.ContainsKey("configid"))
            {
                historyPath = MLFactory.GetTrainingHistoryPath(mlconfigPath, dicMParameters["configid"].Trim(' '));
            }

            //create trainer 
            var trainer = tr.CreateTrainer(retVal.nnModel, retVal.lrData, retVal.trData, modelCheckPoint, historyPath);

            //perform training
            var result = tr.Train(trainer, retVal.nnModel, retVal.trData, retVal.mbs, Device, token, trainingProgress, modelCheckPoint, historyPath);

            //delete previous best model before change variable values
            retVal.trData.LastBestModel =  MLFactory.ReplaceBestModel(retVal.trData, mlconfigPath, result.BestModelFile);

            //save best model to mlconifg file
            var trStrData = retVal.trData.ToString();
            var d = new Dictionary<string, string>();
            d.Add( "training", trStrData );
            //save to file
            MLFactory.SaveMLConfiguration(mlconfigPath, d);
            return result;
        }

        /// <summary>
        /// Prints the performance analysis on the console
        /// </summary>
        /// <param name="mlConfigPath"></param>
        public static void PrintPerformance(string mlConfigPath)
        {
            try
            {
                //print evaluation result on console
                var performanceData = MLExport.PrintPerformance(mlConfigPath, DataSetType.Validation, DeviceDescriptor.UseDefaultDevice());
                performanceData.Wait();
                foreach (var s in performanceData.Result)
                    Console.WriteLine(s);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mlConfigPath"></param>
        public static Dictionary<string, List<Tuple<int, float, float, float, float>>> ShowTrainingHistory(string mlConfigPath)
        {
            try
            {
                var mlConfigId = MLFactory.GetMLConfigId(mlConfigPath);
                var historyPath = MLFactory.GetTrainingHistoryPath(mlConfigPath, mlConfigId);
                //read history from file
                //load history of training in case continuation of training is requested
                var historyData = MLFactory.LoadTrainingHistory(historyPath);
                var historyHeader = File.ReadAllLines(historyPath).FirstOrDefault();
                //create graph for Training and validation set
                var d = new Dictionary<string, List<Tuple<int, float, float, float, float>>>();
                d.Add(historyHeader, historyData);
                return d;
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        /// <summary>
        /// Export result for specific dataset based on trained model in the mlconfig file
        /// </summary>
        /// <param name="mlConfigPath"></param>
        /// <param name="resultPath"></param>
        public static void ExportResult(string mlConfigPath, string resultPath)
        {
            //TODO
        }

        /// <summary>
        /// Export result for specific dataset based on trained model in the mlconfig file
        /// </summary>
        /// <param name="mlConfigPath"></param>
        /// <param name="resultPath"></param>
        public static void Predict(string mlConfigPath, string resultPath)
        {
            //TODO
        }
    }
}
