using CNTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ANNdotNET.Net.Lib.Entities;

namespace ANNdotNET.Net.Lib
{
    public class Factory
    {
        //iteration, loss, eval, actual data, predicted data
        public Action<int, float, float, (List<List<float>>, List<List<float>>),(List<List<float>>, List<List<float>>)> report;
        static DeviceDescriptor m_Device;
        string m_FeatureName = "features";
        string m_LabelName = "labels";
        string m_modelFile = "modelName";
      
        public ANNModel Model { get; set; }

        private string m_TrainingPath;
        private string m_TestPath;

        public uint Iterations { get; set; }

        string savedModelChechState = "";
        Trainer m_Trainer;
        Function m_model;


        static Factory()
        {
            //chech if GPU device is available. Set it by default
            var dvc = DeviceDescriptor.AllDevices();
            var gpu= dvc.Where(x => x.Type == DeviceKind.GPU).FirstOrDefault();
            if (gpu == null)
                m_Device = DeviceDescriptor.CPUDevice;
            else
                m_Device = gpu;
        }

        public void Init(ANNModel model, string fileName)
        {
            Model = model;
            m_TrainingPath = fileName + "_train";
            m_TestPath = fileName + "_test";
        }



        #region Training Model

        public void Train(CancellationToken token)
        {
            Model.LossData.Clear();
            Model.EvalData.Clear();

            //stream configuration to distinct features and labels in the file
            var streamConfig = new StreamConfiguration[]
               {
                   new StreamConfiguration(m_FeatureName, Model.InputDim),
                   new StreamConfiguration(m_LabelName, Model.OutputDim)
               };

            // prepare the training data
            var minibatchSource = MinibatchSource.TextFormatMinibatchSource(m_TrainingPath, streamConfig, Iterations * Model.ModelData.MinibatchSize, Model.Randomize);


            //define stream info
            var featureStreamInfo = minibatchSource.StreamInfo(m_FeatureName);
            var labelStreamInfo = minibatchSource.StreamInfo(m_LabelName);

            // build a model
            var inputVar = Variable.InputVariable(new int[] { Model.InputDim }, DataType.Float, m_FeatureName, null, false);

            Variable outputVar = null;
            if(Model.ModelData.NetworkType== (int)NetworkTypes.LSTMRecurrent || Model.ModelData.NetworkType == (int)NetworkTypes.EmbdLSTMRecurrent)
                outputVar = Variable.InputVariable(new int[] { Model.OutputDim }, DataType.Float, m_LabelName, new List<CNTK.Axis>() { CNTK.Axis.DefaultBatchAxis() }, false);
            else
                outputVar = Variable.InputVariable(new int[] { Model.OutputDim }, DataType.Float, m_LabelName, null, false);

            //network creation
            var net = createCNTKModel(inputVar);

            // create function for training and evaluation
            var trainingLoss = CreateFunction(net, outputVar, Model.ModelData.LossFunction, "lossFunction"); 
            var prediction = CreateFunction(net, outputVar, Model.ModelData.EvaluationFunction, "evalFunction");

            //Create trainer
            var trainer = createTrainer(net, trainingLoss, prediction);
           
            // Feed data to the trainer for number of epochs. 
            int counter = 0;
            //report progress
            int canPass = 1;
            while (true)
            {
                var minibatchData = minibatchSource.GetNextMinibatch(Model.ModelData.MinibatchSize, m_Device);

                // Stop training once max epochs is reached.
                if (minibatchData.empty())
                {
                    Console.WriteLine($"Training process finished at {counter} epoch.");
                    break;
                }

                //create dictionary for training
                var dicData = new Dictionary<Variable, MinibatchData>()
                        {
                            {
                                inputVar, minibatchData[featureStreamInfo]
                            },
                            {
                                outputVar, minibatchData[labelStreamInfo]
                            }
                        };

                //train model
                trainer.TrainMinibatch(dicData, m_Device);

                
                //output training process
                if(canPass==1)
                {
                    var loss = (float)trainer.PreviousMinibatchLossAverage();
                    var eval = (float)trainer.PreviousMinibatchEvaluationAverage();
                    var it = counter;
                    var model = net.Clone();
                    Interlocked.Exchange(ref canPass, 0);
                    Task.Run(() => progressReport(loss, eval, model, counter)).ContinueWith(x => Interlocked.Exchange(ref canPass, 1));
                }
                
                
                //in case the search process is canceled
                if (token.IsCancellationRequested)
                {
                    break;
                }

                //increment iterations
                counter++;

                //save temporary state of the model
                //if (counter % 10000 == 0)
                //    net.Save(cntkModel + $"{counter}");
            }

            {
                var loss = (float)trainer.PreviousMinibatchLossAverage();
                var eval = (float)trainer.PreviousMinibatchEvaluationAverage();
                var it = counter;
                var model = net.Clone();
            
                Task.Run(() => progressReport(loss, eval, model, counter)).
                    ContinueWith(x =>
                    {
                        // once the training process is over save the model 
                        var modelName = Function.Combine(new List<Variable>() { trainingLoss, prediction, net }, "ANNDotNETModel");
                        modelName.Save(m_modelFile);
                    });
            }

            
        }

