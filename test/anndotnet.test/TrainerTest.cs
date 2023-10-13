////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Data;
using Anndotnet.Core.Mlconfig;
using Anndotnet.Core.Trainers;
using System.Collections.Generic;
using System;
using System.Linq;
using Xunit;

namespace AnnDotNet.test;

public class Trainertest
{
    
    [Fact]
    public void SplitTraininegData_NoShuffling_test()
    {
        var mlConfig = MlFactory.CreatEmptyMlConfig("c15b26ba-2876-4cdd-b97a-02b3fa83944b");
        mlConfig.TrainingParameters.ShuffleWhenSplit = false;

        var model = new AnnModel(mlConfig.Name, mlConfig.Network, 1, 1);
        var data = TestDataProvider.Prepare1DData();
        var ds = new DataFeed(mlConfig.Name, data.x,data.y);

        var trainer = new TvTrainer(null,ds,mlConfig.TrainingParameters,mlConfig.LearningParameters,null);
         
        var (train, valid) = trainer.Split(ds, 1234);

        foreach (var d in train)
        {
            Assert.True(d["X"].shape[0] == 8);
            Assert.True(d["y"].shape[0] == 8);

            Assert.Equal(1.1f, (float)d["X"][0], 0.01);
            Assert.Equal(8.8f, (float)d["X"][7], 0.01);

        }

        foreach (var d in valid)
        {
            Assert.Equal(9.9f, (float)d["X"][0], 0.01);
            Assert.Equal(10.10f, (float)d["X"][1], 0.01);
        }
    }

    [Fact]
    public void SplitTraininegData_WithShuffling_test()
    {
        var mlConfig = MlFactory.CreatEmptyMlConfig("c15b26ba-2876-4cdd-b97a-02b3fa83944b");
        mlConfig.TrainingParameters.ShuffleWhenSplit = true;

        var model = new AnnModel(mlConfig.Name, mlConfig.Network, 1, 1);
        var data = TestDataProvider.Prepare1DData();
        var ds = new DataFeed(mlConfig.Name, data.x, data.y);

        var trainer = new TvTrainer(null, ds, mlConfig.TrainingParameters, mlConfig.LearningParameters, null);

        var (train, valid) = trainer.Split(ds, 1234);

        foreach (var d in train)
        {
            Assert.True(d["X"].shape[0] == 8);
            Assert.True(d["y"].shape[0] == 8);

            Assert.Equal(1.1f, (float)d["X"][0], 0.01);
            Assert.Equal(9.9f, (float)d["X"][3], 0.01);
            Assert.Equal(8.8f, (float)d["X"][7], 0.01);

        }

        foreach (var d in valid)
        {
            Assert.Equal(4.4f, (float)d["X"][0], 0.01);
            Assert.Equal(10.10f, (float)d["X"][1], 0.01);
        }
    }


    [Fact]
    public void TorchMetrcs_Accuracy_test()
    {
        // Sample color data
       var colors = new List<string> { "red", "green", "blue", "green", "red" };

        // Create a dictionary to map color names to ordinal values
        Dictionary<string, int> colorMapping = colors.Distinct().Select((color, index) => new { Color = color, Index = index }).ToDictionary(x => x.Color, x => x.Index);

        // Encode the colors
        List<int> encodedColors = colors.Select(color => colorMapping[color]).ToList();

        // Print the encoded values
        Console.WriteLine("Ordinal Encoded Colors:");
        foreach (int encodedColor in encodedColors)
        {
            Console.WriteLine(encodedColor);
        }
    }




}