using Anndotnet.Core.Data;
using Anndotnet.Core.Interface;
using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnndotnET.Core.Evaluators
{
    public class Evaluator : IEvaluator
    {


        public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed)
        {
            throw new NotImplementedException();
        }

        public NDArray Predict(Tensor model, NDArray input)
        {
            return null;
        }
    }
}
