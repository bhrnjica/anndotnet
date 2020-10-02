using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public interface ILearner
    {
       public AnnLearner Create(Tensor y, Tensor model, TrainingParameters par);
    }
}
