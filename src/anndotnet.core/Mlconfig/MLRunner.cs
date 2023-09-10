////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Data;
using Anndotnet.Core.Util;

using Anndotnet.Core.Entities;

using Daany.MathStuff.Random;
using Anndotnet.Core.Trainers;

using Anndotnet.Core.Interfaces;
using Anndotnet.Core.MlMetrics;

using Daany;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Mlconfig
{
    public class MlRunner :IRunner,IDataSplitter,IEvaluator, IDisposable
    {
        private AnnModel _model;

        private DataLoader _train;
        private DataLoader _val;

        private readonly MlConfig _mlConfig;
        private readonly IPrintResults _reportProgress;

        public MlRunner(MlConfig mlConfig, IPrintResults reportProgress)
        {
            _mlConfig = mlConfig;
            _reportProgress = reportProgress;
        }

        public async Task TrainAsync(DataFeed trainData, IProgressTraining progress, Device device = null)
        {

            //create a model
            int inputDim = (int)trainData.InputDimension;
            int outputDim = (int)trainData.OutputDimension;

            _model = MlFactory.CreateNetwork(_mlConfig.Name, _mlConfig.Network, inputDim, outputDim, device);

            //Train - Validation training
            (_train, _val) = Split(trainData, _mlConfig.TrainingParameters.SplitPercentage,
                                            _mlConfig.TrainingParameters.ShuffleWhenSplit, _mlConfig.TrainingParameters.MiniBatchSize);

            ITrainer trainer = null;
            if (_mlConfig.TrainingParameters.TrainingType == TrainingType.TvTraining)
            {
                trainer = new TvTrainer(_model,_train,_val,
                                                    _mlConfig.TrainingParameters,
                                                    _mlConfig.LearningParameters, 
                                                    progress);

            }
            else
            {
                trainer = new CvTrainer(_model, trainData,
                                                    _mlConfig.TrainingParameters,
                                                    _mlConfig.LearningParameters,
                                                    progress);
            }

            await trainer.RunAsync();
        }
        
        public void CalculatePerformance()
        {
          var (trainPrediction, trainTarget) = EvaluateModel(_mlConfig.LearningParameters.LossFunction, true );
          var (validPrediction, validTarget) = EvaluateModel(_mlConfig.LearningParameters.LossFunction, false);

          //multiclass
          var col = _mlConfig.Metadata.First(x => x.MLType == MLColumnType.Label);
          var classes = col.Transformer.ClassValues; 
          
          
          if (classes!= null && classes.Length > 2)
          {
                var cm = new ConfusionMatrix(trainPrediction.Select(x => Convert.ToInt32(x)).ToArray(),
                                          trainTarget.Select(x => Convert.ToInt32(x)).ToArray(), 3);

             var cmTest = new ConfusionMatrix(validPrediction.Select(Convert.ToInt32).ToArray(),
                                              validTarget.Select(Convert.ToInt32).ToArray(), 3);


             Console.WriteLine("Train data:");
             _reportProgress.ConsolePrintConfusionMatrix(_mlConfig.Name + "_Train",cm, classes);

             Console.WriteLine("Valid data:");
             _reportProgress.ConsolePrintConfusionMatrix(_mlConfig.Name + "_Test", cmTest,classes);

             _reportProgress.PrintMultiClassClassificationMetrics(_mlConfig.Name,cmTest);
          }

          else if (classes != null && classes.Length == 2)
          {
              var rm = new BinaryClassificationMetrics(validPrediction, validTarget, classes);
              _reportProgress.PrintBinaryClassificationMetrics(_mlConfig.Name, rm);
              _reportProgress.ConsolePrintConfusionMatrix(_mlConfig.Name,rm.ConfusionMatrix, rm.Classes);
          }
          else
          {
              var rm = new RegressionMetrics(validPrediction, validTarget);
              _reportProgress.PrintRegressionMetrics(_mlConfig.Name,rm);
          }

        }

        public (List<float> predicted, List<float> target) EvaluateModel(LossFunction loss, bool isTrainingData)
        {
           _model.eval();

           var totPredicted = new List<float>();
           var totTarget = new List<float>();

           using (var d = torch.NewDisposeScope())
           {
               var evData = isTrainingData ? _train : _val;

               foreach (var data in evData)
               {
                   var predicted = _model.forward(data["X"]);

                    var target = TvTrainer.TargetTransform(data["y"], _mlConfig.LearningParameters.LossFunction);

                    AccumulateResults(totPredicted, predicted, target, totTarget);
               }

               return (totPredicted, totTarget);
           }

        }

        private static void AccumulateResults(List<float> totPredicted, Tensor predicted, Tensor target, List<float> totTarget)
        {
            //predicted output is always of float type
            if (predicted.shape.Last() > 2)
            {
                totPredicted.AddRange(predicted.argmax(1).to_type(ScalarType.Float32).data<float>().ToList());
            }
            else
            {
                totPredicted.AddRange(predicted.data<float>().ToList());
                

            }

            //target data type can be long or float depending of the ML problem type
            // for classification problems target is int or long
            // for regression is float
            if (target.dtype == ScalarType.Int64 || target.dtype == ScalarType.Int32)
            {
                totTarget.AddRange(target.data<long>().ToList().Select(t => Convert.ToSingle(t)));
            }
            else
            {
                totTarget.AddRange(target.data<float>().ToList());
            }
        }

        public (DataLoader train, DataLoader validation) Split(DataFeed data,int testPercentage, bool shuffle, int batchSize, int seed = 1234)
        {
            var trainSize = (long)(data.Count * testPercentage / 100.0);
            var evalSize = data.Count - trainSize;

            var lst = LongEnumerable.Range(0, trainSize + evalSize).ToArray();

            var trainIds = shuffle ? TSRandom.Rand<long>(lst, (int)trainSize, seed).ToList() : lst.Take((int)trainSize).ToList();
            var testIds = lst.Except(trainIds).ToList();

            var train = new DataLoader(data, batchSize, trainIds);
            var valid = new DataLoader(data, batchSize, testIds);

            return (train, valid);
        }

        public void Dispose()
        {
            if (_train != null)
            {
                _train.Dispose();
            }
            if (_val != null)
            {
                _val.Dispose();
            }
           
        }

    }
}
