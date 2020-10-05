using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace AnnDotNET.Common
{
    public class ProgressTVTraining : IProgressTraining
    {
        public void Run(TrainingProgress tp)
        {
            if (tp.ProgressType == ProgressType.Initialization)
                Console.WriteLine($"_________________________________________________________");

            Console.WriteLine($"Iteration={tp.Iteration}, Loss={Math.Round(tp.TrainLoss, 3)}, Eval={Math.Round(tp.TrainEval, 3)}");

            if (tp.ProgressType == ProgressType.Completed)
                Console.WriteLine($"_________________________________________________________");

        }
    }
}
