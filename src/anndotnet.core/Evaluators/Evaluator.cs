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
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Interfaces;
using System;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;

namespace AnnDotNet.Core.Evaluators;

public class Evaluator : IEvaluator
{

    public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed)
    {
        throw new NotImplementedException();
    }

    public Task<NDArray> PredictAsync(Session session, Tensor data)
    {
        throw new NotImplementedException();
    }

}