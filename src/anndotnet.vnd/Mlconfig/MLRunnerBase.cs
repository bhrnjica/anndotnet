///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnnDotNet.Core;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Core.TensorflowEx;
using AnnDotNet.Vnd.Extensions;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

using Daany.MathStuff;
using Anndotnet.Core.Interfaces;
using Daany.MathStuff.Stats;
using Anndotnet.Core.Entities;

namespace AnnDotNet.Vnd.Mlconfig;

public class MLRunnerBase : IRunner, IMlModel, IEvaluator
{
    protected TrainingHistory _history;
    protected readonly IPrintResults _printer;
    protected ConfigProto           _config;
    protected Operation             _init;
    protected LearningParameters    _lParameters { get; set; }
    protected TrainingParameters    _tParameters { get; set; }
    protected List<ILayer>          _network { get; set; }
    protected NDArray               _x;
    protected NDArray               _y;

    public MLRunnerBase(IPrintResults printer)
    {
        _printer=printer;

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

    protected Session Evaluate(Session session, ProgressReport tp)
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

    public void PredictionMetrics(NDArray predicted, NDArray actual, IReadOnlyList<ColumnInfo> metadata)
    {
        if (predicted.shape != actual.shape)
        {
            throw new Exception("Predicted and actual data have different shapes"); 
        }

        int dimension = (int)predicted.shape.dims.Last();
        if (dimension == 1)
        {
            //PrintBinaryClassificationMetrics(string name, BinaryClassificationMetrics metrics)
            var labels = metadata.Where(x => x.MLType == MLColumnType.Label).SelectMany(x => x.Transformer.ClassValues).ToArray();
            var pred = predicted.Select(x => (float)x).ToArray();
            var cm = new ConfusionMatrix(predicted.Select(x => (int)x).ToArray(),
                                        actual.Select(x => (int)x).ToArray(), labels.Length);

            var metrics = new BinaryClassificationMetrics(cm);
            
            //regression
            //_printer.PrintBinaryClassificationMetrics("Titanic", metrics);

        }
        else if (dimension == 2 )
        {
            //binary classification
            //var predictedClasses = tf.arg_max(predicted, 1);
            //var actualClasses = tf.arg_max(predicted, 1);
            //ConfusionMatrix cm = new ConfusionMatrix(predictedClasses.ToArray<long>(), actualClasses.ToArray<long>());
            //_printer.ConsolePrintConfusionMatrix();
            //_printer.ShowConfusionMatrix(predictedClasses.ToArray<long>(), actualClasses.ToArray<long>());


        }
        else
        {
            //multi-class classification
            var labels = metadata.Where(x=>x.MLType== MLColumnType.Label).SelectMany(x=>x.Transformer.ClassValues).ToArray();//new string[]{"setosa", "verisicolor", "virginica"});
            var predictedClasses = tf.arg_max(predicted, 1).ToArray<long>();
            var actualClasses = tf.arg_max(predicted, 1).ToArray<long>();

            ConfusionMatrix s= new ConfusionMatrix(actualClasses.Select(x => (int)x).ToArray(), predictedClasses.Select(x => (int)x).ToArray(), labels.Length );
            
            _printer.ConsolePrintConfusionMatrix(s, labels);

            //var accuracy = (predictedClasses == actualClasses).mean();
            //var accuracy1 = (predictedClasses == actualClasses).mean();
        }
    }



    #region IMlModel implementation
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
        var z = MLFactory.CreateNetwork(_network, x, y);

        Tensor loss = null;
            
        //define learner
        tf_with(tf.variable_scope(TfScopes.Train), delegate
        {
            tf_with(tf.variable_scope(TfScopes.Loss), delegate
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

    public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed)
    {
        throw new NotImplementedException();
    }

    public async Task<NDArray> PredictAsync(Session session, Tensor data)
    {
        try
        {
            //get input placeholder 
            var x = session.graph.get_tensor_by_name($"{TfScopes.Input}/X:0");
                
            //get model output tensor
            var output = session.graph.get_tensor_by_name($"{TfScopes.OutputLayer}/Y:0").outputs.First();

            //evaluate model
            var prediction = await Task.Run<NDArray>(() => session.run(output, (x, data)));
                
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

