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

namespace AnnDotNET.Tool
{
   
    class Program
    {

        static void Main(string[] args)
        {

            var mData = loadMetaData();
            var mlConfig = new MLConfig();
            mlConfig.Id = Guid.NewGuid();
            mlConfig.Parser = new DataParser();
            mlConfig.Metadata = mData;
            mlConfig.LParameters = new LearningParameters()

                    { EvaluationFunctions = new List<Metrics>() 
                    { Metrics.ClassificationAccuracy, Metrics.ClassificationError }, 

                    LossFunction = Losses.ClassificationCrossEntroy, 
                    LearnerType = LearnerType.AdamLearner, LearningRate = 0.01 };

            mlConfig.TParameters = new TrainingParameters();
            mlConfig.Network = new List<LayerBase>()
            {
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 15 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLuLayer", Activation=Activation.ReLU},
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 5 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLuLayer", Activation=Activation.Softmax},
            };
            mlConfig.Paths = new Dictionary<string, string>()
            {
                { "Training" ,"mlconfigs/airquality_rawdata.txt"}
            };

            MLFactory.Save(mlConfig, @"..\..\..\..\\mlconfigs\airquality.mlconfig").Wait();

            var mlCOnf = MLFactory.Load(@"..\..\..\..\\mlconfigs\airquality.mlconfig");
            mlCOnf.Wait();

            MLRunner.Run(mlCOnf.Result);

          //  MLRunner.Run("mlconfigs/titanic.mlconfig");

            //MLRunner.Run("mlconfigs/iris.mlconfig");

            //regressonModel_cv_training();
            //regressonModel();

            //binaryCassModel();

            //multiclassModel();
            

        }

        private static List<ColumnInfo> loadMetaData()
        {
            var metaData = new List<ColumnInfo>() { 

                //col1:
                new ColumnInfo()
                {
                    Id = 1,
                    Name = "Data",
                    MLType = MLColumnType.None,
                    ValueColumnType = ColType.DT,
                    ValueFormat = "mm/dd/yyy",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

                //col2:
                new ColumnInfo()
                {
                    Id = 2,
                    Name = "Time",
                    MLType = MLColumnType.None,
                    ValueColumnType = ColType.STR,
                    ValueFormat = "hh:mm:ss AM",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

                //col3
                new ColumnInfo()
                {
                    Id = 3,
                    Name = "month",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.IN,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.Ordinal,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

                //col4
                new ColumnInfo()
                {
                    Id = 4,
                    Name = "hour",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.IN,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.Ordinal,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

                //col5
                new ColumnInfo()
                {
                    Id = 5,
                    Name = "CO_GT",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

                //col6
                new ColumnInfo()
                {
                    Id = 6,
                    Name = "PT08_S1_CO",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col7
                new ColumnInfo()
                {
                    Id = 7,
                    Name = "Column07",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col8
                new ColumnInfo()
                {
                    Id = 8,
                    Name = "Column08",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col9
                new ColumnInfo()
                {
                    Id = 9,
                    Name = "Column09",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col10
                new ColumnInfo()
                {
                    Id = 10,
                    Name = "Column10",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col11
                new ColumnInfo()
                {
                    Id = 11,
                    Name = "Column11",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col12
                new ColumnInfo()
                {
                    Id = 12,
                    Name = "Column12",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col13
                new ColumnInfo()
                {
                    Id = 13,
                    Name = "Column13",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col14
                new ColumnInfo()
                {
                    Id = 14,
                    Name = "Column14",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col15
                new ColumnInfo()
                {
                    Id = 15,
                    Name = "Column15",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col16
                new ColumnInfo()
                {
                    Id = 16,
                    Name = "Column16",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                //col17
                new ColumnInfo()
                {
                    Id = 17,
                    Name = "Column17",
                    MLType = MLColumnType.Feature,
                    ValueColumnType = ColType.F32,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.None,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },
                ////col18
                //new ColumnInfo()
                //{
                //    Id = 18,
                //    Name = "Column18",
                //    MLType = MLColumnType.Feature,
                //    ValueColumnType = ColType.F32,
                //    ValueFormat = "",
                //    Encoding = CategoryEncoding.None,
                //    ClassValues = null,
                //    MissingValue = Aggregation.None

                //},
                ////col19
                //new ColumnInfo()
                //{
                //    Id = 19,
                //    Name = "Column19",
                //    MLType = MLColumnType.Feature,
                //    ValueColumnType = ColType.F32,
                //    ValueFormat = "",
                //    Encoding = CategoryEncoding.None,
                //    ClassValues = null,
                //    MissingValue = Aggregation.None

                //},
                ////col20
                //new ColumnInfo()
                //{
                //    Id = 20,
                //    Name = "Column20",
                //    MLType = MLColumnType.Feature,
                //    ValueColumnType = ColType.F32,
                //    ValueFormat = "",
                //    Encoding = CategoryEncoding.None,
                //    ClassValues = null,
                //    MissingValue = Aggregation.None

                //},
                //col21
                new ColumnInfo()
                {
                    Id = 21,
                    Name = "Column21",
                    MLType = MLColumnType.Label,
                    ValueColumnType = ColType.IN,
                    ValueFormat = "",
                    Encoding = CategoryEncoding.OneHot,
                    ClassValues = null,
                    MissingValue = Aggregation.None

                },

            };
            return metaData;
        }

        private static void LoadMLConfig()
        {
            

            

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
            var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
            tr.Run(x, y, lr, new TrainingParameters());

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
            var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
            tr.Run(x, y, lr, new TrainingParameters());

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
            var lr = learner.Create(y, z, new LearningParameters());

            //training process
            TVTrainer tr = new TVTrainer(xData, yData, 20);
            tr.Run(x, y, lr, new TrainingParameters() { MinibatchSize = 65 });

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
            var lr = learner.Create(y, z, new LearningParameters());

            //training process
            var tr = new CVTrainer(xData, yData, 5);
            tr.Run(x, y, lr, new TrainingParameters() { Epochs= 500, MinibatchSize = 65 });

            //evaluation


            //prediction
            return;
        }

        //public static void CVProgress(TrainingProgress tp)
        //{
        //    if (tp.ProgressType == ProgressType.Initialization)
        //        Console.WriteLine($"_________________________________________________________");

        //    Console.WriteLine($"Fold={tp.FoldIndex}, Iteration={tp.Iteration}, Loss={Math.Round(tp.TrainLoss, 3)}, Eval={Math.Round(tp.TrainEval, 3)}");

        //    if (tp.ProgressType == ProgressType.Completed)
        //        Console.WriteLine($"_________________________________________________________");
        //}
        //public static void Progress(TrainingProgress tp)
        //{
        //    if(tp.ProgressType== ProgressType.Initialization)
        //        Console.WriteLine($"_________________________________________________________");

        //    Console.WriteLine($"Iteration={tp.Iteration}, Loss={Math.Round(tp.TrainLoss, 3)}, Eval={Math.Round(tp.TrainEval, 3)}");

        //    if (tp.ProgressType == ProgressType.Completed)
        //        Console.WriteLine($"_________________________________________________________");
        //}

       


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
