////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Linq;
using Anndotnet.Core.Data;
using TorchSharp;
using Xunit;

using static TorchSharp.torch;


namespace AnnDotNet.test;

public class DataFeedTests
{
    [Fact]
    public void Test_1D_MiniBatching()
    {
        (Tensor x, Tensor y) = TestDataProvider.Prepare1DData();

        var df = new DataFeed("IrisData",x, y);
        var batchIndex = 0;
        var batchSize = 4;

        foreach (var d in df.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.True(d.xBatch.shape[0] == 4);

                Assert.Equal(1.1f, (float)d.xBatch[0], 0.01f);
                Assert.Equal(2.2f, (float)d.xBatch[1], 0.01f);
                Assert.Equal(3.3f, (float)d.xBatch[2], 0.01f);
                Assert.Equal(4.4f, (float)d.xBatch[3], 0.01f);

                Assert.Equal(11.1f, (float)d.yBatch[0], 0.01f);
                Assert.Equal(21.2f, (float)d.yBatch[1], 0.01f);
                Assert.Equal(31.3f, (float)d.yBatch[2], 0.01f);
                Assert.Equal(41.4f, (float)d.yBatch[3], 0.01f);
            }
            else if (batchIndex == 1)
            {
                Assert.True(d.xBatch.shape[0] == 4);

                Assert.Equal(5.5f, (float)d.xBatch[0], 0.01f);
                Assert.Equal(6.6f, (float)d.xBatch[1], 0.01f);
                Assert.Equal(7.7f, (float)d.xBatch[2], 0.01f);
                Assert.Equal(8.8f, (float)d.xBatch[3], 0.01f);

                Assert.Equal(51.5f, (float)d.yBatch[0], 0.01f);
                Assert.Equal(61.6f, (float)d.yBatch[1], 0.01f);
                Assert.Equal(71.7f, (float)d.yBatch[2], 0.01f);
                Assert.Equal(81.8f, (float)d.yBatch[3], 0.01f);
            }
            else
            {
                Assert.True(d.xBatch.shape[0] == 2);

                Assert.Equal(9.9f, (float)d.xBatch[0], 0.01f);
                Assert.Equal(10.10f, (float)d.xBatch[1], 0.01f);

                Assert.Equal(91.90f, (float)d.yBatch[0], 0.01f);
                Assert.Equal(101.10f, (float)d.yBatch[1], 0.01f);
            }    
            batchIndex++;
        }

    }

    [Fact]
    public void Test_2D_MiniBatching()
    {
        // arrange
        var (data, labels) = TestDataProvider.Prepare2DData();
        var batchSize = 4;
        var feed = new DataFeed("DataFeed",data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[1];
           
        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        { 
            if (batchIndex == 0)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(1.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(11.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(4.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(14.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.Equal(21.0f, (float)d.yBatch[0]);
                Assert.Equal(24.0f, (float)d.yBatch[batchSize - 1]);
            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(15.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(8.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(18.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.Equal(25.0f, (float)d.yBatch[0]);
                Assert.Equal(28.0f, (float)d.yBatch[batchSize - 1]);
            }
            else
            {
                Assert.Equal(2, d.xBatch.shape[0]);

                Assert.Equal(9.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(20.0f, (float)d.xBatch[2 - 1, 1]);

                Assert.Equal(29.0f, (float)d.yBatch[0]);
                Assert.Equal(30.0f, (float)d.yBatch[2 - 1]);
            }

            batchIndex++;
        }
    }

    [Fact]
    public void Test_2D_MiniBatching_One_Hot_Encoding()
    {
        var (data, labels) = TestDataProvider.PrepareIrisData();
        var batchSize = 70;
        var feed = new DataFeed("DataFeed",data, labels);
        int batchIndex = 0;

        var totalSamples = data.shape[1];

        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(6.5f, (float)d.xBatch[0, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[0, 1]);
                Assert.Equal(5.5f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[0, 3]);

                Assert.Equal(6.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(4.9f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0, 0]);
                Assert.Equal(0, (float)d.yBatch[0, 1]);
                Assert.Equal(0, (float)d.yBatch[0, 2]);

                Assert.Equal(1, (float)d.yBatch[batchSize - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 1]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 2]);
            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.9f, (float)d.xBatch[0, 1]);
                Assert.Equal(4.2f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(5.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.3f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(1.7f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(0.5f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(0, (float)d.yBatch[0, 0]);
                Assert.Equal(1, (float)d.yBatch[0, 1]);
                Assert.Equal(0, (float)d.yBatch[0, 2]);

                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 1]);
                Assert.Equal(1, (float)d.yBatch[batchSize - 1, 2]);
            }
            else
            {
                Assert.Equal(10, d.xBatch.shape[0]);

                Assert.Equal(7.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.6f, (float)d.xBatch[0, 1]);
                Assert.Equal(6.9f, (float)d.xBatch[0, 2]);
                Assert.Equal(2.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(4.8f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 0]);
                Assert.Equal(3f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 1]);
                Assert.Equal(1.4f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 2]);
                Assert.Equal(0.1f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0, 0]);
                Assert.Equal(0, (float)d.yBatch[0, 1]);
                Assert.Equal(0, (float)d.yBatch[0, 2]);

                Assert.Equal(0, (float)d.yBatch[(int)d.yBatch.shape[0] - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[(int)d.yBatch.shape[0] - 1, 1]);
                Assert.Equal(1, (float)d.yBatch[(int)d.yBatch.shape[0] - 1, 2]);
            }

            batchIndex++;
        }
    }

    [Fact]
    public void Test_2D_MiniBatching_Ordinal()
    {
        // arrange
        var (data, labels) = TestDataProvider.PrepareIrisDataOrdinal();
        var batchSize = 70;
        var feed = new DataFeed("DataFeed", data, labels);
        int batchIndex = 0;

        // act
        var totalSamples = data.shape[0];

        // assert
        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(6.5f, (float)d.xBatch[0, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[0, 1]);
                Assert.Equal(5.5f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[0, 3]);

                Assert.Equal(6.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(4.9f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0]);

                Assert.Equal(1, (float)d.yBatch[batchSize - 1]);
            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.9f, (float)d.xBatch[0, 1]);
                Assert.Equal(4.2f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(5.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.3f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(1.7f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(0.5f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(2, (float)d.yBatch[0]);

                Assert.Equal(3, (float)d.yBatch[batchSize - 1]);
            }
            else
            {
                Assert.Equal(10, d.xBatch.shape[0]);

                Assert.Equal(7.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.6f, (float)d.xBatch[0, 1]);
                Assert.Equal(6.9f, (float)d.xBatch[0, 2]);
                Assert.Equal(2.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(4.8f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 0]);
                Assert.Equal(3f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 1]);
                Assert.Equal(1.4f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 2]);
                Assert.Equal(0.1f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0]);

                Assert.Equal(3, (float)d.yBatch[(int)d.yBatch.shape[0] - 1]);
            }

            batchIndex++;
        }
    }


    [Fact]
    public void Test_2D_MiniBatching_Dummy()
    {
        var (data, labels) = TestDataProvider.PrepareIrisDataDummy();
        var batchSize = 70;
        var feed = new DataFeed("DataFeed", data, labels);
        int batchIndex = 0;

        var totalSamples = data.shape[0];

        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(batchSize, d.xBatch.shape[0]);

                Assert.Equal(6.5f, (float)d.xBatch[0, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[0, 1]);
                Assert.Equal(5.5f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[0, 3]);

                Assert.Equal(6.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(4.9f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(1.8f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0, 0]);
                Assert.Equal(0, (float)d.yBatch[1, 0]);
                Assert.Equal(1, (float)d.yBatch[batchSize - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 1]);

            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.9f, (float)d.xBatch[0, 1]);
                Assert.Equal(4.2f, (float)d.xBatch[0, 2]);
                Assert.Equal(1.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(5.1f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(3.3f, (float)d.xBatch[batchSize - 1, 1]);
                Assert.Equal(1.7f, (float)d.xBatch[batchSize - 1, 2]);
                Assert.Equal(0.5f, (float)d.xBatch[batchSize - 1, 3]);

                Assert.Equal(0, (float)d.yBatch[0, 0]);
                Assert.Equal(1, (float)d.yBatch[0, 1]);

                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1, 1]);
            }
            else
            {
                Assert.Equal(10, d.xBatch.shape[0]);

                Assert.Equal(7.7f, (float)d.xBatch[0, 0]);
                Assert.Equal(2.6f, (float)d.xBatch[0, 1]);
                Assert.Equal(6.9f, (float)d.xBatch[0, 2]);
                Assert.Equal(2.3f, (float)d.xBatch[0, 3]);

                Assert.Equal(4.8f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 0]);
                Assert.Equal(3.0f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 1]);
                Assert.Equal(1.4f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 2]);
                Assert.Equal(0.1f, (float)d.xBatch[(int)d.xBatch.shape[0] - 1, 3]);

                Assert.Equal(1, (float)d.yBatch[0, 0]);
                Assert.Equal(0, (float)d.yBatch[0, 1]);

                Assert.Equal(0, (float)d.yBatch[(int)d.yBatch.shape[0] - 1, 0]);
                Assert.Equal(0, (float)d.yBatch[(int)d.yBatch.shape[0] - 1, 1]);

            }

            batchIndex++;
        }
    }

    [Fact]
    public void Test_2D_MiniBatching_Binary()
    {
        var (data, labels) = TestDataProvider.Prepare2DData_binary_label();
        const int batchSize = 4;
        var feed = new DataFeed("DataFeed", data, labels);
        int batchIndex = 0;


        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(batchSize, d.xBatch.shape[0]);

                Assert.Equal(1.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(11.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(4.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(14.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.True((bool)d.yBatch[0]);
                Assert.False((bool)d.yBatch[batchSize - 1]);
            }
            else if (batchIndex == 1)
            {
                Assert.Equal(batchSize, d.xBatch.shape[0]);

                Assert.Equal(5.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(15.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(8.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(18.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.True((bool)d.yBatch[0]);
                Assert.False((bool)d.yBatch[batchSize - 1]);
            }
            else
            {
                Assert.Equal(2, d.xBatch.shape[0]);

                Assert.Equal(9.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(20.0f, (float)d.xBatch[2 - 1, 1]);

                Assert.False((bool)d.yBatch[0]);
                Assert.True((bool)d.yBatch[2 - 1]);
            }

            batchIndex++;
        }
    }

    [Fact]
    public void Test_2D_MiniBatching_Binary1()
    {

        var (data, labels) = TestDataProvider.Prepare2DData_binary1_encoding();
        var batchSize = 4L;
        var feed = new DataFeed("DataFeed", data, labels);
        int batchIndex = 0;

        foreach (var d in feed.GetNextBatch((int)batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(1.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(11.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(4.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(14.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.Equal(1, (float)d.yBatch[0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1]);
            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(15.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(8.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(18.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.Equal(1, (float)d.yBatch[0]);
                Assert.Equal(0, (float)d.yBatch[batchSize - 1]);
            }
            else
            {
                Assert.Equal(2, d.xBatch.shape[0]);

                Assert.Equal(9.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(20.0f, (float)d.xBatch[2 - 1, 1]);

                Assert.Equal(0, (float)d.yBatch[0]);
                Assert.Equal(1, (float)d.yBatch[2 - 1]);
            }

            batchIndex++;
        }
    }


    [Fact]
    public void Test2D_MiniBatch_Binary2()
    {
        var (data, labels) = TestDataProvider.Prepare2DData_binary2_encoding();
        var batchSize = 4;
        var feed = new DataFeed("DataFeed", data, labels);
        int batchIndex = 0;


        foreach (var d in feed.GetNextBatch(batchSize))
        {
            if (batchIndex == 0)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(1.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(11.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(4.0f, (float)d.xBatch[batchSize - 1,0]);
                Assert.Equal(14.0f, (float)d.xBatch[batchSize - 1, 1]);


                Assert.Equal(1, (float)d.yBatch[0]);
                Assert.Equal(-1, (float)d.yBatch[batchSize - 1]);

            }
            else if (batchIndex == 1)
            {
                Assert.Equal(d.xBatch.shape[0], batchSize);

                Assert.Equal(5.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(15.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(8.0f, (float)d.xBatch[batchSize - 1, 0]);
                Assert.Equal(18.0f, (float)d.xBatch[batchSize - 1, 1]);



                Assert.Equal(1, (float)d.yBatch[0]);
                Assert.Equal(-1, (float)d.yBatch[batchSize - 1]);

            }
            else
            {
                Assert.Equal(2, d.xBatch.shape[0]);

                Assert.Equal(9.0f, (float)d.xBatch[0, 0]);
                Assert.Equal(19.0f, (float)d.xBatch[0, 1]);

                Assert.Equal(10.0f, (float)d.xBatch[2 - 1, 0]);
                Assert.Equal(20.0f, (float)d.xBatch[2 - 1, 1]);


                Assert.Equal(-1, (float)d.yBatch[0]);
                Assert.Equal(1, (float)d.yBatch[2 - 1]);
            }

            batchIndex++;
        }
    }


    [Fact]
    public void Test_3D_MiniBatching()
    {
        (Tensor x, Tensor y) = TestDataProvider.PrepareSimple3DData();

        //TO DO:

    }


    [Fact]
    public void CrossValidation_Sets_test()
    {
        int kFold = 5;

        float[] data = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        //int trainSize = 8;
        int testSize = 2;

        int n = 3; // Number of elements to take
        int k = 2; // Number of elements to skip

        for(int fold = 0; fold< kFold; fold++)
        {
            n = fold * testSize; // Number of elements to take
            k = testSize; // Number of elements to skip

            var result = data.Take(n)
                .Concat(data.Skip(n + k).Take(data.Length - n - k))
                .ToArray();

            Trace.WriteLine("");
            Trace.WriteLine($"FOLD:({fold})");
            Trace.WriteLine("TrainSet:");
            foreach (var element in result)
            {
                Trace.Write(element + " ");
            }

            Trace.WriteLine("TestSet:");
            foreach (var element in data.Except(result))
            {
                Trace.Write(element + " ");
            }
        }
        
    }

    [Fact]
    void EnumerateBatch()
    {
        // Create a 30x4 tensor (example data)
        var tensor = torch.rand(new long[] { 30, 4 });

        // Extract the first two columns and the last two columns
        var firstColumns = tensor.narrow(1, 0, 2);
        var lastColumns = tensor.narrow(1, 2, 2);

        

        // Perform element-wise multiplication on corresponding columns
        var result = firstColumns * lastColumns;

        // Print the original tensor
        Trace.WriteLine("Original Tensor:");
        Trace.WriteLine(tensor);

        // Print the result tensor
        Trace.WriteLine("\nResult Tensor:");
        Trace.WriteLine(result);
    }

}