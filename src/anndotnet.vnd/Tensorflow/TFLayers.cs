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
using Anndotnet.Core;
using NumSharp;

namespace Anndotnet.Vnd
{
    public class TFLayers : TFBase
    {

        public Tensor Dense(Tensor inX, int outputDim, ValueInitializer initValueType, string name, int seed= 1234)
        {
            Tensor z = null;
            tf_with(tf.variable_scope(name), delegate
            {
                int inDim = inX.shape.Last();
                int ouDim = outputDim;

                //generate initial values
                Tensor initValueW = RandomValues(initValueType, (inDim, ouDim), TF_DataType.TF_FLOAT, seed: seed);
                Tensor initValueb = RandomValues(initValueType, shape: ouDim, TF_DataType.TF_FLOAT, seed: seed);

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

        public Tensor Drop(Tensor inX, float rate, string name, int seed= 1234)
        {
            Tensor z = null;
            tf_with(tf.variable_scope(name), delegate
            {
                z = tf.nn.dropout(inX, rate: rate, name: name, seed: seed) ;
            });

            return z;
        }

    }
}
