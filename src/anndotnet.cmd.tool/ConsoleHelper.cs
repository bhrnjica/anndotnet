using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.ML.Data;
using Microsoft.ML;
using System.Diagnostics;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Entities;
using BinaryClassificationMetrics = Anndotnet.Core.Entities.BinaryClassificationMetrics;
using ConfusionMatrix = Daany.MathStuff.Stats.ConfusionMatrix;
using System.Diagnostics.Contracts;
using System.Text;

namespace AnnDotNet.Tool
{
    /// <summary>
    /// This class is taken from ML.NET (github.com/dotnet/machinelearning)
    /// </summary>
    public class ConsoleHelper:IPrintResults
    {
        public void PrintPrediction(string prediction)
        {
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"Predicted : {prediction}");
            Console.WriteLine($"*************************************************");
        }

        public void PrintRegressionPredictionVersusObserved(string predictionCount, string observedCount)
        {
            Console.WriteLine($"-------------------------------------------------");
            Console.WriteLine($"Predicted : {predictionCount}");
            Console.WriteLine($"Actual:     {observedCount}");
            Console.WriteLine($"-------------------------------------------------");
        }

        public void PrintRegressionMetrics(string name, Anndotnet.Core.Entities.RegressionMetrics metrics)
        {
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Metrics for {name} regression model      ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine($"*************************************************");
        }

        public void PrintBinaryClassificationMetrics(string name, BinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*       Metrics for {name} binary classification model      ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"*       Area Under Curve:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"*       Area under Precision recall Curve:  {metrics.AreaUnderPrecisionRecallCurve:P2}");
            Console.WriteLine($"*       F1Score:  {metrics.F1Score:P2}");
            Console.WriteLine($"*       LogLoss:  {metrics.LogLoss:#.##}");
            Console.WriteLine($"*       LogLossReduction:  {metrics.LogLossReduction:#.##}");
            Console.WriteLine($"*       PositivePrecision:  {metrics.PositivePrecision:#.##}");
            Console.WriteLine($"*       PositiveRecall:  {metrics.PositiveRecall:#.##}");
            Console.WriteLine($"*       NegativePrecision:  {metrics.NegativePrecision:#.##}");
            Console.WriteLine($"*       NegativeRecall:  {metrics.NegativeRecall:P2}");
            Console.WriteLine($"************************************************************");
        }

        //public static void PrintAnomalyDetectionMetrics(string name, AnomalyDetectionMetrics metrics)
        //{
        //    Console.WriteLine($"************************************************************");
        //    Console.WriteLine($"*       Metrics for {name} anomaly detection model      ");
        //    Console.WriteLine($"*-----------------------------------------------------------");
        //    Console.WriteLine($"*       Area Under ROC Curve:                       {metrics.AreaUnderRocCurve:P2}");
        //    Console.WriteLine($"*       Detection rate at false positive count: {metrics.DetectionRateAtFalsePositiveCount}");
        //    Console.WriteLine($"************************************************************");
        //}

        public void PrintMultiClassClassificationMetrics(string name, MulticlassClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*    Metrics for {name} multi-class classification model   ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"    AccuracyMacro = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    AccuracyMicro = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
            Console.WriteLine($"    LogLoss for class 1 = {metrics.PerClassLogLoss[0]:0.####}, the closer to 0, the better");
            Console.WriteLine($"    LogLoss for class 2 = {metrics.PerClassLogLoss[1]:0.####}, the closer to 0, the better");
            if (metrics.PerClassLogLoss.Count > 2)
                Console.WriteLine($"    LogLoss for class 3 = {metrics.PerClassLogLoss[2]:0.####}, the closer to 0, the better");
            Console.WriteLine($"************************************************************");
        }

