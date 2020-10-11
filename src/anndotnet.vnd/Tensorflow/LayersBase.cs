using System;
using System.Collections.Generic;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using Anndotnet.Core;
namespace Anndotnet.Vnd
{
    public class LayersBase
    {
        protected Tensor RandomValues(ValueInitializer initValueType, TensorShape shape, TF_DataType type)
        {
            switch (initValueType)
            {
                case ValueInitializer.GlorotUniform:
                case ValueInitializer.GlorotNormal:
                case ValueInitializer.RandomUniform:
                case ValueInitializer.RandomNormal:
                    return tf.random_uniform(shape: shape, dtype: type);
            }

            return tf.random_uniform(shape: shape, dtype: type);
        }
    }
}
