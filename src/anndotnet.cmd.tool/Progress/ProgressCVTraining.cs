////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////


using System;
using System.Linq;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interfaces;

namespace Anndotnet.Tool.Progress;

public class ProgressCvTraining : IProgressTraining
{
    public void Run(ProgressReport tp)
    {
        var n = (int)(((float)tp.Epoch / (float)tp.Epochs) * 100f / 5f);

        n = n == 0 ? 1 : n; 
            
        var progress = string.Join("", Enumerable.Range(1, n).Select(x => "="));

        var evalT = string.Join(" - ", tp.TrainEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
        var evalV = string.Join(" - ", tp.ValidEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
            
        Console.WriteLine($"Fold {tp.Fold}/{tp.KFold} \t Epoch {tp.Epoch}/{tp.Epochs} \t[{progress}] \n\r- loss:{Math.Round(tp.TrainLoss, 3)} - {evalT} - val_loss:{Math.Round(tp.ValidLoss, 3)} - {evalV}");
        Console.WriteLine();
    }

}