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
using System;
using Tensorflow;

namespace Anndotnet.Core.Learners
{
    public class ClassificationLearner : ILearner 
    {
      
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
