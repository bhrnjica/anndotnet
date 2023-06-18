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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tensorflow;
using Tensorflow.Lite;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace Anndotnet.Vnd
{
    public class MLRunnerBase : IRunner, IMLModel
    {
        protected TrainingHistory       _history { get; set; }
        protected ConfigProto           _config;
        protected Operation             _init;
        protected LearningParameters    _lParameters { get; set; }
        protected TrainingParameters    _tParameters { get; set; }
        protected List<ILayer>          _network { get; set; }
        protected NDArray               _x;
        protected NDArray               _y;

        public MLRunnerBase()
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true,
            };

            _history= new TrainingHistory 
            {
                History=new List<TrainingEvent>()
            };    
        }


        public virtual void Run(IProgressTraining progress)
        {
            //// prepare for training
            //Prepare();

            ////Train model
            //Train(X, Y, session);


            ////evaluation


            ////prediction
            //return;

            throw new NotImplementedException();

        }

        protected virtual Session Prepare(Dictionary<string, string > paths)
        {
            Session session = null;
            tf.compat.v1.disable_eager_execution();

            //load trained model if exists
            if (_tParameters.Retrain && paths.ContainsKey("BestModel"))
            {
                var root = paths.ContainsKey("Root") ? paths["Root"].GetPathInCurrentOS() : "";
                var models= paths.ContainsKey("Models") ? paths["Models"].GetPathInCurrentOS() : "";
                var path = paths["BestModel"];

                session = loadCheckPoint(Path.Combine(root, models, path));
            }

            //create network from network collection
            if (ReferenceEquals(session, null))
            {
                //clear training history
                _history.History.Clear();

                //create graph from machine learning configuration
                var shapeX = _x.shape;
                var shapeY = _y.shape;

                //first dimension for input tensors is undefined
                shapeX[0] = -1;
                shapeY[0] = -1;

                //create graph by providing input tensors
                var model = CreateModel(shapeX, shapeY);

                session = tf.Session(model);

                // Initialize the variables (i.e. assign their default value)
                _init = tf.global_variables_initializer();

                // Run the initializer
                session.run(_init);
            }

            return session; 
        }

        protected virtual void Train(NDArray xData, NDArray yData, Session session, IProgressTraining progress)
        {
            throw new NotImplementedException();
        }

        protected virtual Session Evaluate(Session session, ProgressReport tp)
        {
            throw new NotImplementedException();
        }

        protected bool saveModel1(Session session, Dictionary<string, string> paths)
        {
            var saver = tf.train.Saver();

            if (!paths.ContainsKey("BestModel"))
            {
                paths.Add("BestModel", "");
            }

            if (!paths.ContainsKey("Models"))
            {
                paths.Add("Models", "models");
            }

            //generate main folder path if it is missing
            var curDir = Directory.GetCurrentDirectory();

            if (!paths.ContainsKey("MainFolder"))
            {
                paths.Add("MainFolder", curDir);
            }

            // Restore variables from checkpoint
            var root = $"{paths["MainFolder"]}".GetPathInCurrentOS();
            
            Directory.SetCurrentDirectory(root);

            //delete all previous models
            var path= paths["Models"].GetPathInCurrentOS();   
            var modelPath = Path.Combine(root, path);
            var di = new DirectoryInfo(modelPath);
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                di.Create();
            }

            var tfModel = Path.Combine(modelPath, $"{DateTime.Now.Ticks}.ckp");
            var strPath = saver.save(session, tfModel);

            paths["BestModel"] = Path.GetRelativePath(paths["MainFolder"], strPath);

            Directory.SetCurrentDirectory(curDir);

            return true;
        }

        protected Session loadCheckPoint(string modelFilePath)
        {
            if (string.IsNullOrEmpty(modelFilePath))
            {
                return null;    
            }


            //set current directory if exists in MLConfig            
            var modelFilePathMeta = modelFilePath + ".meta";
            var f = new FileInfo(modelFilePathMeta);
            if (f.Exists)
            {
                var graph = tf.Graph().as_default();
                var sess = tf.Session(graph);
                try
                {
                    var saver = tf.train.latest_checkpoint("mlconfig/iris/models/", modelFilePathMeta);
                    var saver1 = tf.train.import_meta_graph(modelFilePathMeta);
                    // Restore variables from checkpoint
                    saver1.restore(sess, modelFilePath);               
                    return sess;

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return null;
        }

        protected string saveCheckPoint(Session session, string folder)
        {
            var saver = tf.train.Saver();
            var name = $"{DateTime.Now.Ticks}.ckp";
            var mname = Path.Combine(folder, name);
            var strPath = saver.save(session, mname);
           
            return name;
        }
        protected Graph createGraph2(List<ILayer> net,LearningParameters lParams, Shape shapeX, Shape shapeY)
        {
            //create variable
            var graph = new Graph().as_default();

            Tensor x = null;
            Tensor y = null;
            tf_with(tf.name_scope("Input"), delegate
            {
                // Placeholders for inputs (x) and outputs(y)
                //create placeholders
                (x, y) = MLFactory.CreatePlaceholders(shapeX, shapeY);
            });
                    
            //create network
            var z = MLFactory.CreateNetwrok(net, x, y);

            //define learner for the network
            Tensor loss = null;
            tf_with(tf.variable_scope("Train"), delegate
            {
                tf_with(tf.variable_scope("Loss"), delegate
                {
                    loss = FunctionEx.Create(y, z, lParams.LossFunction);
                });

                tf_with(tf.variable_scope("Optimizer"), delegate
                {
                   var optimizer = FunctionEx.Optimizer(lParams, loss);
                });

                for(int i=0; i< lParams.EvaluationFunctions.Count; i++)
                {
                    var e = lParams.EvaluationFunctions[i];
                    tf_with(tf.variable_scope($"Eval{i}"), delegate
                    {
                        var ev  = FunctionEx.Create(y, z, e);
                    });
                }
            });


            return graph;
        }


        #region IMLModel implementation
        public Graph CreateModel(Shape shapeX, Shape shapeY)
        {
            //create variable
            var graph = new Graph().as_default();

            Tensor x = null;
            Tensor y = null;
            tf_with(tf.name_scope("Input"), delegate
            {
                // Placeholders for inputs (x) and outputs(y)
                (x, y) = MLFactory.CreatePlaceholders(shapeX, shapeY);
            });

            //create network
            var z = MLFactory.CreateNetwrok(_network, x, y);

            Tensor loss = null;
            
            //define learner
            tf_with(tf.variable_scope("Train"), delegate
            {
                tf_with(tf.variable_scope("Loss"), delegate
                {
                    loss = FunctionEx.Create(y, z, _lParameters.LossFunction);
                });

                tf_with(tf.variable_scope("Optimizer"), delegate
                {
                    var optimizer = FunctionEx.Optimizer(_lParameters, loss);
                });

                for (int i = 0; i < _lParameters.EvaluationFunctions.Count; i++)
                {
                    var e = _lParameters.EvaluationFunctions[i];

                    tf_with(tf.variable_scope($"Eval{i}"), delegate
                    {
                        var ev = FunctionEx.Create(y, z, e);
                    });
                }
            });

            return graph;

        }

        public Session LoadModel(string modelPath)
        {
            throw new NotImplementedException();
        }

        public string SaveModel(Session session, string folderPath)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
