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
using System.Text.Json.Serialization;

namespace AnnDotNet.Core.Entities;

public class TrainingParameters
{
    public TrainingType TrainingType { get; set; }

    public EarlyStopping EarlyStopping { get; set; }

    public bool Retrain { get; set; }
    public int Epochs { get; set; }

    public int ProgressStep { get; set; }

    public int MiniBatchSize { get; set; }

    public int KFold { get; set; }

    public int SplitPercentage { get; set; }

    [JsonIgnore]
    public Action<ProgressReport> Progress { get; set; }
    public string LastBestModel { get; set; }

    public TrainingParameters()
    {
        Progress = null;
        TrainingType = TrainingType.TVTraining;
        EarlyStopping = EarlyStopping.None;
        Epochs = 500;
        ProgressStep = 10;
        MiniBatchSize = 100;
        KFold = 5;
        SplitPercentage = 20;
        Retrain = true;
    }
}