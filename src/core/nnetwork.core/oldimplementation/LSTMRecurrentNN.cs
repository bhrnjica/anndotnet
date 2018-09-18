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
using NNetwork.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNetwork.Core
{
    /// <summary>
    /// Implementation of the LSTM Recurrent Neural Networks
    /// </summary>
    public class LSTMReccurentNN : FeedForwaredNN
    {
        #region Ctor and Private Members

        public LSTMReccurentNN(DeviceDescriptor device) : base(device)
        {

        }

        public LSTMReccurentNN(int lstmDim, int cellDim, DeviceDescriptor device) : base(device)
        {
            m_lstmDim = lstmDim;
            m_cellDim = cellDim;
        }
        
        //number of lstm cells
        private int m_lstmDim;
        //number of cell dimensions
        private int m_cellDim;
        #endregion

        #region Public methods
        /// <summary>
        /// Create pure Recurrence LSTM layer. Use this function if you want to create complex network structure combining with drop layer etc.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Function CreateRecurrence(Variable input, string name)
        {
            //prepare for cell creations
            NDShape hShape = new int[] { m_lstmDim };
            NDShape cShape = new int[] { m_cellDim };

            //create lstm layer
            var lstmLayer = createComponent<float>(input, hShape, cShape);

            //created component
            var prevOut = lstmLayer.Item1;
            var prevCell = lstmLayer.Item2;
            //
            var seqLayer = CNTKLib.SequenceLast(prevOut);

            return seqLayer;
        }

        public (Function h, Function c) CreateLSTM(Variable input, string name)
        {
            //prepare for cell creations
            NDShape hShape = new int[] { m_lstmDim };
            NDShape cShape = new int[] { m_cellDim };

            //create lstm layer
            var lstmLayer = createComponent<float>(input, hShape, cShape);

            //created component
            var prevOut = lstmLayer.Item1;
            var prevCell = lstmLayer.Item2;
            //
            var seqLayer = CNTKLib.SequenceLast(prevOut);

            return (prevOut,prevCell);
        }

        #endregion

        #region Private Methods
        //defines the past value function for the input variable
        // Func<Variable, Function> pastValueRecurrenceHook = (x) => CNTKLib.PastValue(x);
        private Function pastValueRecurrence(Variable input)
        {
            return CNTKLib.PastValue(input);
        }

        /// <summary>
        /// Multiplication of layer output with constant scalar. In case the scalar is learnable learning speed will increase 
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Function stabilizeOutput(Variable output)
        {
            //define floating number
            var f = Constant.Scalar(4.0f, m_device);

            //make inversion of prev. value
            var fInv = Constant.Scalar(f.DataType, 1.0 / 4.0f);

            //create param with initial value
            var param = new Parameter(new NDShape(), f.DataType, 0.99537863 /* 1/f*ln (e^f-1) */, m_device);

            //make exp of product scalar and parameter
            var expValue = CNTKLib.Exp(CNTKLib.ElementTimes(f, param));

            //
            var cost = Constant.Scalar(f.DataType, 1.0) + expValue;

            var log = CNTKLib.Log(cost);

            var beta = CNTKLib.ElementTimes(fInv, log);

            //multiplication of the output layer with constant scalar beta
            return CNTKLib.ElementTimes(beta, output, "stabilize");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ElementType"></typeparam>
        /// <param name="input"></param>
        /// <param name="outputShape"></param>
        /// <param name="cellShape"></param>
        /// <returns></returns>
        private Tuple<Function, Function> createComponent<ElementType>(Variable input, NDShape outputShape, NDShape cellShape)
        {
            //Define previous output and previous cell state
            var dh = Variable.PlaceholderVariable(outputShape, input.DynamicAxes);
            var dc = Variable.PlaceholderVariable(cellShape, input.DynamicAxes);

            //create LSTM cell with previous output and prev cell state
            var LSTMCell = createCell<ElementType>(input, dh, dc);

            //hook the previous output and prevCell state in order to get correct prev values
            var actualDh = pastValueRecurrence(LSTMCell.Item1);
            var actualDc = pastValueRecurrence(LSTMCell.Item2);

            // Form the recurrence loop by replacing the dh and dc placeholders with the actualDh and actualDc
            (LSTMCell.Item1).ReplacePlaceholders(new Dictionary<Variable, Variable> { { dh, actualDh }, { dc, actualDc } });

           
            return new Tuple<Function, Function>(LSTMCell.Item1, LSTMCell.Item2);
        }

        /// <summary>
        /// Create LSTM cell with default behaviors
        /// </summary>
        /// <typeparam name="ElementType"></typeparam>
        /// <param name="input">input dimensions</param>
        /// <param name="prevOutput">previous output state</param>
        /// <param name="prevCellState">previous cell state</param>
        /// <returns></returns>
        private Tuple<Function, Function> createCell<ElementType>(Variable input, Variable prevOutput, Variable prevCellState)
        {
            int outputDim = prevOutput.Shape[0];
            int cellDim = prevCellState.Shape[0];

            bool isFloatType = typeof(ElementType).Equals(typeof(float));
            DataType dataType = isFloatType ? DataType.Float : DataType.Double;

            Func<int, string, Parameter> createBiasParam = createBiasParam = (dim, name) => new Parameter(new int[] { dim }, 0.01f, m_device, name);
            if (isFloatType)
                createBiasParam = (dim, name) => new Parameter(new int[] { dim }, 0.01f, m_device, name);
            else
                createBiasParam = (dim, name) => new Parameter(new int[] { dim }, 0.01, m_device, name);

            uint seed2 = 1;
            Func<int,string, Parameter> createProjectionParam = (oDim, name) => new Parameter(new int[] { oDim, NDShape.InferredDimension },
                    dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), m_device, name);

            Func<int, string, Parameter> createDiagWeightParam = (dim, name) =>
                new Parameter(new int[] { dim }, dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), m_device, name);

            //stabilization function for both output and cell state
            Function stabilizedPrevOutput = stabilizeOutput(prevOutput);
            Function stabilizedPrevCellState = stabilizeOutput(prevCellState);

            Func<Variable> projectInput = () => createBiasParam(cellDim, "_b") + (createProjectionParam(cellDim, "_w") * input);

            // Input gate
            Function it =
                CNTKLib.Sigmoid(
                    (Variable)(projectInput() + (createProjectionParam(cellDim,"_u") * stabilizedPrevOutput)) +
                    CNTKLib.ElementTimes(createDiagWeightParam(cellDim,"_peep"), stabilizedPrevCellState));

            Function bit = CNTKLib.ElementTimes(
                it,
                CNTKLib.Tanh(projectInput() + (createProjectionParam(cellDim, "_u") * stabilizedPrevOutput)));

            // Forget-me-not gate
            //ft= si(Wf [ht-1, xt] + bf)            
            Function ft = CNTKLib.Sigmoid(
                (Variable)(projectInput() + (createProjectionParam(cellDim, "_u") * stabilizedPrevOutput)) +
                        CNTKLib.ElementTimes(createDiagWeightParam(cellDim, "_u"),stabilizedPrevCellState));

            Function bft = CNTKLib.ElementTimes(ft, prevCellState);

            Function ct = (Variable)bft + bit;

            // Output gate
            Function ot = CNTKLib.Sigmoid(
                (Variable)(projectInput() + (createProjectionParam(cellDim, "_u") * stabilizedPrevOutput)) +
                        CNTKLib.ElementTimes(createDiagWeightParam(cellDim, "_u"), stabilizeOutput(ct)));

            // ht=ot*tanh(ct)
            Function ht = CNTKLib.ElementTimes(ot, CNTKLib.Tanh(ct));

            Function c = ct;
            Function h = (outputDim != cellDim) ? (createProjectionParam(outputDim, "_u") * stabilizeOutput(ct)) : ht;
            
            return new Tuple<Function, Function>(h, c);
        }


        /// <summary>
        /// Create embedding sequence
        /// </summary>
        /// <param name="input"></param>
        /// <param name="embeddingDim"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public Function Embedding(Variable input, int embeddingDim)
        {
            //checking the dimension of the input variable
            //it has to be always a vector which represents the tensor with rank=1
            System.Diagnostics.Debug.Assert(input.Shape.Rank == 1);

            //extract input dimensions from the input variable
            int inputDim = input.Shape[0];

            //initialization of the default parameters
            var glorotInit = CNTKLib.GlorotUniformInitializer();

            //create ND shape and parameter
            var dims = new int[] { embeddingDim, inputDim };
            var embededParam = new Parameter(NDShape.CreateNDShape(dims), DataType.Float, glorotInit, m_device);

            Function embededL = CNTKLib.Times(embededParam, input, "embedded_"+input.Name);

            return embededL;
        }

        #endregion

    }
}
