using ANNdotNET.Net.Lib;
using CNTK;
using libSVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANNdotNET_clt.models
{
    public class CNTK106A_Tutorial
    {
        // set fast mode to true
        static bool isFast = true;

        // Training parameters
        static int TRAINING_SIZE = 8000;
        static int VALIDATION_SIZE = 1000;
        static int TESTING_SIZE = 1000;
        static int BATCH_SIZE = 100;
        static int EPOCHS = isFast ? 10 : 100;

        /// <summary>
        /// Build and train a RNN model.
        /// </summary>
        /// <param name="device">CPU or GPU device to train and run the model</param>
        public static void Train(DeviceDescriptor device)
        {
            const int N = 5;// input: N subsequent values
            const int M = 5; // output: predict 1 value M steps ahead
            const int outDim = 1;
            const int embeddingDim = 20;
            const int hiDim = 20;
            const int cellDim = 20;

            //load data in to memory
            var xdata = Numpy.LinSpace(0, 100.0, 10000).Select(x => (float)x).ToArray<float>();
            var ds = loadWaveDataset(Math.Sin, xdata, N, M);

            //split dataset on train, validate and test parts
            var featureSet = ds["features"];
            var labelSet = ds["label"];

            // build the model
            var featuresName = "feature";
            var feature = Variable.InputVariable(new int[] { N }, DataType.Float, featuresName, null, false /*isSparse*/);

            var labelsName = "label";
            var label = Variable.InputVariable(new int[] { outDim }, DataType.Float, labelsName, new List<Axis>() { Axis.DefaultBatchAxis() }, false);


            //
            var lstmNN = new LSTMReccurentNN(hiDim, cellDim, device);
            var lstmModel = lstmNN.CreateNet(feature, outDim, "timeSeriesOutput");
            // var lstmModel = LSTMHelper.LSTMSequenceNet(feature, ouDim, embDim, hiDim, cellDim, dropPercentige, device, "timeSeriesOutput");

            var trainingLoss = CNTKLib.SquaredError(lstmModel, label, "squarederrorLoss");
            var prediction = CNTKLib.SquaredError(lstmModel, label, "squarederrorEval");
            var modelParams = lstmModel.Parameters();


            // prepare for training
            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.0005, 1);
            TrainingParameterScheduleDouble momentumTimeConstant = CNTKLib.MomentumAsTimeConstantSchedule(256);

            IList<Learner> parameterLearners = new List<Learner>() {
                Learner.MomentumSGDLearner(modelParams, learningRatePerSample, momentumTimeConstant, /*unitGainMomentum = */true)  };

            //create trainer
            var trainer = Trainer.CreateTrainer(lstmModel, trainingLoss, prediction, parameterLearners);

            // train the model
            int outputFrequencyInMinibatches = 5;
            int it = 0;
            for (int i = 1; i <= EPOCHS; i++)
            {
                //get the next minibatch amount of data
                foreach (var miniBatchData in nextBatch(featureSet.train, labelSet.train, BATCH_SIZE))
                {
                    var xValues = Value.CreateBatch<float>(new NDShape(1, N), miniBatchData.X, device);
                    var yValues = Value.CreateBatch<float>(new NDShape(1, 1), miniBatchData.Y, device);

                    ////Combine variables and data in to Dictionary for the training
                    var batchData = new Dictionary<Variable, Value>();
                    batchData.Add(feature, xValues);
                    batchData.Add(label, yValues);

                    //train minibarch data
                    trainer.TrainMinibatch(batchData, false, device);
                }

                //output training process
                PrintTrainingProgress(trainer, i, outputFrequencyInMinibatches);
            }
        }

        public static void PrintTrainingProgress(Trainer trainer, int minibatchIdx, int outputFrequencyInMinibatches)
        {
            if ((minibatchIdx % outputFrequencyInMinibatches) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
            {
                float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
                float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
                Console.WriteLine($"Minibatch: {minibatchIdx} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
            }

        }
        static (float[][] train, float[][] valid, float[][] test)
          splitData(float[][] data, float valSize = 0.1f, float testSize = 0.1f)
        {
            //calculate
            var posTest = (int)(data.Length * (1 - testSize));
            var posVal = (int)(posTest * (1 - valSize));

            return (data.Skip(0).Take(posVal).ToArray(), data.Skip(posVal).Take(posTest - posVal).ToArray(), data.Skip(posTest).ToArray());
        }

        private static IEnumerable<(float[] X, float[] Y)>
           nextBatch(float[][] X, float[][] Y, int mMSize)
        {

            float[] asBatch(float[][] data, int start, int count)
            {
                var lst = new List<float>();
                for (int i = start; i < start + count; i++)
                {
                    if (i >= data.Length)
                        break;

                    lst.AddRange(data[i]);
                }
                return lst.ToArray();
            }

            for (int i = 0; i < X.Length - mMSize; i += mMSize)
                yield return (asBatch(X, i, mMSize), asBatch(Y, i, mMSize));
        }

        static Dictionary<string, (float[][] train, float[][] valid, float[][] test)> loadWaveDataset(Func<double, double> fun, float[] x0,
                      int timeSteps, int timeShift)
        {
            ////fill data
            float[] xsin = new float[x0.Length];//all data
            for (int l = 0; l < x0.Length; l++)
                xsin[l] = (float)fun(x0[l]);


            //split data on training and testing part
            var a = new float[xsin.Length - timeShift];
            var b = new float[xsin.Length - timeShift];

            for (int l = 0; l < xsin.Length; l++)
            {
                //
                if (l < xsin.Length - timeShift)
                    a[l] = xsin[l];

                //
                if (l >= timeShift)
                    b[l - timeShift] = xsin[l];
            }

            //make arrays of data
            var a1 = new List<float[]>();
            var b1 = new List<float[]>();
            for (int i = 0; i < a.Length - timeSteps + 1; i++)
            {
                //features
                var row = new float[timeSteps];
                for (int j = 0; j < timeSteps; j++)
                    row[j] = a[i + j];
                //create features row
                a1.Add(row);
                //label row
                b1.Add(new float[] { b[i + timeSteps - 1] });
            }

            //split data into train, validation and test data set
            var xxx = splitData(a1.ToArray(), 0.1f, 0.1f);
            var yyy = splitData(b1.ToArray(), 0.1f, 0.1f);


            var retVal = new Dictionary<string, (float[][] train, float[][] valid, float[][] test)>();
            retVal.Add("features", xxx);
            retVal.Add("label", yyy);
            return retVal;
        }


    }
}
