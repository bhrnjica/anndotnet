using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Entities;
using ConfusionMatrix = Daany.MathStuff.Stats.ConfusionMatrix;

namespace Anndotnet.Core.Interfaces
{
    public interface IPrintResults
    {
        void PrintPrediction(string prediction);
        void PrintRegressionPredictionVersusObserved(string predictionCount, string observedCount);
        void PrintRegressionMetrics(string name, RegressionMetrics metrics);
        void ConsolePrintConfusionMatrix(ConfusionMatrix confusionMatrix, string[] labels);
    }
}
