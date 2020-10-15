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

            tr.Loss = createLossFunction(y, model, par.LossFunction);

            //evaluation function
            tr.Evals = new List<Tensor>();
            foreach (var f in par.EvaluationFunctions)
            {
                var ef = createEvaluationFunction(y, model, f);
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
                case LearnerType.SGDLearner:
                    return tf.train.GradientDescentOptimizer(par.LearningRate);
                case LearnerType.AdamLearner:
                    return tf.train.AdamOptimizer(par.LearningRate);
                default:
                    throw new NotSupportedException();
            }
        }

        private Tensor createEvaluationFunction(Tensor y, Tensor model, Metrics f)
        {
            switch (f)
            {
                case Metrics.SquaredError:
                    return FunctionEx.SquaredError(y, model);
                case Metrics.MSError:
                    return FunctionEx.MeanSquaredError(y, model);
                case Metrics.RMSError:
                    return FunctionEx.RootMeanSquaredError(y, model);
                case Metrics.AbsoluteError:
                    return FunctionEx.AbsoluteError(y, model);
                default:
                    throw new NotSupportedException($"Not supported eval function '{f.ToString()}' for classification Learner.");
            }
        }

        private Tensor createLossFunction(Tensor y, Tensor model, Losses lossFunction)
        {
            switch (lossFunction)
            {
                case Losses.SquaredError:
                    return FunctionEx.SquaredError(y, model);
                default:
                    throw new NotSupportedException("Not supported loss function.");
            }
        }
    }
}
