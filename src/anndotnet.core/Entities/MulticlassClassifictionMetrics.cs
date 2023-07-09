using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Entities
{
    public class MulticlassClassifictionMetrics
    {
        public double LogLoss { get; }
        public double LogLossReduction { get; private set; }
        public double MacroAccuracy { get; }
        public double MicroAccuracy { get; }
        public double TopKAccuracy { get; }
        public int TopKPredictionCount { get; }
        public IReadOnlyList<double> TopKAccuracyForAllK { get; }
        public IReadOnlyList<double> PerClassLogLoss { get; }
        public ConfusionMatrix ConfusionMatrix { get; }
    }
}
