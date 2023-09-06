using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Interfaces;

namespace Anndotnet.Core.Entities
{
    public class RegressionMetrics : IMetrics
    {
        public RegressionMetrics(IList<float> predicted, IList<float> target)
        {
            LossFunction = 0;
            RootMeanSquaredError = Daany.MathStuff.Stats.Metrics.RMSE<float, float>(predicted, target);
            MeanAbsoluteError = Daany.MathStuff.Stats.Metrics.MSE<float, float>(predicted, target);
            MeanSquaredError = Daany.MathStuff.Stats.Metrics.MSE<float, float>(predicted, target);
            RSquared = Daany.MathStuff.Stats.Metrics.RSquared<float, float>(predicted, target);

        }
        public float MeanAbsoluteError { get; set; }
        public float MeanSquaredError { get; set; }
        public float RootMeanSquaredError { get; set; }
        public float LossFunction { get; set; }
        public float RSquared { get; set; }
        public void foo()
        {
            throw new NotImplementedException();
        }
    }
}
