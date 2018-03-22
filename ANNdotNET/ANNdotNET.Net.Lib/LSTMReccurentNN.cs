using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET.Net.Lib
{
    /// <summary>
    /// Implementation of the LSTM Recurrent Neural Networks
    /// </summary>
    public class LSTMReccurentNN  : FeedForwaredNN
    {
        #region Ctor and Private Members
        public LSTMReccurentNN (int lstmDim, int cellDim, DeviceDescriptor device) : base(device)
        {
            m_lstmDim = lstmDim;
            m_cellDim = cellDim;
        }

        private int m_lstmDim;
        private int m_cellDim;
        #endregion

        #region Public methods
        /// <summary>
        /// Build a one direction recurrent neural network (RNN) with long-short-term-memory (LSTM) cells.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Function CreateSequenceNet(Variable input, int seqDim, int outDim, string name)
        {
            //First create embedded layer, which defines number of input and the number of embedding layers 
            var embededL = Embedding(input, seqDim);

            var outL = CreateNet(embededL, outDim, name);

            //
            return outL;
        }

        /// <summary>
        /// Create pure LSTM Recurrent network
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outDim"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Function CreateNet(Variable input, int outDim, string name)
        {
            //prepare for cell creations
            NDShape hShape = new int[] { m_lstmDim };
            NDShape cShape = new int[] { m_cellDim };
            var lstmLayer = createComponent<float>(input, hShape, cShape);
            //created component
            var prevOut = lstmLayer.Item1;
            var prevCell = lstmLayer.Item2;

            var seqLayer = CNTKLib.SequenceLast(prevOut);

            //create output layer which should be a simple linear full connected layer
            var outL = FullyConnectedLinearLayer(seqLayer, outDim, name);
            //
            return outL;
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

            var log  = CNTKLib.Log(cost);

            var beta = CNTKLib.ElementTimes(fInv,log);
            
            //multiplication of the output layer with constant scalar beta
            return CNTKLib.ElementTimes(beta, output);
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

            //hook the previous output and prevCell state in order to get corect prev values
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

            Func<int, Parameter> createBiasParam = createBiasParam = (dim) => new Parameter(new int[] { dim }, 0.01f, m_device, "");
            if (isFloatType)
                createBiasParam = (dim) => new Parameter(new int[] { dim }, 0.01f, m_device, "");
            else
                createBiasParam = (dim) => new Parameter(new int[] { dim }, 0.01, m_device, "");

            uint seed2 = 1;
            Func<int, Parameter> createProjectionParam = (oDim) => new Parameter(new int[] { oDim, NDShape.InferredDimension },
                    dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), m_device);

            Func<int, Parameter> createDiagWeightParam = (dim) =>
                new Parameter(new int[] { dim }, dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), m_device);

            //stabilization function for both output and cell state
            Function stabilizedPrevOutput = stabilizeOutput(prevOutput);
            Function stabilizedPrevCellState = stabilizeOutput(prevCellState);

            Func<Variable> projectInput = () =>
                createBiasParam(cellDim) + (createProjectionParam(cellDim) * input);

            // Input gate
            Function it =
                CNTKLib.Sigmoid(
                    (Variable)(projectInput() + (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
                    CNTKLib.ElementTimes(createDiagWeightParam(cellDim), stabilizedPrevCellState));
            Function bit = CNTKLib.ElementTimes(
                it,
                CNTKLib.Tanh(projectInput() + (createProjectionParam(cellDim) * stabilizedPrevOutput)));

            // Forget-me-not gate
            //ft= si(Wf [ht-1, xt] + bf)            
            Function ft = CNTKLib.Sigmoid(
                (Variable)(
                        projectInput() + 
                        (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
                        CNTKLib.ElementTimes(createDiagWeightParam(cellDim), 
                        stabilizedPrevCellState));
            Function bft = CNTKLib.ElementTimes(ft, prevCellState);

            Function ct = (Variable)bft + bit;

            // Output gate
            Function ot = CNTKLib.Sigmoid(
                (Variable)(projectInput() + (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
                CNTKLib.ElementTimes(createDiagWeightParam(cellDim), stabilizeOutput(ct)));

            // ht=ot*tanh(ct)
            Function ht = CNTKLib.ElementTimes(ot, CNTKLib.Tanh(ct));

            Function c = ct;
            Function h = (outputDim != cellDim) ? (createProjectionParam(outputDim) * stabilizeOutput(ct)) : ht;

            return new Tuple<Function, Function>(h, c);
        }


        private void createForgetGate()
        {
            //Func<Variable> projectInput = () =>
            //   createBiasParam(cellDim) + (createProjectionParam(cellDim) * input);

            //// Forget-me-not gate
            ////ft= si(Wf [ht-1, xt] + bf)            
            //Function ft = CNTKLib.Sigmoid(
            //    (Variable)(
            //            projectInput() +
            //            (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
            //            CNTKLib.ElementTimes(createDiagWeightParam(cellDim),
            //            stabilizedPrevCellState));
        }
        /// <summary>
        /// Create embedding sequence
        /// </summary>
        /// <param name="input"></param>
        /// <param name="embeddingDim"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private Function Embedding(Variable input, int embeddingDim)
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

            Function embededL = CNTKLib.Times(embededParam, input);

            return embededL;
        }

        #endregion

    }
}
