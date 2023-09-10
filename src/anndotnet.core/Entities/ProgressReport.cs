////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace Anndotnet.Core.Entities;

public class ProgressReport
{
    public ProgressType ProgressType { get; set; }
    
    public int Fold { get; set; }
    public int KFold { get; set; }
    
    public int Epoch { get; set; }
    public int Epochs { get; set; }
    
    public float TrainLoss { get; set; }
    public float ValidLoss { get; set; }

    public Dictionary<string, float> TrainEval { get; set; }
    public Dictionary<string, float> ValidEval { get; set; }
}