using Anndotnet.Core;
using Anndotnet.Core.Extensions;
using Anndotnet.Core.Interface;
using Anndotnet.Core.Interfaces;
using Anndotnet.Vnd.Layers;
using Daany;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tensorflow.NumPy;
using XPlot.Plotly;
using Anndotnet.Vnd.Extensions;
namespace Anndotnet.Vnd.Samples
{

    public  class IrisSample : ISample
    {
        public List<ColumnInfo> Metadata { get; set; }
        public DataParser Parser { get; set; }

        public async Task<(NDArray X, NDArray Y)> GenerateData()
        {
            Parser = new DataParser();
            Parser.ColumnSeparator = ';';
            Parser.RawDataName = "mlconfigs/iris/iris_raw.txt";

            Parser.Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species"};

            //unzip the file
            var rawdata = DFExtensions.FromDataParser(Parser);

            await Task.CompletedTask;
            var mData = rawdata.ParseMetadata("species");
            Metadata = mData;


            //fix missing values
            rawdata.HandlingMissingValue(mData);

            (NDArray X, NDArray Y) = rawdata.TransformData(mData);

            return (X,Y);
        }

        public async Task<(NDArray X, NDArray Y)> GeneratePredictionData(int count)
        {
            Parser = new DataParser();
            Parser.ColumnSeparator = ';';
            Parser.RawDataName = "mlconfigs/iris/iris_raw.txt";

            Parser.Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species" };

            //unzip the file
            var rawdata = DFExtensions.FromDataParser(Parser);

            await Task.CompletedTask;
            var mData = rawdata.ParseMetadata("species");
            Metadata = mData;


            //fix missing values
            rawdata.HandlingMissingValue(mData);

            var data = rawdata.TakeRandom(count);


            (NDArray X, NDArray Y) = data.TransformData(mData);

            return (X, Y);
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

        public  List<ILayer>  CreateNet()
        {
            return new List<ILayer>()
            {
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 7 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLu", Activation=Activation.ReLU},
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 3 },
                new ActLayer(){Type= LayerType.Activation, Name="Softmax", Activation=Activation.Softmax},
            };
        }
       
    
    }
}
