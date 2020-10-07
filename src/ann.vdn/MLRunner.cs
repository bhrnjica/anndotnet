using AnnDotNET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
namespace ann.vdn
{
    public class MLRunner
    {
        public static void Run(string mlconfigPath)
        {
            var retVal = MLFactory.LoadMLConfiguration(mlconfigPath);

            (var xData, var yData) = MLFactory.PrepareData(retVal["metadata"], retVal["paths"]);
            //var f = MLFactory.CreateMLFactory(retVal);

            (Tensor x, Tensor y) = MLFactory.CreatePlaceholders(xData, yData);

            var z = MLFactory.CreateNetworkModel(retVal["network"], x, y);

            //create learning params
            var strLearning = retVal["learning"];
            LearningParameters lrData = MLFactory.CreateLearningParameters(strLearning);

            //create training param
            var strTraining = retVal["training"];
            TrainingParameters trData = MLFactory.CreateTrainingParameters(strTraining);


            //define learner
            AnnLearner lr= null;
            if(y.dims.Last() > 1)
            {
                var learner = new ClassificationLearner();
                lr = learner.Create(y, z, new LearningParameters());
            }
            else
            {
                var learner = new RegressionLearner();
                lr = learner.Create(y, z, new LearningParameters());
            }
            

            //training process
            if (trData.TrainingType == TrainingType.TVTraining)
            {
                var tr = new TVTrainer(xData, yData, 1);
                tr.Run(x, y, lr, trData);
            }
            else
            {
                var tr = new CVTrainer(xData, yData, trData.KFold);
                tr.Run(x, y, lr, trData);
            }


            //evaluation


            //prediction
            return;
        }
    }
}
