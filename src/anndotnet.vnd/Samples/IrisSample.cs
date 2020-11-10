using Anndotnet.Core;
using Anndotnet.Vnd;
using Anndotnet.Vnd.Layers;
using Daany;
using Daany.Ext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Extensions;
using NumSharp;

namespace Anndotnet.Vnd.Samples
{
    public  class IrisSample
    {

        public async Task<(NDArray X, NDArray Y)> GenerateData()
        {
            var names = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species"};

            //unzip the file
            var rawdata = DataFrame.FromCsv("mlconfigs/iris/iris_raw.txt",';', names:names);

            await Task.Delay(1);

            var mData = rawdata.ParseMetadata("species");

            (NDArray X, NDArray Y) = rawdata.TransformData(mData);

            return (X,Y);
        }
        public  (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
        {
            var lParams = new LearningParameters()

            {
                EvaluationFunctions = new List<Metrics>()
                    { Metrics.CAcc, Metrics.CErr },

                LossFunction = Metrics.CCE,
                LearnerType = LearnerType.Adam,
                LearningRate = 0.01f
            };

            var tParams = new TrainingParameters();

            return (tParams, lParams);
        }

        public  List<LayerBase>  CreateNet()
        {
            return new List<LayerBase>()
            {
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 7 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLu", Activation=Activation.ReLU},
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 3 },
                new ActLayer(){Type= LayerType.Activation, Name="Softmax", Activation=Activation.Softmax},
            };
        }
       
    
    }
}
