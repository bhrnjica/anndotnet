using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using Anndotnet.Core.Interfaces;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Core.Trainers;
using AnnDotNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anndotnet.Core.Mlconfig
{
    public class MLRunner
    {
        private readonly MlConfig _mlConfig;
        private readonly IPrintResults _reportProgress;

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

            var model = MlFactory.CreateNetwork(_mlConfig.Name, _mlConfig.Network, inputDim, outputDim, device);

            //Train - Validation trainining
            ITrainer trainer = null;
            if (_mlConfig.TrainingParameters.TrainingType == TrainingType.TvTraining)
            {
                trainer = new TvTrainer(model, trainData, 
                                                    _mlConfig.TrainingParameters,
                                                    _mlConfig.LearningParameters, 
                                                    progress);

            }
            else
            {
                trainer = new CvTrainer(model, trainData,
                                                    _mlConfig.TrainingParameters,
                                                    _mlConfig.LearningParameters,
                                                    progress);
            }

            await trainer.RunAsync();
        }

    }
}
