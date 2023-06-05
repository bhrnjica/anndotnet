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
using System.Text.Json.Serialization;

namespace Anndotnet.Core
{
    public class TrainingParameters
    {
        public TrainingType TrainingType { get; set; }

        public EarlyStopping EarlyStopping { get; set; }

        public bool Retrain { get; set; }
        public int Epochs { get; set; }

        public int ProgressStep { get; set; }

        public int MinibatchSize { get; set; }

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
            MinibatchSize = 100;
            KFold = 5;
            SplitPercentage = 20;
            Retrain = true;
        }
    }
}
