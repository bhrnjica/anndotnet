using System.Xml;
using AnnDotNet.Core;
using Anndotnet.Core.Util;
using AnnDotNet.Core.Data;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Model;
using Daany.MathStuff.Random;
using AnnDotNet.Core.Trainers;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using Anndotnet.Core.Interfaces;
using Daany;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Mlconfig
{
    public class MLRunner : IDisposable
    {
        private readonly MlConfig _mlConfig;
        private readonly IPrintResults _reportProgress;
        private AnnModel _model;
        private DataLoader _train;
        private DataLoader _val;
        public MLRunner(MlConfig mlConfig, IPrintResults reportProgress)
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
          var tresult = EvaluateModel(_mlConfig.LearningParameters.LossFunction,true );
          var vresult = EvaluateModel(_mlConfig.LearningParameters.LossFunction, false);

          //multiclass
          var col = _mlConfig.Metadata.First(x => x.MLType == MLColumnType.Label);
          var classes = col.Transformer.ClassValues; 
          
          
          if (classes!= null && classes.Length > 2)
          {
             var cm = new ConfusionMatrix(tresult.predicted.Select(x=>Convert.ToInt32(x)).ToArray(),
                                tresult.target.Select(x => Convert.ToInt32(x)).ToArray(), 3 );

             var cmTest = new ConfusionMatrix(vresult.predicted.Select(x => Convert.ToInt32(x)).ToArray(),
                 vresult.target.Select(x => Convert.ToInt32(x)).ToArray(), 3);


             Console.WriteLine("Train data:");
             _reportProgress.ConsolePrintConfusionMatrix(_mlConfig.Name + "_Train",cm, classes);
             Console.WriteLine("Valid data:");
             _reportProgress.ConsolePrintConfusionMatrix(_mlConfig.Name + "_Test", cmTest,classes);

             _reportProgress.PrintMultiClassClassificationMetrics(_mlConfig.Name,cmTest);
          }

          else if (classes != null && classes.Length == 2)
          {
              var rm = new BinaryClassificationMetrics(tresult.predicted, tresult.target);
              _reportProgress.PrintBinaryClassificationMetrics(_mlConfig.Name, rm);
          }
          else
          {
              var rm = new RegressionMetrics(tresult.predicted, tresult.target);
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


                    totPredicted.AddRange(predicted.data<float>().ToList());
                    if (target.dtype == ScalarType.Int64 || target.dtype == ScalarType.Int32)
                    {
                        totTarget.AddRange(target.data<long>().ToList().Select(t => Convert.ToSingle(t)));
                    }
                    else
                    {
                        totTarget.AddRange(target.data<float>().ToList());
                    }

               }

               return (totPredicted, totTarget);
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
