using Daany;
using AnnDotNET.Common;
using NumSharp;
using NUnit.Framework;

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