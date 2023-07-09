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

using AnnDotNet.Core.Interfaces;
namespace AnnDotNet.Core.Data;

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
using Tensorflow;
using static Tensorflow.Binding;


public class Placeholders : IPlaceholders
{
    public (Tensor X, Tensor Y) Create(Shape shape, Shape output,
        TF_DataType inType = TF_DataType.TF_FLOAT,
        TF_DataType outType = TF_DataType.TF_FLOAT)
    {
        var xPlaceholder = tf.placeholder(inType, shape: shape);
        var yPlaceholder = tf.placeholder(outType, shape: output);
     
        return (xPlaceholder, yPlaceholder);
    }

    public Tensor CreatePlaceholder(Shape shape, string name, TF_DataType inType = TF_DataType.TF_FLOAT)
    {
        var placeholder = tf.placeholder(inType, shape: shape, name: name);

        return placeholder;
    }

    public (Tensor X, Tensor Y) Create(int inDim, int outDim)
    {
        Shape input = (-1, inDim);
        Shape output = (-1, outDim);
        
        return Create(input, output);
    }
}