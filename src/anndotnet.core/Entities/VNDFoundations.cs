using Daany.MathStuff;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core
{
    public enum Metrics
    {
        SquaredError,
        RMSError,
        MSError,
        ClassificationError,
        ClassificationAccuracy,
        BinaryCrossEntropy,
        CrossEntropyWithSoftmax,
        
    }

    public enum Losses
    {
        SquaredError,
        BinaryCrossEntropy,
        ClassificationCrossEntroy,
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
        public Losses LossFunction { get; set; }
        public List<Metrics> EvaluationFunctions { get; set; }
        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        public double L1Regularizer { get; set; }
        public double L2Regularizer { get; set; }
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

    public class TrainingParameters
    {
        public TrainingType TrainingType { get; set; }

        public EarlyStopping EarlyStopping { get; set; }

        public int Epochs { get; set; }

        public int ProgressStep { get; set; }

        public int MinibatchSize { get; set; }

        public int KFold { get; set; }

        public int SplitPercentage { get; set; }

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
        }
    }

    public class TrainingProgress
    {
        public ProgressType ProgressType { get; set; }
        public int FoldIndex { get; set; }
        public int Iteration { get; set; }
        public float TrainLoss { get; set; }
        public float TrainEval { get; set; }
        public float ValidLoss { get; set; }
        public float ValidEval { get; set; }
    }
   public  class Ann
    {
        //placeholders
        Tensor x { get; set; }
        Tensor y { get; set; }
        //model
        Tensor z { get; set; }
    }

}
