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


using AnnDotNet.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tensorflow;
using XPlot.Plotly;
using static Tensorflow.Binding;

namespace AnnDotNet.Vnd;

public class TFLayers : TFBase
{
    public Tensor Output(Tensor inputX, Tensor outputY)
    {
        Tensor z = null;
        tf_with(tf.variable_scope(TfScopes.OutputLayer), delegate
        {
            z = tf.identity(inputX, "Y");
        });

        return z;
    }

    public Tensor Dense(Tensor inX, int outputDim, ValueInitializer initValueType, string name, int seed= 1234)
    {
        Tensor z = null;
        tf_with(tf.variable_scope(name), delegate
        {
            int inDim = (int)inX.shape.dims.Last();
            int ouDim = outputDim;

            //generate initial values
            Tensor initValueW = RandomValues(initValueType, (inDim, ouDim), TF_DataType.TF_FLOAT, seed: seed);
            Tensor initValueB = RandomValues(initValueType, shape: ouDim, TF_DataType.TF_FLOAT, seed: seed);

            var w = tf.Variable<Tensor>(initValueW, name: "w");
            var b = tf.Variable<Tensor>(initValueB, name: "b");

            z = tf.matmul(inX, w) + b;

        });

        return z;
    }

    public Tensor ActivationLayer (Tensor inX, Activation fun, string name)
    {
        Tensor z = null;
        tf_with(tf.variable_scope(name), delegate
        {
            switch (fun)
            {
                case Activation.None:
                    z= inX;
                    break;
                case Activation.ReLU:
                    z = tf.nn.relu(inX);
                    break;
                case Activation.Sigmoid:
                    z =  tf.nn.sigmoid(inX);
                    break;
                case Activation.Softmax:
                    z =  tf.nn.softmax(inX);
                    break;
                case Activation.TanH:
                    z =  tf.nn.tanh(inX);
                    break;
                default:
                    z = inX;
                    break;
            }

        });

        return z;
    }

    public Tensor Drop(Tensor inX, float rate, string name, int seed= 1234)
    {
        Tensor z = null;
        tf_with(tf.variable_scope(name), delegate
        {
            z = tf.nn.dropout(inX, rate: rate, name: name, seed: seed) ;
        });

        return z;
    }

    public Tensor Embedding(Tensor inX, int outputDim, ValueInitializer initValueType, string name, int seed = 1234)
    {
        Tensor z = null;
        tf_with(tf.variable_scope(name), delegate
        {
            int inDim = (int)inX.shape.dims.Last();
            int ouDim = outputDim;

            //var ss = tf.keras.layers.Embedding(inDim, ouDim).Apply(inX);
            //return ss;

            //generate initial values
            var embeddedMatrix = RandomValues(initValueType, (inDim, ouDim), TF_DataType.TF_INT32, seed: seed);
            var embeddingLookup = tf.nn.embedding_lookup(embeddedMatrix,inX, name: "embedding_lookup");

            z = embeddingLookup;
        });

        return z;
    }

}