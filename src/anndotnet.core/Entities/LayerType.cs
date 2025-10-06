////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

namespace Anndotnet.Core.Entities;


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
    Activation=15,
    
    // New layer types for improved TorchSharp support
    BatchNormalization=16,
    LayerNormalization=17,
    MaxPool1D=18,
    MaxPool2D=19,
    AvgPool2D=20,
    GlobalAvgPool=21,
    Flatten=22,
    Reshape=23,
    Attention=24
}