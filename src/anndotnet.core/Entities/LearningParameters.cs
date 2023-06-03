﻿//////////////////////////////////////////////////////////////////////////////////////////
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
    public class LearningParameters
    {
        public LearnerType LearnerType { get; set; }
        public Metrics LossFunction { get; set; }
        public List<Metrics> EvaluationFunctions { get; set; }
        public float LearningRate { get; set; }
        public bool UsePolyDecay { get; set; }
        public float StartLRate { get; set; }
        public float EndLRate { get; set; }
        public float DecayPower { get; set; }
        public float DecaySteps { get; set; }
        public double Momentum { get; set; }
        public double L1Regularizer { get; set; }
        public double L2Regularizer { get; set; }
    }
}