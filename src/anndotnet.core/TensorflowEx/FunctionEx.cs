﻿//////////////////////////////////////////////////////////////////////////////////////////
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
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
namespace Anndotnet.Core.TensorflowEx
{
    public static class FunctionEx
    {
        public static Operation Optimizer(LearningParameters lp, Tensor loss)
        {
            // We add the training operation, ...
            var adam = tf.train.AdamOptimizer(lp.LearningRate);
            return  adam.minimize(loss, name: LearnerType.Adam.ToString());     
        }
        public static Tensor Create(Tensor y, Tensor model, Metrics f)
        {
            switch (f)
            {
                case Metrics.Precision:
                    return FunctionEx.Precision(y, model);
                case Metrics.Recall:
                    return FunctionEx.Recall(y, model);
                case Metrics.CErr:
                    return FunctionEx.ClassificationError(y, model);
                case Metrics.CAcc:
                    return FunctionEx.Accuracy(y, model);
                case Metrics.BCE:
                    return FunctionEx.BinaryCrossEntropy(y, model);
                case Metrics.CCE:
                    return FunctionEx.MultiClassCrossEntropy(y, model);
                default:
                    throw new NotSupportedException($"Not supported function '{f.ToString()}' for classification Learner.");
            }
        }

        /// <summary>
        /// Calculates RMSE between two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor RootMeanSquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            var losses = tf.pow(z - y, 2.0f);

