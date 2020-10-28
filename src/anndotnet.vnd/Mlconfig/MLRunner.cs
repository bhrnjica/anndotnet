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

namespace Anndotnet.Vnd
{
    public class MLRunner
    {
        MLConfig MLConfig { get; set; }
        TrainingHistory History { get; set; }

        ConfigProto _config;
        Operation _init;

        public MLRunner(MLConfig mlConfig)
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true,
            };

            // Initialize the variables (i.e. assign their default value)
            _init = tf.global_variables_initializer();

            MLConfig = mlConfig;
        }

        public void Run()
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
                var graph = createGraph(MLConfig, shapeX, shapeY);
                session = tf.Session(graph);

                // Run the initializer
                session.run(_init);

            }

            //Train model
            Train(xData, yData, session);


            //evaluation


            //prediction
            return;

        }

        private void Train(NDArray xData, NDArray yData, Session session)
        {
            //training process
            if (MLConfig.TParameters.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData, MLConfig.TParameters.SplitPercentage);
                //tr.Run(x, y, lr, MLConfig.TParameters, History, MLConfig.Paths);
                tr.Run(session, MLConfig.LParameters, MLConfig.TParameters, processModel);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, MLConfig.TParameters.KFold);
                tr.Run(session, MLConfig.LParameters, MLConfig.TParameters, processModel);
            }
        }      

        private Graph createGraph(MLConfig mLConfig, List<int> shapeX, List<int> shapeY)
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
            
            var learner = new Learner();
            //define learner
            tf_with(tf.variable_scope("Train"), delegate
            {
                tf_with(tf.variable_scope("Loss"), delegate
                {
                    learner.Loss = FunctionEx.Create(y, z, mLConfig.LParameters.LossFunction);
                });

                tf_with(tf.variable_scope("Optimizer"), delegate
                {
                    learner.Optimizer = FunctionEx.Optimizer(mLConfig.LParameters, learner.Loss);
                });

                for(int i=0; i< mLConfig.LParameters.EvaluationFunctions.Count; i++)
                {
                    var e = mLConfig.LParameters.EvaluationFunctions[i];
                    tf_with(tf.variable_scope($"Eval{i}"), delegate
                    {
                        var ev  = FunctionEx.Create(y, z, e);
                        learner.Evals.Add(ev);
                    });
                }
            });

            //
            return graph;

        }

        private Session processModel(Session session, TrainingProgress tp)
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

        private Session saveModel(Session sess, TrainingProgress tp)
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
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
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
