//////////////////////////////////////////////////////////////////////////////////////////
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
