////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Daany;
using Daany.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using Anndotnet.Core.Entities;

using TorchSharp;
using static TorchSharp.torch;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.Extensions;

public static class DfExtensions
{

    public static (Tensor X, Tensor Y) TransformData(this DataFrame df, List<ColumnInfo> metadata)
    {
        //extract features and label from DataFrame
        var feats = metadata.Where(x => x.MLType == MLColumnType.Feature).ToList();
        var labelInfo = metadata.Where(x => x.MLType == MLColumnType.Label).ToList();

        //transform feature
        var dfF = df[feats.Select(x => x.Name).ToArray()];
        var featureDf = PrepareDf(dfF, feats);

        //transform label
        var lDf = df.Create((labelInfo.Select(x => x.Name).FirstOrDefault(), null));
        var labelDf = PrepareDf(lDf, labelInfo);

        //dataFrame to tensor
        var x = featureDf.ToTensor(metadata.Where(x=>x.MLType==MLColumnType.Feature).ToList(), true);
        var y = labelDf.ToTensor(metadata.Where(x => x.MLType == MLColumnType.Label).ToList(), false);

        return (x, y);
    }

    private static DataFrame PrepareDf(DataFrame df, List<ColumnInfo> metadata)
    {
        var cols = df.Columns;

        //check if all columns have valid type
        if (df.ColTypes.Any(x => x == ColType.DT))
        {
            throw new Exception("DataTime column cannot be directly prepare to ML. Consider to transform it to another type.");
        }

        //string and categorical column should be transformed in to OneHot Encoding
        var finalColumns = new List<String>();
        var finalDf = df[df.Columns.ToArray()];

        for (var j = 0; j < df.ColCount(); j++)
        {
            //categorical data encoding
            if (metadata[j].Transformer.DataNormalization == ColumnTransformer.Binary1 ||
                metadata[j].Transformer.DataNormalization == ColumnTransformer.Binary2 ||
                metadata[j].Transformer.DataNormalization == ColumnTransformer.Dummy ||
                metadata[j].Transformer.DataNormalization == ColumnTransformer.OneHot ||
                metadata[j].Transformer.DataNormalization == ColumnTransformer.Ordinal)
            {
                var (edf, vVal, eVal) = df.TransformColumn(cols[j], metadata[j].Transformer.DataNormalization, true);
                finalDf = finalDf.Append(edf, verticaly: false);

                //store encoded class values to metadata
                metadata[j].Transformer.ClassValues = eVal;

                //add to column list
                if (edf == null)
                    continue;

                foreach (var c in edf.Columns)
                {
                    finalColumns.Add(c);
                }

            }
            //Data normalization or scaling
            else if (metadata[j].Transformer.DataNormalization == ColumnTransformer.MinMax ||
                     metadata[j].Transformer.DataNormalization == ColumnTransformer.Standardizer)
            {
                var (ndf, nVal, sVal) = df.TransformColumn(cols[j], metadata[j].Transformer.DataNormalization, true);
                metadata[j].Transformer.NormalizationValues = nVal;

                finalColumns.Add(cols[j]);
            }
            else if (metadata[j].ValueColumnType == ColType.STR)
            {
                continue;
            }
            else
                finalColumns.Add(cols[j]);
        }

        return finalDf[finalColumns.ToArray()];
    }

    public static Tensor ToTensor(this DataFrame df, List<ColumnInfo> metadata, bool isFeatureTensor)
    {
        var (row, col) = (df.RowCount(), df.ColCount());

        //features 
        var lstValues = df.Values.Select(x => (x is int i) ? i: Convert.ToSingle(x)).ToList();

        //all columns in the DF must be same type, otherwise cannot be converted into tensor
        var tensorType =  df.ColTypes.First() == ColType.I32 ? ScalarType.Int32 : ScalarType.Float32;

        //Feature tensor must be float type
        if (isFeatureTensor)
        {
            tensorType = ScalarType.Float32;
        }

        //as_tensor doesnt create an new data it shares the data already created,
        var t=  torch.as_tensor(lstValues, tensorType);
        var tt = t.reshape(row, col);
        return tt;

        //return col == 1 ?
        //    //create 1D tensor                      //create multi-dim (2D) tensor
        //    torch.tensor(lstValues, tensorType) : torch.tensor(lstValues, row, col, tensorType);
    }

    public static List<ColumnInfo> ParseMetadata(this DataFrame df, string label)
    {
        var cols = new List<ColumnInfo>();

        for (int i = 0; i < df.ColCount(); i++)
        {
            var name = df.Columns[i];
            var type = df.ColTypes[i];

            var c = new ColumnInfo();
            if (name == label && type != ColType.STR)
            {
                c.MLType = MLColumnType.Label;
            }
            else if(name != label && type != ColType.STR )
            {
                c.MLType = MLColumnType.Feature;
            }
            else//string columns are excluded from parsing by default
            {
                c.MLType = MLColumnType.None;
            }

            //categorical column
            if (type == ColType.IN && c.MLType != MLColumnType.None)
            {
                c.ValueColumnType = ColType.IN;

                c.Transformer.ClassValues = df[name].Distinct().Select(x => x.ToString()).ToArray();
                
                if (c.MLType != MLColumnType.Label)
                {
                    c.Transformer.DataNormalization = ColumnTransformer.Ordinal;
                }
                else if (c.MLType == MLColumnType.Label && c.Transformer.ClassValues.Length == 2)//binary label
                {
                    c.Transformer.DataNormalization = ColumnTransformer.Binary1;
                }
                else if(c.MLType == MLColumnType.Label) //multi class label
                {
                    c.Transformer.DataNormalization = ColumnTransformer.Ordinal;
                }
                else
                {
                    new NotSupportedException("This code should neve be called.");
                }

            }
            else if (type == ColType.I2 && c.MLType != MLColumnType.None)
            {
                c.ValueColumnType = ColType.I2;

                if (c.MLType == MLColumnType.Label)
                {
                    c.Transformer.DataNormalization = ColumnTransformer.Binary1;
                }
                else
                {
                    c.Transformer.DataNormalization = ColumnTransformer.Ordinal;
                }

                c.Transformer.ClassValues = df[name].Distinct().Select(x => x.ToString()).ToArray();
            }
            else if (c.MLType != MLColumnType.None)
            {
                c.ValueColumnType = ColType.F32;

                c.Transformer.DataNormalization = ColumnTransformer.Standardizer;

                var col = df[name].Select(x => Convert.ToSingle(x)).ToList();

                c.Transformer.NormalizationValues = new float[]
                                                    {
                                                        Metrics.Mean<float, float>( col ),
                                                        Metrics.Stdev<float, float>( col )
                                                    };
            }



            //
            c.Id = i;
            c.MissingValue = Aggregation.None;
            c.Name = name;

            cols.Add(c);
        }

        return cols;
    }

    public static DataFrame HandlingMissingValue(this DataFrame df, List<ColumnInfo> metadata)
    {
        foreach (var m in metadata)
        {
            df.FillNA(m.Name, m.MissingValue);
        }
        return df;
    }

    public static DataFrame FromDataParser(DataParser parser)
    {
        return DataFrame.FromCsv(filePath: parser.RawDataName, sep: parser.ColumnSeparator, names: parser.Header,
            dformat: parser.DateFormat, missingValues: parser.MissingValueSymbol,colTypes:parser.ColTypes, skipLines: parser.SkipLines);
    }
}