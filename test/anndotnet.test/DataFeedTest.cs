using Daany;
using AnnDotNET.Common;
using NumSharp;
using NUnit.Framework;
using System;

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
            var df = new DataFeed(x, y, 88);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            foreach (var d in df.GetNextBatch())
            {
                Assert.IsTrue(d.xBatch[0][0].GetValue<float>() == batchIndex * df.BatchSize+1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex==2);
        }

        [Test]
        public void Test2()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y, 1);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            foreach (var d in df.GetNextBatch())
            {
                Assert.IsTrue(d.xBatch[0][0].GetValue<float>() == batchIndex * df.BatchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 150);
        }

        [Test]
        public void Test3()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y, 50);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            foreach (var d in df.GetNextBatch())
            {
                Assert.IsTrue(d.xBatch[0][0].GetValue<float>() == batchIndex * df.BatchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 3);
        }

        [Test]
        public void Test4()
        {
            (NDArray x, NDArray y) = PrepareData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y, 0);

            //enumerate data with minibatchsize
            int batchIndex = 0;
            foreach (var d in df.GetNextBatch())
            {
                Assert.IsTrue(d.xBatch[0][0].GetValue<float>() == batchIndex * df.BatchSize + 1);
                batchIndex++;
            }

            Assert.IsTrue(batchIndex == 1);
        }

        [Test]
        public void Test_Split_Data()
        {
            (NDArray x, NDArray y) = PrepareSimpleData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y, 0);

            var data = df.Split(20);
            var train = data.train.GetFullBatch();
            var test = data.validation.GetFullBatch();


            Assert.IsTrue(train.xBatch[0].GetValue<float>()== 1.1f);
            Assert.IsTrue(train.xBatch[1].GetValue<float>() == 2.2f);
            Assert.IsTrue(train.xBatch[2].GetValue<float>() == 3.3f);
            Assert.IsTrue(train.xBatch[3].GetValue<float>() == 4.4f);
            Assert.IsTrue(train.xBatch[4].GetValue<float>() == 5.5f);
            Assert.IsTrue(train.yBatch[5].GetValue<float>() == 61.6f);
            Assert.IsTrue(train.yBatch[7].GetValue<float>() == 81.8f);
        }

        [Test]
        public void Test_Split_Data_suffle()
        {
            (NDArray x, NDArray y) = PrepareSimpleData();

            //Create DataFeed with features and labels and minibatch size 
            var df = new DataFeed(x, y, 0);

            var data = df.Split(20,true,1);
            var train = data.train.GetFullBatch();
            var test = data.validation.GetFullBatch();


            Assert.IsTrue(train.xBatch[0].GetValue<float>() == 9.9f);
            Assert.IsTrue(train.xBatch[1].GetValue<float>() == 2.2f);
            Assert.IsTrue(train.xBatch[2].GetValue<float>() == 1.1f);
            Assert.IsTrue(train.xBatch[3].GetValue<float>() == 7.7f);
            Assert.IsTrue(train.xBatch[4].GetValue<float>() == 6.6f);
            Assert.IsTrue(test.yBatch[0].GetValue<float>() == 41.4f);
            Assert.IsTrue(test.yBatch[1].GetValue<float>() == 81.8f);
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
            var features = new string[] {"Id", "sepal_length", "sepal_width", "petal_length", "petal_width" };
            var label = "species";
            //
            return df.PrepareData(features, label);

        }
    }
}