//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Threading;
using ANNdotNET.Lib;
using NNetwork.Core.Common;

namespace anndotnet.capp
{
    class Program
    {
        static void Main(string[] args)
        {
            //loading anndotnet project file
            openProject();

            //create new project
           // newproject();
        }
        private static void newproject()
        {
            var annProject = new Project();
            var projPath = "../../../../pr_example/new_project_file";
            Project.NewProjectFile("newProject", projPath);
            annProject.Load(projPath);
        }
        private static void openProject()
        {
            var annProject = new Project();

            //annproject file
            var projPath = "../../../../pr_example/ann_project_config.txt";
            annProject.Load(projPath);


            //create model based on the current project data
           // annProject.CreateModel("BHModel01");

            //open model within project 
           // annProject.OpenModel("MLModel2");

            //save model within the project
            //annProject.SaveCurrentModel("MLModel2");

            //run model
            
            annProject.RunModel("MLModel2", new CancellationToken(), trainingProgress, ProcessDevice.Default);


        }

        static void trainingProgress(ProgressData progress)
        {
            //
            Console.WriteLine($"Epoch={progress.EpochCurrent} of {progress.EpochTotal};\t Evaluation of {progress.EvaluationFunName}=" +
                $"(TrainMB = {progress.MinibatchAverageEval},TrainFull = {progress.TrainEval}, Valid = {progress.ValidationEval})");
        }

    }
}
