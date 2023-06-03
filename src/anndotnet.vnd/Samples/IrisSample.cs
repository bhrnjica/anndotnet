using Anndotnet.Core;
using Anndotnet.Core.Extensions;
using Anndotnet.Vnd.Layers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tensorflow.NumPy;

namespace Anndotnet.Vnd.Samples
{

    public  class IrisSample : SampleBase
    {
        
        public async Task<(NDArray X, NDArray Y)> GenerateData()
        {
            Parser = new DataParser();
            Parser.ColumnSeparator = ';';
            Parser.RawDataName = "mlconfigs/iris/iris_raw.txt";

            Parser.Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species"};

            //unzip the file
            var rawdata = DFExtensions.FromDataParser(Parser);

            await Task.Delay(1);

            var mData = rawdata.ParseMetadata("species");
            Metadata = mData;
            

            //fix missing values
            rawdata.HandlingMissingValue(mData);

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
