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
using AnnDotNet.Core;
using Google.Protobuf.WellKnownTypes;
using ShellProgressBar;
using System;
using System.Linq;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using XPlot.Plotly;

namespace AnnDotNet.Tool.Progress;


public class ProgressTVTraining : IProgressTraining, IDisposable
{
    const int totalTicks = 10;
    ProgressBarOptions options;
    ProgressBar pbar;
    public ProgressTVTraining()
    {
        options = new ProgressBarOptions
        {
            ProgressCharacter = '─',
            ProgressBarOnBottom = true
        };

       

    }

    public void Dispose()
    {
        pbar.Dispose();
    }
    public void Run(ProgressReport tp)
    {
       /* if (pbar == null)
        {
            pbar = new ProgressBar(tp.Epochs, "Training-Validation Training", options);    
        }
        pbar.Tick(); //will advance pbar to 1 out of 10.
                     //we can also advance and update the progressbar text
        pbar.Tick($"Epoch {tp.Epoch} of {tp.Epochs}");

        */
        var n = (int)(((float)tp.Epoch / (float)tp.Epochs) * 100f/5f);
        
        n = n == 0 ? 1 : n;
        
        var progress = string.Join("", Enumerable.Range(1,n).Select(x=>"="));
        var evalT = string.Join(" - ", tp.TrainEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
        var evalV = string.Join(" - ", tp.ValidEval.Select(x => $"{x.Key}: {Math.Round(x.Value, 3)}"));
        
        Console.WriteLine($"Epoch {tp.Epoch}/{tp.Epochs} [{progress}] \n\r- loss:{Math.Round(tp.TrainLoss,3)} - {evalT} - val_loss:{Math.Round(tp.TrainLoss, 3)} - {evalV}");
        Console.WriteLine();



    }



}
