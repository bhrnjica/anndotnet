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
using static TorchSharp.torch;

namespace AnnDotNet.Core.Learners;

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