        /// <summary>
        /// Report progress 
        /// </summary>
        /// <param name="trainer"></param>
        /// <param name="model"></param>
        /// <param name="iteration"></param>
        void progressReport(float loss, float eval, Function model, int iteration)
        {
            try
            {
               
                var dataT = evaluateModel(model, m_TrainingPath, Model.TrainCount);
                var dataV = evaluateModel(model, m_TestPath, Model.TestCount);

                //add loss and evauation values
                Model.LossData.Add(loss);
                Model.EvalData.Add(eval);
                //add set of data
                Model.ActualT.Clear();
                Model.ActualV.Clear();
                Model.PredictedT.Clear();
                Model.PredictedV.Clear();
                Model.ActualT.AddRange(dataT.Item1);
                Model.ActualV.AddRange(dataV.Item1);
                Model.PredictedT.AddRange(dataT.Item2);
                Model.PredictedV.AddRange(dataV.Item2);

                //report the progress
                if (report != null)
                    report(iteration, loss, eval, dataT, dataV);

            }
            catch
            {
                throw;
            }

        }

        private (List<List<float>>, List<List<float>>) evaluateModel(Function model, string dataFilePath, uint dataCount)
        {
            var actual = new List<List<float>>();
            var predicted = new List<List<float>>();

            //stream configuration to distinct features and labels in the file
            var streamConfig = new StreamConfiguration[]
               {
                   new StreamConfiguration(m_FeatureName, Model.InputDim),
                   new StreamConfiguration(m_LabelName, Model.OutputDim)
               };

            var mb = MinibatchSource.TextFormatMinibatchSource(dataFilePath, streamConfig,dataCount,false);

            //define train stream info
            var featureStreamInfo = mb.StreamInfo(m_FeatureName);
            var labelStreamInfo = mb.StreamInfo(m_LabelName);

            // get input and output vars from the model
            var inputVar = model.Arguments[0];
            var outputVar = model.Output;
            while(true)//for(int j =0; j < dataCount; j++)
            {
                var data = mb.GetNextMinibatch(dataCount);

                // Stop training once max epochs is reached.
                if (data.empty())
                {
                    break;
                }
                //
                var inDataMap = new Dictionary<Variable, Value>() { { inputVar, data[featureStreamInfo].data } };
                var outDataMap = new Dictionary<Variable, Value>() { { outputVar, null } };

                //evaluate model
                model.Evaluate(inDataMap, outDataMap, m_Device);
                //process results
                var aData = data[labelStreamInfo].data.GetDenseData<float>(outputVar);
                var pData = outDataMap[outputVar].GetDenseData<float>(model.Output);
                //
                for (int i = 0; i < aData.Count; i++)
                {
                    var aa = aData[i].ToList();
                    actual.Add(aa);
                    var bb = pData[i].ToList();
                    predicted.Add(bb);
                }

            }

            return (actual, predicted);

        }



