//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                       //
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
using CNTK;
using NNetwork.Core.Common;
using System;

namespace NNetwork.Core.Network.Modules
{
    /// <summary>
    /// Implementation of Convolution network. 
    /// </summary>
    public class Convolution : NetworkFoundation
    {

        public Function Conv1D(Variable input, int numChannels, int filter, DataType dType, DeviceDescriptor device, bool usePadding, bool useBias, string name = "", uint seed = 1)
        {
            var convolution_map_size = new int[] { numChannels, NDShape.InferredDimension, filter };
            var rtrn = Conv(convolution_map_size, input, dType, device, usePadding, useBias, name, seed);
            return rtrn;
        }

        public Function Conv2D(Variable x, int numOutChannels, int[] filterShape, DataType dType, DeviceDescriptor device, bool usePadding, bool useBias, string name = "", uint seed = 1)
        {
            if (filterShape == null || filterShape.Length < 2)
                throw new Exception("filterShape shape is not in correct size!");
            //create map 
            var convMapSize = new int[] 
                {
                    filterShape[0],
                    filterShape[1],
                    CNTK.NDShape.InferredDimension,
                    numOutChannels };

            //create convolution layer
            var rtrn = Conv(convMapSize, x, dType, device, usePadding, useBias, name, seed);

            return rtrn;
        }


        public Function Pooling2D(Variable x, int[] pShape, int stride, DataType dType, PoolingType pType, DeviceDescriptor device, string name= "", uint seed = 1)
        {
            if (pShape == null || pShape.Length < 2)
                throw new Exception("Pooling shape is not in correct size!");
            //
            var shape = new int[] { pShape[0], pShape[1] };
            var strides = new int[] { stride };

            //create convolution layer
            var rtrn = CNTKLib.Pooling(x, pType, shape, strides);

            return rtrn;
        }

        public Function Pooling1D(Variable x, int pShape, DataType dType, PoolingType pType, DeviceDescriptor device, string name = "", uint seed = 1)
        {
            if (pShape == 0 )
                throw new Exception("Pooling shape is not in correct size!");
            //
            var shape = new int[] { pShape };
            
            //create pooling layer
            var rtrn = CNTKLib.Pooling(x, pType, shape);

            return rtrn;
        }


        private Function Conv(int[] convMapSize, Variable x, DataType dType, DeviceDescriptor device, bool usePadding, bool useBias, string name, uint seed)
        {
            var shape = CNTK.NDShape.CreateNDShape(convMapSize);
            var W = Weights(shape, dType, device, seed);

             //
            var strides = new int[] { 1 };
            var sharing = new bool[] { true };
            var aPadding= new bool[] { usePadding };
            //
            var result = CNTKLib.Convolution(W, x, strides, sharing, aPadding);

            if (useBias)
            {
                var intArray = convMapSize.Length == 4 ? 
                    new int[] { 1, 1, NDShape.InferredDimension } : 
                    new int[] { 1, };

                var bShape = NDShape.CreateNDShape(intArray);

                var b = Bias(bShape, dType, device);

                result = CNTK.CNTKLib.Plus(result, b);
            }

            result = CNTK.CNTKLib.ReLU(result, name);

            return result;
        }

    }
}
