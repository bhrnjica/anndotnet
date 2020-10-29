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
using System;
using System.Collections.Generic;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using Anndotnet.Core;
namespace Anndotnet.Vnd
{
    public class TFBase
    {
        protected Tensor RandomValues(ValueInitializer initValueType, TensorShape shape, TF_DataType type, int seed=1234)
        {
            switch (initValueType)
            {
                case ValueInitializer.GlorotUniform:
                case ValueInitializer.GlorotNormal:
                case ValueInitializer.RandomUniform:
                case ValueInitializer.RandomNormal:
                    return tf.random_uniform(shape: shape, dtype: type, seed: seed);
            }

            return tf.random_uniform(shape: shape, dtype: type, seed: seed);
        }
    }
}
