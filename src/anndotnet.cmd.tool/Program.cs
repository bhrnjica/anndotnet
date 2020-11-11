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
using System.Threading.Tasks;
using Anndotnet.Vnd.Samples;

namespace AnnDotNET.Tool
{
   
    class Program
    {

        static async Task Main(string[] args)
        {
            var str = @"
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ******************ANNdotNET 2.0 - deep learning tool on .NET Framework.**********
    *******        Run Deep learning pre-calculated examples               **********
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Select the example to run:

                1. - Multi-class: Iris example from MLConfig file.
                2. - Multi-class: Iris example with data preparation task.
                3. - Multi-class: AirQuality example from MLConfig file.
                4. - Multi-class: AirQuality example with data preparation task.
                5. - Binary-class: Titanic example from MLCOnfig file.
                6. - Bnary-class: Titanic example with data preparation task.
                7. - Regression: Concrete slump test from MLConfig.
                8. - Regression: Concrete slump test from MLConfig.
                x. - Exit
                    ";
            Console.WriteLine(str);

            while (true)
            {
                var key = Console.ReadLine();
                await RunExample(key);
            }
            // await AirQualitiySample();
            // AirQualityFromMLConfig();

            

            await RunTitanicSample();
            TitanicMLConfig();
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

        private static  async Task RunExample(string key)
        {
            if(key=="1")
            {
                IrisFromMLConfig();
            }
            else if(key=="2")
            {
                await RunIrisSample();
            }
            else if (key == "3")
            {
                AirQualityFromMLConfig();
            }
            else if (key == "4")
            {
                await AirQualitiySample();
            }
            else if (key == "5")
            {
                TitanicMLConfig();
            }
            else if (key == "6")
            {
               await RunTitanicSample();
            }
            else if(key == "x")
            {
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Not recognized example. Please try again.");
            }
        }

        private static void TitanicMLConfig()
        {
            var mlCOnf = MLFactory.Load(@"..\..\..\..\\mlconfigs\titanic\titanic.mlconfig");
            mlCOnf.Wait();
            var mlConfig1 = mlCOnf.Result;
            //
            var mlRunner = new MLConfigRunner(mlConfig1);
            mlRunner.Run();
        }

        private static async Task RunTitanicSample()
        {
            TitanicSample titanic = new TitanicSample();
            (NDArray x, NDArray y) = await titanic.GenerateData();

            (TrainingParameters tParams, LearningParameters lParams) = titanic.GenerateParameters();
            var net = titanic.CreateNet();

            var r = new MLRunner(net, lParams, tParams, x, y, titanic.Metadata);
            r.Run();

           await r.SaveMlConfig(titanic.Metadata,"titanic.mlconifg");

        }

        private static async Task RunIrisSample()
        {
            IrisSample iris = new IrisSample();
           
            (NDArray x, NDArray y) = await iris.GenerateData(); 
            (TrainingParameters tParams, LearningParameters lParams) = iris.GenerateParameters();
            var net = iris.CreateNet();

            var r = new MLRunner(net, lParams, tParams, x, y,null);
            r.Run();
        }

        private static void IrisFromMLConfig()
        {
            var mlCOnf = MLFactory.Load(@"..\..\..\..\\mlconfigs\iris\iris.mlconfig");
            mlCOnf.Wait();
            var mlConfig1 = mlCOnf.Result;
            //
            var mlRunner = new MLConfigRunner(mlConfig1);
            mlRunner.Run();
        }

        private static async Task AirQualitiySample()
        {
            (NDArray x, NDArray y) = await AirQualitySample.generateAirQualityData();
            (TrainingParameters tParams, LearningParameters lParams) = AirQualitySample.generateParameters();
            var net = AirQualitySample.CreateNet();
            
            var r = new MLRunner(net, lParams, tParams, x, y, null);
            r.Run();
        }

        private static void AirQualityFromMLConfig()
        {
            var mlCOnf = MLFactory.Load(@"..\..\..\..\\mlconfigs\air_quality\airquality.mlconfig");
            mlCOnf.Wait();
            var mlConfig1 = mlCOnf.Result;
            //
            var mlRunner = new MLConfigRunner(mlConfig1);
            mlRunner.Run();
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
