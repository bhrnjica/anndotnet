///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using AnnDotNet.Core;
using Daany;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Extensions;
using AnnDotNet.Core.Interfaces;
using Tensorflow.NumPy;
using XPlot.Plotly;
using AnnDotNet.Vnd.Extensions;
using AnnDotNet.Vnd.Layers;

namespace AnnDotNet.Vnd.Samples;

public  class IrisSample : ISample
{
    public List<ColumnInfo> Metadata { get; set; }
    public DataParser Parser { get; set; }

    public IrisSample()
    {
        Parser = new DataParser();
        Parser.ColumnSeparator = ';';
        Parser.RawDataName = "mlconfigs/iris/iris_raw.txt";

        Parser.Header = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width", "species" };
    }

    DataFrame loaddata()
    {
        //unzip the file
        var rawdata = DfExtensions.FromDataParser(Parser);
        var mData = rawdata.ParseMetadata("species");
        Metadata = mData;

        //fix missing values
        return rawdata.HandlingMissingValue(mData);
    }

    public async Task<(NDArray X, NDArray Y)> GenerateData()
    {
        await Task.CompletedTask;

        var rawdata = loaddata();   

        (NDArray X, NDArray Y) = rawdata.TransformData(metadata:Metadata);

        return (X,Y);
    }

    public async Task<(NDArray X, NDArray Y)> GeneratePredictionData(int count)
    {
        await Task.CompletedTask;

        var rawdata = loaddata();

        var data = rawdata.TakeRandom(count);
        
        (NDArray x, NDArray y) = data.TransformData(Metadata);

        return (x, y);
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