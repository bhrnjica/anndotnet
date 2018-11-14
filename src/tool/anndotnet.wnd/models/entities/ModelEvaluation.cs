//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using anndotnet.wnd.Mvvm;
using ANNdotNET.Core;
using System.Collections.Generic;
using ZedGraph;

namespace anndotnet.wnd.Models
{

    public class ModelEvaluation : ObservableObject
    {
        public List<PointPair> TrainingValue { get; set; }
        public List<PointPair> ValidationValue { get; set; }
        public List<PointPair> ModelValueTraining { get; set; }
        public List<PointPair> ModelValueValidation { get; set; }
        public int ModelOutputDim { get; internal set; }
        public List<string> Classes { get; internal set; }

        public ModelPerformance TrainPerformance { get; internal set; }
        public ModelPerformance ValidationPerformance { get; internal set; }

        
    }
}