////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Mlconfig;
using Anndotnet.Core.Data;
using Anndotnet.Tool.Progress;
using Anndotnet.Tool.Samples;

namespace Anndotnet.Tool;

static class Program
{

    static async Task Main(string[] args)
    {
        await RunTitanicSample();
        //await SlumpTestFromNetObjects();
        //await IrisFromNetObject(false);
        //await IrisFromMLConfig();
        return;
        var str = @"
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ***************   ANNdotNET 2.0 - deep learning tool on .NET Framework.**********
    *******        Run Deep learning pre-calculated examples               **********
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    Select the example to run:

                10. - Multi-class: Iris example with Tran-Validate Training.
                11. - Multi-class: Iris example from MLConfig file.
                12. - Multi-class: Iris example with Cross-Validation Training.

                21. - Binary-class: Titanic example from MLConfig file.
                22. - Binary-class: Titanic example with  Tran-Validate Training.
                23. - Binary-class: Titanic example with Cross-Validation Training.

                31. - Regression: Concrete slump test with Tran-Validate Training.
                32. - Regression: Concrete slump test from MLConfig.
                33. - Regression: Concrete slump test with Cross-Validation Training.    

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
            await TitanicMlConfig();
        }
        else if (key == "22")
        {
            await RunTitanicSample(false);
        }
        else if (key == "23")
        {
            await RunTitanicSample(true);
        }
        else if (key == "31")
        {
            await SlumpTestFromNetObjects();
        }
        else if (key == "32")
        {
            ConcreteSlumpMlConfig();
        }
        else if (key == "33")
        {
            ConcreteSlumpMlConfig();
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
    private static async Task SlumpTestFromNetObjects(bool crossValidation= false)
    {
        var slamp = new SlampTestSample();
        var mlConfig = new MlConfig(Guid.NewGuid().ToString(), "SlampTest");

        var (tParams, lParams) = slamp.GenerateParameters();
        mlConfig.LearningParameters = lParams;
        mlConfig.TrainingParameters = tParams;
        mlConfig.Parser = slamp.Parser;
        mlConfig.Network = slamp.CreateNet();
        mlConfig.Paths = new Dictionary<string, string>
                         {
                             { "MLConfig", "slamp.mlconfig" },
                             { "Root", "mlconfigs/slamptest" },
                             { "Models", "models" },
                             { "BestModel", "" }
                         };

        //setup training type
        mlConfig.TrainingParameters.TrainingType = crossValidation ? TrainingType.CvTraining : TrainingType.TvTraining;



        //obtain data
        var (x, y) = await slamp.GenerateData();
        mlConfig.Metadata = slamp.Metadata;
        var slampData = new DataFeed("SlampTest", x, y);

        //run trainer
        var json = MlFactory.SaveToString(mlConfig);
        var mlRunner = new MlRunner(mlConfig, new ConsoleHelper());

        //define progress report
        IProgressTraining progress = crossValidation ? new ProgressCvTraining() : new ProgressTvTraining();

        await mlRunner.TrainAsync(slampData, progress);


        mlRunner.CalculatePerformance();
    }

    private static void ConcreteSlumpMlConfig()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Titanic Example
    private static async Task TitanicMlConfig()
    {
        var mlConfig = await MlFactory.LoadfromFileAsync(@"mlconfigs\titanic\titanic.mlconfig");
        //run trainer
        var mlRunner = new MlRunner(mlConfig, new ConsoleHelper());

        //define progress report
        IProgressTraining progress = mlConfig.TrainingParameters.TrainingType == TrainingType.CvTraining ? new ProgressCvTraining() : new ProgressTvTraining();

        //obtain data
        var (x, y) = MlFactory.LoadData(mlConfig);
        var irisData = new DataFeed("Titanic", x, y);


        await mlRunner.TrainAsync(irisData, progress);


    }

    private static async Task RunTitanicSample(bool crossValidation = false)
    {
        var titanic = new TitanicSample();
        var mlConfig = new MlConfig(Guid.NewGuid().ToString(), "Titanic");

        var (tParams, lParams) = titanic.GenerateParameters();
        mlConfig.LearningParameters = lParams;
        mlConfig.TrainingParameters = tParams;
        mlConfig.Parser = titanic.Parser;
        mlConfig.Network = titanic.CreateNet();
        mlConfig.Paths = new Dictionary<string, string>
                         {
                             { "MLConfig", "iris.mlconfig" },
                             { "Root", "mlconfigs/iris" },
                             { "Models", "models" },
                             { "BestModel", "" }
                         };

        //setup training type
        mlConfig.TrainingParameters.TrainingType = crossValidation ? TrainingType.CvTraining : TrainingType.TvTraining;


        //obtain data
        var (x, y) = await titanic.GenerateData();
        mlConfig.Metadata = titanic.Metadata;
        var data = new DataFeed("Titanic", x, y);

        //run trainer
        var json = MlFactory.SaveToString(mlConfig);
        var mlRunner = new MlRunner(mlConfig, new ConsoleHelper());

        //define progress report
        IProgressTraining progress = crossValidation ? new ProgressCvTraining() : new ProgressTvTraining();

        await mlRunner.TrainAsync(data, progress);


        mlRunner.CalculatePerformance();

        ////predict and deploy
        //var predData = await titanic.GeneratePredictionData(20);



    }
    #endregion

    #region Iris Example
    private static async Task IrisFromNetObject(bool crossValidation= false)
    {
        var iris = new IrisSample();
        var mlConfig = new MlConfig(Guid.NewGuid().ToString(), "Iris");

        var (tParams, lParams) = iris.GenerateParameters();
        mlConfig.LearningParameters = lParams;
        mlConfig.TrainingParameters = tParams;
        mlConfig.Parser = iris.Parser;
        mlConfig.Network = iris.CreateNet();
        mlConfig.Paths = new Dictionary<string, string>
        {
            { "MLConfig", "iris.mlconfig" },
            { "Root", "mlconfigs/iris" },
            { "Models", "models" },
            { "BestModel", "" }
        };

        //setup training type
        mlConfig.TrainingParameters.TrainingType = crossValidation ? TrainingType.CvTraining : TrainingType.TvTraining;  
        
        

        //obtain data
        var (x, y) = await iris.GenerateData();
        mlConfig.Metadata = iris.Metadata;
        var irisData = new DataFeed("Iris", x, y);

        //run trainer
        var  json = MlFactory.SaveToString(mlConfig);
        var mlRunner = new MlRunner(mlConfig, new ConsoleHelper());

        //define progress report
        IProgressTraining progress = crossValidation ? new ProgressCvTraining() : new ProgressTvTraining();
        
        await mlRunner.TrainAsync(irisData,  progress);


        mlRunner.CalculatePerformance();

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
        var mlConfig = await MlFactory.LoadfromFileAsync(@"mlconfigs\iris\iris.mlconfig");
        //run trainer
        var mlRunner = new MlRunner(mlConfig, new ConsoleHelper());

        //define progress report
        IProgressTraining progress = mlConfig.TrainingParameters.TrainingType== TrainingType.CvTraining ? new ProgressCvTraining() : new ProgressTvTraining();

        //obtain data
        var (x, y) = MlFactory.LoadData(mlConfig);
        var irisData = new DataFeed("Iris", x, y);


        await mlRunner.TrainAsync(irisData, progress);

       
    }
    #endregion

    
}

