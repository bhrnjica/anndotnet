﻿using Anndotnet.Core;
using Anndotnet.Core.Interface;
using Anndotnet.Core.Trainers;
using Anndotnet.Tool.Progress;
using Anndotnet.Vnd;
using Anndotnet.Vnd.Samples;
using AnnDotNET.Tool.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tensorflow.NumPy;

namespace AnnDotNET.Tool
{

    class Program
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

                31. - Binary-class: Titanic example from MLCOnfig file.
                32. - Binary-class: Titanic example with  Tran-Validate Training.
                33. - Binary-class: Titanic example with Corss-Validation Training.

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
               await RunTitanicSample();
            }
            else if (key == "33")
            {
                await RunTitanicSample();
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

        private static async Task RunConcreteSlumpSample()
        {
            SlampTestSample slump = new SlampTestSample();
            (NDArray x, NDArray y) = await slump.GenerateData();

            (TrainingParameters tParams, LearningParameters lParams) = slump.GenerateParameters();
            var net = slump.CreateNet();

            var r = new MLRunner(net, lParams, tParams, x, y, slump.Metadata);
            r.Run(null);

            await r.SaveMlConfig(slump.Metadata, new DataParser(), "slump.mlconifg");
        }

        private static void ConcreteSlumpMLConfig()
        {
            throw new NotImplementedException();
        }

        private static async Task TitanicMLConfig()
        {
            var mlCOnf = await MLFactory.Load(@"mlconfigs\titanic\titanic.mlconfig");
        
            var mlConfig1 = mlCOnf;

            //
            var mlRunner = new MLRunner(mlConfig1);
            mlRunner.Run(null);
        }

        private static async Task RunTitanicSample()
        {
            TitanicSample titanic = new TitanicSample();
            (NDArray x, NDArray y) = await titanic.GenerateData();

            (TrainingParameters tParams, LearningParameters lParams) = titanic.GenerateParameters();
            var net = titanic.CreateNet();

            var r = new MLRunner(net, lParams, tParams, x, y, titanic.Metadata);
            r.Run(null);

           await r.SaveMlConfig(titanic.Metadata,new DataParser(),"titanic.mlconifg");

        }

        //private static async Task RunCVIrisSample()
        //{
        //    IrisSample iris = new IrisSample();

        //    //load the data
        //    (var x, var y) = await iris.GenerateData();

        //    //training and learning params
        //    (var tParams, var lParams) = iris.GenerateParameters();
        //    tParams.KFold = 5;
        //    tParams.TrainingType = TrainingType.CVTraining;

        //    //ann model
        //    var net = iris.CreateNet();

        //    //mlconfig paths
        //    var paths = new Dictionary<string, string>();
        //    paths.Add("MainFolder", "mlconfigs/iris");
        //    paths.Add("MLConfig", "iris.mlconfig");
        //    paths.Add("BestModel", "");
        //    paths.Add("Models", "models"); 

        //    //run ML
        //    var r = new MLRunner(net, lParams, tParams, x, y, null,paths);
        //    r.Run();

        //    //save mlconfig
        //    await r.SaveMlConfig(iris.Metadata, iris.Parser, "mlconfigs/iris/iris.mlconfig");
        //}

        //private static async Task RunIrisSample()
        //{
        //    IrisSample iris = new IrisSample();
           
        //    (NDArray x, NDArray y) = await iris.GenerateData(); 
        //    (TrainingParameters tParams, LearningParameters lParams) = iris.GenerateParameters();
        //    var net = iris.CreateNet();
            
        //    var pats = new Dictionary<string, string>();
        //    pats.Add("MLConfig", @"iris.mlconfig");
            
        //    var r = new MLRunner(net, lParams, tParams, x, y,null);
        //    r.Run();

        //    await r.SaveMlConfig(iris.Metadata,iris.Parser, "mlconfigs/iris/iris.mlconfig");
        //}

        private static async Task IrisFromNetObject(bool crossValidation= false)
        {
            var iris = new IrisSample();

            var net = iris.CreateNet();
            var lParams = iris.GenerateParameters().lParams;
            var tParams = iris.GenerateParameters().tPArams;

            tParams.TrainingType = crossValidation ? TrainingType.CVTraining : TrainingType.TVTraining;  
            
            IProgressTraining progress = crossValidation ? new ProgressCVTraining() : new ProgressTVTraining();   

            var data = await iris.GenerateData();

            var paths = new Dictionary<string, string>() { {"MLConfig", "iris.mlconfig" } };
            paths.Add("Root", "mlconfigs/iris");
            paths.Add("Models", "models");  
            paths.Add("BestModel", "638227190040989949.ckp");

            var r = new MLRunner(net, lParams, tParams, data.X, data.Y, null, paths);
            
            //Training validation
            r.Run( progress);

            //predict and deploy
            var predData = await iris.GeneratePredictionData(20);

            var fullModelPath = Path.Combine(paths["Root"],paths["Models"], paths["BestModel"]);
            var model = await r.LoadModelAsync(fullModelPath);

            var result = await r.PredictAsync(model, predData.X);

            r.PredictionMetrics(result, predData.Y);

            //await r.SaveMlConfig(iris.Metadata, iris.Parser, "mlconfigs/iris/iris.mlconfig"); 
        }       

        private static async Task IrisFromMLConfig()
        {
            var mlCOnf = await MLFactory.Load(@"mlconfigs\iris\iris.mlconfig");

            IProgressTraining progress = mlCOnf.TParameters.TrainingType== TrainingType.CVTraining ? new ProgressCVTraining() : new ProgressTVTraining();

            var mlConfig1 = mlCOnf;
            //
            var mlRunner = new MLRunner(mlConfig1);
            mlRunner.Run(new ProgressTVTraining());
        }

        private static async Task AirQualitiySample()
        {
            var data = await AirQualitySample.generateAirQualityData();

            var par = AirQualitySample.generateParameters();

            var net = AirQualitySample.CreateNet();
            
            var r = new MLRunner(net, par.lParams, par.tParams, data.X, data.Y, null);

            r.Run(null);

           // await r.SaveMlConfig(iris.Metadata, "..\..\..\..\\mlconfigs\air_quality\airquality.mlconfig");
        }

        private static async Task AirQualityFromMLConfig()
        {
            var mlCOnf = await MLFactory.Load(@"mlconfigs\air_quality\airquality.mlconfig");
            
            var mlConfig1 = mlCOnf;
            //
            var mlRunner = new MLRunner(mlConfig1);
            mlRunner.Run(null);
        }
        
    }
}
