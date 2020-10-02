using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public class Evaluator : IEvaluator
    {
        public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed)
        {
            return true;
        }
        public NDArray Predict(Tensor model, NDArray input)
        {
            return null;
        }
    }
}
