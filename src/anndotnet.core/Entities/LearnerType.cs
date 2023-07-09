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

public enum LearnerType
{
    SGD = 0,
    MomentumSGD = 1,
    RMSProp = 2,
    FSAdaGrad = 3,
    Adam = 4,
    AdaGrad = 5,
    AdaDelta = 6
}