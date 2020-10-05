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
    public class RegressionLearner : ILearner 
    {
        public AnnLearner Create(Tensor y, Tensor model, LearningParameters par)
        {
            var tr = new AnnLearner();

            tf_with(tf.variable_scope("Loss"), delegate
            {
                // Squared error
                tr.Loss = tf.reduce_sum(tf.pow(model - y, 2.0f));
            });

            tf_with(tf.variable_scope("Evaluation"), delegate
            {
                // Mean squared error
                var cost = tf.reduce_sum(tf.pow(model - y, 2.0f));
                tr.Eval = tf.reduce_mean(cost);
            });

            // We add the training operation, ...
            var adam = tf.train.AdamOptimizer(0.01f);
            tr.Learner = adam.minimize(tr.Loss, name: "train_op");

            return tr;
        }
    }
}
