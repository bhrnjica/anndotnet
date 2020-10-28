using Daany.MathStuff;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Tensorflow;

namespace Anndotnet.Core
{
    public enum Metrics
    {
        AE,//absolute error
        RMSE,//root mean squared error
        MSE,//mean squared error
        CAcc,//Classification accuracy
        CErr,//classification error
        Precision,//precision
        Recall,//recall
        SE,//squared error
        BCE,//binary cross entropy
        CCE,//classification cross entropy

    }

    public enum ProgressType
    {
        Initialization,
        Training,
        Completed,
    }
    public enum ValueInitializer
    {
        GlorotUniform,
        GlorotNormal,
        RandomUniform,
        RandomNormal,
    }

    public enum EarlyStopping
    {
        None,
    }
    public enum TrainingType
    {
        TVTraining,
        CVTraining,
    }
    public class Learner
    {
        public List<Tensor> Evals { get; set; }
        public Tensor Loss { get; set; }
        public Operation Optimizer { get; set; }

        public Learner()
        {
            Evals = new List<Tensor>();
        }
    }

    public class LearningParameters
    {
        public LearnerType LearnerType { get; set; }
        public Metrics LossFunction { get; set; }
        public List<Metrics> EvaluationFunctions { get; set; }
        public float LearningRate { get; set; }
        public double Momentum { get; set; }
        public double L1Regularizer { get; set; }
        public double L2Regularizer { get; set; }
    }

    public enum LearnerType
    {
        SGD = 0,
        MomentumSGD = 1,
        RMSProp = 2,
        FSAdaGrad = 3,
        Adam = 4,
        AdaGrad = 5,
        AdaDelta = 6
    }

    public class TrainingParameters
    {
        public TrainingType TrainingType { get; set; }

        public EarlyStopping EarlyStopping { get; set; }

        public bool Retrain { get; set; }
        public int Epochs { get; set; }

        public int ProgressStep { get; set; }

        public int MinibatchSize { get; set; }

        public int KFold { get; set; }

        public int SplitPercentage { get; set; }
        [JsonIgnore]
        public Action<TrainingProgress> Progress { get; set; }
        public string LastBestModel { get; set; }

        public TrainingParameters()
        {
            Progress = null;
            TrainingType = TrainingType.TVTraining;
            EarlyStopping = EarlyStopping.None;
            Epochs = 500;
            ProgressStep = 10;
            MinibatchSize = 100;
            KFold = 5;
            SplitPercentage = 20;
            Retrain = true;
        }
    }

    public class TrainingProgress
    {
        public ProgressType ProgressType { get; set; }
        public int Fold { get; set; }
        public int KFold { get; set; }
        public int Epoch { get; set; }
        public int Epochs { get; set; }
        public float TrainLoss { get; set; }
        public Dictionary<string, float> TrainEval { get; set; }
        public float ValidLoss { get; set; }
        public Dictionary<string, float> ValidEval { get; set; }

    }
   //public  class Ann
   // {
   //     //placeholders
   //     Tensor x { get; set; }
   //     Tensor y { get; set; }
   //     //model
   //     Tensor z { get; set; }
   // }

}
