using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface ITrainer
    {
        public bool Run(Tensor x, Tensor y, Learner lr, TrainingParameters tr);
        public bool RunOffline(Tensor x, Tensor y, Learner lr, TrainingParameters tr);
    }
}
