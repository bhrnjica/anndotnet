////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Core.TensorflowEx;
using static TorchSharp.torch;

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