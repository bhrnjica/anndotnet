//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //                //
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
    /// Class implementation of Neural Arithmetic Logic Units (NALU) which
    /// can learn to track time, perform arithmetic over images of numbers, translate numerical 
    /// language into real-valued scalars, execute computer code, and count objects in 
    /// images https://arxiv.org/abs/1808.00508 
    /// </summary>
    public class NALU : NetworkFoundation
    {
        /// <summary>
        /// Input Variable 
        /// </summary>
        public Variable X { get; set; }

        /// <summary>
        /// NALU Output 
        /// </summary>
        public Function H { get; set; }

        
        public NALU()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="outputDim"></param>
        /// <param name="dataType"></param>
        /// <param name="device"></param>
        /// <param name="seed"></param>
        public NALU(Variable x, int outputDim, DataType dataType, DeviceDescriptor device, uint seed, string name = "")
        {
            var inputDim = x.Shape[0];
            var error = new Constant(new NDShape(0), dataType, 1e-10);
            var one = new Constant(new NDShape(0), dataType, 1.0f);

            var W_hat = Weights(inputDim,outputDim, dataType,device, seed, "w_hat");
            var M_hat = Weights(inputDim, outputDim, dataType, device, seed, "m_hat");
            var G = Weights(inputDim, outputDim, dataType, device, seed, "g");
            
            //first construct NAC
            var W = CNTKLib.ElementTimes(CNTKLib.Tanh(W_hat), CNTKLib.Sigmoid(M_hat));
            Variable a = CNTKLib.Times(W, x);
            Variable g = CNTKLib.Sigmoid(CNTKLib.Times(G, x));
            
            var t2 = CNTKLib.Log(CNTKLib.Abs(x) + error);
            var t1 = CNTKLib.Times(W, t2);
            Variable m = CNTKLib.Exp(t1);

            //construct NALU terms
            var o1 = CNTKLib.ElementTimes(g,a);
            var o2 = CNTKLib.ElementTimes(CNTKLib.Minus(one, g), m);

            //NALU output
            var n = string.IsNullOrEmpty(name) ? "NALU" : name;
            var output = CNTKLib.Plus(o1 , o2, n);

            //initialize output 
            X = x;
            H= output;
        }


    }
}
