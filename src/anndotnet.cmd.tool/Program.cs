﻿using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Tool.Progress;
using AnnDotNET.Tool.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AnnDotNet.Tool;
using AnnDotNet.Vnd.Samples;


namespace AnnDotNET.Tool;

static class Program
{

    static async Task Main(string[] args)
    {
        var str = @"
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ***************   ANNdotNET 2.0 - deep learning tool on .NET Framework.**********
    *******        Run Deep learning pre-calculated examples               **********
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Select the example to run:

                10. - Multi-class: Iris example with Tran-Validate Training.
                11. - Multi-class: Iris example from MLConfig file.
                12. - Multi-class: Iris example with Cross-Validation Training.

                21. - Multi-class: AirQuality example from MLConfig file.
                22. - Multi-class: AirQuality example with Tran-Validate Training.
                23. - Multi-class: AirQuality example with Cross-Validation Training.

                31. - Binary-class: Titanic example from MLConfig file.
                32. - Binary-class: Titanic example with  Tran-Validate Training.
                33. - Binary-class: Titanic example with Cross-Validation Training.

                41. - Regression: Concrete slump test with Tran-Validate Training.
                42. - Regression: Concrete slump test from MLConfig.
                43. - Regression: Concrete slump test with Cross-Validation Training.    

                x. - Exit
                    ";
        Console.WriteLine(str);

        while (true)
        {
            var key = Console.ReadLine();
            await RunExamples(key);
            Console.WriteLine("Select the example number to run, or 'x' to Exit:");
        }
    }

