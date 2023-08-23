///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//            Copyright 2017-2021 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                           //
//             For feedback:https://github.com/bhrnjica/anndotnet/issues     //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace AnnDotNet.Core.Entities;

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