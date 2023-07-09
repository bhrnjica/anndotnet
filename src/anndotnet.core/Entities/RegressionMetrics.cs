using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Interfaces;

namespace Anndotnet.Core.Entities
{
    public class RegressionMetrics:IMetrics
    {
        public void foo()
        {
            throw new NotImplementedException();
        }
        public float MeanAbsoluteError { get; }
        public float MeanSquaredError { get; }
        public float RootMeanSquaredError { get; }
        public float LossFunction { get; }
        public float RSquared { get; }
    }
}
