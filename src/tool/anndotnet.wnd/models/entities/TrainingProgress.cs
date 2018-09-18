//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using anndotnet.wnd.Mvvm;
using LiveCharts;
using LiveCharts.Defaults;
using System.Collections.Generic;
using ZedGraph;

namespace anndotnet.wnd.Models
{
    
    public class TrainingProgress : ObservableObject
    {
        string m_Iteration;
        public string Iteration
        {
            get
            {
                return m_Iteration;
            }
            set
            {
                if (m_Iteration != value)
                {
                    m_Iteration = value;
                    RaisePropertyChangedEvent("Iteration");
                }
            }
        }

        string m_TrainingLoss;
        public string TrainingLoss
        {
            get
            {
                return m_TrainingLoss;
            }
            set
            {
                if (m_TrainingLoss != value)
                {
                    m_TrainingLoss = value;
                    RaisePropertyChangedEvent("TrainingLoss");
                }
            }
        }


        //Evaluation of current Minibatch 
        public List<PointPair> MBLossValue { get; set; }
        public List<PointPair> MBEvaluationValue { get; set; }


        //Model evaluation against full Training and Validation set
        public List<PointPair> TrainEvalValue { get; set; }
        public List<PointPair> ValidationEvalValue { get; set; }

    }
}