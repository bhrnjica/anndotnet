using Anndotnet.Core;
using Anndotnet.Core.Extensions;
using Anndotnet.Core.Interfaces;
using Anndotnet.Vnd.Layers;
using Daany;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tensorflow.NumPy;

namespace Anndotnet.Vnd.Samples
{
    public  class SlampTestSample
    {

        public List<ColumnInfo> Metadata { get; set; }
        public async Task<(NDArray X, NDArray Y)> GenerateData()
        {

            var coltypes = new ColType[] { ColType.I32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32, ColType.F32 };
            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("mlconfigs/slumptest/slump.txt", sep: ';', colTypes: coltypes);

            //prepare the data
            var features = new string[] { "Cement", "Slag", "Fly_ash", "Water", "SP", "Coarse_Aggr" };
            var label = "Strength";

            var cols = features.Union(new string[] { label }).ToArray();
            var data = df[cols];
            await Task.CompletedTask;

            //parse the data and generate metadata.
            var mData = data.ParseMetadata(label);

            //define handling missing value
            mData[0].MissingValue = Aggregation.None;//pclass
            mData[1].MissingValue = Aggregation.None;//sex
            mData[2].MissingValue = Aggregation.None;//age
            mData[3].MissingValue = Aggregation.None;//sibsp
            mData[4].MissingValue = Aggregation.None; //fare
            mData[5].MissingValue = Aggregation.None;//embarked
            mData[6].MissingValue = Aggregation.None;//survived

            //normalization data
            mData[0].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[1].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[2].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[3].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[4].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[5].Transformer.DataNormalization = ColumnTransformer.MinMax;

            //fix missing values
            data.HandlingMissingValue(mData);

            (NDArray X, NDArray Y) = data.TransformData(mData);

            Metadata = mData;
            return (X,Y);
        }
        public  (TrainingParameters tPArams, LearningParameters lParams) GenerateParameters()
        {
            var lParams = new LearningParameters()

            {
                EvaluationFunctions = new List<Metrics>()
                    { Metrics.RMSE, Metrics.MSE },

                LossFunction = Metrics.SE,
                LearnerType = LearnerType.Adam,
                LearningRate = 0.01f
            };

            var tParams = new TrainingParameters() { Retrain= false};

            return (tParams, lParams);
        }

        public  List<ILayer>  CreateNet()
        {
            return new List<ILayer>()
            {
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 7 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLu", Activation=Activation.ReLU},
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer02", OutDim= 1 },
            };
        }
       
    
    }
}
