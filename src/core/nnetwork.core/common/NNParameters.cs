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
using CNTK;
using System.Collections.Generic;

namespace NNetwork.Core.Common
{
    public delegate void TrainingProgress(ProgressData progress);
    public delegate Function CreateCustomModel(List<Variable> variables, DeviceDescriptor device);

    public class TrainResult
    {
      public ProcessState ProcessState { get; set; }
      public string BestModelFile { get; set; }
      //last iteration training process stopped/interrupted/completed
      public int Iteration { get; set; }
      //training history file
      public string TrainingHistoryFile { get; set; }
    }
    public enum ProcessState
    {
        Stopped,
        Compleated,
        Crashed,
        Unknown
    }
    public enum ProcessDevice
    {
        Default = 0,
        CPU = 1,
        GPU = 2
    }

    public enum Activation
    {
        None = 0,
        ReLU = 1,
        Sigmoid = 2,
        Softmax = 3,
        TanH = 4
    }

    public enum MinibatchType
    {
        Default = 0,
        Custom = 1,
    }

    public enum NetworkTypes
    {
        FeedForward = 0,
        DeepFF = 1,
        LSTMRecurrent = 2,
        EmbeddedLSTM = 3
    }

    public enum LearnerType
    {
        SGDLearner = 0,
        MomentumSGDLearner = 1,
        RMSPropLearner = 2,
        FSAdaGradLearner = 3,
        AdamLearner = 4,
        AdaGradLearner = 5,
        AdaDeltaLearner = 6
    }

    public enum EFunction
    {
        SquaredError,
        ClassificationError,
        ClassificationAccuracy,
        BinaryCrossEntropy,
        CrossEntropyWithSoftmax,
        RMSError,
        MSError,
    }
    public class LearningParameters
    {
        //Learning parameters
        public EFunction LossFunction { get; set; }
        public EFunction EvaluationFunction { get; set; }
        public LearnerType LearnerType { get; set; }
        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        public double L1Regularizer { get; set; }
        public double L2Regularizer { get; set; }
    }

    public class TrainingParameters
    {
        public MinibatchType Type { get; set; }
        public int Epochs { get; set; }
        public uint BatchSize { get; set; }
        public string[] Normalization { get; set; }
        public bool RandomizeBatch { get; set; }
        public int ProgressFrequency { get; set; }
        
        public bool ContinueTraining { get; set; }
        public bool EarlyStopping { get; set; }

        public bool SaveModelWhileTraining { get; set; }

        public bool FullTrainingSetEval { get; set; }
        public string ModelTempLocation { get; set; }
        public string ModelFinalLocation { get; set; }

        //this is root path
        public string ModelFolderPath { get; set; }
        public string LastBestModel { get; set; }
    }
}
