//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                          //
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
namespace NNetwork.Core.Network.Modules
{
    /// <summary>
    /// Implementation of Embedding layer. 
    /// </summary>
    public class ScalingLayer// : NetworkFoundation
    {
        /// <summary>
        /// Create scaling layer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="embeddingDim"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Function Scale(Variable x, int value1, int value2, DeviceDescriptor device, string name= "scaleL")
        {
            var scaleFactor = CNTK.Constant.Scalar<float>(value1/(float)value2, device);
            var net  = (Variable)CNTK.CNTKLib.ElementTimes(scaleFactor, x);
            return net;
        }
    }
}
