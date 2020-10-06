using AnnDotNET.Common;
using NumSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
namespace ann.vdn
{
    public class Layers : LayersBase
    {

        public Tensor Dense(Tensor inX, int outputDim, ValueInitializer initValueType, string name)
        {
            Tensor z = null;
            tf_with(tf.variable_scope(name), delegate
            {
                int inDim = inX.shape.Last();
                int ouDim = outputDim;

                //generate initial values
                Tensor initValueW = RandomValues(initValueType, (inDim, ouDim), TF_DataType.TF_FLOAT);
                Tensor initValueb = RandomValues(initValueType, shape: ouDim, TF_DataType.TF_FLOAT);

                var w = tf.Variable<Tensor>(initValueW, name: "w");
                var b = tf.Variable<Tensor>(initValueb, name: "b");

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


        public Tensor Drop(Tensor inX, float rate, string name)
        {
            Tensor z = null;
            tf_with(tf.variable_scope(name), delegate
            {
                z = tf.nn.dropout(inX, rate: rate, name: name) ;
            });

            return z;
        }

    }
}
