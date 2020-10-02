using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public interface IEvaluator
    {
        public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed);
        public NDArray Predict(Tensor model, NDArray input);
    }
}
