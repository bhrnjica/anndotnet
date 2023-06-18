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
using anndotnet.vnd.Extensions;
using Anndotnet.Core;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interface;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.TensorflowEx;
using Anndotnet.Core.Trainers;
using Anndotnet.Vnd.Layers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace Anndotnet.Vnd
{
    public class MLRunner : MLRunnerBase
    {
        List<ColumnInfo>            _metadata;
        Dictionary<string, string>  _paths;

        public MLRunner(List<ILayer> network, LearningParameters lParam, TrainingParameters tParam, 
                                        NDArray xData, NDArray yData, List<ColumnInfo> metadata, Dictionary<string,string> paths= null):base()
        {
            _network = network;
            _lParameters = lParam;
            _tParameters = tParam;
            _x = xData;
            _y = yData;
            _metadata = metadata;
            _paths = paths;
            checkPaths();
        }

        private void checkPaths()
        {
            if(!_paths.ContainsKey("Root") || !_paths.ContainsKey("Models") || !_paths.ContainsKey("MLConfig"))
            {
                throw new Exception("Invalid paths for MLRunner. Root, Models and MLConfig must be defined.");  
            }
        }

        public MLRunner(MLConfig mlConfig) : base()
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true,
            };

            _paths      = mlConfig.Paths;    
            _network    = mlConfig.Network;
            _lParameters= mlConfig.LParameters;
            _tParameters= mlConfig.TParameters;
            _metadata   = mlConfig.Metadata;

            //data preparation and transformation
            (_x, _y)    = MLFactory.PrepareData(mlConfig);

        }

        public async Task SaveMlConfig(List<ColumnInfo> metadata, DataParser parser, string filePath)
        {
            var mlCOnfig = getMLConfig();
            mlCOnfig.Metadata = metadata;
            mlCOnfig.Parser = parser;
            
            await MLFactory.Save(mlCOnfig, filePath);
        }

        public override void Run(IProgressTraining progress)
        {
            var session = Prepare(_paths);

            //Train model
            Train(_x, _y, session, progress );

            //prediction
            Complete();

            return;

        }

        private void Complete()
        {
            var bestLoss = _history.History.Min(x=>x.Loss);
            var bestModel = _history.History.FirstOrDefault(x => x.Loss == bestLoss);  
            
            if(!ReferenceEquals(bestModel, null))
            {
                if (_paths.ContainsKey("BestModel"))
                {
                    _paths["BestModel"] = bestModel.ModelName;
                }
                else
                {
                   _paths.Add("BestModel", bestModel.ModelName);
                }
            }
            var root = _paths["Root"].GetPathInCurrentOS();
            var modelsFolder = _paths["Models"].GetPathInCurrentOS();
            var path = Path.Combine(root, modelsFolder);    
            cleanModels(path);
            
        }

        private void cleanModels(string modelsFolder)
        {
            var di = new DirectoryInfo(modelsFolder);

            var bestModle = _paths["BestModel"].GetPathInCurrentOS();

            var bestModelName = Path.GetFileName(bestModle).ToLower(); 
            
            foreach (var file in di.GetFiles())
            {
                var f = Path.GetFileNameWithoutExtension(file.Name).ToLower();
                if (!string.Equals(bestModelName, f))
                {
                    file.Delete();  
                }

            }
        }

        protected override void Train(NDArray xData, NDArray yData, Session session, IProgressTraining progress)
        {
            //training process
            if (_tParameters.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData,progress, _tParameters.SplitPercentage);
                
                tr.Run(session, _lParameters, _tParameters, Evaluate);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, progress,_tParameters.KFold);
                tr.Run(session, _lParameters, _tParameters, Evaluate);
            }
        }

        protected override Session Evaluate(Session session, ProgressReport tp)
        {
            var iteration = new TrainingEvent()
            {
                Id = (tp.Fold - 1) * tp.Epochs + tp.Epoch,
                Loss = tp.ValidLoss,
                Evals = tp.ValidEval,
                ModelName = "modelname",
            };

            //save only when training is completed.
            if (tp.ProgressType == ProgressType.Training || tp.ProgressType == ProgressType.Completed)
            {
                var mainFolder = _paths.ContainsKey("Root") ? _paths["Root"]?.GetPathInCurrentOS() : throw new Exception("Main Folder must be defined.");
                var modelsFolder = _paths.ContainsKey("Model") ? _paths["Model"]?.GetPathInCurrentOS() : null;

                if (string.IsNullOrEmpty(modelsFolder))
                {
                    modelsFolder = "models";
                }

                var modelsPath = Path.Combine(mainFolder, modelsFolder);
                var modelName = saveCheckPoint(session, modelsPath);

                iteration.ModelName = modelName;
            }

            _history.History.Add(iteration);

            return null;

        }


        private MLConfig getMLConfig()
        {
            var mlConfig = new MLConfig();
            mlConfig.Id = Guid.NewGuid();
            mlConfig.LParameters = _lParameters;
            mlConfig.TParameters = _tParameters;
            mlConfig.Metadata = _metadata;
            mlConfig.Network = _network;
            mlConfig.Parser = new DataParser();
            mlConfig.Paths = _paths;    

            return mlConfig;
        }


    }
}
