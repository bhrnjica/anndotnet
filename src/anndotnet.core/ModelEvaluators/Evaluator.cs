﻿//////////////////////////////////////////////////////////////////////////////////////////
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
using Anndotnet.Core.Data;
using Anndotnet.Core.Interface;
using System;
using Tensorflow;
using Tensorflow.NumPy;
namespace AnndotnET.Core.Evaluators
{
    public class Evaluator : IEvaluator
    {

        public bool Evaluate(Tensor model, Tensor y, DataFeed dFeed)
        {
            throw new NotImplementedException();
        }

        public NDArray Predict(Tensor model, NDArray input)
        {
            return null;
        }
    }
}
