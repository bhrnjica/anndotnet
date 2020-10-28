using Anndotnet.Core.Interface;
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
    public class RegressionLearner : ILearner 
    {
        public Learner Create(Tensor y, Tensor model, LearningParameters par)
        {
            var tr = new Learner();

            tr.Loss = createFunction(y, model, par.LossFunction);

            //evaluation function
            tr.Evals = new List<Tensor>();
            foreach (var f in par.EvaluationFunctions)
            {
                var ef = createFunction(y, model, f);
                tr.Evals.Add(ef);
            }

            // We add the training operation, ...
            Optimizer opt = createOptimizer(par);
            tr.Optimizer = opt.minimize(tr.Loss, name: "train_op");

            return tr;
        }

        private Optimizer createOptimizer(LearningParameters par)
        {
            switch (par.LearnerType)
            {
                case LearnerType.SGD:
                    return tf.train.GradientDescentOptimizer(par.LearningRate);
                case LearnerType.Adam:
                    return tf.train.AdamOptimizer(par.LearningRate);
                default:
                    throw new NotSupportedException();
            }
        }

        private Tensor createFunction(Tensor y, Tensor model, Metrics f)
        {
            switch (f)
            {
                case Metrics.SE:
                    return FunctionEx.SquaredError(y, model);
                case Metrics.MSE:
                    return FunctionEx.MeanSquaredError(y, model);
                case Metrics.RMSE:
                    return FunctionEx.RootMeanSquaredError(y, model);
                case Metrics.AE:
                    return FunctionEx.AbsoluteError(y, model);
                default:
                    throw new NotSupportedException($"Not supported function '{f.ToString()}' for regression Learner.");
            }
        }
    }
}
