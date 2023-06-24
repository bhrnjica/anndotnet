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
using Anndotnet.Core.Interface;
using Tensorflow;
using static Tensorflow.Binding;

namespace Anndotnet.Core.Data
{
    public class Placeholders : IPlaceholders
    {
       public (Tensor X, Tensor Y) Create(Shape shape, Shape output, 
                                                TF_DataType inType= TF_DataType.TF_FLOAT, 
                                                TF_DataType outType= TF_DataType.TF_FLOAT)
        {
            //
            var X = tf.placeholder(inType, shape: shape);
            var Y = tf.placeholder(outType, shape: output);
            //
            return (X, Y);
        }

        public Tensor CreatePlaceholder(Shape shape, string name, TF_DataType inType = TF_DataType.TF_FLOAT)
        {
            Tensor X = null;
            X = tf.placeholder(inType, shape: shape, name: name);
            return X;
        }

        public (Tensor X, Tensor Y) Create(int inDim, int outDim)
        {
            Shape input = (-1, inDim);
            Shape output = (-1, outDim);
            //
            return Create(input, output);
        }
    }
}
