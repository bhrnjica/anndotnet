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
using NNetwork.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace NNetwork.Core.Network
{
    /// <summary>
    /// Implementation of the basic Neural Network layers and activation function
    /// </summary>
    public class FeedForwaredNN : NetworkFoundation
    {
        #region Ctor and Private Members
        protected DeviceDescriptor m_device;
        protected DataType m_DataType = DataType.Float;
        public FeedForwaredNN(DeviceDescriptor device, DataType dataType = DataType.Float)
        {
            m_device = device;
            m_DataType = dataType;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Create full Dense layer with specific activation function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputDim"></param>
        /// <param name="activation"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public Function Dense(Variable input, int outputDim, Activation activation = Activation.None, string name = "")
        {
            Function layer = Linear(input, outputDim, name);

            //in case no activation function is set skip it
            if(activation != Activation.None)
                layer = AFunction(layer, activation, name);
            return layer;
        }

        /// <summary>
        /// Create Linear Layer witout Activation function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputDim"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public Function Linear(Variable input, int outputDim, string name = "")
        {
            var l = Layer(input, outputDim, m_DataType, m_device, 1, name);
            return l;
        }

        /// <summary>
        /// Create Neural Network with arbitrary numbers of hidden and output layers
        /// </summary>
        /// <param name="input">input variable</param>
        /// <param name="outputVars">list of output variables</param>
        /// <param name="numHiddenLayers">number of hidden layers</param>
        /// <param name="hiddenLayerDims"> dimension of each hidden layer</param>
        /// <param name="actHidden"></param>
        /// <param name="actOutput"></param>
        /// <returns></returns>
        public Function CreateNet(Variable input, List<Variable> outputVars, int numHiddenLayers, int[] hiddenLayerDims, Activation actHidden, Activation actOutput)
        {
            //first input layer creation
            var h = Dense(input, hiddenLayerDims[0], actHidden);

            //hidden layer creation
            int j = 1;
            for (int i = 1; i < numHiddenLayers; i++, j++)
            {
                //if only one activation is defined all hidden layers' neurons have the same activation function
                if (i >= hiddenLayerDims.Length)
                    j = 0;

                h = Dense(h, hiddenLayerDims[j], actHidden);
            }
            //
            return CreateOutputLayer(h, outputVars, actOutput);

        }

        /// <summary>
        /// creates output layer with arbitrary number of output variables
        /// </summary>
        /// <param name="inputVar">input variable</param>
        /// <param name="outputVars">output variable(s)</param>
        /// <param name="actOutput"> activation function</param>
        /// <returns></returns>
        public Function CreateOutputLayer(Variable inputVar, List<Variable> outputVars, Activation actOutput)
        {
            //creation of the output layer(s)
            var outputs = new List<Function>();
            foreach (var zi in outputVars)
            {
                var outputDim = zi.Shape.Dimensions.Last();
                var outputName = zi.Name;
                //
                var z = Dense(inputVar, outputDim, actOutput, outputName);
                outputs.Add(z);
            }
            //in case multiple output layer we use combine function
            if (outputVars.Count == 1)
                return outputs.First();
            else
                return CNTKLib.Combine(new VariableVector(outputs.Select(x => x.Output).ToList()));
        }

        /// <summary>
        /// creates output layer 
        /// </summary>
        /// <param name="inputVar">input variable</param>
        /// <param name="outputVar">output variable</param>
        /// <param name="activation"> activation function</param>
        /// <returns></returns>
        public Function CreateOutputLayer(Variable inputVar, Variable outputVar, Activation activation)
        {
            //creation of the output layer(s)
            var outputDim = outputVar.Shape.Dimensions.Last();
            var outputName = outputVar.Name;
            //
            var z = Dense(inputVar, outputDim, activation, outputName);

            return z;
        }
        #endregion
    }
}
