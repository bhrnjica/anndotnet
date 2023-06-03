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
using Anndotnet.Core.TensorflowEx;
using Anndotnet.Vnd.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace Anndotnet.Vnd
{
    public class MLRunnerBase : IRunner
    {
        protected TrainingHistory History { get; set; }
        protected ConfigProto _config;
        protected Operation _init;

        public MLRunnerBase()
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true,
            };
        }


        public virtual void Run()
        {
            //// prepare for training
            //Prepare();

            ////Train model
            //Train(X, Y, session);


            ////evaluation


            ////prediction
            //return;

        }

        protected virtual void Prepare()
        {
            throw new NotImplementedException();
        }

        protected virtual void Train(NDArray xData, NDArray yData, Session session)
        {
            throw new NotImplementedException();
        }

        protected virtual void Evaluate()
        {
            throw new NotImplementedException();
        }

        protected Session saveModel(Session sess, Dictionary<string, string> paths)
        {
            var saver = tf.train.Saver();

            if (!paths.ContainsKey("BestModel"))
                paths.Add("BestModel", "");
            if (!paths.ContainsKey("Models"))
                paths.Add("Models", "Models");

            //generate main folder path if it is missing
            var curDir = Directory.GetCurrentDirectory();
            if (!paths.ContainsKey("MainFolder"))
                paths.Add("MainFolder", curDir);

            // Restore variables from checkpoint
            var root = $"{paths["MainFolder"]}";
            
            Directory.SetCurrentDirectory(root);

            //delete all previous models
            var di = new DirectoryInfo(paths["Models"]);
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            var strPath = saver.save(sess, $"{paths["Models"]}/{DateTime.Now.Ticks}.ckp");
            paths["BestModel"] = strPath + ".meta";
            Directory.SetCurrentDirectory(curDir);
            return null;
        }

        protected Session loadModelCheckPoint(Dictionary<string, string> paths)
        {
            var modelFilePath = paths["BestModel"];
            var root = $"{paths["MainFolder"]}";
            var curDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(root);

            var f = new FileInfo(modelFilePath);
            if (f.Exists)
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
        protected Graph createGraph(List<LayerBase> net,LearningParameters lParams, List<int> shapeX, List<int> shapeY)
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
    }
}
