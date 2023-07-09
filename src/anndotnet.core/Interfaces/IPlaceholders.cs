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

namespace AnnDotNet.Core.Interfaces;

public interface IPlaceholders
{
    (Tensor X, Tensor Y) Create(Shape input, Shape output, TF_DataType inType, TF_DataType outType);
    (Tensor X, Tensor Y) Create(int inDim, int outDim);
}