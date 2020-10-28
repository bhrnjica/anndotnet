using Anndotnet.Core.Interface;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;

namespace Anndotnet.Core.Data
{
    public class Placeholders : IPlaceholders
    {
       public (Tensor X, Tensor Y) Create(Shape input, Shape output, TF_DataType inType= TF_DataType.TF_FLOAT, TF_DataType outType= TF_DataType.TF_FLOAT)
        {
            Tensor X = null;
            Tensor Y = null;
            //
            tf_with(tf.variable_scope("placeholders"), delegate
            {
                X = tf.placeholder(inType, shape: input);
                Y = tf.placeholder(outType, shape:output);
            });
            //
            return (X, Y);
        }

        public Tensor Create(Shape input, string name, TF_DataType inType = TF_DataType.TF_FLOAT)
        {
            Tensor X = null;
            //
            //tf_with(tf.variable_scope($"x_placeholder"), delegate
            //{
                X = tf.placeholder(inType, shape: input, name: name);
            //});
            //
            return X;
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
