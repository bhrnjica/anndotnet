using CNTK;
using Emgu.CV.Structure;
using NNetwork.Core.Common;
using NNetwork.Core.Network;
using NNetwork.Core.Network.Modules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anndotnet.test.net.app
{
    public static class MNIST_test
    {
        public static void Run_MNIST_Test()
        {
            //
            var device = DeviceDescriptor.UseDefaultDevice();
            //dims
            var inDim = 784;
            var outDim = 10;

            // MNIST images are 28x28=784 pixels 
            var input = CNTKLib.InputVariable(new NDShape(1, inDim), DataType.Float, "features");
            var labels = CNTKLib.InputVariable(new NDShape(1, outDim), DataType.Float, "labels");

            //create network
            var nnModel = createModel(input, outDim, 1, device);

            //Loss and Eval functions
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(nnModel, labels, "lossFunction");
            var prediction = CNTKLib.ClassificationError(nnModel, labels, "classificationError");

            //create learners and trainer
            // set per sample learning rate and momentum
            var learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(0.001, 1);
            var momentumPerSample = new CNTK.TrainingParameterScheduleDouble(0.9, 1);
            var nnParams = nnModel.Parameters();
            var parameterLearners = new List<Learner>()
                {
                    CNTKLib.AdamLearner(new ParameterVector(nnModel.Parameters().ToList()), learningRatePerSample,momentumPerSample)
                };

            var trainer = Trainer.CreateTrainer(nnModel, trainingLoss, prediction, parameterLearners);

            //create minibatch source
            var sConfigs = new StreamConfiguration[]
                { new StreamConfiguration("features", inDim), new StreamConfiguration("labels", outDim) };
            var minibatchSource = MinibatchSource.TextFormatMinibatchSource("C:\\sc\\Datasets\\MNIST\\MNIST-TrainData.txt", sConfigs, MinibatchSource.InfinitelyRepeat);
            var minibatchSize = (uint)754;
            var featureStreamInfo = minibatchSource.StreamInfo("features");
            var labelStreamInfo = minibatchSource.StreamInfo("labels");
            var maxIt = 250;
            var curIt = 1;
            while (true)
            {
                var minibatchData = minibatchSource.GetNextMinibatch(minibatchSize, device);
                var arguments = new Dictionary<Variable, MinibatchData>
                {
                    { input, minibatchData[featureStreamInfo] },
                    { labels, minibatchData[labelStreamInfo] }
                };

                trainer.TrainMinibatch(arguments, device);

                //
                if (minibatchData[featureStreamInfo].sweepEnd)
                {
                    if(curIt%50==0 || curIt==1)
                        printProgress(trainer, curIt);
                    curIt++;
                }

                if (maxIt <= curIt)
                    break;
            }

            // save the trained model
            nnModel.Save("mnist_classifier");

            // validate the model
            var minibatchSourceNewModel = MinibatchSource.TextFormatMinibatchSource("../../../data/MNIST-TestData.txt", sConfigs, MinibatchSource.InfinitelyRepeat);
            //prepare vars to accept results
            List<List<float>> X = new List<List<float>>();
            List<float> Y = new List<float>();
            //Model validation       
            ValidateModel("mnist_classifier", minibatchSourceNewModel, new int[] { 28, 28 }, 10, "features", "labels", device, 1000, X, Y, false);

            //show image classification result
            showResult(X, Y);
        }

        private static void printProgress(Trainer trainer, int curIt)
        {
            Console.WriteLine($"Iteration={curIt}, Loss={trainer.PreviousMinibatchLossAverage()}, Eval={trainer.PreviousMinibatchEvaluationAverage()}");
        }


        private static void showResult(List<List<float>> X, List<float> Y)
        {
            for (int i = 0; i < X.Count; i++)
            {
                var img = X[i];
                var result = $"Output = {Y[i]}";
                Bitmap bmp = ArrayToImg(28, 28, img);
                var emgImg = new Emgu.CV.Image<Bgr, byte>(bmp);
                var resizedImg = emgImg.Resize(250, 250, Emgu.CV.CvEnum.Inter.Nearest);
                // show output
                Emgu.CV.UI.ImageViewer.Show(resizedImg, result);
            }
        }


        internal static Bitmap ArrayToImg(int width, int height, List<float> img)
        {

            Bitmap bmp = new Bitmap(width, height);
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int rgb = (int)img[index];
                    var col = Color.FromArgb(rgb);
                    bmp.SetPixel(x, y, col);
                    index++;
                }
            }

            return bmp;
        }

        private static Function createModel(Variable x, int outDim, int nnType, DeviceDescriptor device)
        {

            //scale data before first layer
            var scaledInput = CNTKLib.ElementTimes(Constant.Scalar<float>(0.00391f, device), x);
            if (nnType == 0/*useNALU*/)
            {
                FeedForwaredNN ffn = new FeedForwaredNN(device, DataType.Float);

                Function dense1 = ffn.Dense(scaledInput, 200, Activation.Sigmoid, "");
                var nalu = new NALU(dense1, outDim, DataType.Float, device, 1);
                return nalu.H;
            }
            else if (nnType == 1/*FeedForward*/)
            {
                FeedForwaredNN ffn = new FeedForwaredNN(device, DataType.Float);

                Function dense1 = ffn.Dense(scaledInput, 200, Activation.Sigmoid, "");
                Function classifierOutput = ffn.Dense(dense1, outDim, Activation.None, "MNIST Classifier");
                return classifierOutput;
            }
            else if (nnType == 2/*Convolution*/)
            {
                return null;
            }
            else
                return null;


        }

        public static float ValidateModel(string modelFile, MinibatchSource testMinibatchSource, int[] imageDim, int numClasses,
                      string featureInputName, string labelInputName, DeviceDescriptor device,
                      int maxCount = 1000, List<List<float>> X = null, List<float> Y = null, bool useConvolution = true)
        {
            Function model = Function.Load(modelFile, device);
            var imageInput = model.Arguments[0];

            var labelOutput = model.Output;

            var featureStreamInfo = testMinibatchSource.StreamInfo(featureInputName);
            var labelStreamInfo = testMinibatchSource.StreamInfo(labelInputName);

            int batchSize = 1000;
            int miscountTotal = 0, totalCount = 0;

            while (true)
            {
                var minibatchData = testMinibatchSource.GetNextMinibatch((uint)batchSize, device);
                if (minibatchData == null || minibatchData.Count == 0)
                    break;
                totalCount += (int)minibatchData[featureStreamInfo].numberOfSamples;

                // expected labels are in the minibatch data.

                var labelData = minibatchData[labelStreamInfo].data.GetDenseData<float>(labelOutput);
                var expectedLabels = labelData.Select(l => l.IndexOf(l.Max())).ToList();

                var inputDataMap = new Dictionary<Variable, Value>() {
                    { imageInput, minibatchData[featureStreamInfo].data }
                };

                var outputDataMap = new Dictionary<Variable, Value>() {
                    { labelOutput, null }
                };

                model.Evaluate(inputDataMap, outputDataMap, device);


                var faetureData = minibatchData[featureStreamInfo].data.GetDenseData<float>(CNTKLib.InputVariable(minibatchData[featureStreamInfo].data.Shape, DataType.Float, model.Arguments[0].Name));

                var outputData = outputDataMap[labelOutput].GetDenseData<float>(labelOutput);
                var actualLabels = outputData.Select(l => l.IndexOf(l.Max())).ToList();

                int misMatches = actualLabels.Zip(expectedLabels, (a, b) => a.Equals(b) ? 0 : 1).Sum();

                miscountTotal += misMatches;
                Console.WriteLine($"Validating Model: Total Samples = {totalCount}, Mis-classify Count = {miscountTotal}");

                if (totalCount > 10001)
                {
                    //writes some result in to array 

                    for (int i = 0; i < outputData.Count && X != null && Y != null; i++)
                    {
                        var imgDIm = imageDim.Aggregate(1, (acc, val) => acc * val);
                        var inputVector = faetureData[0].Skip(imgDIm * i).Take(imgDIm).Select(x => (float)x).ToList();
                        X.Add(inputVector);
                        var currLabel = actualLabels[i];
                        Y.Add(currLabel);

                    };
                    break;
                }

            }



            float errorRate = 1.0F * miscountTotal / totalCount;
            Console.WriteLine($"Model Validation Error = {errorRate}");
            return errorRate;
        }

    }
}
