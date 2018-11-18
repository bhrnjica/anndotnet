//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool  on .NET Platform                                     //
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

namespace NNetwork.Core.Network
{
    /// <summary>
    /// Class implementation of the basic Neural Network components: Weights, Bias, Layer,etc.
    /// </summary>
    public class NetworkFoundation
    {
        /// <summary>
        /// Implementation of the Layer with arbitrary number of neurons. The layer consists of contribution from input(s), hidden, and bias.
        /// In a typical artificial neural network each neuron/activity in one "layer" is connected - via 
        /// - a weight - to each neuron in the next activity. 
        /// Each of these activities stores some sort of computation, normally a composite of the weighted activities in previous layers.
        /// A bias unit is an "extra" neuron added to each pre-output layer that stores the value of 1. 
        /// Bias units aren't connected to any previous layer and in this sense don't represent a true "activity".
        /// </summary>
        /// <param name="x">Input variable.</param>
        /// <param name="hiddenDim">Number of neurons, dimension of the layer.</param>
        /// <param name="dataType">Number type of data.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="seed">Random seed.</param>
        /// <returns></returns>
        public Variable Layer(Variable x, int outDim, DataType dataType, DeviceDescriptor device, uint seed = 1, string name="")
        {
            //bias
            var b = Bias(outDim, dataType, device);

            //weights
            var W = Weights(outDim, dataType, device, seed);

            //matrix product
            var Wx = CNTKLib.Times(W, x, name + "wx");

            //layer
            var n = string.IsNullOrEmpty(name) ? "wx_b" : name;
            var l = CNTKLib.Plus(b, Wx, n);

            return l;

        }
        
        /// <summary>
        /// Create bias for the layer.
        /// </summary>
        /// <param name="nDimension">Dimension of bias</param>
        /// <param name="dataType">Number type of data.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="name">Name of bias.</param>
        /// <returns></returns>
        public Parameter Bias(int nDimension, DataType dataType, DeviceDescriptor device)
        {
            //initial value
            var initValue = 0.01;

            //create shape which for bias should be 1xn
            NDShape shape = new int[] { nDimension };

            var b = new Parameter(shape, dataType, initValue, device, "b");
            //
            return b;
        }

        /// <summary>
        /// Create Weights parameters for the layer.
        /// </summary>
        /// <param name="nDimension">Dimension of the hidden layer.</param>
        /// <param name="dataType">Numeric type of data.</param>
        /// <param name="device">Device where computing will happen.</param>
        /// <param name="seed">Random seed.</param>
        /// <param name="name">Name of weights parameters.</param>
        /// <returns></returns>
        public Parameter Weights(int outputDim, DataType dataType, DeviceDescriptor device, uint seed = 1, string name = "")
        {
            //initializer of parameter
            var glorotI = CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed);

            //create shape the dimension is partially known
            NDShape shape = new int[] { outputDim, NDShape.InferredDimension };

            //create parameter
            var w = new Parameter(shape, dataType, glorotI, device, name == "" ? "w" : name);

            //
            return w;
        }

        /// <summary>
        /// Create Weights parameters for the layer.
        /// </summary>
        /// <param name="inputDim">Input dimension</param>
        /// <param name="outputDim">Output dimension</param>
        // <param name="dataType">Numeric type of data.</param>
        // <param name="device">Device where computing will happen.</param>
        // <param name="seed">Random seed.</param>
        // <param name="name">Name of weights parameters.</param>
        /// <returns></returns>
        public Parameter Weights(int inputDim, int outputDim, DataType dataType, DeviceDescriptor device, uint seed = 1, string name = "")
        {
            //initializer of parameter
            var glorotI = CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed);

            //create shape the dimension 
            NDShape shape = new int[] { outputDim, inputDim };

            //create parameter
            var w = new Parameter(shape, dataType, glorotI, device, name == "" ? "w" : name);

            //
            return w;
        }

        /// <summary>
        /// Applying the activation function to the layer.
        /// </summary>
        /// <param name="x">Input variable</param>
        /// <param name="activation">Activation function name.</param>
        /// <param name="name">Name of the output function.</param>
        /// <returns></returns>
        public Function AFunction(Function x, Activation activation, string name = "")
        {
            switch (activation)
            {
                default:
                case Activation.None:
                        return x;
                case Activation.ReLU:
                    return CNTKLib.ReLU(x, name);
                case Activation.Sigmoid:
                    return CNTKLib.Sigmoid(x, name);
                case Activation.Softmax:
                    return CNTKLib.Softmax(x, name);
                case Activation.TanH:
                    return CNTKLib.Tanh(x, name);
            }
        }
    }
}
