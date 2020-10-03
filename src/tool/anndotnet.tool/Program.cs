using System;
using System.Linq;
using Daany;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
using Daany.Ext;
using System.Diagnostics;
using AnnDotNET.Common;
using ann.vdn;
using System.IO;

namespace AnnDotNET.Tool
{
   
    class Program
    {

        static void Main(string[] args)
        {
            //regressonModel();

            //binaryCassModel();

            multiclassModel();
            

        }

        private static void multiclassModel()
        {
            (var xData, var yData) = PrepareIrisData();

            var dFeed = new DataFeed(xData, yData, 50);


            Placeholders plcHolder = new Placeholders();
            (Tensor x, Tensor y) = plcHolder.Create(input: (-1, xData.Shape.Dimensions.Last()),
                                                    output: (-1, yData.Shape.Dimensions.Last()));

            //create network
            int outDim = y.shape.Last();
            NetworkModel model = new NetworkModel();
            Tensor z = model.Create(x, outDim);

            //define learner
            var learner = new ClassificationLearner();
            var lr = learner.Create(y, z, new TrainingParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 1);
            tr.Run(x, y, lr, new TrainingParameters() { Progress = Progress });

            //evaluation


            //prediction
            return;
        }

        private static void binaryCassModel()
        {
            //prepare the data
            (var xData, var yData) = PrepareTitanicData();

             
            //create place holders
            Placeholders plcHolder = new Placeholders();
            (Tensor x, Tensor y) = plcHolder.Create(input: (-1, xData.Shape.Dimensions.Last()),
                                                    output: (-1, yData.Shape.Dimensions.Last()));

            //create network
            int outDim = y.shape.Last();
            NetworkModel model = new NetworkModel();
            Tensor z = model.CreateLogisticRegression(x);

            //define learner
            var learner = new ClassificationLearner();
            var lr = learner.Create(y, z, new TrainingParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 1);
            tr.Run(x, y, lr, new TrainingParameters() { Progress = Progress });

            //evaluation


            //prediction


            return;
        }

        private static void regressonModel()
        {
            (var xData, var yData) = PrepareSlumpData();

             

            Placeholders plcHolder = new Placeholders();
            (Tensor x, Tensor y) = plcHolder.Create(input: (-1, xData.Shape.Dimensions.Last()),
                                                    output: (-1, yData.Shape.Dimensions.Last()));

            //create network
            int outDim = y.shape.Last();
            NetworkModel model = new NetworkModel();
            Tensor z = model.CreateSimpleRegression(x);

            //define learner
            var learner = new RegressionLearner();
            var lr = learner.Create(y, z, new TrainingParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 1);
            tr.Run(x, y, lr, new TrainingParameters() { Progress = Progress, MinibatchSize = 0 });

            //evaluation


            //prediction
            return;
        }

        public static void Progress(TrainingProgress tp)
        {
            if(tp.ProgressType== ProgressType.Initialization)
                Console.WriteLine($"_________________________________________________________");

            Console.WriteLine($"Iteration={tp.Iteration}, Loss={Math.Round(tp.Loss, 3)}, Eval={Math.Round(tp.Eval, 3)}");

            if (tp.ProgressType == ProgressType.Completed)
                Console.WriteLine($"_________________________________________________________");
        }

       


        public static (NDArray, NDArray) PrepareIrisData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("iris.txt", sep: '\t');

            //prepare the data
            var features = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width" };
            var label = "species";
            //
            return df.PrepareData(features, label);

        }

        public static (NDArray, NDArray) PrepareTitanicData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("titanic.txt", sep: ',');

            //prepare the data
            var features = new string[] { "Pclass", "Sex", "SibSp", "Parch" };
            var label = "Survived";
            //
            return df.PrepareData(features, label);

        }

        public static (NDArray, NDArray) PrepareSlumpData()
        {
            var coltypes = new ColType[] { ColType.I32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32 };
            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("slump.txt", sep: ';', colTypes:coltypes );

            //prepare the data
            var features = new string[] { "Cement", "Slag","Fly_ash", "Water", "SP", "Coarse_Aggr"};
            var label = "Strength";
            //
            var retVal =  df.PrepareData(features, label);

            return retVal;
        }

    }
}
