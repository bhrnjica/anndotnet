//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET.Net.Lib
{
    public enum Activation
    {
        None=0,
        ReLU=1,
        Softmax=2,
        Tanh=3
    }

    public class FeedForwaredNN
    {
        #region Ctor and Private Members
        protected DeviceDescriptor m_device;

        public FeedForwaredNN(DeviceDescriptor device)
        {
            m_device = device;
        }
        #endregion

        #region Public Members
        public Function CreateDenseLayer(Variable input, int outputDim, Activation activation = Activation.None, string outputName = "")
        {
            if (input.Shape.Rank != 1)
            {
                // 
                int newDim = input.Shape.Dimensions.Aggregate((d1, d2) => d1 * d2);
                input = CNTKLib.Reshape(input, new int[] { newDim });
            }

            //
            if(activation== Activation.None)
                return FullyConnectedLinearLayer(input, outputDim, outputName);

            Function fullyConnected = FullyConnectedLinearLayer(input, outputDim, outputName);

            switch (activation)
            {
                default:
                case Activation.None:
                    return fullyConnected;
                case Activation.ReLU:
                    return CNTKLib.ReLU(fullyConnected, outputName);
                case Activation.Softmax:
                    return CNTKLib.Sigmoid(fullyConnected, outputName);
                case Activation.Tanh:
                    return CNTKLib.Tanh(fullyConnected, outputName);
            }
        }

        public Function FullyConnectedLinearLayer(Variable input, int outputDim, string outputName = "")
        {
            System.Diagnostics.Debug.Assert(input.Shape.Rank == 1);
            int inputDim = input.Shape[0];

            //
            var glorotInit = CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1);

            int[] s = { outputDim, inputDim };

            //
            var weights = new Parameter((NDShape)s, DataType.Float, glorotInit, m_device, "timesParam");
            //
            var timesFunction = CNTKLib.Times(weights, input, "times");
            //
            int[] s2 = { outputDim };
            var plusParam = new Parameter(s2, 0.0f, m_device, "plusParam");

            return CNTKLib.Plus(plusParam, timesFunction, outputName);
        }

        /// <summary>
        /// int, 2, 1,300, Softmax
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputDim"></param>
        /// <param name="numHiddenLayers"></param>
        /// <param name="hiddenLayerDims"></param>
        /// <param name="activation"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public Function CreateNet(Variable input, int outputDim, int numHiddenLayers, int[] hiddenLayerDims, Activation actHidden, Activation actOutput, string outputName="")
        {
            //first input layer creation
            var h = CreateDenseLayer(input, hiddenLayerDims[0], actHidden);

            //hidden layer creation
            int j = 0;
            for (int i = 1; i < numHiddenLayers; i++,j++)
            {
                //if only one activation is defined all hidden layers' neurons have the same activation function
                if (i >= hiddenLayerDims.Length)
                    j = 0;
                h = CreateDenseLayer(h, hiddenLayerDims[j], actHidden);
            }
            //creation of the output layer
            var z = CreateDenseLayer(h, outputDim, actOutput, outputName);
            
            //

            return z;
        }
        #endregion
    }
}
