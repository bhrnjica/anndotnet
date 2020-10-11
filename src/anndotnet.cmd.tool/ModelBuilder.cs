using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
namespace AnnDotNET.Tool
{
    public enum Activation
    {
        None = 0,
        ReLU = 1,
        Sigmoid = 2,
        Softmax = 3,
        TanH = 4,
        Max = 5,
        Avg = 6
    }

    public enum Loss
    {
        SE,
        //MSE,
        //RMSE,
        Logg,
        Sigmoid,
        Softmax,
    }

    public enum Eval
    {
        SE,
        MSE,
        RMSE,
        Accuracy,
        Pearson,
    }

    public enum Trainer
    {
        SGD,
        ADAM,
    }


    //available network layer in the library
    public enum LayerType
    {
        Activation,
        Dense,
        Embedding,
        Drop,
        LSTM,
        Conv,
        Pooling,
        Custom,
    }

    public enum Initializer
    {
        RandomNormal,
        Glorot,
    }

    public enum Regularization
    {
        None,
        L1,
        L2,
        L1L2,
    }


    /// <summary>
    /// Generic ANN Layer class for holding information about ANN Layer (Dense, Embedding, LSTM, Pooling, Convolution, etc...) 
    /// </summary>
    public class Layer : LayerBase
    {
        //LSTM Cell dimension, number of layers for StackedLSTM, GRU , TanH and ReLU, Pooling and ConvLayers
        public int CellDim { get; set; }
        public Initializer Initializer { get; set; }
        public Regularization Regularizer { get; internal set; }
        public float DropValue { get; internal set; }

        public Layer()
        {
            A = Activation.None;
            Name = "dense";
            Type = LayerType.Dense;
            HasBias = true;
            Initializer = Initializer.Glorot;
            Regularizer = Regularization.None;
        }
    }
    public class NNConfig
    {
        public string Name{ get; set; }
        
        public Tensor X { get; set; }
        public Tensor Y { get; set; }
        public List<Layer> Layers { get; set; }

        public Trainer Trainer { get; set; }
        public Eval Evaluation { get; set; }
        public Loss Loss { get; set; }
    }


    public class ModelBuilder
    {
        NNConfig _nnCofnig;
        public Tensor X { get; set; }
        public Tensor Y { get; set; }
        public Tensor model { get; set; }
        public Operation trainer { get; set; }

        public Tensor Cost { get; set; }
        public Tensor Evaluation { get; set; }
        public ModelBuilder(NNConfig nConfig)
        {
            _nnCofnig = nConfig;
        }

        public void Build()
        {
            X = _nnCofnig.X;
            Y = _nnCofnig.Y;
            model = BuildNetwork(_nnCofnig);
            trainer = BuildTrainer(model);
        }
        public Tensor BuildNetwork(NNConfig nConfig)
        {
            Tensor z = nConfig.X;
            foreach(var l in nConfig.Layers)
            {
                switch (l.Type)
                {
                    case LayerType.Activation:
                        z = DLLayer.ActivationLayer(z, l.A); 
                        break;
                    case LayerType.Dense:
                        z = DLLayer.FCLayer(z, l.Name, l.OutDim, l.HasBias, l.Initializer);
                        break;
                    case LayerType.Embedding:
                        z = DLLayer.Embedding(z, l.Name, l.OutDim, l.Initializer);
                        break;
                    case LayerType.Drop:
                        z = DLLayer.Drop(z, l.Name, l.DropValue, l.Seed);
                        break;
                    //case LayerType.LSTM:
                    //    break;
                    //case LayerType.Conv:
                    //    break;
                    //case LayerType.Pooling:
                    //    break;
                    //case LayerType.Custom:
                    //    break;
                    default:
                        z = DLLayer.ActivationLayer(z, l.A);
                        break;
                }
            }

            return z;
        }

        public Operation BuildTrainer(Tensor model)
        {
            var g = tf.get_default_graph();

            Tensor eval_op = null;
            Tensor loss_op = null;
            Operation train_op = null;

            //
            tf_with(tf.variable_scope("Loss"), delegate
            {
                switch (_nnCofnig.Loss)
                {
                    case Loss.SE:
                        {
                            // Squared error
                            var losses = tf.reduce_sum(tf.pow(model - _nnCofnig.Y, 2.0f));
                            loss_op = tf.reduce_mean(losses);
                            break;
                        }

                    //case Loss.MSE:
                    //    {
                    //        // Squared error
                    //        var losses = tf.reduce_sum(tf.pow(net - Y, 2.0f)) / (2.0f * n_samples);
                    //        loss_op = tf.reduce_mean(losses);
                    //        break;
                    //    }
                    //case Loss.RMSE:
                    //    break;
                    case Loss.Logg:
                        break;
                    case Loss.Sigmoid:
                        {
                            //https://www.tensorflow.org/api_docs/python/tf/nn/sigmoid_cross_entropy_with_logits
                            var losses = tf.nn.sigmoid_cross_entropy_with_logits(tf.cast(_nnCofnig.Y, tf.float32), model);
                            loss_op = tf.reduce_mean(losses);
                            break;
                        }
                    case Loss.Softmax:
                        {
                            //https://www.tensorflow.org/api_docs/python/tf/nn/softmax_cross_entropy_with_logits
                            var losses = tf.nn.softmax_cross_entropy_with_logits(tf.cast(_nnCofnig.Y, tf.float32), model);
                            loss_op = tf.reduce_mean(losses);
                            break;
                        }
                }

            });

            tf_with(tf.variable_scope("Evaluation"), delegate
            {


                switch (_nnCofnig.Evaluation)
                {
                    case Eval.SE:
                        {
                            // Squared error
                            var eval = tf.reduce_sum(tf.pow(model - _nnCofnig.Y, 2.0f));
                            eval_op = tf.reduce_mean(eval);
                            break;
                        }

                    case Eval.Accuracy:
                        {
                            //
                            var y_pred = tf.cast(model > 0, tf.int32);
                            eval_op = tf.reduce_mean(tf.cast(tf.equal(y_pred, _nnCofnig.Y), tf.float32));
                            break;
                        }

                }

            });

            // We add the training operation, ...
            switch (_nnCofnig.Trainer)
            {
                case Trainer.SGD:
                    var gdo = tf.train.GradientDescentOptimizer(0.01f);
                    train_op = gdo.minimize(loss_op);
                    break;
                case Trainer.ADAM:
                    var adam = tf.train.AdamOptimizer(0.01f);
                    train_op = adam.minimize(loss_op);
                    break;
            }

            //
            return train_op;
        }

    }

