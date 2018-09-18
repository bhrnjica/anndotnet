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
using NNetwork.Core.Common;
using NNetwork.Core.Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Class implements Training models using CTK library.
    /// </summary>
    public class MLTrainerEx : MLTrainer
    {
        #region Ctor and private members
        
        public MLTrainerEx(List<StreamConfiguration> stream, List<Variable> inputs, List<Variable> outputs)
            :base(stream,inputs,outputs)
        {
           
        }
        #endregion


        
        #region Private Members
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
        protected override ProgressData progressTraining(TrainingParameters trParams, Trainer trainer, 
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
                var evParams = new EvalParameters()
                {

                    MinibatchSize = trParams.BatchSize,
                    MBSource = new MinibatchSourceEx(mbs.Type, StreamConfigurations.ToArray(), mbs.TrainingDataFile, null, MinibatchSource.FullDataSweep, false),
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
                var evParams = new EvalParameters()
                {
                    MinibatchSize = trParams.BatchSize,
                    //StrmsConfig = StreamConfigurations.ToArray(),
                    MBSource = new MinibatchSourceEx(mbs.Type, StreamConfigurations.ToArray(), mbs.ValidationDataFile, null, MinibatchSource.FullDataSweep, false),
                    Ouptut = OutputVariables,
                    Input = InputVariables,
                };
                //
                var result = MLEvaluator.EvaluateFunction(trainer.Model(), evParams, device);
                validEval = MLEvaluator.CalculateMetrics(trainer.EvaluationFunction().Name, result.actual, result.predicted, device);

            }

            //here we should decide if the current model worth to be saved into temp location
            // depending of the Evaluation function which sometimes can be better if it is greater that previous (e.g. ClassificationAccuracy)
            if(isBetterThanPrevious(trainEval, validEval, StatMetrics.IsGoalToMinimize(trainer.EvaluationFunction())) && trParams.SaveModelWhileTraining)
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

        
    
        #endregion

    }
}
