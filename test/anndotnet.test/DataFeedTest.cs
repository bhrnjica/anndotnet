using AnnDotNet.Core;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Trainers;
using AnnDotNet.Vnd;
using Daany;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.NumPy;

namespace AnnDotNet.test;

public class DataFeedTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Test_1D_Minibatching()
    {
        (NDArray x, NDArray y) = TestDataProvider.Prepare1DData();

        //Create DataFeed with features and labels and minibatch size 
        var df = new DataFeed(x, y);

        //enumerate data with minibatchsize
        int batchIndex = 0;
        int batchSize = 4;
        foreach (var d in df.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.IsTrue(d.xBatch.dims[0] == 4);

                Assert.AreEqual((float)d.xBatch[0], 1.1f, 0.01f);
                Assert.AreEqual((float)d.xBatch[1], 2.2f, 0.01f);
                Assert.AreEqual((float)d.xBatch[2], 3.3f, 0.01f);
                Assert.AreEqual((float)d.xBatch[3], 4.4f, 0.01f);

                Assert.AreEqual((float)d.yBatch[0], 11.1f, 0.01f);
                Assert.AreEqual((float)d.yBatch[1], 21.2f, 0.01f);
                Assert.AreEqual((float)d.yBatch[2], 31.3f, 0.01f);
                Assert.AreEqual((float)d.yBatch[3], 41.4f, 0.01f);
            }
            else if (batchIndex == 1)
            {
                Assert.IsTrue(d.xBatch.dims[0] == 4);

                Assert.AreEqual((float)d.xBatch[0], 5.5f, 0.01f);
                Assert.AreEqual((float)d.xBatch[1], 6.6f, 0.01f);
                Assert.AreEqual((float)d.xBatch[2], 7.7f, 0.01f);
                Assert.AreEqual((float)d.xBatch[3], 8.8f, 0.01f);

                Assert.AreEqual((float)d.yBatch[0], 51.5f, 0.01f);
                Assert.AreEqual((float)d.yBatch[1], 61.6f, 0.01f);
                Assert.AreEqual((float)d.yBatch[2], 71.7f, 0.01f);
                Assert.AreEqual((float)d.yBatch[3], 81.8f, 0.01f);
            }
            else
            {
                Assert.IsTrue(d.xBatch.dims[0] == 2);

                Assert.AreEqual((float)d.xBatch[0],  9.9f, 0.01f);
                Assert.AreEqual((float)d.xBatch[1], 10.10f, 0.01f);

                Assert.AreEqual((float)d.yBatch[0],  91.90f, 0.01f);
                Assert.AreEqual((float)d.yBatch[1], 101.10f, 0.01f);
            }    
            batchIndex++;
        }

    }

    [Test]
    public void Test_2D_Minibatching()
    {
        // arrange
        var (data, labels) = TestDataProvider.Prepare2DData();
        var batchSize = 4;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];
           
        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        { 
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 1.0f);
                Assert.AreEqual((float)d.xBatch[0, 1], 11.0f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 4.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 14.0f);


                Assert.AreEqual((float)d.yBatch[0], 21.0f);
                Assert.AreEqual((float)d.yBatch[batchSize - 1], 24.0f);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.0f);
                Assert.AreEqual((float)d.xBatch[0, 1], 15.0f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 8.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 18.0f);


                Assert.AreEqual((float)d.yBatch[0], 25.0f);
                Assert.AreEqual((float)d.yBatch[batchSize - 1], 28.0f);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[0], 2);

                Assert.AreEqual((float)d.xBatch[0, 0], 9.0f);
                Assert.AreEqual((float)d.xBatch[2 - 1, 1], 20.0f);

                Assert.AreEqual((float)d.yBatch[0], 29.0f);
                Assert.AreEqual((float)d.yBatch[2 - 1], 30.0f);
            }

            batchIndex++;
        }
    }

    [Test]
    public void Test_2D_Minibatching_One_Hot_Encoding()
    {
        // arrange
        var (data, labels) = TestDataProvider.PrepareIrisData();
        var batchSize = 70;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 6.5f);
                Assert.AreEqual((float)d.xBatch[0, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[0, 2], 5.5f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.8f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1,0], 6.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1,1], 3.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1,2], 4.9f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1,3], 1.8f);

                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[0, 1], 0);
                Assert.AreEqual((float)d.yBatch[0, 2], 0);

                Assert.AreEqual((float)d.yBatch[batchSize - 1, 0], 1);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 1], 0);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 2], 0);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.9f);
                Assert.AreEqual((float)d.xBatch[0, 2], 4.2f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.3f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 5.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 3.3f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 2], 1.7f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 3], 0.5f);

                Assert.AreEqual((float)d.yBatch[0, 0], 0);
                Assert.AreEqual((float)d.yBatch[0, 1], 1);
                Assert.AreEqual((float)d.yBatch[0, 2], 0);

                Assert.AreEqual((float)d.yBatch[batchSize - 1, 0], 0);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 1], 0);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 2], 1);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[0], 10);

                Assert.AreEqual((float)d.xBatch[0, 0], 7.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.6f);
                Assert.AreEqual((float)d.xBatch[0, 2], 6.9f);
                Assert.AreEqual((float)d.xBatch[0, 3], 2.3f);

                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 2], 1.4f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 3], 0.1f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 0], 4.8f);

                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[0, 1], 0);
                Assert.AreEqual((float)d.yBatch[0, 2], 0);

                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1, 0], 0);
                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1, 1], 0);
                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1, 2], 1);
            }

            batchIndex++;
        }
    }

    [Test]
    public void Test_2D_Minibatching_Ordinal()
    {
        // arrange
        var (data, labels) = TestDataProvider.PrepareIrisDataOrdinal();
        var batchSize = 70;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 6.5f);
                Assert.AreEqual((float)d.xBatch[0, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[0, 2], 5.5f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.8f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 6.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 2], 4.9f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 3], 1.8f);

                Assert.AreEqual((float)d.yBatch[0], 1);

                Assert.AreEqual((float)d.yBatch[batchSize - 1], 1);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.9f);
                Assert.AreEqual((float)d.xBatch[0, 2], 4.2f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.3f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 5.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 3.3f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 2], 1.7f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 3], 0.5f);

                Assert.AreEqual((float)d.yBatch[0], 2);

                Assert.AreEqual((float)d.yBatch[batchSize - 1], 3);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[0], 10);

                Assert.AreEqual((float)d.xBatch[0, 0], 7.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.6f);
                Assert.AreEqual((float)d.xBatch[0, 2], 6.9f);
                Assert.AreEqual((float)d.xBatch[0, 3], 2.3f);

                Assert.AreEqual((float)d.xBatch[(int)d.xBatch.shape[0] - 1, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[(int)d.xBatch.shape[0] - 1, 2], 1.4f);
                Assert.AreEqual((float)d.xBatch[(int)d.xBatch.shape[0] - 1, 3], 0.1f);
                Assert.AreEqual((float)d.xBatch[(int)d.xBatch.shape[0] - 1, 0], 4.8f);

                Assert.AreEqual((float)d.yBatch[0], 1);

                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1], 3);
            }

            batchIndex++;
        }
    }


    [Test]
    public void Test_2D_Minibatching_Dummy()
    {
        // arrange
        var (data, labels) = TestDataProvider.PrepareIrisDataDummy();
        var batchSize = 70;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[0];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 6.5f);
                Assert.AreEqual((float)d.xBatch[0, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[0, 2], 5.5f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.8f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 6.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 2], 4.9f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 3], 1.8f);

                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[0, 1], 0);
                //Assert.AreEqual((float)d.yBatch[2, 0], 0);

                Assert.AreEqual((float)d.yBatch[batchSize - 1, 0], 1);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 1], 0);
                //Assert.AreEqual((float)d.yBatch[2, batchSize - 1], 0);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.9f);
                Assert.AreEqual((float)d.xBatch[0, 2], 4.2f);
                Assert.AreEqual((float)d.xBatch[0, 3], 1.3f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 5.1f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 3.3f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 2], 1.7f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 3], 0.5f);

                Assert.AreEqual((float)d.yBatch[0, 0], 0);
                Assert.AreEqual((float)d.yBatch[0, 1], 1);
                //Assert.AreEqual((float)d.yBatch[2, 0], 0);

                Assert.AreEqual((float)d.yBatch[batchSize - 1,0], 0);
                Assert.AreEqual((float)d.yBatch[batchSize - 1, 1], 0);
                //Assert.AreEqual((float)d.yBatch[2, batchSize - 1], 1);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[0], 10);

                Assert.AreEqual((float)d.xBatch[0, 0], 7.7f);
                Assert.AreEqual((float)d.xBatch[0, 1], 2.6f);
                Assert.AreEqual((float)d.xBatch[0, 2], 6.9f);
                Assert.AreEqual((float)d.xBatch[0, 3], 2.3f);

                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 1], 3.0f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 2], 1.4f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 3], 0.1f);
                Assert.AreEqual((float)d.xBatch[(int)d.yBatch.shape[0] - 1, 0], 4.8f);

                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[0, 1], 0);
                //Assert.AreEqual((float)d.yBatch[2, 0], 0);

                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1, 0], 0);
                Assert.AreEqual((float)d.yBatch[(int)d.yBatch.shape[0] - 1, 1], 0);
                //Assert.AreEqual((float)d.yBatch[2, (int)d.yBatch.shape[1] - 1], 1);
            }

            batchIndex++;
        }
    }

    [Test]
    public void Test_2D_Minibatching_Binary()
    {
        // arrange
        var (data, labels) = TestDataProvider.Prepare2DData_binary_label();
        var batchSize = 4;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[0];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 1.0f);
                Assert.AreEqual((float)d.xBatch[0, 1], 11.0f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 4.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 14.0f);


                Assert.AreEqual((bool)d.yBatch[0], true);
                Assert.AreEqual((bool)d.yBatch[batchSize - 1], false);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[0], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.0f);
                Assert.AreEqual((float)d.xBatch[0, 1], 15.0f);

                Assert.AreEqual((float)d.xBatch[batchSize - 1, 0], 8.0f);
                Assert.AreEqual((float)d.xBatch[batchSize - 1, 1], 18.0f);


                Assert.AreEqual((bool)d.yBatch[0], true);
                Assert.AreEqual((bool)d.yBatch[batchSize - 1], false);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[0], 2);

                Assert.AreEqual((float)d.xBatch[0, 0], 9.0f);
                Assert.AreEqual((float)d.xBatch[2 - 1, 1], 20.0f);

                Assert.AreEqual((bool)d.yBatch[0], false);
                Assert.AreEqual((bool)d.yBatch[2 - 1], true);
            }

            batchIndex++;
        }
    }

    [Test]
    public void Test_2D_Minibatching_Binary1()
    {
        //TODO: fix data transofrmation in DataFrame
        // arrange
        var (data, labels) = TestDataProvider.Prepare2DData_binary1_encoding();
        var batchSize = 4;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[1], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 1.0f);
                Assert.AreEqual((float)d.xBatch[1, 0], 11.0f);

                Assert.AreEqual((float)d.xBatch[0, batchSize - 1], 4.0f);
                Assert.AreEqual((float)d.xBatch[1, batchSize - 1], 14.0f);


                Assert.AreEqual((float)d.yBatch[0], 1);
                Assert.AreEqual((float)d.yBatch[batchSize - 1], 0);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[1], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.0f);
                Assert.AreEqual((float)d.xBatch[1, 0], 15.0f);

                Assert.AreEqual((float)d.xBatch[0, batchSize - 1], 8.0f);
                Assert.AreEqual((float)d.xBatch[1, batchSize - 1], 18.0f);


                Assert.AreEqual((float)d.yBatch[0], 1);
                Assert.AreEqual((float)d.yBatch[batchSize - 1], 0);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[1], 2);

                Assert.AreEqual((float)d.xBatch[0, 0], 9.0f);
                Assert.AreEqual((float)d.xBatch[1, 2 - 1], 20.0f);

                Assert.AreEqual((float)d.yBatch[0], 0);
                Assert.AreEqual((float)d.yBatch[2 - 1], 1);
            }

            batchIndex++;
        }
    }


    [Test]
    public void Test_2D_Minibatching_Binary2()
    {
        //TODO: fix data transofrmation in DataFrame
        // arrange
        var (data, labels) = TestDataProvider.Prepare2DData_binary2_encoding();
        var batchSize = 4;
        var feed = new DataFeed(data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.AreEqual(d.xBatch.shape[1], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 1.0f);
                Assert.AreEqual((float)d.xBatch[1, 0], 11.0f);

                Assert.AreEqual((float)d.xBatch[0, batchSize - 1], 4.0f);
                Assert.AreEqual((float)d.xBatch[1, batchSize - 1], 14.0f);


                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[1,0], 0);
                Assert.AreEqual((float)d.yBatch[0, batchSize - 1], 0);
                Assert.AreEqual((float)d.yBatch[1, batchSize - 1], 0);
            }
            else if (batchIndex == 1)
            {
                Assert.AreEqual(d.xBatch.shape[1], batchSize);

                Assert.AreEqual((float)d.xBatch[0, 0], 5.0f);
                Assert.AreEqual((float)d.xBatch[1, 0], 15.0f);

                Assert.AreEqual((float)d.xBatch[0, batchSize - 1], 8.0f);
                Assert.AreEqual((float)d.xBatch[1, batchSize - 1], 18.0f);



                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[1, 0], 0);
                Assert.AreEqual((float)d.yBatch[0, batchSize - 1], 0);
                Assert.AreEqual((float)d.yBatch[1, batchSize - 1], 0);
            }
            else
            {
                Assert.AreEqual(d.xBatch.shape[1], 2);

                Assert.AreEqual((float)d.xBatch[0, 0], 9.0f);
                Assert.AreEqual((float)d.xBatch[1, 2 - 1], 20.0f);


                Assert.AreEqual((float)d.yBatch[0, 0], 1);
                Assert.AreEqual((float)d.yBatch[1, 0], 0);
                Assert.AreEqual((float)d.yBatch[0, 2 - 1], 0);
                Assert.AreEqual((float)d.yBatch[1, 2 - 1], 0);
            }

            batchIndex++;
        }
    }


    [Test]
    public void Test_3D_Minibatching()
    {
        (NDArray x, NDArray y) = TestDataProvider.PrepareSimple3DData();

        //TO DO:

    }

       

}