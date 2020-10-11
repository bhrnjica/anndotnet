using Anndotnet.Core.Interface;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;

namespace Anndotnet.Core.Learners
{
    public class ClassificationLearner : ILearner 
    {
        public AnnLearner Create(Tensor y, Tensor model, LearningParameters par)
        {
            var tr = new AnnLearner();

            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.nn.sigmoid_cross_entropy_with_logits(tf.cast(y, tf.float32), model);
                tr.Loss = tf.reduce_mean(losses);
            });

            tf_with(tf.variable_scope("Evaluation"), delegate
            {
                var y_pred = tf.cast(model > 0, tf.int32);
                tr.Eval = tf.reduce_mean(tf.cast(tf.equal(y_pred,tf.cast(y,tf.int32)), tf.float32));
                // accuracy = tf.Print(accuracy, data =[accuracy], message = "accuracy:")
            });

            // We add the training operation, ...
            var adam = tf.train.AdamOptimizer(0.01f);
            tr.Learner = adam.minimize(tr.Loss, name: "train_op");

            return tr;
        }

    }
}
