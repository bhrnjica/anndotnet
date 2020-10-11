using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface ILearner
    {
       public AnnLearner Create(Tensor y, Tensor model, LearningParameters par);
    }
}
