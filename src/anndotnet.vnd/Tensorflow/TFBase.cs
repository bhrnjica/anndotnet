///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using Tensorflow;
using static Tensorflow.Binding;
using AnnDotNet.Core;
using AnnDotNet.Core.Entities;

namespace AnnDotNet.Vnd;

public class TFBase
{
    protected Tensor RandomValues(ValueInitializer initValueType, Shape shape, TF_DataType type,float mean = 0, float stddev=0, int? seed=1234)
    {
        switch (initValueType)
        {
            case ValueInitializer.GlorotUniform:
                return tf.glorot_uniform_initializer.Apply(new InitializerArgs(shape: shape, dtype: type));
            case ValueInitializer.GlorotNormal:
                return tf.truncated_normal_initializer().Apply(new InitializerArgs(shape: shape, dtype: type));
            case ValueInitializer.RandomUniform:
                return tf.random_uniform(shape: shape, dtype: type, seed: seed);    
            case ValueInitializer.RandomNormal:
                return tf.random_normal_initializer(mean:mean, stddev:stddev, dtype: type, seed: seed).Apply(new InitializerArgs(shape: shape, dtype: type));
        }

        return tf.random_uniform(shape: shape, dtype: type, seed: seed);
    }
}