        /// <summary>
        /// Create Trainer by providing parameters
        /// </summary>
        /// <param name="net"></param>
        /// <param name="loss"></param>
        /// <param name="eval"></param>
        /// <returns></returns>
        private Trainer createTrainer(Function net, Function loss, Function eval)
        {
            // prepare for training
            var lr = new TrainingParameterScheduleDouble(Model.ModelData.LearningRate, 1);
            var mm = CNTKLib.MomentumAsTimeConstantSchedule(Model.ModelData.Momentum);

            ///
            var addParam = new AdditionalLearningOptions();
            if (Model.ModelData.L1Regularizer > 0)
                addParam.l1RegularizationWeight = Model.ModelData.L1Regularizer;
            //
            if (Model.ModelData.L2Regularizer > 0)
                addParam.l2RegularizationWeight = Model.ModelData.L2Regularizer;
            //
            if (Model.ModelData.L1Regularizer <= 0 && Model.ModelData.L1Regularizer <= 0)
                addParam = null;

            //SGD Momentum learner
            if (Model.ModelData.LearnerType == LearnerType.MomentumSGDLearner)   
            {
                //
                var llr = new List<Learner>();
                var msgd = Learner.MomentumSGDLearner(net.Parameters(), lr, mm, /*unitGainMomentum = */true, addParam);
                llr.Add(msgd);
                var trainer = Trainer.CreateTrainer(net, loss, eval, llr);
                return trainer;
            }
            //SGD simple learner
            if (Model.ModelData.LearnerType == LearnerType.SGDLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = Learner.SGDLearner(net.Parameters(), lr, addParam);
                llr.Add(msgd);
                var trainer = Trainer.CreateTrainer(net, loss, eval, llr);
                return trainer;
            }

            throw new Exception("Unsupported Learner!");
        }

        /// <summary>
        /// Helper for creation different kind of Loss and Evaluation functions
        /// </summary>
        /// <param name="network"></param>
        /// <param name="variable"></param>
        /// <param name="lossFunction"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Function CreateFunction(Function network, Variable variable, LossFunctions lossFunction, string name)
        {
            switch (lossFunction)
            {
                case LossFunctions.BinaryCrossEntropy:
                    return CNTKLib.BinaryCrossEntropy(network, variable, name);

                case LossFunctions.CrossEntropyWithSoftmax:
                    return CNTKLib.CrossEntropyWithSoftmax(network, variable, name);

                case LossFunctions.ClassificationError:
                    return CNTKLib.ClassificationError(network, variable, name);

                case LossFunctions.SquaredError:
                    return CNTKLib.SquaredError(network, variable, name);

                default:
                    return CNTKLib.SquaredError(network, variable, name);
            }
        }

        /// <summary>
        /// Helper for Network creation and using ANNdotNET lib
        /// </summary>
        /// <param name="inputVar"></param>
        /// <returns></returns>
        private Function createCNTKModel(Variable inputVar)
        {
            
            //Simple Feed Forward
            if (Model.ModelData.NetworkType == 0 || Model.ModelData.NetworkType == 1)
            {
                if (Model.ModelData.NetworkType == 0)
                    Model.ModelData.HLayers = 1;
                var ffnn = new FeedForwaredNN(m_Device);

                var net = ffnn.CreateNet(inputVar, Model.OutputDim, Model.ModelData.HLayers,new int[1] { Model.ModelData.Neurons},
                    Model.ModelData.ActivationHidden, Model.ModelData.ActivationOutput,"feedForwardNet");
                return net;
            }
            else if (Model.ModelData.NetworkType == 2)
            {
                //
                var lstm = new LSTMReccurentNN(Model.ModelData.HLayers, Model.ModelData.Neurons, m_Device);
                var net = lstm.CreateNet(inputVar, Model.OutputDim, "lstmRNNModel");
                return net;
            }
            else if (Model.ModelData.NetworkType == 1)
            {
                //
                var lstm = new LSTMReccurentNN(Model.ModelData.HLayers, Model.ModelData.Neurons, m_Device);
                var net = lstm.CreateSequenceNet(inputVar, Model.ModelData.Embeding, Model.OutputDim, "lstmSequenceLSTMModel");
                return net;
            }
            else
                throw new Exception("Unknown Network type!");
        }


       
       
        private void reportOnGraphs(Trainer trainer, Function model, int iteration)
        {
            
        }

        public static void PrintTrainingProgress(Trainer trainer, int minibatchIdx, int outputFrequencyInMinibatches)
        {
            if ((minibatchIdx % outputFrequencyInMinibatches) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
            {
                float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
                float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
                Console.WriteLine($"Mini batch: {minibatchIdx} Squared Error = {trainLossValue}, Classification Error = {evaluationValue}");
            }

        }

        #endregion
    }
}
