///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Core.TensorflowEx;
using Tensorflow;
using static Tensorflow.Binding;

namespace AnnDotNet.Core.Learners;

public class RegressionLearner : ILearner 
{
      

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