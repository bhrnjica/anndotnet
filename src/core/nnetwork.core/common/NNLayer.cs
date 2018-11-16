//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
namespace NNetwork.Core.Common
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
        Polling1D,
        Polling2D,
        CudaStackedLSTM,
        CudaStackedGRU,
        Custom,
    }



    /// <summary>
    /// Generic ANN Layer class for holding information about ANN Layer (Dense, Embedding, LSTM, Pooling, Convolution, etc...) 
    /// </summary>
    public class NNLayer
    {
        public int Id { get; set; }

        //layer type (dense, LSTM, drop, ...)
        public LayerType Type { get; set; }

        //name of a layer
        public string Name { get; set; }

        //Output dimension for the layer (Dense, Embedding, LSTM, CudaLSTM, GRU, TanH and ReLU)
        public int Param1 { get; set; }

        //LSTM Cell dimension, number of layers for StackedLSTM, GRU , TanH and ReLU, Pooling and ConvLayers
        public int Param2 { get; set; }

        //Parameter used in DropLayer (as dropRate), Pooling and ConvLayers
        public int Param3 { get; set; }

        //Activation function (in case of LSTM this is TanH activation)
        public Activation FParam { get; set; }

        //LSTM layer stabilization, Pooling and Conv layers only
        public bool BParam1 { get; set; }

        //for LSTM layer only 
        public bool BParam2 { get; set; }

        //is activation function may be used in the layer
        public bool UseFParam { get; set; }
    }
}