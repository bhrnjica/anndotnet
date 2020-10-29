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
using Anndotnet.Core;
using Anndotnet.Vnd.Layers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anndotnet.Vnd
{
    public class MLConfig
    {
        public Guid Id { get; set; }
        public DataParser Parser { get; set; }
        public List<ColumnInfo> Metadata { get; set; }
        public List<LayerBase> Network { get; set; }
        public LearningParameters LParameters { get; set; }
        public TrainingParameters TParameters { get; set; }
        public Dictionary<string, string> Paths { get; set; }
    }
}
