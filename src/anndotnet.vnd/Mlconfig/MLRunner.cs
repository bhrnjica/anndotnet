using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using NumSharp;
using Anndotnet.Core;
using Anndotnet.Core.Learners;
using Anndotnet.Core.Trainers;

namespace Anndotnet.Vnd
{
    public class MLRunner
    {
        public static void Run(MLConfig mlConfig)
        {
           //data preparation and tranformation
            (NDArray xData, NDArray yData) = MLFactory.PrepareData(mlConfig, "Training");
            
            //create placeholders
            (Tensor x, Tensor y) = MLFactory.CreatePlaceholders(xData, yData);

            //create network
            var z = MLFactory.CreateNetwrok(mlConfig.Network, x, y);

            

            //define learner
            Learner lr = null;
            if (y.dims.Last() > 1)
            {
                var learner = new ClassificationLearner();
                lr = learner.Create(y, z,mlConfig.LParameters);
            }
            else
            {
                var learner = new RegressionLearner();
                lr = learner.Create(y, z, mlConfig.LParameters);
            }


            //training process
            if (mlConfig.TParameters.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData, 1);
                tr.Run(x, y, lr, mlConfig.TParameters);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, mlConfig.TParameters.KFold);
                tr.Run(x, y, lr, mlConfig.TParameters);
            }


            //evaluation


            //prediction
            return;

        }
        public static void Run(string mlconfigPath)
        {
            //var retVal = MLFactory_old.LoadMLConfiguration(mlconfigPath);

            //(var xData, var yData) = MLFactory_old.PrepareData(retVal["metadata"], retVal["paths"]);
            ////var f = MLFactory.CreateMLFactory(retVal);

            //(Tensor x, Tensor y) = MLFactory_old.CreatePlaceholders(xData, yData);

            //var z = MLFactory_old.CreateNetworkModel(retVal["network"], x, y);

            ////create learning params
            //var strLearning = retVal["learning"];
            //LearningParameters lrData = MLFactory_old.CreateLearningParameters(strLearning);

            ////create training param
            //var strTraining = retVal["training"];
            //TrainingParameters trData = MLFactory_old.CreateTrainingParameters(strTraining);


            ////define learner
            //Learner lr= null;
            //if(y.dims.Last() > 1)
            //{
            //    var learner = new ClassificationLearner();
            //    lr = learner.Create(y, z, new LearningParameters());
            //}
            //else
            //{
            //    var learner = new RegressionLearner();
            //    lr = learner.Create(y, z, new LearningParameters());
            //}
            

            ////training process
            //if (trData.TrainingType == TrainingType.TVTraining)
            //{
            //    var tr = new TVTrainer(xData, yData, 1);
            //    tr.Run(x, y, lr, trData);
            //}
            //else
            //{
            //    var tr = new CVTrainer(xData, yData, trData.KFold);
            //    tr.Run(x, y, lr, trData);
            //}


            ////evaluation


            ////prediction
            //return;
        }
    }
}
