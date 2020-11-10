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
using Anndotnet.Vnd.Layers;
using System.Threading.Tasks;
using System.Xml;

namespace Anndotnet.Vnd
{
    public class MLRunner : MLRunnerBase
    {
        List<ColumnInfo> Metadata { get; set; }
        List<LayerBase> Network { get; set; }
        LearningParameters LParameters { get; set; }
        TrainingParameters TParameters { get; set; }
        NDArray X;
        NDArray Y;

        public MLRunner(List<LayerBase> network, LearningParameters lParam, TrainingParameters tParam, NDArray xData, NDArray yData, List<ColumnInfo> metadata):base()
        {
            Network = network;
            LParameters = lParam;
            TParameters = tParam;
            X = xData;
            Y = yData;
            Metadata = metadata;
        }

        public async Task SaveMlConfig(List<ColumnInfo> metadata, string filePath)
        {
            var mlCOnfig = getMLConfig();
            mlCOnfig.Metadata = metadata;
            await MLFactory.Save(mlCOnfig, filePath);
        }

        public override void Run()
        {
            //
            List<int> shapeX = new List<int>();
            List<int> shapeY = new List<int>();
            shapeX.Add(-1);//first dimension
            shapeX.AddRange(X.Shape.Dimensions.Skip(1));
            shapeY.Add(-1);//first dimension
            //
            if(Y.Shape.Dimensions.Length > 1)
                shapeY.AddRange(Y.Shape.Dimensions.Skip(1));

            Session session = null;
            tf.compat.v1.disable_eager_execution();

            //create network from network collection
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
            Train(X, Y, session);

            //evaluation

            //prediction
            return;

        }


        protected override void Train(NDArray xData, NDArray yData, Session session)
        {
            //training process
            if (TParameters.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData, TParameters.SplitPercentage);
                //tr.Run(x, y, lr, MLConfig.TParameters, History, MLConfig.Paths);
                tr.Run(session, LParameters, TParameters, processModel);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, TParameters.KFold);
                tr.Run(session, LParameters, TParameters, processModel);
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
            var z = MLFactory.CreateNetwrok(Network, x, y);
            Tensor loss = null;
            //define learner
            tf_with(tf.variable_scope("Train"), delegate
            {
                tf_with(tf.variable_scope("Loss"), delegate
                {
                    loss = FunctionEx.Create(y, z, LParameters.LossFunction);
                });

                tf_with(tf.variable_scope("Optimizer"), delegate
                {
                   var optimizer = FunctionEx.Optimizer(LParameters, loss);
                });

                for(int i=0; i< LParameters.EvaluationFunctions.Count; i++)
                {
                    var e = LParameters.EvaluationFunctions[i];
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
                return null;// loadModelCheckPoint();
            }
            else
            {
                //save only when training is completed.
                if(tp.ProgressType== ProgressType.Completed)
                {
                    var paths = new Dictionary<string,string>();
                    var mlConfig = getMLConfig();
                    saveModel(session, paths);
                }
              
                return null;
            }

        }
        private MLConfig getMLConfig()
        {
            var mlConfig = new MLConfig();
            mlConfig.Id = Guid.NewGuid();
            mlConfig.LParameters = LParameters;
            mlConfig.TParameters = TParameters;
            mlConfig.Metadata = Metadata;
            mlConfig.Network = Network;
            mlConfig.Paths = null;

            return mlConfig;
        }

        //private Session saveModel(Session sess, ProgressReport tp)
        //{
        //    var paths = new Dictionary<string, string>();
        //    paths.Add("BestModel", "");
        //    paths.Add("Models", "Models");
        //    var saver = tf.train.Saver();

        //    // Restore variables from checkpoint
        //    var root = $"{paths["MainFolder"]}";
        //    var curDir = Directory.GetCurrentDirectory();
        //    Directory.SetCurrentDirectory(root);

        //    //delete all previous models
        //    var di = new DirectoryInfo(paths["Models"]);
        //    foreach (FileInfo file in di.GetFiles())
        //    {
        //        file.Delete();
        //    }

        //    var strPath = saver.save(sess, $"{paths["Models"]}/{DateTime.Now.Ticks}.ckp");
        //    paths["BestModel"] = strPath + ".meta";
        //    Directory.SetCurrentDirectory(curDir);
        //    return null;
        //}

        //private Session loadModelCheckPoint()
        //{
        //    var paths = MLConfig.Paths;
        //    var modelFilePath = paths["BestModel"];
        //    var root = $"{paths["MainFolder"]}";
        //    var curDir = Directory.GetCurrentDirectory();
        //    Directory.SetCurrentDirectory(root);

        //    var f = new FileInfo(modelFilePath);
        //    if(f.Exists)
        //    {
        //        var graph = tf.Graph().as_default();
        //        var sess = tf.Session(graph);
        //        var saver = tf.train.import_meta_graph(modelFilePath);
        //        // Restore variables from checkpoint
        //        saver.restore(sess, tf.train.latest_checkpoint(new DirectoryInfo(modelFilePath).Parent.Name));
        //        Directory.SetCurrentDirectory(curDir);
        //        return sess;
        //    }
        //    return null;
        //}

    }
}
