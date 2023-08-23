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
using AnnDotNet.Core.Entities;
using System.Threading.Tasks;
using static TorchSharp.torch;

namespace AnnDotNet.Core.Interfaces;

public interface IEvaluator
{
    ///// <summary>
    ///// Evaluates the model during training process
    ///// </summary>
    ///// <param name="model">Model to be evaluated</param>
    ///// <param name="y"></param>
    ///// <param name="dFeed">Data for the evaluation.</param>
    ///// <returns>returns true if the evaluation process succeeded.</returns>
    //bool Evaluate(Tensor model, Tensor y, DataFeed dFeed);

    ///// <summary>
    ///// For input tensor calculated the the output data
    ///// </summary>
    ///// <param name="data">Input data to predict</param>
    ///// <returns>Output tensor as the result of the model prediction.</returns>
    //Task<Tensor> PredictAsync(Tensor data);

    Dictionary<string, float> CalculateMetrics(List<EvalFunction> evalFunctions, Tensor predicted, Tensor target);
    Tensor CalculateLoss(Tensor predicted, Tensor actual);
}