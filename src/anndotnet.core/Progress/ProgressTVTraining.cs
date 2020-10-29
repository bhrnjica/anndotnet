//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
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
        public void Run(ProgressReport tp)
        {
            var n = (int)(((float)tp.Epoch / (float)tp.Epochs) * 100f/5f);
            if (n == 0)
                n = 1;
            var progress = string.Join("", Enumerable.Range(1,n).Select(x=>"="));
            var evalT = string.Join(" - ", tp.TrainEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
            var evalV = string.Join(" - ", tp.ValidEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
            Console.WriteLine($"Epoch {tp.Epoch}/{tp.Epochs} [{progress}] \n\r- loss:{Math.Round(tp.TrainLoss,3)} - {evalT} - val_loss:{Math.Round(tp.TrainLoss, 3)} - {evalV}");
            Console.WriteLine();

        }
    }
}
