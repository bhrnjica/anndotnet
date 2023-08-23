using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Util
{
    public class TorchMetrics
    {
        public static float AbsoluteError(Tensor predicted, Tensor expected)
        {
            var ae = torch.nn.L1Loss(Reduction.Sum).forward(predicted, expected);
            return ae.ToSingle();
        }
        public static float MeanAbsoluteError(Tensor predicted, Tensor expected)
        {
            var ae = torch.nn.L1Loss().forward(predicted, expected);
            return ae.ToSingle();
        }

        public static float SquaredError(Tensor predicted, Tensor expected)
        {
            var ae = torch.nn.MSELoss(reduction:Reduction.Sum).forward(predicted, expected);
            return ae.ToSingle();
        }
        public static float MeanSquaredError(Tensor predicted, Tensor expected)
        {
            var ae = torch.nn.MSELoss().forward(predicted, expected);
            return ae.ToSingle();
        }

        public static float RootMeanSquaredError(Tensor predicted, Tensor expected)
        {
            var ae = torch.sqrt( torch.nn.MSELoss().forward(predicted, expected));
            return ae.ToSingle();
        }

        public static float MCAccuracy(Tensor predicted, Tensor expected)
        {
            var acc = predicted.argmax(1).eq(expected).sum().ToInt32() / (float)expected.shape.First();
            return acc;
        }
        public static float BIAccuracy(Tensor predicted, Tensor expected)
        {
            var acc = predicted.round().eq(expected).sum().ToInt32() / (float)expected.shape.First();
            return acc;
        }
        public static float MCError(Tensor predicted, Tensor expected)
        {
            var acc = predicted.argmax(1).eq(expected).sum().ToInt32() / (float)expected.shape.First();
            return 1.0f-acc;
        }
        public static float BIError(Tensor predicted, Tensor expected)
        {
            var acc = predicted.round().eq(expected).sum().ToInt32() / (float)expected.shape.First();
            return 1.0f-acc;
        }

        public static KeyValuePair<string, float> Evaluate(EvalFunction eval, Tensor predicted, Tensor target)
        {
            return eval switch
            {
                EvalFunction.AE => new KeyValuePair<string, float>(EvalFunction.CAcc.ToString(),AbsoluteError(predicted, target)),
                
                EvalFunction.MAE => new KeyValuePair<string, float>(EvalFunction.MAE.ToString(),MeanAbsoluteError(predicted, target)),
                
                EvalFunction.SE => new KeyValuePair<string, float>(EvalFunction.SE.ToString(),SquaredError(predicted, target)),
                
                EvalFunction.MSE => new KeyValuePair<string, float>(EvalFunction.MSE.ToString(),MeanSquaredError(predicted, target)),
                
                EvalFunction.RMSE => new KeyValuePair<string, float>(EvalFunction.RMSE.ToString(),RootMeanSquaredError(predicted, target)),
                
                EvalFunction.CAcc => new KeyValuePair<string, float>(EvalFunction.CAcc.ToString(),MCAccuracy(predicted, target)),
                
                EvalFunction.CErr => new KeyValuePair<string, float>(EvalFunction.CErr.ToString(),MCError(predicted, target)),
                
                EvalFunction.BAcc => new KeyValuePair<string, float>(EvalFunction.BAcc.ToString(),BIAccuracy(predicted, target)),
                
                EvalFunction.BErr => new KeyValuePair<string, float>(EvalFunction.BErr.ToString(),BIError(predicted, target)),
                
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        /*
         *
         * switch (eval)
             {
                 case EvalFunction.AE:
                     return new KeyValuePair<string,float>(EvalFunction.CAcc.ToString(), TorchMetrics.AbsoluteError(predicted, target))
                     metrics.Add(EvalFunction.CAcc.ToString(), TorchMetrics.AbsoluteError(predicted, target));
                     break;
                 case EvalFunction.MAE:
                     metrics.Add(EvalFunction.MAE.ToString(), TorchMetrics.MeanAbsoluteError(predicted, target));
                     break;
                 case EvalFunction.SE:
                     metrics.Add(EvalFunction.SE.ToString(), TorchMetrics.SquaredError(predicted, target));
                     break;
                 case EvalFunction.MSE:
                     metrics.Add(EvalFunction.MSE.ToString(), TorchMetrics.MeanSquaredError(predicted, target));
                     break;
                 case EvalFunction.RMSE:
                     metrics.Add(EvalFunction.RMSE.ToString(), TorchMetrics.RootMeanSquaredError(predicted, target));
                     break;
                 case EvalFunction.CAcc:
                     metrics.Add(EvalFunction.CAcc.ToString(), TorchMetrics.MCAccuracy(predicted, target));
                     break;
                 case EvalFunction.CErr:
                     metrics.Add(EvalFunction.CErr.ToString(), TorchMetrics.MCError(predicted, target));
                     break;
                 case EvalFunction.BAcc:
                     metrics.Add(EvalFunction.BAcc.ToString(), TorchMetrics.BIAccuracy(predicted, target));
                     break;
                 case EvalFunction.BErr:
                     metrics.Add(EvalFunction.BErr.ToString(), TorchMetrics.BIError(predicted, target));
                     break;
                 default:
                     throw new ArgumentOutOfRangeException();
             }
         */
    }
}
