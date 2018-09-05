using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CNTK;

//taken from https://github.com/Microsoft/CNTK/issues/2870
namespace ANNdotNET_clt
{
    public class Wordsfile
    {
        static Random rnd = new Random();

        public static void build(string fname)
        {
            using (StreamWriter file = new StreamWriter(fname))
            {
                int seqsz = 5;
                int seqid = 0;
                int tot = 0;
                int nin = 10;
                int i;
                for (i = 0; i < 100; i++)
                {
                    seqid++;
                    for (int k = 0; k < seqsz; k++)
                    {
                        int b = seqid % (nin - seqsz - 1);
                        tot++;
                        int[] m = new int[30];
                        m[k + b] = 1;

                        file.Write("{0}\t|x ", i);
                        for (int j = 0; j < nin; j++) file.Write("{0} ", m[j]);
                        file.Write("|y ");
                        m[k + b] = 0;
                        m[k + 1 + b] = 1;
                        for (int j = 0; j < nin; j++) file.Write("{0} ", m[j]);
                        file.WriteLine("");
                    }
                }

            }


        }
        public static void Train(string fname)
        {
            DeviceDescriptor device = DeviceDescriptor.CPUDevice;
            const int inputDim = 10; // 0---9  I don't know what these are...
            const int cellDim = 25;
            const int hiddenDim = 256;
            const int embeddingDim = 25;
            const int numOutputClasses = 10; // more or less than 5 :-) 

            // build the model
            var featuresName = "features";
            var features = Variable.InputVariable(new int[] { inputDim }, DataType.Float, featuresName, null, false);
            var labelsName = "labels";

            Function model = LSTM_model(features, numOutputClasses, embeddingDim, hiddenDim, cellDim, device, "MyModel");
            var labels = Variable.InputVariable(new int[] { numOutputClasses }, DataType.Float, labelsName, null, false);

            Function ce = CNTKLib.CrossEntropyWithSoftmax(model, labels, "lossFunction");
            Function errs = CNTKLib.ClassificationError(model, labels, "classificationError");


            // prepare training data
            IList<StreamConfiguration> streamConfigurations = new StreamConfiguration[]
            { new StreamConfiguration(featuresName, inputDim,false, "x"), new StreamConfiguration(labelsName, numOutputClasses,false, "y") };
            var minibatchSource = MinibatchSource.TextFormatMinibatchSource(fname, streamConfigurations, MinibatchSource.InfinitelyRepeat, true);
            var featureStreamInfo = minibatchSource.StreamInfo(featuresName);
            var labelStreamInfo = minibatchSource.StreamInfo(labelsName);

            // prepare for training
            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.001, 1);
            TrainingParameterScheduleDouble momentumTimeConstant = CNTKLib.MomentumAsTimeConstantSchedule(1100);
            IList<Learner> parameterLearners = new List<Learner>() {
            Learner.MomentumSGDLearner(model.Parameters(), learningRatePerSample, momentumTimeConstant, /*unitGainMomentum = */true)  };
            var trainer = Trainer.CreateTrainer(model, ce, errs, parameterLearners);

            // train the model
            uint minibatchSize = 100;
            int outputFrequencyInMinibatches = 20;
            int miniBatchCount = 0;
            int numEpochs = 50;

