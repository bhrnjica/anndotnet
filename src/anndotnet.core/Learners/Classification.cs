﻿using Anndotnet.Core.Interface;
using Anndotnet.Core.TensorflowEx;
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
        public Learner Create(Tensor y, Tensor model, LearningParameters par)
        {
            var tr = new Learner();

            tr.Loss = createFunction(y, model, par.LossFunction);
            tr.Evals = new List<Tensor>();
            foreach (var f in par.EvaluationFunctions)
            {
                var ef = createFunction(y, model, f);
                tr.Evals.Add(ef);
            }

            // We add the training operation, ...
            var adam = tf.train.AdamOptimizer(0.01f);
            tr.Optimizer = adam.minimize(tr.Loss, name: "train_op");
            
            return tr;
        }

        private Tensor createFunction(Tensor y, Tensor model, Metrics f)
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

    }
}
