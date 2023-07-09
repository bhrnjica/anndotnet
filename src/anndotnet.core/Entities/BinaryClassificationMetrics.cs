using Daany.MathStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Entities
{
    public class BinaryClassificationMetrics
    {
        
        public string Accuracy { get; set; }
        public string AreaUnderRocCurve { get; set; }
        public string AreaUnderPrecisionRecallCurve { get; set; }
        public string F1Score { get; set; }
        public string LogLoss { get; set; }
        public string LogLossReduction { get; set; }
        public string PositivePrecision { get; set; }
        public string PositiveRecall { get; set; }
        public string NegativePrecision { get; set; }
        public string NegativeRecall { get; set; }


        public ConfusionMatrix ConfusionMatrix { get; }

        public BinaryClassificationMetrics(ConfusionMatrix cm)
        {
            Accuracy = ConfusionMatrix.AAC(cm.Matrix).ToString("0.00");
            F1Score = ConfusionMatrix.Fscore(cm.Matrix).ToString("0.00");
        }

        private double calculateAUC(ConfusionMatrix[] cm)
        {
            var coll = cm;
            double area = 0.0;
            double h = 0.0;
            //
            for (int i = 0; i < coll.Length - 1; i++)
            {
                var cm0 = coll[i];
                var cm1 = coll[i + 1];
                var fpr0 = 1.0 - ConfusionMatrix.Specificity(cm0.Matrix, 0);
                var tpr0 = ConfusionMatrix.Recall(cm0.Matrix, 1);//Recall
                var fpr1 = 1.0 - ConfusionMatrix.Specificity(cm1.Matrix, 0);
                var tpr1 = ConfusionMatrix.Recall(cm1.Matrix, 1);//Recall

                //calc average y
                h = (tpr0 + tpr1) / 2.0;

                var temp = h * Math.Abs(fpr1 - fpr0);

                area += temp;
            }


            if (area < 0.5)
                area = 1.0 - area;

            return area;
        }

    }
}
