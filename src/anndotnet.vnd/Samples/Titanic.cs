using Anndotnet.Core;
using Anndotnet.Core.Extensions;
using Anndotnet.Core.Interfaces;
using Anndotnet.Vnd.Layers;
using Daany;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tensorflow.NumPy;

namespace Anndotnet.Vnd.Samples
{
    public  class TitanicSample
    {

        public List<ColumnInfo> Metadata { get; set; }
        public async Task<(NDArray X, NDArray Y)> GenerateData()
        {
 
            //prepare the data
            var columns = new string[] { "pclass", "survived", "name", "sex", "age", "sibsp", "parch", "ticket", "fare", "cabin", "embarked", "boat", "body", "home.dest" };
            var colTypes = new ColType[] { ColType.IN, ColType.IN, ColType.STR, ColType.IN, ColType.F32, ColType.F32, ColType.F32, ColType.STR, ColType.F32, ColType.STR, ColType.IN, ColType.STR, ColType.I32, ColType.IN };

            
            //load titanic data into Daany.DataFrame
            var rawdata = DataFrame.FromCsv("mlconfigs/titanic/titanic_full_raw.csv", ',',missingValues: new char[] { '?' }, colTypes: colTypes);
            await Task.CompletedTask;

            //remove unnesessary columns
            var data = rawdata["pclass", "sex", "age", "sibsp", "fare", "embarked","survived"];
            //parse the data and generate metadata.
            var mData = data.ParseMetadata("survived");

            //define handling missing value
            mData[0].MissingValue = Aggregation.Random;//pclass
            mData[1].MissingValue = Aggregation.Random;//sex
            mData[2].MissingValue = Aggregation.Avg;//age
            mData[2].Transformer.DataNormalization = ColumnTransformer.MinMax;
            mData[3].MissingValue = Aggregation.Avg;//sibsp
            mData[4].MissingValue = Aggregation.Min; //fare
            mData[5].MissingValue = Aggregation.Mode;//embarked
            mData[6].MissingValue = Aggregation.Random;//survived
            mData[6].Transformer.DataNormalization = ColumnTransformer.Binary1;

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
                    { Metrics.CAcc, Metrics.CErr },

                LossFunction = Metrics.BCE,
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
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 1 },
                new ActLayer(){Type= LayerType.Activation, Name="Sigmoid", Activation=Activation.Sigmoid},
            };
        }
       
    
    }
}
