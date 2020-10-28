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
            var n = (int)(((float)tp.Epoch / (float)tp.Epochs) * 100f / 5f);
            if (n == 0)
                n = 1;
            var progress = string.Join("", Enumerable.Range(1, n).Select(x => "="));
            var evalT = string.Join(" - ", tp.TrainEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
            var evalV = string.Join(" - ", tp.TrainEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
            Console.WriteLine($"Fold {tp.Fold}/{tp.KFold} \t Epoch {tp.Epoch}/{tp.Epochs} \t[{progress}] \n\r- loss:{Math.Round(tp.TrainLoss, 3)} - {evalT} - val_loss:{Math.Round(tp.TrainLoss, 3)} - {evalV}");
            Console.WriteLine();
        }
    }
}
