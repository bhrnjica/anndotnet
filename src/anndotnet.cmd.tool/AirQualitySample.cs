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

namespace Anndotnet.cmd.tool
{
    public static class AirQualitySample
    {
        public static async Task<(NDArray X, NDArray Y)> generateAirQualityData()
        {
            var names = new string[] { "Date","Time","Month","Hour","CO(GT)","PT08.S1(CO)","NMHC(GT)","C6H6(GT)","PT08.S2(NMHC)","NOx(GT)",
                                        "PT08.S3(NOx)","NO2(GT)","PT08.S4(NO2)","PT08.S5(O3)","T","RH","AH", "Quality"};
            //unzip the file
            var rawdata = DataFrame.FromCsv("mlconfigs/air_quality/airquality_rawdata.txt",';', names:names,dformat:"m/d/yyyy");

            //remove unnecessary columns
            var data = rawdata["Month", "Hour", "CO(GT)", "PT08.S1(CO)", "NMHC(GT)", "C6H6(GT)", "PT08.S2(NMHC)", "NOx(GT)",
                "PT08.S3(NOx)", "NO2(GT)", "PT08.S4(NO2)", "PT08.S5(O3)", "T", "RH", "AH", "Quality"];

            await Task.Delay(1);

            var mData = data.ParseMetadata("Quality");

            (NDArray X, NDArray Y) = data.TransformData(mData);

            return (X,Y);
        }
        public static (TrainingParameters tPArams, LearningParameters lParams) generateParameters()
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

        public static List<LayerBase>  CreateNet()
        {
            return new List<LayerBase>()
            {
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 15 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLuLayer", Activation=Activation.ReLU},
                new FCLayer(){Type= LayerType.Dense, Name="FCLAyer01", OutDim= 5 },
                new ActLayer(){Type= LayerType.Activation, Name="ReLuLayer", Activation=Activation.Softmax},
            };
        }
        private static List<ColumnInfo> loadAirQualityMetaData()
        {
            var metaData = new List<ColumnInfo>() { 

                    //col1:
                    new ColumnInfo()
                    {
                        Id = 1,
                        Name = "Date",
                        MLType = MLColumnType.None,
                        ValueColumnType = ColType.DT,
                        ValueFormat = "m/d/yyyy",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                    //col2:
                    new ColumnInfo()
                    {
                        Id = 2,
                        Name = "Time",
                        MLType = MLColumnType.None,
                        ValueColumnType = ColType.STR,
                        ValueFormat = "hh:mm:ss AM",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                    //col3
                    new ColumnInfo()
                    {
                        Id = 3,
                        Name = "month",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.IN,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.Ordinal,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                    //col4
                    new ColumnInfo()
                    {
                        Id = 4,
                        Name = "hour",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.IN,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.Ordinal,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                    //col5
                    new ColumnInfo()
                    {
                        Id = 5,
                        Name = "CO_GT",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                    //col6
                    new ColumnInfo()
                    {
                        Id = 6,
                        Name = "PT08_S1_CO",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col7
                    new ColumnInfo()
                    {
                        Id = 7,
                        Name = "Column07",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col8
                    new ColumnInfo()
                    {
                        Id = 8,
                        Name = "Column08",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col9
                    new ColumnInfo()
                    {
                        Id = 9,
                        Name = "Column09",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col10
                    new ColumnInfo()
                    {
                        Id = 10,
                        Name = "Column10",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col11
                    new ColumnInfo()
                    {
                        Id = 11,
                        Name = "Column11",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col12
                    new ColumnInfo()
                    {
                        Id = 12,
                        Name = "Column12",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col13
                    new ColumnInfo()
                    {
                        Id = 13,
                        Name = "Column13",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col14
                    new ColumnInfo()
                    {
                        Id = 14,
                        Name = "Column14",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col15
                    new ColumnInfo()
                    {
                        Id = 15,
                        Name = "Column15",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col16
                    new ColumnInfo()
                    {
                        Id = 16,
                        Name = "Column16",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    //col17
                    new ColumnInfo()
                    {
                        Id = 17,
                        Name = "Column17",
                        MLType = MLColumnType.Feature,
                        ValueColumnType = ColType.F32,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.None,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },
                    ////col18
                    //new ColumnInfo()
                    //{
                    //    Id = 18,
                    //    Name = "Column18",
                    //    MLType = MLColumnType.Feature,
                    //    ValueColumnType = ColType.F32,
                    //    ValueFormat = "",
                    //    Encoding = CategoryEncoding.None,
                    //    ClassValues = null,
                    //    MissingValue = Aggregation.None

                    //},
                    ////col19
                    //new ColumnInfo()
                    //{
                    //    Id = 19,
                    //    Name = "Column19",
                    //    MLType = MLColumnType.Feature,
                    //    ValueColumnType = ColType.F32,
                    //    ValueFormat = "",
                    //    Encoding = CategoryEncoding.None,
                    //    ClassValues = null,
                    //    MissingValue = Aggregation.None

                    //},
                    ////col20
                    //new ColumnInfo()
                    //{
                    //    Id = 20,
                    //    Name = "Column20",
                    //    MLType = MLColumnType.Feature,
                    //    ValueColumnType = ColType.F32,
                    //    ValueFormat = "",
                    //    Encoding = CategoryEncoding.None,
                    //    ClassValues = null,
                    //    MissingValue = Aggregation.None

                    //},
                    //col21
                    new ColumnInfo()
                    {
                        Id = 21,
                        Name = "Column21",
                        MLType = MLColumnType.Label,
                        ValueColumnType = ColType.IN,
                        ValueFormat = "",
                        Encoding = CategoryEncoding.OneHot,
                        ClassValues = null,
                        MissingValue = Aggregation.None

                    },

                };
            return metaData;
        }
    
    }
}
