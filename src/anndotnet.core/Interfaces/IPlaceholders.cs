using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;

namespace Anndotnet.Core.Interface
{
    public interface IPlaceholders
    {
       (Tensor X, Tensor Y) Create(Shape input, Shape output, TF_DataType inType, TF_DataType outType);
       (Tensor X, Tensor Y) Create(int inDim, int outDim);
    }
}
