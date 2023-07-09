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

public class TrainingHistory
{
    public List<TrainingEvent> History { get; set; }
}

public class TrainingEvent
{
    public int Id { get; set; }
    public string ModelName { get; set; }
    public float Loss { get; set; }
    public Dictionary<string, float> Evals { get; set; }
}