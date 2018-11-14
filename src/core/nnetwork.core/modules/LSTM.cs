//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
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

namespace NNetwork.Core.Network.Modules
{
    /// <summary>
    /// Class implementation of the typical LSTM cell based on Hochreiter & Schmidhuber (1997) paper
    /// http://www.bioinf.jku.at/publications/older/2604.pdf.
    /// Details of LSTM network can be found here: http://colah.github.io/posts/2015-08-Understanding-LSTMs/
    /// </summary>
    public class LSTM : NetworkFoundation
    {
        /// <summary>
        /// LSTM Cell Input 
        /// </summary>
        public Variable X { get; set; }

        /// <summary>
        /// LSTM Cell Output 
        /// </summary>
        public Function H { get; set; }

        /// <summary>
        /// LSTM Cell State
        /// </summary>
        public Function C { get; set; }

        /// <summary>
        /// Default constructor. Use this constructor in case you want to create specific LSTM component.
        /// </summary>
        public LSTM()
        {

        }


        /// <summary>
        /// Create typical LSTM cell.
        /// </summary>
        /// <param name="input">Input variable.</param>
        /// <param name="outputDim">Placeholder for previous output.</param>
        /// <param name="cellDim">Dimension of the LSTM cell.</param>
        /// <param name="dataType">Type of data.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="actFun">Type of activation function for update cell state.</param>
        /// <param name="usePeephole">Include peephole connection in the gate.</param>
        /// <param name="useStabilizer">Use self stabilization for output.</param>
        /// <param name="seed">Random seed.</param>
        public LSTM(Variable input, Variable dh, Variable dc, DataType dataType, Activation actFun, bool usePeephole, bool useStabilizer, uint seed, DeviceDescriptor device)
        {
            //create cell state
            var c = CellState(input, dh, dc, dataType, actFun, usePeephole, useStabilizer, device, ref seed);

            //create output from input and cell state
            var h = CellOutput(input, dh, c, dataType, device, useStabilizer, usePeephole, actFun, ref seed);

            //initialize properties
            X = input;
            H = h;
            C = c;
        }

        /// <summary>
        /// Create a LSTM Output for given input, previous output and previous cell state.
        /// </summary>
        /// <param name="input">Input variable.</param>
        /// <param name="ht_1">Placeholder for previous output.</param>
        /// <param name="ct">Placeholder for previous cell state.</param>
        /// <param name="dataType">Type of data.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="useStabilizer">Use self stabilization for output.</param>
        /// <param name="usePeephole">Include peephole connection in the gate.</param>
        /// <param name="actFun">Type of activation function for update cell state.</param>
        /// <param name="seed">Random seed.</param>
        /// <returns></returns>
        public Function CellOutput(Variable input, Variable ht_1, Variable ct, DataType dataType, DeviceDescriptor device, 
            bool useStabilizer, bool usePeephole, Activation actFun ,ref uint seed)
        {
            var ot = AGate(input, ht_1, ct, dataType, usePeephole, useStabilizer, device, ref seed, "OutputGate");

            //apply activation function to cell state
            var tanHCt = AFunction(ct, actFun, "TanHCt");

            //calculate output
            var ht = CNTKLib.ElementTimes(ot, tanHCt,"Output");

            //create output layer in case different dimensions between cell and output
            var c = ct;
            Function h = null;
            if (ht_1.Shape[0] != ct.Shape[0])
            {
                //rectified dimensions by adding linear layer
                var so = !useStabilizer? ct : Stabilizer(ct, device);
                var wx_b = Weights(ht_1.Shape[0], dataType, device, seed++);
                h = wx_b * so;
            }
            else
                h = ht;

            return h;
        }

        /// <summary>
        /// Create a LSTM Cell state for a given input, previous output, and previous cell state.
        /// </summary>
        /// <param name="x">Input variable.</param>
        /// <param name="ht_1">Placeholder for previous output.</param>
        /// <param name="ct_1">Placeholder for previous cell state.</param>
        /// <param name="dataType">Type of data.</param>
        /// <param name="activationFun">Type of activation function for update cell state.</param>
        /// <param name="usePeephole">Include peephole connection in the gate.</param>
        /// <param name="useStabilizer">Use self stabilization for output.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="seed">Random seed.</param>
        /// <returns></returns>
        public Function CellState(Variable x, Variable ht_1, Variable ct_1, DataType dataType, 
            Activation activationFun, bool usePeephole, bool useStabilizer, DeviceDescriptor device, ref uint seed)
        {
            var ft = AGate(x, ht_1, ct_1, dataType, usePeephole, useStabilizer, device, ref seed, "ForgetGate");
            var it = AGate(x, ht_1, ct_1, dataType, usePeephole, useStabilizer, device, ref seed, "InputGate");
            var tan = Gate(x, ht_1, ct_1.Shape[0], dataType, device, ref seed);

            //apply Tanh (or other) to gate
            var tanH = AFunction(tan, activationFun, "TanHCt_1" );

            //calculate cell state
            var bft = CNTKLib.ElementTimes(ft, ct_1,"ftct_1");
            var bit = CNTKLib.ElementTimes(it, tanH, "ittanH");

            //cell state
            var ct = CNTKLib.Plus(bft, bit, "CellState");
            //
            return ct;
        }

