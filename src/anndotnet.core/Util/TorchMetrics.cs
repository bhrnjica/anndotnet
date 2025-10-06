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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Entities;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Util
{
    public class TorchMetrics
    {
        /// <summary>
        /// Calculates absolute error on GPU and transfers minimal data to CPU
        /// </summary>
        public static float AbsoluteError(Tensor predicted, Tensor expected)
        {
            using var ae = torch.nn.L1Loss(Reduction.Sum).forward(predicted, expected);
            return ae.ToSingle();
        }

        /// <summary>
        /// Calculates mean absolute error on GPU
        /// </summary>
        public static float MeanAbsoluteError(Tensor predicted, Tensor expected)
        {
            using var mae = torch.nn.L1Loss().forward(predicted, expected);
            return mae.ToSingle();
        }

        /// <summary>
        /// Calculates squared error on GPU
        /// </summary>
        public static float SquaredError(Tensor predicted, Tensor expected)
        {
            using var se = torch.nn.MSELoss(reduction: Reduction.Sum).forward(predicted, expected);
            return se.ToSingle();
        }

        /// <summary>
        /// Calculates mean squared error on GPU
        /// </summary>
        public static float MeanSquaredError(Tensor predicted, Tensor expected)
        {
            using var mse = torch.nn.MSELoss().forward(predicted, expected);
            return mse.ToSingle();
        }

        /// <summary>
        /// Calculates root mean squared error on GPU
        /// </summary>
        public static float RootMeanSquaredError(Tensor predicted, Tensor expected)
        {
            using var mse = torch.nn.MSELoss().forward(predicted, expected);
            using var rmse = torch.sqrt(mse);
            return rmse.ToSingle();
        }

        /// <summary>
        /// Calculates multiclass accuracy more efficiently on GPU
        /// </summary>
        public static float MCAccuracy(Tensor predicted, Tensor expected)
        {
            using var predictedClasses = predicted.argmax(1);
            using var correct = predictedClasses.eq(expected);
            using var correctSum = correct.sum();
            return correctSum.ToSingle() / expected.shape[0];
        }

        /// <summary>
        /// Calculates binary accuracy more efficiently on GPU
        /// </summary>
        public static float BIAccuracy(Tensor predicted, Tensor expected)
        {
            using var roundedPredicted = predicted.round();
            using var correct = roundedPredicted.eq(expected);
            using var correctSum = correct.sum();
            return correctSum.ToSingle() / expected.shape[0];
        }

        /// <summary>
        /// Calculates multiclass error more efficiently on GPU
        /// </summary>
        public static float MCError(Tensor predicted, Tensor expected)
        {
            return 1.0f - MCAccuracy(predicted, expected);
        }

        /// <summary>
        /// Calculates binary error more efficiently on GPU
        /// </summary>
        public static float BIError(Tensor predicted, Tensor expected)
        {
            return 1.0f - BIAccuracy(predicted, expected);
        }

        /// <summary>
        /// Calculates F1 score for binary classification
        /// </summary>
        public static float F1Score(Tensor predicted, Tensor expected, float threshold = 0.5f)
        {
            using var predictedBinary = predicted.gt(threshold);
            using var expectedBinary = expected.gt(0.5f);
            
            using var tp = predictedBinary.logical_and(expectedBinary).sum();
            using var fp = predictedBinary.logical_and(expectedBinary.logical_not()).sum();
            using var fn = predictedBinary.logical_not().logical_and(expectedBinary).sum();
            
            var tpVal = tp.ToSingle();
            var fpVal = fp.ToSingle();
            var fnVal = fn.ToSingle();
            
            var precision = tpVal / (tpVal + fpVal + 1e-8f);
            var recall = tpVal / (tpVal + fnVal + 1e-8f);
            
            return 2 * precision * recall / (precision + recall + 1e-8f);
        }

        /// <summary>
        /// Batch calculation of multiple metrics to reduce GPU-CPU transfers
        /// </summary>
        public static Dictionary<EvalFunction, float> CalculateMetricsBatch(
            List<EvalFunction> evalFunctions, 
            Tensor predicted, 
            Tensor target)
        {
            var results = new Dictionary<EvalFunction, float>();
            
            // Group calculations to minimize GPU-CPU transfers
            Tensor predictedClasses = null;
            Tensor correct = null;
            Tensor correctSum = null;
            
            try
            {
                foreach (var eval in evalFunctions)
                {
                    switch (eval)
                    {
                        case EvalFunction.AE:
                            results[eval] = AbsoluteError(predicted, target);
                            break;
                        case EvalFunction.MAE:
                            results[eval] = MeanAbsoluteError(predicted, target);
                            break;
                        case EvalFunction.SE:
                            results[eval] = SquaredError(predicted, target);
                            break;
                        case EvalFunction.MSE:
                            results[eval] = MeanSquaredError(predicted, target);
                            break;
                        case EvalFunction.RMSE:
                            results[eval] = RootMeanSquaredError(predicted, target);
                            break;
                        case EvalFunction.CAcc:
                        case EvalFunction.CErr:
                            // Reuse calculations for both accuracy and error
                            if (predictedClasses is null)
                            {
                                predictedClasses = predicted.argmax(1);
                                correct = predictedClasses.eq(target);
                                correctSum = correct.sum();
                            }
                            var accuracy = correctSum.ToSingle() / target.shape[0];
                            if (eval == EvalFunction.CAcc)
                                results[eval] = accuracy;
                            else
                                results[eval] = 1.0f - accuracy;
                            break;
                        case EvalFunction.BAcc:
                        case EvalFunction.BErr:
                            var binaryAcc = BIAccuracy(predicted, target);
                            if (eval == EvalFunction.BAcc)
                                results[eval] = binaryAcc;
                            else
                                results[eval] = 1.0f - binaryAcc;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            finally
            {
                // Clean up intermediate tensors
                predictedClasses?.Dispose();
                correct?.Dispose();
                correctSum?.Dispose();
            }
            
            return results;
        }

        public static KeyValuePair<string, float> Evaluate(EvalFunction eval, Tensor predicted, Tensor target)
        {
            return eval switch
            {
                EvalFunction.AE => new KeyValuePair<string, float>(EvalFunction.AE.ToString(), AbsoluteError(predicted, target)),
                EvalFunction.MAE => new KeyValuePair<string, float>(EvalFunction.MAE.ToString(), MeanAbsoluteError(predicted, target)),
                EvalFunction.SE => new KeyValuePair<string, float>(EvalFunction.SE.ToString(), SquaredError(predicted, target)),
                EvalFunction.MSE => new KeyValuePair<string, float>(EvalFunction.MSE.ToString(), MeanSquaredError(predicted, target)),
                EvalFunction.RMSE => new KeyValuePair<string, float>(EvalFunction.RMSE.ToString(), RootMeanSquaredError(predicted, target)),
                EvalFunction.CAcc => new KeyValuePair<string, float>(EvalFunction.CAcc.ToString(), MCAccuracy(predicted, target)),
                EvalFunction.CErr => new KeyValuePair<string, float>(EvalFunction.CErr.ToString(), MCError(predicted, target)),
                EvalFunction.BAcc => new KeyValuePair<string, float>(EvalFunction.BAcc.ToString(), BIAccuracy(predicted, target)),
                EvalFunction.BErr => new KeyValuePair<string, float>(EvalFunction.BErr.ToString(), BIError(predicted, target)),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
