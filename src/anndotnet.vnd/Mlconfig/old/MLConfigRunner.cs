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
using Anndotnet.Core.TensorflowEx;
using Anndotnet.Core.Trainers;
using Anndotnet.Vnd;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace anndotnet.vnd.Mlconfig.old
{
    public class MLConfigRunner : MLRunnerBase
    {
        MLConfig MLConfig { get; set; }

        public MLConfigRunner(MLConfig mlConfig) : base()
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
            (NDArray xData, NDArray yData) = MLFactory.PrepareData(MLConfig);

            Session session = null;
            tf.compat.v1.disable_eager_execution();

            //load trained model if exists
            if (MLConfig.TParameters.Retrain && MLConfig.Paths.ContainsKey("BestModel"))
            {
                session = loadModelCheckPoint(MLConfig.Paths);

            }
            //create network from mlconfig file
            if (session == null)
            {
                //create graph from machine learning configuration
                var shapeX = xData.shape;
                var shapeY = yData.shape;

                shapeX[0] = -1;
                shapeY[0] = -1;

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

        protected Graph createGraph(Shape shapeX, Shape shapeY)
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

                for (int i = 0; i < MLConfig.LParameters.EvaluationFunctions.Count; i++)
                {
                    var e = MLConfig.LParameters.EvaluationFunctions[i];
                    tf_with(tf.variable_scope($"Eval{i}"), delegate
                    {
                        var ev = FunctionEx.Create(y, z, e);
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
                return loadModelCheckPoint(MLConfig.Paths);
            }
            else
            {
                //save only when training is completed.
                if (tp.ProgressType == ProgressType.Completed)
                {
                    saveModel(session, MLConfig.Paths);
                    //MLFactory.Save(MLConfig, MLConfig.Paths["MLConfig"]).Wait();
                }

                return null;
            }

        }

        public async Task SaveMlConfig(string filePath)
        {
            await MLFactory.Save(MLConfig, filePath);
        }
    }
}
