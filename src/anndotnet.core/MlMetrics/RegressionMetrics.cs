////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////
/// 
using Anndotnet.Core.Interfaces;
using Daany.MathStuff;

namespace Anndotnet.Core.MlMetrics;
public class RegressionMetrics : IMetrics
{
    public RegressionMetrics(IList<float> predicted, IList<float> target)
    {
        MAE = Daany.MathStuff.Stats.Metrics.MAE<float, float>(predicted,  target);
        MSE = Daany.MathStuff.Stats.Metrics.MSE<float, float>(predicted, target);
        RMSE = Daany.MathStuff.Stats.Metrics.RMSE<float, float>(predicted, target);
        R2 = Daany.MathStuff.Stats.Metrics.RSquared<float, float>(predicted,   target);
        NNSE = Daany.MathStuff.Stats.Metrics.NNSE<float, float>(predicted, target);
        SP = float.NaN;// Daany.MathStuff.Stats.Metrics.SP<float, float>(predicted,   target);
    }
    public float MAE { get; set; }
    public float MSE { get; set; }
    public float RMSE { get; set; }
    public float R2  { get; set; }
    public float NNSE { get; set; }

    public float SP { get; set; }

    public void foo()
    {
        throw new NotImplementedException();
    }
}