        /// <summary>
        /// Defines the LSTM gate with activation function, with optional stabilization and peephole connection.
        /// </summary>
        /// <param name="x">Input variable.</param>
        /// <param name="ht_1">Placeholder for previous output.</param>
        /// <param name="ct_1">Placeholder for previous cell state.<</param>
        /// <param name="dataType">Type of data.</param>
        /// <param name="usePeephole">Include peephole connection in the gate.</param>
        /// <param name="useStabilizer">Use stabilization for output.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="seed">Random seed.</param>
        /// <param name="name">Name of the gate.</param>
        /// <returns></returns>
        public Variable AGate(Variable x, Variable ht_1, Variable ct_1, DataType dataType, bool usePeephole,
            bool useStabilizer, DeviceDescriptor device, ref uint seed, string name)
        {
            //cell dimension
            int cellDim = ct_1.Shape[0];

            //define previous output with stabilization of if defined
            var h_prev = !useStabilizer ? ht_1 : Stabilizer(ht_1, device);

            //create linear gate
            var gate = Gate(x, h_prev, cellDim, dataType, device, ref seed);
            if (usePeephole)
            {
                var c_prev = !useStabilizer ? ct_1 : Stabilizer(ct_1, device);
                gate = gate + Peep(c_prev, dataType, device, ref seed);
            }
            //create forget gate
            var sgate = CNTKLib.Sigmoid(gate, name);
            return sgate;
        }

        /// <summary>
        /// Defines linear the form:  gate = ( W*[ht_1, xt] + b), essential for LSTM cell
        /// </summary>
        /// <param name="x">Input variable.</param>
        /// <param name="hPrev">Placeholder for previous output.</param>
        /// <param name="cellDim"> Dimension of a LSTM Cell.</param>
        /// <param name="dataType">Type of data</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="seed">Random seed.</param>
        /// <returns></returns>
        private Variable Gate(Variable x, Variable hPrev, int cellDim,
                                    DataType dataType, DeviceDescriptor device, ref uint seed)
        {
            //create linear layer
            var xw_b = Layer(x, cellDim, dataType, device, seed++);
            var u = Weights(cellDim, dataType, device, seed++,"_u");
            //
            var gate = xw_b + (u * hPrev);
            return gate;
        }

        /// <summary>
        /// Multiplication of layer variable with constant scalar. In case the scalar is learnable learning speed will increase
        /// Ref: https://www.microsoft.com/en-us/research/wp-content/uploads/2016/11/SelfLR.pdf
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        internal Variable Stabilizer(Variable x, DeviceDescriptor device)
        {
            //define floating number
            var f = Constant.Scalar(4.0f, device);

            //make inversion of prev. value
            var fInv = Constant.Scalar(f.DataType, 1.0 / 4.0f);

            //create value of 1/f*ln (e^f-1)
            double initValue = 0.99537863;

            //create param with initial value
            var param = new Parameter(new NDShape(), f.DataType, initValue, device, "_stabilize");

            //make exp of product scalar and parameter
            var expValue = CNTKLib.Exp(CNTKLib.ElementTimes(f, param));

            //
            var cost = Constant.Scalar(f.DataType, 1.0) + expValue;

            var log = CNTKLib.Log(cost);

            var beta = CNTKLib.ElementTimes(fInv, log);

            //multiplication of the variable layer with constant scalar beta
            var finalValue = CNTKLib.ElementTimes(beta, x);

            return finalValue;
        }

        /// <summary>
        /// Connection with peepholes
        /// Variants on Long Short Term Memory: http://colah.github.io/posts/2015-08-Understanding-LSTMs/
        /// </summary>
        /// <param name="cstate"></param>
        /// <param name="dataType"></param>
        /// <param name="device"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        internal Function Peep(Variable cstate, DataType dataType, DeviceDescriptor device, ref uint seed)
        {
            //initial value
            var initValue = CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed);

            //create shape which for bias should be 1xn
            NDShape shape = new int[] { cstate.Shape[0] };

            var bf = new Parameter(shape, dataType, initValue, device, "_peep");

            var peep = CNTKLib.ElementTimes(bf, cstate);
            return peep;
        }

    }
}
