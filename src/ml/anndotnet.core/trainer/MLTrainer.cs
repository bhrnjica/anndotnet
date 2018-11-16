//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                                      //
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
using NNetwork.Core.Metrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Class implements Training models using CTK library.
    /// </summary>
    public class MLTrainer : MLTrainerBase
    {
        #region Ctor and private members
        //helpers for the model evaluation
        protected double m_PrevTrainingEval = double.MaxValue;
        protected double m_PrevValidationEval = double.MaxValue;
        protected string m_bestModelPath = "";
        protected List<Tuple<double, double, string>> m_ModelEvaluations;
        protected UnorderedMapVariableMinibatchData m_TrainData;
        protected UnorderedMapVariableMinibatchData m_ValidationData;
        //training history: header name of loss and evaluation function
        //data: Iteration, AvgMinibatchLoss, AvgMinibatchEvaluation, FullTrainDSEvaluation, FullValidationDSEvaluation
        protected List<Tuple<int, float, float, float, float>> m_trainingHistory;

        public MLTrainer(List<StreamConfiguration> stream, List<Variable> inputs, List<Variable> outputs)
        {
            m_StreamConfig = stream;
            m_Inputs = inputs;
            m_Outputs = outputs;
            m_TrainData = null;
            m_ValidationData = null;
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Helper method in order to create training before training. It also try to restore trained from 
        /// checkpoint file in order to continue with training
        /// </summary>
        /// <param name="network"></param>
        /// <param name="lrParams"></param>
        /// <param name="trParams"></param>
        /// <param name="modelCheckPoint"></param>
        /// <returns></returns>
        public Trainer CreateTrainer(Function network, LearningParameters lrParams, TrainingParameters trParams, string modelCheckPoint, string historyPath)
        {
            try
            {
                
                //create trainer
                var trainer = createTrainer(network, lrParams, trParams);

                //set initial value for the evaluation value
                m_PrevTrainingEval = StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction()) ? double.MaxValue : double.MinValue;
                m_PrevValidationEval = StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction()) ? double.MaxValue : double.MinValue;

                //in case modelCheckpoint is saved and user select re-training existing trainer
                //first check if the checkpoint is available
                if (trParams.ContinueTraining && !string.IsNullOrEmpty(modelCheckPoint) && File.Exists(modelCheckPoint))
                {
                    //if the network model changed checkpoint state will throw exception
                    //in that case throw exception that re-training is not possible
                    try
                    {
                        trainer.RestoreFromCheckpoint(modelCheckPoint);
                        //load history of training in case continuation of training is requested
                        m_trainingHistory = MLFactory.LoadTrainingHistory(historyPath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("The Trainer cannot be restored from the previous state probably because the network has changed." +
                            "\n Uncheck 'Continue Training' and train the model from scratch.");
                        throw;
                    }

                }

                else//delete checkpoint if exist in case no retraining is required, 
                    //so the next checkpoint saving is free of previous checkpoints
                {
                    //delete heckpoint
                    if (File.Exists(modelCheckPoint))
                        File.Delete(modelCheckPoint);
                    
                    //delete history 
                    if (File.Exists(historyPath))
                        File.Delete(historyPath);
                }
                return trainer;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        
        /// <summary>
        /// Main method for training 
        /// </summary>
        /// <param name="trainer"></param>
        /// <param name="network"></param>
        /// <param name="trParams"></param>
        /// <param name="miniBatchSource"></param>
        /// <param name="device"></param>
        /// <param name="token"></param>
        /// <param name="progress"></param>
        /// <param name="modelCheckPoint"></param>
        /// <returns></returns>
        public override TrainResult Train(Trainer trainer, Function network, TrainingParameters trParams,
    MinibatchSourceEx miniBatchSource, DeviceDescriptor device, CancellationToken token, TrainingProgress progress, string modelCheckPoint, string historyPath)
        {           
            try
            {        
                //create trainer result.
                // the variable indicate how training process is ended
                // completed, stopped, crashed,
                var trainResult = new TrainResult();
                var historyFile = "";
                //create training process evaluation collection
                //for each iteration it is stored evaluationValue for training, and validation set with the model 
                m_ModelEvaluations = new List<Tuple<double, double, string>>();

                //check what is the optimization (Minimization (error) or maximization (accuracy))
                bool isMinimize = StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction());

                //setup first iteration
                if (m_trainingHistory == null)
                    m_trainingHistory = new List<Tuple<int, float, float, float, float>>();
                //in case of continuation of training iteration must start with the last of path previous training process
                int epoch = (m_trainingHistory.Count > 0)? m_trainingHistory.Last().Item1+1:1;
                
                //define progressData
                ProgressData prData=null;

                //define helper variable collection
                var vars = InputVariables.Union(OutputVariables).ToList();
               
                //training process
                while (true)
                {
                    //get next mini batch data 
                    var args = miniBatchSource.GetNextMinibatch(trParams.BatchSize, device);                  
                    var isSweepEnd = args.Any(a => a.Value.sweepEnd);

                    //prepare the data for trainer
                    var arguments = MinibatchSourceEx.ToMinibatchValueData(args, vars); 
                    trainer.TrainMinibatch(arguments, isSweepEnd, device);

                    //make progress
                    if (isSweepEnd)
                    {
                        //check the progress of the training process
                        prData = progressTraining(trParams, trainer, network, miniBatchSource, epoch, progress, device);
                        
                        //check if training process ends
                        if (epoch >= trParams.Epochs)
                        {
                            //save training checkpoint state
                            if(!string.IsNullOrEmpty(modelCheckPoint))
                                trainer.SaveCheckpoint(modelCheckPoint);

                            //save training history
                            if (!string.IsNullOrEmpty(historyPath))
                            {
                                string header = $"{trainer.LossFunction().Name};{trainer.EvaluationFunction().Name};";
                                MLFactory.SaveTrainingHistory(m_trainingHistory, header, historyPath);
                            }

                            //save best or last trained model and send report last time before trainer completes 
                            var bestModelPath = saveBestModel(trParams, trainer.Model(), epoch, isMinimize);
                           //
                            if (progress != null)
                                progress(prData);
                            //
                            trainResult.Iteration = epoch;
                            trainResult.ProcessState = ProcessState.Compleated;
                            trainResult.BestModelFile = bestModelPath;
                            trainResult.TrainingHistoryFile = historyFile;
                            break;
                        }
                        else
                            epoch++;
                    }
                    //stop in case user request it
                    if (token.IsCancellationRequested)
                    {
                        if (!string.IsNullOrEmpty(modelCheckPoint))
                            trainer.SaveCheckpoint(modelCheckPoint);

                        //save training history
                        if (!string.IsNullOrEmpty(historyPath))
                        {
                            string header = $"{trainer.LossFunction().Name};{trainer.EvaluationFunction().Name};";
                            MLFactory.SaveTrainingHistory(m_trainingHistory, header, historyPath);
                        }

                        //sometime stopping training process can be before first epoch passed so make a incomplete progress 
                        if (prData==null)//check the progress of the training process
                            prData = progressTraining(trParams, trainer, network, miniBatchSource, epoch, progress, device);

                        //save best or last trained model and send report last time before trainer terminates   
                        var bestModelPath = saveBestModel(trParams, trainer.Model(), epoch, isMinimize);
                        //
                        if (progress != null)
                            progress(prData);

                        //setup training result
                        trainResult.Iteration = prData.EpochCurrent;
                        trainResult.ProcessState = ProcessState.Stopped;
                        trainResult.BestModelFile = bestModelPath;
                        trainResult.TrainingHistoryFile = historyFile;
                        break;
                    }
                       
                }

                return trainResult;
            }
            catch (Exception ex)
            {
                var ee = ex;
                throw;
            }
            finally
            {

            }
        }


        #endregion

        #region Private Members
        protected virtual ProgressData progressTraining(TrainingParameters trParams, Trainer trainer,
            Function network, MinibatchSourceEx mbs, int epoch, TrainingProgress progress, DeviceDescriptor device)
        {
            //calculate average training loss and evaluation
            var mbAvgLoss = trainer.PreviousMinibatchLossAverage();
            var mbAvgEval = trainer.PreviousMinibatchEvaluationAverage();

            //get training dataset
            double trainEval = mbAvgEval;
            //sometimes when the data set is huge validation model against
            // full training dataset could take time, so we can skip it by setting parameter 'FullTrainingSetEval'
            if (trParams.FullTrainingSetEval)
            {
                var evParams = new EvaluationParameters()
                {

                    MinibatchSize = trParams.BatchSize,
                    MBSource = new MinibatchSourceEx(mbs.Type, this.StreamConfigurations.ToArray(), this.InputVariables, this.OutputVariables, mbs.TrainingDataFile, null, MinibatchSource.FullDataSweep, false),
                    Ouptut = OutputVariables,
                    Input = InputVariables,
                };

                var result = MLEvaluator.EvaluateFunction(trainer.Model(), evParams, device);
                trainEval = MLEvaluator.CalculateMetrics(trainer.EvaluationFunction().Name, result.actual, result.predicted, device);

            }

            string bestModelPath = m_bestModelPath;
            double validEval = 0;

            //in case validation data set is empty don't perform testminibatch
            if (!string.IsNullOrEmpty(mbs.ValidationDataFile))
            {
                var evParams = new EvaluationParameters()
                {
                    MinibatchSize = trParams.BatchSize,
                    //StrmsConfig = StreamConfigurations.ToArray(),
                    MBSource = new MinibatchSourceEx(mbs.Type, this.StreamConfigurations.ToArray(), this.InputVariables, this.OutputVariables, mbs.ValidationDataFile, null, MinibatchSource.FullDataSweep, false),
                    Ouptut = OutputVariables,
                    Input = InputVariables,
                };
                //
                var result = MLEvaluator.EvaluateFunction(trainer.Model(), evParams, device);
                validEval = MLEvaluator.CalculateMetrics(trainer.EvaluationFunction().Name, result.actual, result.predicted, device);

            }

            //here we should decide if the current model worth to be saved into temp location
            // depending of the Evaluation function which sometimes can be better if it is greater that previous (e.g. ClassificationAccuracy)
            if (isBetterThanPrevious(trainEval, validEval, StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction())) && trParams.SaveModelWhileTraining)
            {
                //save model
                var strFilePath = $"{trParams.ModelTempLocation}\\model_at_{epoch}of{trParams.Epochs}_epochs_TimeSpan_{DateTime.Now.Ticks}";
                if (!Directory.Exists(trParams.ModelTempLocation))
                    Directory.CreateDirectory(trParams.ModelTempLocation);

                //save temp model
                network.Save(strFilePath);

                //set training and validation evaluation to previous state
                m_PrevTrainingEval = trainEval;
                m_PrevValidationEval = validEval;
                bestModelPath = strFilePath;

                var tpl = Tuple.Create<double, double, string>(trainEval, validEval, strFilePath);
                m_ModelEvaluations.Add(tpl);
            }


            m_bestModelPath = bestModelPath;

            //create progressData object
            var prData = new ProgressData();
            prData.EpochTotal = trParams.Epochs;
            prData.EpochCurrent = epoch;
            prData.EvaluationFunName = trainer.EvaluationFunction().Name;
            prData.TrainEval = trainEval;
            prData.ValidationEval = validEval;
            prData.MinibatchAverageEval = mbAvgEval;
            prData.MinibatchAverageLoss = mbAvgLoss;
            //prData.BestModel = bestModelPath;

            //the progress is only reported if satisfied the following condition
            if (progress != null && (epoch % trParams.ProgressFrequency == 0 || epoch == 1 || epoch == trParams.Epochs))
            {
                //add info to the history
                m_trainingHistory.Add(new Tuple<int, float, float, float, float>(epoch, (float)mbAvgLoss, (float)mbAvgEval,
                    (float)trainEval, (float)validEval));

                //send progress 
                progress(prData);
                //
                //Console.WriteLine($"Epoch={epoch} of {trParams.Epochs} processed.");
            }

            //return progress data
            return prData;
        }

        /// <summary>
        /// Calback from the training in order to inform user about trining progress
        /// </summary>
        /// <param name="trParams"></param>
        /// <param name="trainer"></param>
        /// <param name="network"></param>
        /// <param name="mbs"></param>
        /// <param name="epoch"></param>
        /// <param name="progress"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        //protected virtual ProgressData progressTraining(TrainingParameters trParams, Trainer trainer, 
        //    Function network, MinibatchSourceEx mbs, int epoch, TrainingProgress progress, DeviceDescriptor device)
        //{
        //    //calculate average training loss and evaluation
        //    var mbAvgLoss = trainer.PreviousMinibatchLossAverage();
        //    var mbAvgEval = trainer.PreviousMinibatchEvaluationAverage();
        //    var vars = InputVariables.Union(OutputVariables).ToList();
        //    //get training dataset
        //    double trainEval = mbAvgEval;
        //    //sometimes when the data set is huge validation model against
        //    // full training dataset could take time, so we can skip it by setting parameter 'FullTrainingSetEval'
        //    if (trParams.FullTrainingSetEval)
        //    {
        //        if (m_TrainData == null || m_TrainData.Values.Any(x => x.data.IsValid == false))
        //        {
        //            using (var streamDatat = MinibatchSourceEx.GetFullBatch(mbs.Type, mbs.TrainingDataFile, mbs.StreamConfigurations, device))
        //            {
        //                //get full training dataset
        //                m_TrainData = MinibatchSourceEx.ToMinibatchData(streamDatat, vars, mbs.Type);

        //            }
        //            //perform evaluation of the current model on whole training dataset
        //            trainEval = trainer.TestMinibatch(m_TrainData, device);
        //        }

        //    }

        //    string bestModelPath = m_bestModelPath;
        //    double validEval = 0;

        //    //in case validation data set is empty don't perform test-minibatch
        //    if (!string.IsNullOrEmpty(mbs.ValidationDataFile))
        //    {
        //        //get validation dataset
        //        using (var streamData = MinibatchSourceEx.GetFullBatch(mbs.Type, mbs.ValidationDataFile, mbs.StreamConfigurations, device))
        //        {
        //            //store validation data for future testing
        //            m_ValidationData = MinibatchSourceEx.ToMinibatchData(streamData, vars, mbs.Type);

        //        }
        //        //perform evaluation of the current model with validation dataset
        //        validEval = trainer.TestMinibatch(m_ValidationData, device);
        //    }

        //    //here we should decide if the current model worth to be saved into temp location
        //    // depending of the Evaluation function which sometimes can be better if it is greater that previous (e.g. ClassificationAccuracy)
        //    if(isBetterThanPrevious(trainEval, validEval, StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction())) && trParams.SaveModelWhileTraining)
        //    {
        //        //save model
        //        var strFilePath = $"{trParams.ModelTempLocation}\\model_at_{epoch}of{trParams.Epochs}_epochs_TimeSpan_{DateTime.Now.Ticks}";
        //        if (!Directory.Exists(trParams.ModelTempLocation))
        //            Directory.CreateDirectory(trParams.ModelTempLocation);

        //        //save temp model
        //        network.Save(strFilePath);

        //        //set training and validation evaluation to previous state
        //        m_PrevTrainingEval = trainEval;
        //        m_PrevValidationEval = validEval;
        //        bestModelPath = strFilePath;

        //        var tpl = Tuple.Create<double, double, string>(trainEval, validEval, strFilePath);
        //        m_ModelEvaluations.Add(tpl);
        //    }


        //    m_bestModelPath = bestModelPath;

        //    //create progressData object
        //    var prData = new ProgressData();
        //    prData.EpochTotal = trParams.Epochs;
        //    prData.EpochCurrent = epoch;
        //    prData.EvaluationFunName = trainer.EvaluationFunction().Name;
        //    prData.TrainEval = trainEval;
        //    prData.ValidationEval = validEval;
        //    prData.MinibatchAverageEval = mbAvgEval;
        //    prData.MinibatchAverageLoss = mbAvgLoss;

        //    //the progress is only reported if satisfied the following condition
        //    if (progress != null && (epoch % trParams.ProgressFrequency == 0 || epoch == 1 || epoch == trParams.Epochs))
        //    {
        //        //add info to the history
        //        m_trainingHistory.Add(new Tuple<int, float, float, float, float>(epoch, (float)mbAvgLoss, (float)mbAvgEval,
        //            (float)trainEval, (float)validEval));

        //        //send progress 
        //        progress(prData);
        //        //
        //        //Console.WriteLine($"Epoch={epoch} of {trParams.Epochs} processed.");
        //    }

        //    //return progress data
        //    return prData;
        //}

        /// <summary>
        /// Try to figure out is the current model better than previous. 
        /// Since the evaluation function can also be such Accuracy which is better if it is greater the
        /// function resolve this fact and returns correct result value 
        /// </summary>
        /// <param name="trainEval"></param>
        /// <param name="validEval"></param>
        /// <param name="isMinimization"></param>
        /// <returns></returns>
        protected bool isBetterThanPrevious(double trainEval, double validEval, bool isMinimization)
        {
            if(isMinimization)
            {
                if(m_PrevTrainingEval > trainEval && m_PrevValidationEval > validEval)
                {
                    return true;
                }
            }
            else
            {
                if (m_PrevTrainingEval < trainEval && m_PrevValidationEval <= validEval)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// After training process completes, the method tries to find best saved model and identify it as the best model. 
        /// In case no models are saved during training process it uses the last trained model.
        /// </summary>
        /// <param name="trParams"></param>
        /// <param name="lastModel"></param>
        /// <param name="currentIteration">the last iteration when the traininig process stopped</param>
        /// <param name="isMinimized"></param>
        /// <returns></returns>
        private string saveBestModel(TrainingParameters trParams, Function lastModel,int currentIteration, bool isMinimized)
        {
            //
            var orderredModels = isMinimized ? m_ModelEvaluations.OrderBy(x => x.Item1).ToList() : m_ModelEvaluations.OrderByDescending(x => x.Item1).ToList();

            //in case no models are stored during training process save last trained model
            if (orderredModels.Count == 0)
            {
                if (lastModel != null)
                {
                    //save model in temp folder
                    var strFilePath = $"{trParams.ModelTempLocation}\\model_at_{currentIteration}of{trParams.Epochs}_epochs_TimeSpan_{DateTime.Now.Ticks}";
                    if (!Directory.Exists(trParams.ModelTempLocation))
                        Directory.CreateDirectory(trParams.ModelTempLocation);
                    //save temp model
                    lastModel.Save(strFilePath);
                    //then save model to final folder with 
                    var modelE = new Tuple<double, double, string>(-1,-1, strFilePath);
                    return saveModel(trParams, modelE);
                }
                else
                    return "";//no model at all
                
            }
            //in case we have only one model
            if (orderredModels.Count == 1)
                return saveModel(trParams, orderredModels[0]);

            //first set first model in the collection as the best model 
            var valValid = orderredModels.First().Item2;

            //go through all ordered model and find the one which has the lowest error on validation dataset
            for (int i = 1; i < orderredModels.Count; i++)
            {
                var tplNext = orderredModels[i];
                //
                if ((isMinimized &&  tplNext.Item2 >= valValid) || (!isMinimized && tplNext.Item2 <= valValid))
                {
                    return saveModel(trParams, orderredModels[i - 1]);
                }
                //
                valValid = tplNext.Item2;
            }

            return "";//no model at all
        }

        /// <summary>
        /// save specific model into final model location,  delete temporary models and return model path
        /// </summary>
        /// <param name="trParams">Training parameters</param>
        /// <param name="tpl">Temporary stored model information</param>
        /// <returns></returns>
        private static string saveModel(TrainingParameters trParams, Tuple<double, double, string> tpl)
        {
            //extract file name from temp_models dir
            var tempModelsDir = Path.GetDirectoryName(tpl.Item3);
            var dirInfo = new DirectoryInfo(tempModelsDir);

            //folder for final model
            var finDir = trParams.ModelFinalLocation;
            //in case directory doesn't exist
            if (!Directory.Exists(finDir))
                Directory.CreateDirectory(finDir);

            //copy best model to final location
            var fName = Path.GetFileName(tpl.Item3);
            var fullPath =Path.Combine(finDir, fName);    
            File.Copy(tpl.Item3, fullPath);

            //delete temp_modes folder
            MLFactory.DeleteAllFiles(tempModelsDir);
            var bestModelPath = $"{MLFactory.m_MLModelFolder}\\{fName}";//return always relative path 
            return bestModelPath;
        }

        /// <summary>
        /// Convert the data to suitable type trainer supports 
        /// </summary>
        /// <param name="args"> data being converted</param>
        /// <returns></returns>
        private UnorderedMapVariableMinibatchData prepareForTrainerEx(UnorderedMapStreamInformationMinibatchData args, bool cloneData)
        {
            // var d = new Dictionary<Variable, MinibatchData>();
            var d = new UnorderedMapVariableMinibatchData();
            for (int i = 0; i < args.Count; i++)
            {
                var k = args.ElementAt(i);

                var var = InputVariables.Union(OutputVariables).Where(x => x.Name.Equals(k.Key.m_name)).FirstOrDefault();
                if (var == null)
                    throw new Exception("Variable doesn't exist!");

                //clone data first
                var key = k.Key;
                MinibatchData mbData = null;
                if(cloneData)
                    mbData = new MinibatchData(k.Value.data.DeepClone(), k.Value.numberOfSequences, k.Value.numberOfSamples, k.Value.sweepEnd);
                else
                    mbData = k.Value;
                d.Add(var, mbData);
            }

            return d;
        }

        
        /// <summary>
        /// Creates trained based on training and learning parameters
        /// </summary>
        /// <param name="network">Network model being trained</param>
        /// <param name="lrParams">Learning parameters</param>
        /// <param name="trParams">Training parameters</param>
        /// <returns></returns>
        private Trainer createTrainer(Function network, LearningParameters lrParams, TrainingParameters trParams)
        {
            //network parameters
            var zParams = new ParameterVector(network.Parameters().ToList());

            //create loss and eval
            (Function loss, Function eval) = createLossEvalFunction(lrParams, network);

            //learners
            var learners = createLearners(network, lrParams);

            //trainer
            var trainer = Trainer.CreateTrainer(network, loss, eval, learners);
            //
            return trainer;
        }

        /// <summary>
        /// Creating Loss an Evaluation function for defined output(s)
        /// </summary>
        /// <param name="lrParams">learning parameters</param>
        /// <param name="network">network model</param>
        /// <returns></returns>
        private (Function loss, Function eval) createLossEvalFunction(LearningParameters lrParams, Function network)
        {
            //In case of single output variable create tuple of only two functions
            if (OutputVariables.Count == 1)
            {
                return (createFunction(lrParams.LossFunction, network, OutputVariables[0]),
                    createFunction(lrParams.EvaluationFunction, network, OutputVariables[0]));
            }

            //handling multiple outputs by accumulating loss and evaluation function for each output
            var trLosses = new VariableVector();
            var trEvals = new VariableVector();
            for (int i = 0; i < OutputVariables.Count; i++)
            {
                var outVar = OutputVariables[i];
                var output = network.Outputs[i];
                var l = createFunction(lrParams.LossFunction, output, outVar);
                trLosses.Add(l);
                var e = createFunction(lrParams.EvaluationFunction, output, outVar);
                trEvals.Add(e);
            }

            //create cumulative evaluation and loss function
            var loss = CNTKLib.Sum(trLosses, "Overall_" + lrParams.LossFunction.ToString());
            var eval = CNTKLib.Sum(trEvals, "Overall_" + lrParams.EvaluationFunction.ToString());

            //
            return (loss, eval);
        }

        /// <summary>
        /// Defines CNTK Function with certain arguments.
        /// </summary>
        /// <param name="function">CNTK function</param>
        /// <param name="prediction">First parameters of the function.</param>
        /// <param name="target">Second parameters of the function</param>
        /// <returns></returns>
        private Function createFunction(EFunction function, Function prediction, Variable target)
        {
            switch (function)
            {
                case EFunction.BinaryCrossEntropy:
                    return CNTKLib.BinaryCrossEntropy(prediction, target, function.ToString());
                case EFunction.CrossEntropyWithSoftmax:
                    return CNTKLib.CrossEntropyWithSoftmax(prediction, target, function.ToString());
                case EFunction.ClassificationError:
                    return CNTKLib.ClassificationError(prediction, target, function.ToString());
                case EFunction.SquaredError:
                    return CNTKLib.SquaredError(prediction, target, function.ToString());
                case EFunction.RMSError:
                    return StatMetrics.RMSError(prediction, target, function.ToString());
                case EFunction.MSError:
                    return StatMetrics.MSError(prediction, target, function.ToString());
                case EFunction.ClassificationAccuracy:
                    return StatMetrics.ClassificationAccuracy(prediction, target, function.ToString());

                default:
                    throw new Exception($"The '{function}' function is not supported!");
            }
        }

        /// <summary>
        /// Creates the learner based on learning parameters. 
        /// ToDo: Not all learners parameters defined
        /// </summary>
        /// <param name="network">Network model being trained</param>
        /// <param name="lrParams">Learning parameters.</param>
        /// <returns></returns>
        private List<Learner> createLearners(Function network, LearningParameters lrParams)
        {
            //learning rate and momentum values
            var lr = new TrainingParameterScheduleDouble(lrParams.LearningRate);
            var mm = CNTKLib.MomentumAsTimeConstantSchedule(lrParams.Momentum);
            var addParam = new AdditionalLearningOptions();

            //
            if (lrParams.L1Regularizer > 0)
                addParam.l1RegularizationWeight = lrParams.L1Regularizer;
            if (lrParams.L2Regularizer > 0)
                addParam.l2RegularizationWeight = lrParams.L2Regularizer;

            //SGD Momentum learner
            if (lrParams.LearnerType == LearnerType.MomentumSGDLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = Learner.MomentumSGDLearner(network.Parameters(), lr, mm, true, addParam);
                llr.Add(msgd);
                return llr;
            }
            //SGDLearner - rate and regulars
            else if (lrParams.LearnerType == LearnerType.SGDLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = Learner.SGDLearner(network.Parameters(), lr, addParam);
                llr.Add(msgd);
                return llr;
            }
            //FSAdaGradLearner learner - rate, moment regulars
            else if (lrParams.LearnerType == LearnerType.FSAdaGradLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = CNTKLib.FSAdaGradLearner(new ParameterVector(network.Parameters().ToList()), lr, mm);
                llr.Add(msgd);
                return llr;
            }
            //AdamLearner learner
            else if (lrParams.LearnerType == LearnerType.AdamLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = CNTKLib.AdamLearner(new ParameterVector(network.Parameters().ToList()), lr, mm);
                llr.Add(msgd);
                return llr;
            }
            //AdaGradLearner learner - Learning rate and regularizers
            else if (lrParams.LearnerType == LearnerType.AdaGradLearner)
            {
                //
                var llr = new List<Learner>();
                var msgd = CNTKLib.AdaGradLearner(new ParameterVector(network.Parameters().ToList()), lr, false, addParam);
                llr.Add(msgd);
                return llr;
            }
            else
                throw new Exception("Learner type is not supported!");
        }
    
        #endregion

    }
}
