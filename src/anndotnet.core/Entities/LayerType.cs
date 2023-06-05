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
namespace Anndotnet.Core
{
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
}