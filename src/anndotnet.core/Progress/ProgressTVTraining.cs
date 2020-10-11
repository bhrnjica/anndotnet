using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using Anndotnet.Core.Interface;
using NumSharp;
using Anndotnet.Core;

namespace Anndotnet.Core.Progress
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