            retVal = tf.sqrt(tf.reduce_mean(losses), name: Metrics.RMSE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates MSE between two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor MeanSquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            var losses = tf.pow(z - y, 2.0f);

            retVal = tf.reduce_mean(losses, name: Metrics.MSE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates SE between two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor SquaredError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            var losses = tf.pow(z - y, 2.0f);
            retVal = tf.reduce_sum(losses, name: Metrics.SE.ToString());
            return retVal;
        }

        /// <summary>
        /// Calculates Absolute Error between two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor AbsoluteError(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            var losses = tf.abs(z - y);

            retVal = tf.reduce_mean(losses, name: Metrics.AE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates Binary Cross Entropy of two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor BinaryCrossEntropy(Tensor y, Tensor z)
        {
            //
            Tensor retVal = null;
            var losses = tf.nn.sigmoid_cross_entropy_with_logits(labels: y, logits: z);

            retVal = tf.reduce_mean(losses, name: Metrics.BCE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates Multi-class Cross Entropy of two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        private static Tensor MultiClassCrossEntropy(Tensor y, Tensor z,float epsilon = 1e-9f)
        {
            Tensor retVal = null;
            // tf_with(tf.variable_scope("Loss"), delegate
            retVal = -tf.reduce_sum(y * tf.math.log(z + epsilon), axis: z.dims.Length - 1, name: Metrics.CCE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates Multi-class Cross Entropy of two tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor MultiClassCrossEntropy(Tensor y, Tensor z)
        {
            Tensor retVal = null;
            retVal = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(labels: y, logits: z), name: Metrics.CCE.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates Accuracy of two one-hot tensors of the same shape.In case binary value is 
        /// represents as probability, the threshold value is used to determine the value.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor Accuracy(Tensor y, Tensor z, float thrashold = 0.5f)
        {
            Tensor retVal = null;
            Tensor correct = null;
            //check if the output is one hot vectors
            if (y.dims.Last() > 1)
                correct = tf.equal(tf.cast(tf.argmax(y, 1), tf.int32), tf.cast(tf.argmax(z, 1), tf.int32));
            else
            {
                var trasholdT = tf.constant(thrashold);
                var yy = tf.greater_equal(y, trasholdT);
                var zz = tf.greater_equal(z, trasholdT);
                correct = tf.equal(yy, zz);
            }

            //calculate accuracy
            var cast = tf.cast(correct, tf.float32);
            retVal = tf.reduce_mean(cast, name: Metrics.CAcc.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculates Error value of two one-hot tensors of the same shape.
        /// </summary>
        /// <param name="y">Actual values</param>
        /// <param name="z">Predicted values</param>
        /// <returns></returns>
        public static Tensor ClassificationError(Tensor y, Tensor z, float trashold = 0.5f)
        {
            Tensor retVal = null;
            retVal = 1.0f - Accuracy(y, z, trashold);
            //set name
            retVal = tf.identity(retVal, name: Metrics.CErr.ToString());
            return retVal;
        }

        /// <summary>
        /// Calculate precision (x=0)
        /// </summary>
        /// <param name="Actual"></param>
        /// <param name="Predicted"></param>
        /// <returns></returns>
        public static Tensor Precision(Tensor Actual, Tensor Predicted)
        {
            Tensor retVal = null;
            var cm = ConfusionMatrix(Actual, Predicted);
            //prepare for calculation 
            var once = tf.ones(new Shape(1, cm.dims[0]));
            var sum = tf.matmul(once, cm);
            var iden = tf.diag(tf.ones(new Shape(cm.dims[0]), TF_DataType.TF_FLOAT));
            var tpm = tf.multiply(cm, iden);
            var tp = tf.matmul(once, tpm);
            //-----------------------------------------
            retVal = tp / sum;

            //in case binary classification with dummy encoding ()
            if (Actual.dims.Last() == 1)
            {
                //create 
                var d = tf.constant(new float[,] { { 1 }, { 0 } });
                retVal = tf.matmul(retVal, d);
            }
            //set name
            retVal = tf.identity(retVal, name: Metrics.Precision.ToString());

            return retVal;
        }

        /// <summary>
        /// Calculate precision (axis x=1) of the 'one hot' tensors of the same shape
        /// </summary>
        /// <param name="Actual"></param>
        /// <param name="Predicted"></param>
        /// <returns></returns>
        public static Tensor Recall(Tensor Actual, Tensor Predicted)
        {
            Tensor retVal = null;
            var cm = ConfusionMatrix(Actual, Predicted);
            //prepare for calculation 
            var once = tf.ones(new Shape(1, cm.dims[0]), TF_DataType.TF_FLOAT);
            var sum = tf.matmul(once, tf.transpose(cm, new int[] { 1, 0 }));
            var iden = tf.diag(tf.ones(new Shape(cm.dims[0]), TF_DataType.TF_FLOAT));//np.identity(cm.dims[0],dtype:typeof(float));
            var tpm = tf.multiply(cm, iden);
            var tp = tf.matmul(once, tpm);
            //-----------------------------------------
            retVal = tp / sum;

            //in case binary classification with dummy encoding ()
            if (Actual.dims.Last() == 1)
            {
                //create 
                var d = tf.constant(new float[,] { { 1 }, { 0 } });
                retVal = tf.matmul(retVal, d);


            }
            //set name
            retVal = tf.identity(retVal, name: Metrics.Recall.ToString());

            return retVal;

        }
        
        /// <summary>
        /// Calculate Confusion matrix for 'one-hot' tensors of the same shape
        /// </summary>
        /// <param name="Actual"></param>
        /// <param name="Predicted"></param>
        /// <returns></returns>
        public static Tensor ConfusionMatrix(Tensor Actual, Tensor Predicted)
        {
            Tensor retVal = null;
            if (Actual.dims.Last() > 1)
            {
                var tt = tf.transpose(Actual, new int[] { 1, 0 });
                //Calculate confusion matrix as multiplication of one hot encoding values of actual and predicted
                var cm = tf.matmul(tt, tf.round(Predicted));
                retVal = cm;
            }
            else
            {
                var one = tf.constant(1f);
                // Compute accuracy
                var actual1 = one - Actual;
                var pred = tf.round(Predicted);
                var predicted1 = one - pred;

                var a = tf.concat(new List<Tensor>() { Actual, actual1 }, axis: 1);
                var p = tf.concat(new List<Tensor>() { pred, predicted1 }, axis: 1);

                //make a confusion matrix
                var tt = tf.transpose(a, new int[] { 1, 0 });
                //Calculate confusion matrix as multiplication of one hot encoding values of actual and predicted
                var cm = tf.matmul(tt, tf.round(p));
                retVal = cm;
            }

            return retVal;
        }
    }
}
