//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET.Net.Lib
{
    public enum NetworkTypes
    {
        SimpleFeedForward=0,
        DeepFeedForward=1,
        LSTMRecurrent=2,
        EmbdLSTMRecurrent=3
    }
    public enum LearnerType
    {
        SGDLearner=0,
        MomentumSGDLearner=1,
        RMSPropLearner=2,
        FSAdaGradLearner=3,
        AdamLearner=4,
        AdaGradLearner=5,
        AdaDeltaLearner=6
    }
    public enum LossFunctions
    {
        BinaryCrossEntropy,
        CrossEntropyWithSoftmax,
        ClassificationError,
        SquaredError,
    }
    public class ActiveModelData
    {
        //Iterations
        public int IterType { get; set; }
        public float IterValue { get; set; }
        public uint MinibatchSize { get; set; }

        //Learning parameters
        public LearnerType LearnerType { get; set; }
        public float LearningRate { get; set; }
        public float Momentum { get; set; }
        public float L1Regularizer { get; set; }
        public float L2Regularizer  { get; set; }

        //network settings
        public int NetworkType { get; set; }
        public int Neurons { get; set; }
        public int HLayers { get; set; }
        public int Embeding { get; set; }
        public bool UseStabilisation { get; set; }
        public bool UseDropRate { get; set; }
        public float DropRate { get; set; }

        public Activation ActivationOutput { get; set; }
        public Activation ActivationHidden { get; set; }

        public LossFunctions LossFunction { get; set; }
        public LossFunctions EvaluationFunction { get; set; }
        public uint TrainCount { get;  set; }
        public uint TestCount { get; set; }

        public ActiveModelData()
        { }
        public ActiveModelData(ActiveModelData data)
        {
            //
            IterType = data.IterType;
            IterValue = data.IterValue;
            MinibatchSize = data.MinibatchSize;

            //
            LearnerType = data.LearnerType;
            LearningRate = data.LearningRate;
            Momentum = data.Momentum;
            L1Regularizer = data.L1Regularizer;
            L2Regularizer = data.L2Regularizer;

            //
            NetworkType = data.NetworkType;
            Neurons = data.Neurons;
            HLayers = data.HLayers;
            Embeding = data.Embeding;
            DropRate = data.DropRate;
            UseStabilisation = data.UseStabilisation;
            UseDropRate = data.UseDropRate;

            //
            ActivationHidden = data.ActivationHidden;
            ActivationOutput = data.ActivationOutput;

            //Los and Evaluation functions
            LossFunction = data.LossFunction;
            EvaluationFunction = data.EvaluationFunction;
        }
        /// <summary>
        /// Predefined default parameters values
        /// </summary>
        public static ActiveModelData GetDefaults(NetworkTypes networkType = NetworkTypes.SimpleFeedForward)
        {
            ActiveModelData data = new ActiveModelData();
            //
            data.IterType = 0;
            data.IterValue = 50000;
            data.MinibatchSize = 100;

            //
            data.LearnerType =  LearnerType.MomentumSGDLearner;
            data.LearningRate = 0.0001f;
            data.Momentum = 5f;
            data.L1Regularizer = 0;
            data.L2Regularizer = 0;

            //
            data.NetworkType = (int)networkType;
            data.Neurons = 250;
            data.HLayers = 10;
            data.Embeding = 100;
            data.DropRate = 10;
            data.UseStabilisation = false;
            data.UseDropRate = false;

            //
            data.ActivationHidden =  Activation.Softmax;
            data.ActivationOutput = Activation.None;

            //Los and Evaluation functions
            data.LossFunction = LossFunctions.SquaredError;
            data.EvaluationFunction = LossFunctions.SquaredError;
            return data;
        }

        public static ActiveModelData GetDefaults2(NetworkTypes networkType = NetworkTypes.LSTMRecurrent)
        {
            ActiveModelData data = new ActiveModelData();
            //
            data.IterType = 0;
            data.IterValue = 50000;
            data.MinibatchSize = 125;

            //
            data.LearnerType = LearnerType.MomentumSGDLearner;
            data.LearningRate = 0.000005f;
            data.Momentum = 5f;
            data.L1Regularizer = 0;
            data.L2Regularizer = 0;

            //
            data.NetworkType = (int)networkType;
            data.Neurons = 2500;
            data.HLayers = 10;
            data.Embeding = 100;
            data.DropRate = 10;
            data.UseStabilisation = false;
            data.UseDropRate = false;

            //
            data.ActivationHidden = Activation.Softmax;
            data.ActivationOutput = Activation.None;

            //Los and Evaluation functions
            data.LossFunction = LossFunctions.SquaredError;
            data.EvaluationFunction = LossFunctions.SquaredError;
            return data;
        }
    }
}
