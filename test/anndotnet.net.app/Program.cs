using anndotnet.core.app;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace anndotnet.test.net.app
{
    class Program
    {
        static void Main(string[] args)
        {
            //test for NALU implementation
            //MNIST_test.Run_MNIST_Test();

            ////run all defined samples
            //runAllml_configurations();

            ////run evaluation test 
            runEvaluationText();
        }

        private static void runAllml_configurations()
        {
            runExample("Iris Flower Identification",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.tool\\model_mlconfigs\\iris_mlconfig");

            runExample("Bezier Curve Machine Learning Demonstration",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.tool\\model_mlconfigs\\BCML_mlconfig");

            runExample("Predict Daily Sales for 10 items",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.tool\\model_mlconfigs\\daily_sales_mlconfig");

            runExample("Predict Solar Production",
                "C:\\Users\\bhrnjica\\OneDrive - BHRNJICA\\AI Projects\\ann-custom-models\\solar_production_mlconfig");

            runExample("Predict Future Sales",
                "C:\\Users\\bhrnjica\\OneDrive - BHRNJICA\\AI Projects\\ann-custom-models\\predict_future_sales_mlconfig");

            runExample("Titanic Survival",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\Titanic\\TitanicProject\\DNNModel_mlconfig");
            runExample("Mushroms",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\Mushroom\\MushroomProject\\TwoEmeddedLayers_mlconfig");

            runExample("Glass Identification",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\Glass\\GlassIdentificationProject\\FeedForwardWithRandomDSGeneraton_mlconfig");

            runExample("Concrete SLump Test",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\Concrete\\ConcreteSlumpProject\\FFNModel_mlconfig");

            runExample("Breast Cancer FF config",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\BreastC\\BreastCancerProject\\FeedForwardModel_mlconfig");

            runExample("Breast Cancer Emb config",
                "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\BreastC\\BreastCancerProject\\CategoryEmbeddingModel_mlconfig");

            Console.WriteLine("Press Any Key To Continue.....");
            Console.Read();
        }

        private static void runExample(string title, string mlConfigPath)
        {
            var mlConfigFile2 = mlConfigPath;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"****{title}****");
            Console.WriteLine(Environment.NewLine);
            var token2 = new CancellationToken();
            MachineLearning.Run(mlConfigFile2, DeviceDescriptor.UseDefaultDevice(), token2, trainingProgress, null);
        }

        static void trainingProgress(ProgressData progress)
        {
            //
            Console.WriteLine($"Epoch={progress.EpochCurrent} of {progress.EpochTotal};\t Evaluation of {progress.EvaluationFunName}=" +
                $"(TrainMB = {progress.MinibatchAverageEval},TrainFull = {progress.TrainEval}, Valid = {progress.ValidationEval})");
        }

        private static void runEvaluationText()
        {
            var modelFile = "../../../data/model_to_evaluate.model";
            var inputRow = new List<float>() { 30f, 0f, 106.425f, 1f, 0f, 0f, 1f, 0f };

            var result = MLEvaluator.TestModel(modelFile, inputRow.ToArray(), DeviceDescriptor.UseDefaultDevice());


        }
    }
}
