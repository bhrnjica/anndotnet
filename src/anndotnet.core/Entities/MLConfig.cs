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

using System;
using System.Collections.Generic;
using AnnDotNet.Core.Interfaces;

namespace AnnDotNet.Core.Entities;

public class MlConfig
{
    public Guid Id { get; set; }
    public DataParser Parser { get; set; }
    public List<ColumnInfo> Metadata { get; set; }
    public List<ILayer> Network { get; set; }
    public LearningParameters LearningParameters { get; set; }
    public TrainingParameters TrainingParameters { get; set; }
    public Dictionary<string, string> Paths { get; set; }
    public string Name { get; set; }

    public MlConfig()
    {
        Id = Guid.NewGuid();
        Name = "mlConfig";
        Metadata = new List<ColumnInfo>();
        Network = new List<ILayer>();
        LearningParameters = new LearningParameters();
        TrainingParameters = new TrainingParameters();
        Paths = new Dictionary<string, string>();
    }
    public MlConfig(string guid, string name)
    {
        Id = Guid.Parse(guid);
        Name = name;
        Metadata = new List<ColumnInfo>();
        Network = new List<ILayer>();
        LearningParameters = new LearningParameters();
        TrainingParameters = new TrainingParameters();
        Paths = new Dictionary<string, string>();
    }   
}