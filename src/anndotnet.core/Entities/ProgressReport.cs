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
using System.Collections.Generic;

namespace Anndotnet.Core
{
    public class ProgressReport
    {
        public ProgressType ProgressType { get; set; }
        public int Fold { get; set; }
        public int KFold { get; set; }
        public int Epoch { get; set; }
        public int Epochs { get; set; }
        public float TrainLoss { get; set; }
        public Dictionary<string, float> TrainEval { get; set; }
        public float ValidLoss { get; set; }
        public Dictionary<string, float> ValidEval { get; set; }

    }
}
