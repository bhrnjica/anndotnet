using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using Anndotnet.Core;
using Anndotnet.Core.Interface;

namespace Anndotnet.Core.Progress
{
    public class ProgressCVTraining : IProgressTraining
    {
        public void Run(TrainingProgress tp)
        {
            if (tp.ProgressType == ProgressType.Initialization)
                Console.WriteLine($"_________________________________________________________");

            Console.WriteLine($"Fold={tp.FoldIndex}, Iteration={tp.Iteration}, Loss={Math.Round(tp.TrainLoss, 3)}, Eval={Math.Round(tp.TrainEval, 3)}");

            if (tp.ProgressType == ProgressType.Completed)
                Console.WriteLine($"_________________________________________________________");

        }
    }
}
