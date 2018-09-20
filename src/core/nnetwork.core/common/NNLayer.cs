//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
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
        Dense,
        Embedding,
        Drop,
        LSTM,
        Custom,
    }



    /// <summary>
    /// ANN Network model class 
    /// </summary>
    public class NNLayer
    {
        public int Id { get; set; }

        //layer type (dense, LSTM, drop, ...)
        public LayerType Type { get; set; }

        //name of a layer
        public string Name { get; set; }

        //Output dimension for the layer (Dense, Embedding, LSTM)
        public int HDimension { get; set; }

        //LSTM Cell dimension 
        public int CDimension { get; set; }

        //Parameter used in DropLayer (as dropRate)
        public int Value { get; set; }

        //Activation function (in case of LSTM this is TanH activation)
        public Activation Activation { get; set; }

        //for LSTM layer only
        public bool SelfStabilization { get; set; }

        //for LSTM layer only
        public bool Peephole { get; set; }

        //is activation function may be used in the layer
        public bool UseActivation { get; set; }
    }
}