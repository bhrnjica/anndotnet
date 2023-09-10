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