            while (numEpochs > 0)
            {
                var mb = minibatchSource.GetNextMinibatch(minibatchSize, device);

                var arguments = new Dictionary<Variable, MinibatchData>
            {
                { features, mb[featureStreamInfo] },
                { labels, mb[labelStreamInfo] }
            };

                trainer.TrainMinibatch(arguments, device);
                if ((miniBatchCount++ % outputFrequencyInMinibatches) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
                {
                    float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
                    float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
                    Console.WriteLine($"Minibatch: {miniBatchCount} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
                }
                if ((mb.Values.Any(a => a.sweepEnd))) numEpochs--;
            }
            Console.WriteLine("Now we try and evaluate the model");
            Function m = trainer.Model();
            //  now we have to give it a test sequence... and check it works...
        }
        public static int toint(char c)
        {
            int x = c - 'a' + 1;
            if (x < 0) x = 0;
            if (x > 26) x = 0;
            return x;
        }

        static Function Stabilize<ElementType>(Variable x, DeviceDescriptor device)
        {
            bool isFloatType = typeof(ElementType).Equals(typeof(float));
            Constant f, fInv;
            if (isFloatType)
            {
                f = Constant.Scalar(4.0f, device);
                fInv = Constant.Scalar(f.DataType, 1.0 / 4.0f);
            }
            else
            {
                f = Constant.Scalar(4.0, device);
                fInv = Constant.Scalar(f.DataType, 1.0 / 4.0f);
            }

            var beta = CNTKLib.ElementTimes(
                fInv,
                CNTKLib.Log(
                    Constant.Scalar(f.DataType, 1.0) +
                    CNTKLib.Exp(CNTKLib.ElementTimes(f, new Parameter(new NDShape(), f.DataType, 0.99537863 /* 1/f*ln (e^f-1) */, device)))));
            return CNTKLib.ElementTimes(beta, x);
        }

        static Tuple<Function, Function> LSTMPCellWithSelfStabilization<ElementType>(
            Variable input, Variable prevOutput, Variable prevCellState, DeviceDescriptor device)
        {
            int outputDim = prevOutput.Shape[0];
            int cellDim = prevCellState.Shape[0];

            bool isFloatType = typeof(ElementType).Equals(typeof(float));
            DataType dataType = isFloatType ? DataType.Float : DataType.Double;

            Func<int, Parameter> createBiasParam;
            if (isFloatType)
                createBiasParam = (dim) => new Parameter(new int[] { dim }, 0.01f, device, "");
            else
                createBiasParam = (dim) => new Parameter(new int[] { dim }, 0.01, device, "");

            uint seed2 = 1;
            Func<int, Parameter> createProjectionParam = (oDim) => new Parameter(new int[] { oDim, NDShape.InferredDimension },
                    dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), device);

            Func<int, Parameter> createDiagWeightParam = (dim) =>
                new Parameter(new int[] { dim }, dataType, CNTKLib.GlorotUniformInitializer(1.0, 1, 0, seed2++), device);

            Function stabilizedPrevOutput = Stabilize<ElementType>(prevOutput, device);
            Function stabilizedPrevCellState = Stabilize<ElementType>(prevCellState, device);

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
            Function ft = CNTKLib.Sigmoid(
                (Variable)(
                        projectInput() + (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
                        CNTKLib.ElementTimes(createDiagWeightParam(cellDim), stabilizedPrevCellState));
            Function bft = CNTKLib.ElementTimes(ft, prevCellState);

            Function ct = (Variable)bft + bit;

            // Output gate
            Function ot = CNTKLib.Sigmoid(
                (Variable)(projectInput() + (createProjectionParam(cellDim) * stabilizedPrevOutput)) +
                CNTKLib.ElementTimes(createDiagWeightParam(cellDim), Stabilize<ElementType>(ct, device)));
            Function ht = CNTKLib.ElementTimes(ot, CNTKLib.Tanh(ct));

            Function c = ct;
            Function h = (outputDim != cellDim) ? (createProjectionParam(outputDim) * Stabilize<ElementType>(ht, device)) : ht;

            return new Tuple<Function, Function>(h, c);
        }


        static Tuple<Function, Function> LSTMPComponentWithSelfStabilization<ElementType>(Variable input,
            NDShape outputShape, NDShape cellShape,
            Func<Variable, Function> recurrenceHookH,
            Func<Variable, Function> recurrenceHookC,
            DeviceDescriptor device)
        {
            var dh = Variable.PlaceholderVariable(outputShape, input.DynamicAxes);
            var dc = Variable.PlaceholderVariable(cellShape, input.DynamicAxes);

            var LSTMCell = LSTMPCellWithSelfStabilization<ElementType>(input, dh, dc, device);
            var actualDh = recurrenceHookH(LSTMCell.Item1);
            var actualDc = recurrenceHookC(LSTMCell.Item2);

            // Form the recurrence loop by replacing the dh and dc placeholders with the actualDh and actualDc
            (LSTMCell.Item1).ReplacePlaceholders(new Dictionary<Variable, Variable> { { dh, actualDh }, { dc, actualDc } });

            return new Tuple<Function, Function>(LSTMCell.Item1, LSTMCell.Item2);
        }

        private static Function Embeddingx(Variable input, int embeddingDim, DeviceDescriptor device)
        {
            System.Diagnostics.Debug.Assert(input.Shape.Rank == 1);
            int inputDim = input.Shape[0];
            var embeddingParameters = new Parameter(new int[] { embeddingDim, inputDim }, DataType.Float, CNTKLib.GlorotUniformInitializer(), device);
            return CNTKLib.Times(embeddingParameters, input);

        }

        static Function LSTMSequenceClassifierNet_1(Variable input, int numOutputClasses, int embeddingDim, int LSTMDim, int cellDim, DeviceDescriptor device,
            string outputName)
        {
            //            Function embeddingFunction = Embedding(input, embeddingDim, device);
            Func<Variable, Function> pastValueRecurrenceHook = (x) => CNTKLib.PastValue(x);


            //            Sequential([Stabilizer(), Recurrence(LSTM(hidden_dim), go_backwards = False)])),

            //Function drop = CNTKLib.Dropout(input, 0.1);
            Function LSTMFunction = LSTMPComponentWithSelfStabilization<float>(
                input,
                new int[] { LSTMDim },
                new int[] { cellDim },
                pastValueRecurrenceHook,
                pastValueRecurrenceHook,
                device).Item1;


            Function lastcell = CNTKLib.SequenceLast(LSTMFunction);

            //implement drop out for 10%
            var dropOut = CNTKLib.Dropout(lastcell, 0.1, 1);

            return FullyConnectedLinearLayer(lastcell, numOutputClasses, device, outputName);
        }
        static Function LSTM_model(Variable input, int numOutputClasses, int embeddingDim, int LSTMDim, int cellDim, DeviceDescriptor device,
            string outputName)
        {
            //            Function embeddingFunction = Embedding(input, embeddingDim, device);
            Func<Variable, Function> pastValueRecurrenceHook = (x) => CNTKLib.PastValue(x);


            //            Sequential([Stabilizer(), Recurrence(LSTM(hidden_dim), go_backwards = False)])),

            Function LSTMFunction = LSTMPComponentWithSelfStabilization<float>(
                input,
                new int[] { LSTMDim },
                new int[] { cellDim },
                pastValueRecurrenceHook,
                pastValueRecurrenceHook,
                device).Item1;

            var dropOut = CNTKLib.Dropout(LSTMFunction, 0.1, 1);

            return FullyConnectedLinearLayer(dropOut, numOutputClasses, device, outputName);
        }
        public static Function FullyConnectedLinearLayer(Variable input, int outputDim, DeviceDescriptor device, string outputName = "")
        {
            System.Diagnostics.Debug.Assert(input.Shape.Rank == 1);
            int inputDim = input.Shape[0];

            int[] s = { outputDim, inputDim };
            var timesParam = new Parameter((NDShape)s, DataType.Float,
                CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1),
                device, "timesParam");
            var timesFunction = CNTKLib.Times(timesParam, input, "times");

            int[] s2 = { outputDim };
            var plusParam = new Parameter(s2, 0.0f, device, "plusParam");
            return CNTKLib.Plus(plusParam, timesFunction, outputName);
        }
        public enum Activation
        {
            None,
            ReLU,
            Sigmoid,
            Tanh
        }

        public static Function Dense(Variable input, int outputDim, DeviceDescriptor device, Activation activation = Activation.None, string outputName = "")
        {
            if (input.Shape.Rank != 1)
            {
                // 
                int newDim = input.Shape.Dimensions.Aggregate((d1, d2) => d1 * d2);
                input = CNTKLib.Reshape(input, new int[] { newDim });
            }
            Function fullyConnected = FullyConnectedLinearLayer(input, outputDim, device, outputName);
            switch (activation)
            {
                default:
                case Activation.None:
                    return fullyConnected;
                case Activation.ReLU:
                    return CNTKLib.ReLU(fullyConnected);
                case Activation.Sigmoid:
                    return CNTKLib.Sigmoid(fullyConnected);
                case Activation.Tanh:
                    return CNTKLib.Tanh(fullyConnected);
            }
        }



    }
}