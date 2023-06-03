using Anndotnet.Core;
using Anndotnet.Core.Data;
using Anndotnet.Core.Trainers;
using Anndotnet.Vnd;
using Daany;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tensorflow.NumPy;

namespace anndotnet.test
{
    public class DataFeedTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            int batchSize = 88;
            foreach (var d in df.GetNextBatch(batchSize))
            {
                Assert.IsTrue(d.xBatch[0][0] == batchIndex * batchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex==2);
        }

        [Test]
        public void Test2()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            int batchSize = 1;
            foreach (var d in df.GetNextBatch(batchSize))
            {
                Assert.IsTrue(d.xBatch[0][0] == batchIndex * batchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 150);
        }

        [Test]
        public void Test3()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            int batchSize = 50;
            foreach (var d in df.GetNextBatch(batchSize))
            {
                Assert.IsTrue(d.xBatch[0][0] == batchIndex * batchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 3);
        }

        [Test]
        public void Test4()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            int batchSize = 0;
            foreach (var d in df.GetNextBatch(batchSize))
            {
                Assert.IsTrue(d.xBatch[0][0]/*.GetValue<float>()*/ == batchIndex * batchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 1);
        }

        [Test]
        public void Test_Split_Data()
        {
            (NDArray x, NDArray y) = PrepareSimpleData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new TVTrainer(x, y, 20);

            var data = df.Split(1, false);
            var train = data.train.GetFullBatch();
            var test = data.validation.GetFullBatch();


            Assert.IsTrue(train.xBatch[0] == 1.1f);
            Assert.IsTrue(train.xBatch[1] == 2.2f);
            Assert.IsTrue(train.xBatch[2] == 3.3f);
            Assert.IsTrue(train.xBatch[3] == 4.4f);
            Assert.IsTrue(train.xBatch[4] == 5.5f);
            Assert.IsTrue(train.yBatch[5] == 61.6f);
            Assert.IsTrue(train.yBatch[7] == 81.8f);
        }

        [Test]
        public void Test_Split_Data_suffle()
        {
            (NDArray x, NDArray y) = PrepareSimpleData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new TVTrainer(x, y, 20);

            var data = df.Split(1, true);
            var train = data.train.GetFullBatch();
            var test = data.validation.GetFullBatch();


            Assert.IsTrue(train.xBatch[0] == 9.9f);
            Assert.IsTrue(train.xBatch[1] == 2.2f);
            Assert.IsTrue(train.xBatch[2] == 1.1f);
            Assert.IsTrue(train.xBatch[3] == 7.7f);
            Assert.IsTrue(train.xBatch[4] == 6.6f);
            Assert.IsTrue(test.yBatch[0] == 41.4f);
            Assert.IsTrue(test.yBatch[1] == 81.8f);
        }


        private (NDArray x, NDArray y) PrepareSimpleData()
        {
            var x = np.array(1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, 7.7f, 8.8f, 9.9f, 10.10f);
            var y = np.array(11.1f, 21.2f, 31.3f, 41.4f, 51.5f, 61.6f, 71.7f, 81.8f, 91.9f, 101.10f);

            return (x, y);
        }

        public static (NDArray, NDArray) PrepareData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("files/iris.txt", sep: '\t');

            //prepare the data
            var cols = new List<ColumnInfo>() 
            { 
                new ColumnInfo(){ Id=1, Name= "sepal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
                new ColumnInfo(){ Id=2, Name= "sepal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
                new ColumnInfo(){ Id=3, Name= "petal_length", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
                new ColumnInfo(){ Id=4, Name= "petal_width", MLType=MLColumnType.Feature, ValueColumnType= ColType.F32 },
                new ColumnInfo(){ Id=5, Name= "species", MLType=MLColumnType.Label, ValueColumnType= ColType.IN, Transformer= new DataTransformer(){ DataNormalization = ColumnTransformer.OneHot } },
            };
            var mlCOnfig = new MLConfig();
            //
            //return df.TransformData(mlConfig.Metadata);
            // var tran =  df.TransformData(mlCOnfig);
            throw new NotImplementedException();
          //  (NDArray xData, NDArray yData) = MLFactory.PrepareData(mlConfig, "Training");
        }
    }
}