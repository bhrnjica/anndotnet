﻿///////////////////////////////////////////////////////////////////////////////
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

using System;
using System.Collections.Generic;
using AnnDotNet.Core.Interfaces;

namespace AnnDotNet.Core.Entities;

public class MLConfig
{
    public Guid Id { get; set; }
    public DataParser Parser { get; set; }
    public List<ColumnInfo> Metadata { get; set; }
    public List<ILayer> Network { get; set; }
    public LearningParameters LParameters { get; set; }
    public TrainingParameters TParameters { get; set; }
    public Dictionary<string, string> Paths { get; set; }
}