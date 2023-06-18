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
using System.Collections.Generic;
using System.Text;

namespace Anndotnet.Core.Entities
{
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
}
