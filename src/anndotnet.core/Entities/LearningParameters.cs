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

public class LearningParameters
{
    public LearnerType LearnerType { get; set; }
    public LossFunction LossFunction { get; set; }
    public List<EvalFunction> EvaluationFunctions { get; set; }
    public float LearningRate { get; set; }
    public bool UsePolyDecay { get; set; }
    public float StartLRate { get; set; }
    public float EndLRate { get; set; }
    public float DecayPower { get; set; }
    public float DecaySteps { get; set; }
    public double Momentum { get; set; }
    public double L1Regularizer { get; set; }
    public double L2Regularizer { get; set; }
    public double Alpha { get; set; }
    public double Eps { get; set; }
    public double Beta1 { get; set; }
    public double Beta2 { get; set; }
    public double WeightDecay { get; set; }
    public double Rho { get; set; }
}