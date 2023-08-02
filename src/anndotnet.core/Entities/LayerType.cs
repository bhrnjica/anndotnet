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

using System.ComponentModel;

namespace AnnDotNet.Core.Entities;

//available network layer in the library
public enum LayerType
{

    Normalization=1,
    Scale=2,

    Dense =3,
    Embedding=4,
    Dropout =5,
    LSTM=6,
    NALU=7,
    Conv1D=8,
    Conv2D=9,
    Pooling1D=10,
    Pooling2D=11,
    CudaStackedLSTM=12,
    CudaStackedGRU=13,
    Custom=14,
    Activation=15
}