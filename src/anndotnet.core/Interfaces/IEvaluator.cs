using Anndotnet.Core.Data;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface IEvaluator
    {
        public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed);
        public NDArray Predict(Tensor model, NDArray input);
    }
}
