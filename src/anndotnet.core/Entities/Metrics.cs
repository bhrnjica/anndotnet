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

namespace Anndotnet.Core
{
    public enum Metrics
    {
        AE,//absolute error
        RMSE,//root mean squared error
        MSE,//mean squared error
        CAcc,//Classification accuracy
        CErr,//classification error
        Precision,//precision
        Recall,//recall
        SE,//squared error
        BCE,//binary cross entropy
        CCE,//classification cross entropy

    }
}
