////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Text.Json.Serialization;

namespace Anndotnet.Core.Entities;

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

    public string LastBestModel { get; set; }
    public bool ShuffleWhenSplit { get; set; }
    public bool ShuffleWhenTraining{ get; set; }

public TrainingParameters()
    {
        TrainingType = TrainingType.TvTraining;
        EarlyStopping = EarlyStopping.None;
        Epochs = 500;
        ProgressStep = 1;
        MiniBatchSize = 70;
        KFold = 5;
        SplitPercentage = 80;
        ShuffleWhenTraining= false;
        ShuffleWhenSplit = true;
        Retrain = true;
    }
}