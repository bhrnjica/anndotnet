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

namespace AnnDotNet.Core.Entities;

public enum Activation
{
    None = 0,
    ReLU = 1,
    ELU = 2,

    TanH = 20,
    Tanhshrink=21,
    //Threshold =22,


    Sigmoid = 50,
    Softmax = 51,
    LogSoftmax = 52,
    

    Max = 80,
    Avg = 61
}