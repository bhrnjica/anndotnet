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
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interface;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.TensorflowEx;
using Anndotnet.Vnd.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow;
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
            if (false && _tParameters.Retrain && paths.ContainsKey("BestModel"))
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
     

        #region IMLModel implementation
        public Graph CreateModel(Shape shapeX, Shape shapeY)
        {
            //create variable
            var graph = new Graph().as_default();

            Tensor x = null;
            Tensor y = null;

            tf_with(tf.name_scope(TfScopes.Input), delegate
            {
                // Placeholders for inputs (x) and output (y)
                (x,y) = MLFactory.CreatePlaceholders(shapeX, shapeY);
            });

            //create network
            var z = MLFactory.CreateNetwrok(_network, x, y);

            Tensor loss = null;
            
            //define learner
            tf_with(tf.variable_scope(TfScopes.Train), delegate
            {
                tf_with(tf.variable_scope(TfScopes.LossFun), delegate
                {
                    loss = FunctionEx.Create(y, z, _lParameters.LossFunction);
                });

                tf_with(tf.variable_scope(TfScopes.Optimizer), delegate
                {
                    var optimizer = FunctionEx.Optimizer(_lParameters, loss);
                });

                for (int i = 0; i < _lParameters.EvaluationFunctions.Count; i++)
                {
                    var e = _lParameters.EvaluationFunctions[i];

                    tf_with(tf.variable_scope($"{TfScopes.Evaluation}{i}"), delegate
                    {
                        var ev = FunctionEx.Create(y, z, e);
                    });
                }
            });

            return graph;

        }

        public string SaveModel(Session session, string folderPath)
        {
            throw new NotImplementedException();
        }

        public async Task<Tensor> PredictAsync(Session session, Tensor data)
        {
            try
            {
                //get input placeholder 
                var x = session.graph.get_tensor_by_name($"{TfScopes.Input}/X:0");
                
                //get model output tensor
                var output = session.graph.get_tensor_by_name($"{TfScopes.OutputLayer}/Y:0").outputs.First();

                //evaluate model
                var prediction = await Task.Run<Tensor>(() => session.run(output, (x, data)));
                
                return prediction;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Session> LoadModelAsync(string modelPath)
        {
            await Task.CompletedTask;
            return loadCheckPoint(modelPath);   
        }


        #endregion
    }
}
