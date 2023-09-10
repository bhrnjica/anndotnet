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
using Anndotnet.Core.MlMetrics;


namespace Anndotnet.Core.Interfaces
{
    public interface IPrintResults
    {
        void PrintPrediction(string prediction);
        void PrintRegressionPredictionVersusObserved(string predictionCount, string observedCount);
        void PrintRegressionMetrics(string name, RegressionMetrics metrics);
        void ConsolePrintConfusionMatrix(string name, ConfusionMatrix confusionMatrix, string[] labels);

        void PrintMultiClassClassificationMetrics(string name, ConfusionMatrix matrix);
        void PrintBinaryClassificationMetrics(string name, BinaryClassificationMetrics metrics);
    }
}
