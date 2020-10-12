using System;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
namespace Anndotnet.Core.TensorflowEx
{
    public static class FunctionEx
    {
        public static Tensor RootMeanSquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.pow(z - y, 2.0f);

                retVal = tf.sqrt(tf.reduce_mean(losses));
            });

            return retVal;
        }

        public static Tensor MeanSquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.pow(z - y, 2.0f);

                retVal = tf.reduce_mean(losses);
            });

            return retVal;
        }

        public static Tensor SquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.pow(z - y, 2.0f);

                retVal = tf.reduce_sum(losses);
            });

            return retVal;
        }

        public static Tensor AbsoluteError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.abs(z - y);

                retVal = tf.reduce_mean(losses, name: "Loss");
            });

            return retVal;
        }

        public static Tensor BinaryCrossEntropy(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.nn.sigmoid_cross_entropy_with_logits(labels: y, logits: z);

                retVal = tf.reduce_mean(losses, name: "Loss");
            });

            return retVal;
        }

        private static Tensor MultiClassCrossEntropy(Tensor y, Tensor z,float epsilon = 1e-9f)
        {
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {
                retVal = - tf.reduce_sum(y * tf.math.log(z+epsilon), axis: z.dims.Length-1);
            });

            return retVal;
        }

        public static Tensor MultiClassCrossEntropy(Tensor y, Tensor z)
        {
            Tensor retVal = null;
            tf_with(tf.variable_scope("Loss"), delegate
            {
                retVal = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(labels: y, logits: z), name:"Loss");
            });

            return retVal;
        }
    }
}
