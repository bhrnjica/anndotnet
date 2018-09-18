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
using System;
using System.Collections.Generic;
using CNTK;
using NNetwork.Core.Common;
using NNetwork.Core.Network.Modules;

namespace NNetwork.Core.Network
{
    /// <summary>
    /// Implementation of Recurrent Neural Network
    /// </summary>
    public class RNN
    {
        /// <summary>
        /// Creates the recurrence network based on LSTM cell
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
        /// <returns></returns>
        public static Function RecurrenceLSTM(Variable input, int outputDim, int cellDim, DataType dataType, DeviceDescriptor device,
            Activation actFun = Activation.TanH, bool usePeephole = true, bool useStabilizer = true, uint seed = 1)
        {
            if (outputDim < 0 || cellDim < 0)
                throw new Exception("Dimension of LSTM cell cannot be zero.");
            //prepare output and cell dimensions 
            NDShape hShape = new int[] { outputDim };
            NDShape cShape = new int[] { cellDim };

            //create placeholders
            //Define previous output and previous cell state as placeholder which will be replace with past values later
            var dh = Variable.PlaceholderVariable(hShape, input.DynamicAxes);
            var dc = Variable.PlaceholderVariable(cShape, input.DynamicAxes);

            //create lstm cell
            var lstmCell = new LSTM(input, dh, dc, dataType, actFun, usePeephole, useStabilizer, seed, device);

            //get actual values of output and cell state
            var actualDh = CNTKLib.PastValue(lstmCell.H);
            var actualDc = CNTKLib.PastValue(lstmCell.C);

            // Form the recurrence loop by replacing the dh and dc placeholders with the actualDh and actualDc
            lstmCell.H.ReplacePlaceholders(new Dictionary<Variable, Variable> { { dh, actualDh }, { dc, actualDc } });

            //initialize properties
            var recurrence = CNTKLib.SequenceLast(lstmCell.H);
            //
            return recurrence;
        }

        /// <summary>
        /// Implementation of recurrent neural network based on GRU
        /// TODO:
        /// </summary>
        /// <returns></returns>
        public static Function RecurrenceGRU()
        {
            throw new NotImplementedException();
        }
    }
}
