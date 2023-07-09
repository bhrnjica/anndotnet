﻿///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System;
using AnnDotNet.Core.Entities;
using Tensorflow;
using Tensorflow.NumPy;

namespace AnnDotNet.Core.Interfaces;

public interface ITrainer
{
    bool Run(Session session, LearningParameters lParams, TrainingParameters tParams, Func<Session, ProgressReport, Session> processModel);
   
    ProgressReport CreateProgressReport(TrainingParameters tParams, int fold, int epoch, NDArray[] resultsT, NDArray[] resultsV, string[] evalFuncs);
}