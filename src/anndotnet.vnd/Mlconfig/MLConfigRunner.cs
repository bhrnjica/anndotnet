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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;
using Anndotnet.Core;
using Anndotnet.Core.Learners;
using Anndotnet.Core.Trainers;
using Anndotnet.Core.Entities;
using System.IO;
using Anndotnet.Core.TensorflowEx;
using Anndotnet.Core.Interface;

namespace Anndotnet.Vnd
{
    public class MLConfigRunner : MLRunnerBase
    {
        MLConfig MLConfig { get; set; }

        public MLConfigRunner(MLConfig mlConfig):base()
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true,
            };

            MLConfig = mlConfig;
        }


        public override void Run()
        {
            //data preparation and transformation
            (NDArray xData, NDArray yData) = MLFactory.PrepareData(MLConfig, "Training");

            //
            List<int> shapeX = new List<int>();
            List<int> shapeY = new List<int>();
            shapeX.Add(-1);//first dimension
            shapeX.AddRange(xData.Shape.Dimensions.Skip(1));
            shapeY.Add(-1);//first dimension
            shapeY.AddRange(yData.Shape.Dimensions.Skip(1));

            Session session = null;
            tf.compat.v1.disable_eager_execution();

            //load trained model if exists
            if (MLConfig.TParameters.Retrain && MLConfig.Paths.ContainsKey("BestModel"))
            {
                session = loadModelCheckPoint();
                
            }
            //create network from mlconfig file
            if (session == null)
            {
                //create graph from machine learning configuration
                var graph = createGraph(shapeX, shapeY);
                session = tf.Session(graph);

                // Initialize the variables (i.e. assign their default value)
                _init = tf.global_variables_initializer();
                
                // Run the initializer
                session.run(_init);

            }

            //Train model
            Train(xData, yData, session);

            //evaluation
           // Evaluate();

            //prediction
            
            return;

        }

        protected override void Train(NDArray xData, NDArray yData, Session session)
        {
            //training process
            if (MLConfig.TParameters.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData, MLConfig.TParameters.SplitPercentage, shuffle: true);
                //tr.Run(x, y, lr, MLConfig.TParameters, History, MLConfig.Paths);
                tr.Run(session, MLConfig.LParameters, MLConfig.TParameters, processModel);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, MLConfig.TParameters.KFold);
                tr.Run(session, MLConfig.LParameters, MLConfig.TParameters, processModel);
            }
        }      

        protected Graph createGraph(List<int> shapeX, List<int> shapeY)
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
            var z = MLFactory.CreateNetwrok(MLConfig.Network, x, y);
            Tensor loss = null;
            //define learner
            tf_with(tf.variable_scope("Train"), delegate
            {
                tf_with(tf.variable_scope("Loss"), delegate
                {
                    loss = FunctionEx.Create(y, z, MLConfig.LParameters.LossFunction);
                });

                tf_with(tf.variable_scope("Optimizer"), delegate
                {
                   var optimizer = FunctionEx.Optimizer(MLConfig.LParameters, loss);
                });

                for(int i=0; i< MLConfig.LParameters.EvaluationFunctions.Count; i++)
                {
                    var e = MLConfig.LParameters.EvaluationFunctions[i];
                    tf_with(tf.variable_scope($"Eval{i}"), delegate
                    {
                        var ev  = FunctionEx.Create(y, z, e);
                    });
                }
            });

            //
            return graph;

        }

        private Session processModel(Session session, ProgressReport tp)
        {
            if (session == null)
            {
                return loadModelCheckPoint();
            }
            else
            {
                //save only when training is completed.
                if(tp.ProgressType== ProgressType.Completed)
                {
                    saveModel(session, tp);
                    MLFactory.Save(MLConfig, MLConfig.Paths["MLConfig"]).Wait();
                }
              
                return null;
            }

        }

        private Session saveModel(Session sess, ProgressReport tp)
        {
            var paths = MLConfig.Paths;
            var saver = tf.train.Saver();

            if (!paths.ContainsKey("BestModel"))
                paths.Add("BestModel", "");
            if (!paths.ContainsKey("Models"))
                paths.Add("Models", "Models");


            // Restore variables from checkpoint
            var root = $"{paths["MainFolder"]}";
            var curDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(root);

            //delete all previous models
            var di = new DirectoryInfo(paths["Models"]);
            if(di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            var strPath = saver.save(sess, $"{paths["Models"]}/{DateTime.Now.Ticks}.ckp");
            MLConfig.Paths["BestModel"] = strPath+".meta";
            Directory.SetCurrentDirectory(curDir);
            return null;
        }

        private Session loadModelCheckPoint()
        {
            var paths = MLConfig.Paths;
            var modelFilePath = paths["BestModel"];
            var root = $"{paths["MainFolder"]}";
            var curDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(root);

            var f = new FileInfo(modelFilePath);
            if(f.Exists)
            {
                var graph = tf.Graph().as_default();
                var sess = tf.Session(graph);
                var saver = tf.train.import_meta_graph(modelFilePath);
                // Restore variables from checkpoint
                saver.restore(sess, tf.train.latest_checkpoint(new DirectoryInfo(modelFilePath).Parent.Name));
                Directory.SetCurrentDirectory(curDir);
                return sess;
            }
            return null;
        }

    }
}
