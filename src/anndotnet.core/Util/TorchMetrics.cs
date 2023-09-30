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
    }
}
