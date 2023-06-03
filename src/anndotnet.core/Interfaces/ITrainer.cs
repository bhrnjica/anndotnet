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
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface ITrainer
    {
        bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel);
        // public bool RunOffline(Tensor x, Tensor y, Learner lr, TrainingParameters tr, Dictionary<string,string> modelPaths);
    }
}
