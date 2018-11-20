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
using NNetwork.Core.Network.Modules;
using System;
using System.Linq;
using System.Collections.Generic;

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
        /// <param name="returnSequence">Determines if the return value full sequence or the last element of sequence</param>
        /// <param name="actFun">Type of activation function for update cell state.</param>
        /// <param name="usePeephole">Include peephole connection in the gate.</param>
        /// <param name="useStabilizer">Use self stabilization for output.</param>
        /// <param name="seed">Random seed.</param>
        /// <returns></returns>
        public static Function RecurrenceLSTM(Variable input, int outputDim, int cellDim, DataType dataType, DeviceDescriptor device, bool returnSequence=false,
            Activation actFun = Activation.TanH, bool usePeephole = true, bool useStabilizer = true, uint seed = 1)
        {
            if (outputDim <= 0 || cellDim <= 0)
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

            //return value depending of type of LSTM layer
            //For Stacked LSTM (with more than one LSTM layer in the network), the last LSTM must return last Sequence element,
            // otherwise full sequence is returned
            if (returnSequence)
                return lstmCell.H;
            else
                return CNTKLib.SequenceLast(lstmCell.H); 

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


        public static Function RecurreceCudaStackedLSTM(Variable input, int outputDim, int numLayers, bool isBidirectional, DeviceDescriptor device)

        {
            if (device.Type != DeviceKind.GPU)
            {
                throw new NotSupportedException();
            }
            
            var net = new NetworkFoundation();
            var weights = net.Weights(input.Shape.Dimensions.First(), DataType.Float, device);
                                 
            var cudaStackedLSTM =  CNTKLib.OptimizedRNNStack(input, weights, (uint)outputDim, (uint)numLayers, isBidirectional, "lstm");

            return cudaStackedLSTM;
        }

        public static Function RecurreceCudaStackedGRU(Variable input, int outputDim, int numLayers, bool isBidirectional, DeviceDescriptor device)

        {
            if (device.Type != DeviceKind.GPU)
            {
                throw new NotSupportedException();
            }

            var net = new NetworkFoundation();
            var weights = net.Weights(input.Shape.Dimensions.First(), DataType.Float, device);

            var cudaStackedLSTM = CNTKLib.OptimizedRNNStack(input, weights, (uint)outputDim, (uint)numLayers, isBidirectional, "gru");

            return cudaStackedLSTM;
        }

        public static Function RecurreceCudaStackedTanH(Variable input, int outputDim, int numLayers, bool isBidirectional, DeviceDescriptor device)

        {
            if (device.Type != DeviceKind.GPU)
            {
                throw new NotSupportedException();
            }

            var net = new NetworkFoundation();
            var weights = net.Weights(input.Shape.Dimensions.First(), DataType.Float, device);

            var cudaStackedLSTM = CNTKLib.OptimizedRNNStack(input, weights, (uint)outputDim, (uint)numLayers, isBidirectional, "rnnTanh");

            return cudaStackedLSTM;
        }

        public static Function RecurreceCudaStackedReLU(Variable input, int outputDim, int numLayers, bool isBidirectional, DeviceDescriptor device)

        {
            if (device.Type != DeviceKind.GPU)
            {
                throw new NotSupportedException();
            }

            var net = new NetworkFoundation();
            var weights = net.Weights(input.Shape.Dimensions.First(), DataType.Float, device);

            var cudaStackedLSTM = CNTKLib.OptimizedRNNStack(input, weights, (uint)outputDim, (uint)numLayers, isBidirectional, "rnnReLU");

            return cudaStackedLSTM;
        }
    }
}