    public class DLLayer : NNBase
    {
        public static Tensor FCLayer(Tensor X, string name, int oDim, bool hasBias = true, Initializer init = Initializer.Glorot)
        {
            
            Tensor xw_b = null;
            //set output dimension
            if (oDim < 1)
                throw new ArgumentException("Output dimension should be greater than zero.");

            if (string.IsNullOrEmpty(name))
                throw new Exception("Layer name should not be null.");
            //
            var i = getInitializer(init);
            //var r = getRegularizer(reg);
            var nameW = string.IsNullOrEmpty(name) ? "weight" : name + "W";
            var nameb = string.IsNullOrEmpty(name) ? "bias" : name + "b";

            tf_with(tf.variable_scope("FullyConnected"), delegate
            {
            // create weigh and bias
            var W = tf.get_variable(name: nameW, shape: (X.shape.Last(), oDim), initializer: init, trainable: true, dtype: tf.float32);
            var b = tf.get_variable(name: nameb, shape: (oDim), initializer: init, trainable: true, dtype: tf.float32);

            // Construct a linear model   
            xw_b = tf.add((X * W), b, name: $"{name}_wX+b");
        });

            

            return xw_b;

        }

        internal static Tensor ActivationLayer(Tensor X, Activation activation, string name= "")
        {
            switch (activation)
            {
                case Activation.None:
                    return X;
                case Activation.ReLU:
                   return tf.nn.relu(X, string.IsNullOrEmpty(name) ? "ReLU" : name);
                case Activation.Sigmoid:
                    return tf.nn.sigmoid<Tensor>(X,string.IsNullOrEmpty(name) ? "Sigmoid" : name);
                case Activation.Softmax:
                    return tf.nn.softmax(X, -1, string.IsNullOrEmpty(name) ? "Softmax" : name);
                case Activation.TanH:
                    return tf.tanh(X, string.IsNullOrEmpty(name) ? "TanH" : name);
                case Activation.Max:
                    return tf.tanh(X, string.IsNullOrEmpty(name) ? "TanH" : name);
                case Activation.Avg:
                    return tf.reduce_mean(X, name:string.IsNullOrEmpty(name) ? "TanH" : name);
                default:
                    return X;

            }
        }

        public static Tensor Embedding(Tensor X, string name, int oDim, Initializer init = Initializer.Glorot)
        {
            //set output dimension
            if (oDim < 1)
                throw new ArgumentException("Output dimension should be greater than zero.");
            var i = getInitializer(init);
            //var r = getRegularizer(reg);
            var nameW = string.IsNullOrEmpty(name) ? "embeddings" : name + "_e";

            // create weigh and bias
            var W = tf.get_variable(name: nameW,
                                shape: (X.shape.Last(), oDim),
                                initializer: init);
            var b = tf.get_variable(name: nameW, shape: oDim, initializer: init);

            // Construct a linear model   
            return tf.multiply(X, W);

        }

        internal static Tensor Drop(Tensor X, string name, float dropValue, int? seed=1)
        {
            var nameL = string.IsNullOrEmpty(name) ? "dropout" : name + "_d";
            var fv = tf.constant(dropValue / 100f,TF_DataType.TF_FLOAT);
            return tf.nn.dropout(X, fv, name: nameL, seed: seed);
        }
    }
    public class NNBase
    {
        public static IInitializer getInitializer(Initializer init, int seed = 0)
        {
            switch (init)
            {
                case Initializer.RandomNormal:
                    return tf.random_normal_initializer(seed:seed);
                default:
                    return tf.glorot_uniform_initializer;

            }
        }

        //public IInitializer getRegularizer(Regularization reg, int seed = 0)
        //{
        //    switch (reg)
        //    {
        //        case Regularization.L1:
                    
        //            break;
        //        case Regularization.L2:
        //            break;
        //        case Regularization.L1L2:
        //            break;
        //        default:
        //            return tf.glorot_uniform_initializer;

        //    }
        //}
    
    }
}