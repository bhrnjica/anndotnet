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
        public bool Run(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr);
        public bool RunOffline(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr);
    }
}