        public void PrintRegressionFoldsAverageMetrics(string algorithmName, IReadOnlyList<CrossValidationResult<Microsoft.ML.Data.RegressionMetrics>> crossValidationResults)
        {
            var L1 = crossValidationResults.Select(r => r.Metrics.MeanAbsoluteError);
            var L2 = crossValidationResults.Select(r => r.Metrics.MeanSquaredError);
            var RMS = crossValidationResults.Select(r => r.Metrics.RootMeanSquaredError);
            var lossFunction = crossValidationResults.Select(r => r.Metrics.LossFunction);
            var R2 = crossValidationResults.Select(r => r.Metrics.RSquared);

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for {algorithmName} Regression model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average L1 Loss:    {L1.Average():0.###} ");
            Console.WriteLine($"*       Average L2 Loss:    {L2.Average():0.###}  ");
            Console.WriteLine($"*       Average RMS:          {RMS.Average():0.###}  ");
            Console.WriteLine($"*       Average Loss Function: {lossFunction.Average():0.###}  ");
            Console.WriteLine($"*       Average R-squared: {R2.Average():0.###}  ");
            Console.WriteLine($"*************************************************************************************************************");
        }

        public void PrintMulticlassClassificationFoldsAverageMetrics(
                                         string algorithmName,
                                       IReadOnlyList<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults
                                                                           )
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
            var microAccuracyAverage = microAccuracyValues.Average();
            var microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues);
            var microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues);

            var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
            var macroAccuracyAverage = macroAccuracyValues.Average();
            var macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues);
            var macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues);

            var logLossValues = metricsInMultipleFolds.Select(m => m.LogLoss);
            var logLossAverage = logLossValues.Average();
            var logLossStdDeviation = CalculateStandardDeviation(logLossValues);
            var logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues);

            var logLossReductionValues = metricsInMultipleFolds.Select(m => m.LogLossReduction);
            var logLossReductionAverage = logLossReductionValues.Average();
            var logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues);
            var logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues);

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for {algorithmName} Multi-class Classification model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})");
            Console.WriteLine($"*************************************************************************************************************");

        }

        public double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }

        public void PrintClusteringMetrics(string name, ClusteringMetrics metrics)
        {
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Metrics for {name} clustering model      ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       Average Distance: {metrics.AverageDistance}");
            Console.WriteLine($"*       Davies Bouldin Index is: {metrics.DaviesBouldinIndex}");
            Console.WriteLine($"*************************************************");
        }

       
        public void ConsolePrintConfusionMatrix(ConfusionMatrix confusionMatrix, string[] labels)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ");
            string cm = GetFormattedConfusionTable(confusionMatrix, labels);
            Console.Write(cm);

            Console.ForegroundColor = defaultColor;
        }

        private string GetFormattedConfusionTable(ConfusionMatrix confusionMatrix, string[] labels)
        {
            string prefix = confusionMatrix.IsWeighted ? "Weighted " : "";
            int numLabels = confusionMatrix?.Matrix == null ? 0 : confusionMatrix.Matrix.Length;

            int colWidth = numLabels == 2 ? 8 : 5;
            int maxNameLen = labels.Max(name => name.Length);

            // If the names are too long to fit in the column header, we back off to using class indices
            // in the header. This will also require putting the indices in the row, but it's better than
            // the alternative of having ambiguous abbreviated column headers, or having a table potentially
            // too wide to fit in a console.
            bool useNumbersInHeader = maxNameLen > colWidth;

            int rowLabelLen = maxNameLen;
            int rowDigitLen = 0;
            if (useNumbersInHeader)
            {
                // The row label will also include the index, so a user can easily match against the header.
                // In such a case, a label like "Foo" would be presented as something like "5. Foo".
                rowDigitLen = Math.Max(labels.Length - 1, 0).ToString().Length;

                rowLabelLen += rowDigitLen + 2;
            }


            // The "PREDICTED" in the table, at length 9, dictates the amount of additional padding that will
            // be necessary on account of label names.
            int paddingLen = Math.Max(9, rowLabelLen);
            string pad = new string(' ', paddingLen - 9);
            string rowLabelFormat = null;
            if (useNumbersInHeader)
            {
                int namePadLen = paddingLen - (rowDigitLen + 2);
                rowLabelFormat = string.Format("{{0,{0}}}. {{1,{1}}} ||", rowDigitLen, namePadLen);
            }
            else
                rowLabelFormat = string.Format("{{1,{0}}} ||", paddingLen);

            var confusionTable = confusionMatrix.Matrix;
            var sb = new StringBuilder();
            if (numLabels == 2 && confusionMatrix.IsBinary)
            {
                var positiveCaps = labels[0].ToString().ToUpper();

                var numTruePos = confusionTable[0][0];
                var numFalseNeg = confusionTable[0][1];
                var numTrueNeg = confusionTable[1][1];
                var numFalsePos = confusionTable[1][0];

                sb.AppendFormat("{0}TEST {1} RATIO:\t{2:N4} ({3:F1}/({3:F1}+{4:F1}))", prefix, positiveCaps,
                    1.0 * (numTruePos + numFalseNeg) / (numTruePos + numTrueNeg + numFalseNeg + numFalsePos),
                    numTruePos + numFalseNeg, numFalsePos + numTrueNeg);
            }

            sb.AppendLine();
            sb.AppendFormat("{0}Confusion table", prefix);
            if (confusionMatrix.IsSampled)
                sb.AppendLine(" (sampled)");
            else
                sb.AppendLine();

            sb.AppendFormat("          {0}||", pad);
            for (int i = 0; i < numLabels; i++)
                sb.Append(numLabels > 2 ? "========" : "===========");
            sb.AppendLine();
            sb.AppendFormat("PREDICTED {0}||", pad);
            string format = string.Format(" {{{0},{1}}} |", useNumbersInHeader ? 0 : 1, colWidth);
            for (int i = 0; i < numLabels; i++)
                sb.AppendFormat(format, i, labels[i]);
            sb.AppendLine(" Recall");
            sb.AppendFormat("TRUTH     {0}||", pad);
            for (int i = 0; i < numLabels; i++)
                sb.Append(numLabels > 2 ? "========" : "===========");

            sb.AppendLine();

            string format2 = string.Format(" {{0,{0}:{1}}} |", colWidth,
                string.IsNullOrWhiteSpace(prefix) ? "N0" : "F1");
            for (int i = 0; i < numLabels; i++)
            {
                sb.AppendFormat(rowLabelFormat, i, labels[i]);
                for (int j = 0; j < numLabels; j++)
                    sb.AppendFormat(format2, confusionTable[i][j]);

                sb.AppendFormat(" {0,5:F4}", ConfusionMatrix.Recall(confusionMatrix.Matrix, i)); //confusionMatrix.PerClassRecall[i]);
                sb.AppendLine();
            }
            sb.AppendFormat("          {0}||", pad);
            for (int i = 0; i < numLabels; i++)
                sb.Append(numLabels > 2 ? "========" : "===========");
            sb.AppendLine();
            sb.AppendFormat("Precision {0}||", pad);

            format = string.Format("{{0,{0}:N4}} |", colWidth + 1);
            for (int i = 0; i < numLabels; i++)
                sb.AppendFormat(format, ConfusionMatrix.Precision(confusionMatrix.Matrix, i)); //confusionMatrix.PerClassPrecision[i]);

            sb.AppendLine();
            return sb.ToString();
        }


        public void ConsoleWriteHeader(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" ");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            var maxLength = lines.Select(x => x.Length).Max();
            Console.WriteLine(new string('#', maxLength));
            Console.ForegroundColor = defaultColor;
        }

        public void ConsoleWriterSection(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" ");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            var maxLength = lines.Select(x => x.Length).Max();
            Console.WriteLine(new string('-', maxLength));
            Console.ForegroundColor = defaultColor;
        }

        public void ConsolePressAnyKey()
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ");
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }

        public void ConsoleWriteException(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            const string exceptionTitle = "EXCEPTION";
            Console.WriteLine(" ");
            Console.WriteLine(exceptionTitle);
            Console.WriteLine(new string('#', exceptionTitle.Length));
            Console.ForegroundColor = defaultColor;
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public void ConsoleWriteWarning(params string[] lines)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            const string warningTitle = "WARNING";
            Console.WriteLine(" ");
            Console.WriteLine(warningTitle);
            Console.WriteLine(new string('#', warningTitle.Length));
            Console.ForegroundColor = defaultColor;
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
