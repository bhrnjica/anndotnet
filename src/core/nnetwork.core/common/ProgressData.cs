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

namespace NNetwork.Core.Common
{
    /// <summary>
    /// Class passed by progress writer callback, which contains information about current iteration of the trainer
    /// </summary>
    public class ProgressData
    {
        //total number of epoch. It is constant during training. 
        public int EpochTotal { get; set; }

        //Current epoch
        public int EpochCurrent { get; set; }

        //The name of the evaluation function
        public string EvaluationFunName { get; set; }

        //Evaluation value of the current mini-batch
        public double MinibatchAverageEval { get; set; }

        //Loss value of the current mini-batch
        public double MinibatchAverageLoss { get; set; }

        //Evaluation value of the training dataset
        public double TrainEval { get; set; }

        //Evaluation value of the validation dataset
        public double ValidationEval { get; set; }

        ////the file path of the best found model within the current training process
        //public string BestModel { get; set; }
    }
}
