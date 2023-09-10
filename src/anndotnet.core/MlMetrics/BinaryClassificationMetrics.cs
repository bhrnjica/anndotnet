////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Daany.MathStuff.Stats;

namespace Anndotnet.Core.MlMetrics;

public class BinaryClassificationMetrics
{

    public ConfusionMatrix ConfusionMatrix { get; }
    public string[]        Classes         { get; }


    public BinaryClassificationMetrics(IList<float> predicted, IList<float> target, String[] classes)
    {
        ConfusionMatrix = new ConfusionMatrix(predicted.Select(x => Convert.ToInt32(x)).ToArray(),
            target.Select(x => Convert.ToInt32(x)).ToArray(), 2);

        //
        Classes = classes;
        TP = ConfusionMatrix.Matrix[1][1];
        FP = ConfusionMatrix.Matrix[0][1];
        FN = ConfusionMatrix.Matrix[1][0];
        TN = ConfusionMatrix.Matrix[0][0];

        Acc = (float)Math.Round(ConfusionMatrix.OAC(ConfusionMatrix.Matrix),                3);
        Precision = (float)Math.Round(ConfusionMatrix.Precision(ConfusionMatrix.Matrix, 1), 3);
        Recall = (float)Math.Round(ConfusionMatrix.Recall(ConfusionMatrix.Matrix, 1),       3);
        F1Score = (float)Math.Round(ConfusionMatrix.Fscore(ConfusionMatrix.Matrix, 1),      3);

        HSS = (float)Math.Round(ConfusionMatrix.HSS(ConfusionMatrix.Matrix, target.Count), 3);
        PSS = (float)Math.Round(ConfusionMatrix.PSS(ConfusionMatrix.Matrix, target.Count), 3);
        Auc = (float)Math.Round(CalculateAuc(predicted, target.Select(x=>(int)x).ToArray()), 3);
    }


    public float Auc { get; }

    public float PSS { get; }

    public float HSS { get; }
    public float F1Score { get; }

    public float Recall { get; }

    public float Precision { get; }

    public float Acc { get; }

    public int TN { get; }

    public int FN { get; }

    public int FP { get; }

    public int TP { get;}


    public BinaryClassificationMetrics(ConfusionMatrix cm)
    {
        ConfusionMatrix = cm;

    }

    static float CalculateAuc(IList<float> predictedProbabilities, IList<int> trueLabels)
    {
        // Sort the predicted probabilities and true labels by predicted probabilities in descending order
        var sortedData = predictedProbabilities.Zip(trueLabels, (p, l) => new { Probability = p, Label = l })
                                               .OrderByDescending(x => x.Probability)
                                               .ToList();

        int nPositive = sortedData.Count(x => x.Label == 1);
        int nNegative = sortedData.Count(x => x.Label == 0);

        float auc = 0.0f;
        float xSum = 0.0f;
        float ySum = 0.0f;
        float lastX = 0.0f;

        foreach (var point in sortedData)
        {
            if (point.Label == 1)
            {
                ySum += 1.0f;
            }
            else
            {
                xSum += 1.0f;
                auc += ySum * (point.Probability - lastX);
                lastX = point.Probability;
            }
        }

        auc /= (nPositive * nNegative);

        return auc;
    }

    public static Dictionary<double, Tuple<double, double>> CalculateRocCurve(double[][] oneHotActual, double[][] oneHotPredicted)
    {
        Dictionary<double, Tuple<double, double>> rocCurve = new Dictionary<double, Tuple<double, double>>();

        for (double threshold = 0.0; threshold <= 1.0; threshold += 0.01)
        {
            int truePositive = 0;
            int falsePositive = 0;
            int trueNegative = 0;
            int falseNegative = 0;

            for (int i = 0; i < oneHotActual.Length; i++)
            {
                int actualClass = oneHotActual[i].MaxArg();
                int predictedClass = oneHotPredicted[i].MaxArg();

                if (oneHotActual[i][actualClass] >= threshold)
                {
                    if (actualClass == predictedClass)
                        truePositive++;
                    else
                        falsePositive++;
                }
                else
                {
                    if (actualClass == predictedClass)
                        falseNegative++;
                    else
                        trueNegative++;
                }
            }

            double truePositiveRate = (double)truePositive / (truePositive + falseNegative);
            double falsePositiveRate = (double)falsePositive / (falsePositive + trueNegative);
            rocCurve.Add(threshold, new Tuple<double, double>(truePositiveRate, falsePositiveRate));
        }

        return rocCurve;
    }

    public static double CalculateAuc(Dictionary<double, Tuple<double, double>> rocCurve)
    {
        var sortedPoints = rocCurve.OrderBy(p => p.Value.Item2).ToArray();
        double auc = 0.0;
        double prevFPR = 0.0;
        double prevTPR = 0.0;

        foreach (var point in sortedPoints)
        {
            double fpr = point.Value.Item2;
            double tpr = point.Value.Item1;

            auc += (fpr - prevFPR) * (tpr + prevTPR) / 2.0;

            prevFPR = fpr;
            prevTPR = tpr;
        }

        return auc;
    }


}
