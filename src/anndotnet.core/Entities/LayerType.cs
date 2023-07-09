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

//available network layer in the library
public enum LayerType
{
    Normalization,
    Scale,
    Dense,
    Embedding,
    Drop,
    LSTM,
    NALU,
    Conv1D,
    Conv2D,
    Pooling1D,
    Pooling2D,
    CudaStackedLSTM,
    CudaStackedGRU,
    Custom,
    Activation
}