    private static  async Task RunExamples(string key)
    {
        if (key == "10")
        {
            await IrisFromNetObject(false);
        }
        else if (key == "11")
        {
            await IrisFromMLConfig();
        }
        else if(key == "12")
        {
            await IrisFromNetObject(true);
        }
        else if (key == "21")
        {
            await AirQualityFromMLConfig();
        }
        else if (key == "22")
        {
            await AirQualitiySample();
        }
        else if (key == "23")
        {
            await AirQualitiySample();
        }
        else if (key == "31")
        {
            await TitanicMLConfig();
        }
        else if (key == "32")
        {
            await RunTitanicSample(false);
        }
        else if (key == "33")
        {
            await RunTitanicSample(true);
        }
        else if (key == "41")
        {
            await RunConcreteSlumpSample();
        }
        else if (key == "42")
        {
            ConcreteSlumpMLConfig();
        }
        else if (key == "43")
        {
            ConcreteSlumpMLConfig();
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

    #region SlumpTest Example
    private static async Task RunConcreteSlumpSample()
    {
        throw new NotImplementedException();

        //SlampTestSample slump = new SlampTestSample();
        //(NDArray x, NDArray y) = await slump.GenerateData();

        //(TrainingParameters tParams, LearningParameters lParams) = slump.GenerateParameters();
        //var net = slump.CreateNet();

        //var r = new MLRunner(net, lParams, tParams, x, y, slump.Metadata, new ConsoleHelper());
        //r.Run(null);

        //await r.SaveMlConfig(slump.Metadata, new DataParser(), "slump.mlconifg");
    }

    private static void ConcreteSlumpMLConfig()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Titanic Example
    private static async Task TitanicMLConfig()
    {
        throw new NotImplementedException();

        //var mlCOnf = await MLFactory.Load(@"mlconfigs\titanic\titanic.mlconfig");

        //var mlConfig1 = mlCOnf;

        //IProgressTraining progress = (mlConfig1.TParameters.TrainingType == TrainingType.CVTraining) ? new ProgressCVTraining() : new ProgressTVTraining();

        ////
        //var mlRunner = new MLRunner(mlConfig1, new ConsoleHelper());
        //mlRunner.Run(progress);
    }

    private static async Task RunTitanicSample(bool crossValidation = false)
    {
        throw new NotImplementedException();

        //TitanicSample titanic = new TitanicSample();
        //(NDArray x, NDArray y) = await titanic.GenerateData();

        //(TrainingParameters tParams, LearningParameters lParams) = titanic.GenerateParameters();

        //var net = titanic.CreateNet();

        //var paths = new Dictionary<string, string>
        //{
        //    { "MLConfig", "titanic.mlconfig" },
        //    { "Root", "mlconfigs/titanic" },
        //    { "Models", "models" },
        //    { "BestModel", "" }
        //};

        //var mlRunner = new MLRunner(net, lParams, tParams, x, y, titanic.Metadata, new ConsoleHelper(), paths);

        //IProgressTraining progress = crossValidation ? new ProgressCVTraining() : new ProgressTVTraining();

        //mlRunner.Run(progress);

        ////predict and deploy
        //var predData = await titanic.GeneratePredictionData(20);

        //var fullModelPath = Path.Combine(paths["Root"], paths["Models"], paths["BestModel"]);
        //var model = await mlRunner.LoadModelAsync(fullModelPath);

        //var result = await mlRunner.PredictAsync(model, predData.X);

        //mlRunner.PredictionMetrics(result, predData.Y, titanic.Metadata);

    }
    #endregion

    #region Iris Example
    private static async Task IrisFromNetObject(bool crossValidation= false)
    {
        var iris = new IrisSample();
        var net = iris.CreateNet();
        var lParams = iris.GenerateParameters().lParams;
        var tParams = iris.GenerateParameters().tPArams;
        
        tParams.TrainingType = crossValidation ? TrainingType.CVTraining : TrainingType.TVTraining;  
        IProgressTraining progress = crossValidation ? new ProgressCVTraining() : new ProgressTVTraining();   

        var (x, y) = await iris.GenerateData();

        var paths = new Dictionary<string, string>
        {
            { "MLConfig", "iris.mlconfig" },
            { "Root", "mlconfigs/iris" },
            { "Models", "models" },
            { "BestModel", "" }
        };

       // var mlRunner = new MLRunner(net, lParams, tParams, X, Y, null, new ConsoleHelper(), paths );

        ////Training validation
        //mlRunner.Run( progress);

        ////predict and deploy
        //var predData = await iris.GeneratePredictionData(20);

        //var fullModelPath = Path.Combine(paths["Root"],paths["Models"], paths["BestModel"]);
        //var model = await mlRunner.LoadModelAsync(fullModelPath);

        //var result = await mlRunner.PredictAsync(model, predData.X);

        //mlRunner.PredictionMetrics(result, predData.Y, iris.Metadata);

        ////await r.SaveMlConfig(iris.Metadata, iris.Parser, "mlconfigs/iris/iris.mlconfig"); 
    }       

    private static async Task IrisFromMLConfig()
    {
        throw new NotImplementedException();

        //var mlCOnf = await MLFactory.Load(@"mlconfigs\iris\iris.mlconfig");

        //IProgressTraining progress = mlCOnf.TParameters.TrainingType== TrainingType.CVTraining ? new ProgressCVTraining() : new ProgressTVTraining();

        //var mlConfig1 = mlCOnf;
        ////
        //var mlRunner = new MLRunner(mlConfig1, new ConsoleHelper());
        //mlRunner.Run(new ProgressTVTraining());
    }
    #endregion

    #region AirQuality Example
    private static async Task AirQualitiySample()
    {
        throw new NotImplementedException();

        //var data = await AirQualitySample.generateAirQualityData();

        //var par = AirQualitySample.generateParameters();

        //var net = AirQualitySample.CreateNet();

        //var r = new MLRunner(net, par.lParams, par.tParams, data.X, data.Y, null, new ConsoleHelper());

        //r.Run(null);

        //// await r.SaveMlConfig(iris.Metadata, "..\..\..\..\\mlconfigs\air_quality\airquality.mlconfig");
    }

    private static async Task AirQualityFromMLConfig()
    {
        throw new NotImplementedException();

        //var mlCOnf = await MLFactory.Load(@"mlconfigs\air_quality\airquality.mlconfig");

        //var mlConfig1 = mlCOnf;
        ////
        //var mlRunner = new MLRunner(mlConfig1, new ConsoleHelper());
        //mlRunner.Run(null);
    }
    #endregion
}