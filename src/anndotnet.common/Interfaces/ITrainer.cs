using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public interface ITrainer
    {
        public bool Train(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr, DataFeed dFeed);
        public bool TrainOffline(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr, DataFeed dFeed);
    }
}
