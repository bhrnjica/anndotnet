using System;
using System.Linq;
using Daany;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
using Daany.Ext;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Anndotnet.Core;
using Anndotnet.Vnd;
using Anndotnet.Core.Data;
using Anndotnet.Core.Learners;
using Anndotnet.Core.Trainers;
using Anndotnet.Core.Entities;
using Anndotnet.cmd.tool;
using System.Threading.Tasks;

namespace AnnDotNET.Tool
{
   
    class Program
    {

        static async Task Main(string[] args)
        {
            await AirQualitiySample();

            return;

            var mlCOnf = MLFactory.Load(@"..\..\..\..\\mlconfigs\iris.mlconfig");
            mlCOnf.Wait();
            var mlConfig1 = mlCOnf.Result;

            //
            tf.set_random_seed(888888);
            var mlRunner = new MLConfigRunner(mlConfig1);
            mlRunner.Run();

            //  MLRunner.Run("mlconfigs/titanic.mlconfig");

            //MLRunner.Run("mlconfigs/iris.mlconfig");

            //regressonModel_cv_training();
            //regressonModel();

            //binaryCassModel();

            //multiclassModel();


        }

        private static async Task AirQualitiySample()
        {
            (NDArray x, NDArray y) = await AirQualitySample.generateAirQualityData();
            (TrainingParameters tParams, LearningParameters lParams) = AirQualitySample.generateParameters();
            var net = AirQualitySample.CreateNet();
            
            var r = new MLRunner(net, lParams, tParams, x, y);
            r.Run();
        }

     
        private static void multiclassModel()
        {
            (var xData, var yData) = PrepareIrisData();

            var dFeed = new DataFeed(xData, yData);


            Placeholders plcHolder = new Placeholders();
            (Tensor x, Tensor y) = plcHolder.Create(input: (-1, xData.Shape.Dimensions.Last()),
                                                    output: (-1, yData.Shape.Dimensions.Last()));

            //create network
            int outDim = y.shape.Last();
            NetworkModel model = new NetworkModel();
            Tensor z = model.Create(x, outDim);

            //define learner
            var learner = new ClassificationLearner();
          //  var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
           // tr.Run(x, y, lr, new TrainingParameters(),new TrainingHistory(), new Dictionary<string, string>());

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
           // var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
           // tr.Run(x, y, lr, new TrainingParameters(),new TrainingHistory(), new Dictionary<string, string>());

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
            //var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
           // tr.Run(x, y, lr, new TrainingParameters() { MinibatchSize = 65 }, new TrainingHistory(), new Dictionary<string, string>());

            //evaluation


            //prediction
            return;
        }

        private static void regressonModel_cv_training()
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
           // var lr = learner.Create(y, z, new LearningParameters());

            //training process
            var tr = new CVTrainer(xData, yData, 5);
            //tr.Run(x, y, lr, new TrainingParameters() { Epochs= 500, MinibatchSize = 65 }, new TrainingHistory(), new Dictionary<string, string>());

            //evaluation


            //prediction
            return;
        }

       

        public static (NDArray, NDArray) PrepareIrisData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("iris.txt", sep: '\t');

            //prepare the data
            var features = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width" };
            var label = "species";
            //
            new NotImplementedException();
            return (null,null);

        }

        public static (NDArray, NDArray) PrepareTitanicData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("titanic.txt", sep: ',');

            //prepare the data
            var features = new string[] { "Pclass", "Sex", "SibSp", "Parch" };
            var label = "Survived";
            //
            new NotImplementedException();
            return (null, null);

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
            new NotImplementedException();

            return (null, null);
        }

    }
}
