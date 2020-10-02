using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;

namespace AnnDotNET.Common
{
    public class Placeholders : IPlaceholders
    {
       public (Tensor X, Tensor Y) Create(Shape input, Shape output)
        {
            tf.compat.v1.disable_eager_execution();

            Tensor X = null;
            Tensor Y = null;
            //
            tf_with(tf.variable_scope("placeholders"), delegate
            {
                X = tf.placeholder(tf.float32, shape: input);
                Y = tf.placeholder(tf.int32, shape:output);
            });
            //
            return (X, Y);
        }

        public (Tensor X, Tensor Y) Create(int inDim, int outDim)
        {
            Shape input = (-1, inDim);
            Shape output = (-1, outDim);
            //
            return Create(input, output);
        }
    }
}
