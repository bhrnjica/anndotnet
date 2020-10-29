//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using NumSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Daany;
using Microsoft.ML;
using Daany.Ext;


namespace Anndotnet.Core.Extensions
{
    public static class DFExtensions
    {
       public static (NDArray X, NDArray Y)  TransformData(this DataFrame df, List<ColumnInfo> metadata)
        {
            //extract features and label from DataFrame
            var feats = metadata.Where(x=>x.MLType == MLColumnType.Feature).ToList();
            var labelInfo = metadata.Where(x => x.MLType == MLColumnType.Label).ToList();

            //transform feature
            var dfF = df[feats.Select(x => x.Name).ToArray()];
            var featureDf = prepareDf(dfF, feats);

            
            //transform label
            var lDf = df.Create((labelInfo.Select(x=>x.Name).FirstOrDefault(), null));
            var labelDf = prepareDf(lDf, labelInfo);
            
            //iterate through rows
            var x = featureDf.ToNDArray();
            var y = labelDf.ToNDArray();
            //
            return (x, y);
        }

        private static DataFrame prepareDf(DataFrame df, List<ColumnInfo> metadata)
        {
            var cols = df.Columns;

            //check id all columns have valid type
            if (df.ColTypes.Any(x => x == ColType.DT))
                throw new Exception("DataTime column cannot be directly prepare to ML. Consider to transform it to another type.");

            //string and categorical column should be transformed in to OneHot Encoding
            var finalColumns = new List<String>();
            var finalDf = df[df.Columns.ToArray()];

            for (int j = 0; j < df.ColCount(); j++)
            {
                if (metadata[j].Encoding != CategoryEncoding.None)
                {
                    (var edf, var cValues) = df.EncodeColumn(cols[j], metadata[j].Encoding, true);
                    finalDf = finalDf.Append(edf, verticaly: false);
                    //store encoded class values to metadata
                    metadata[j].ClassValues = cValues;

                    //add to column list
                    if (edf == null)
                        continue;

                    foreach (var c in edf.Columns)
                    {
                        finalColumns.Add(c);
                    }

                }
                else
                    finalColumns.Add(cols[j]);
            }

            return finalDf[finalColumns.ToArray()];
        }

        public static NDArray ToNDArray(this DataFrame df)
        {
            var shape = new Shape(df.RowCount(), df.ColCount());
            var lst = new List<float>();
            foreach(var r in df.GetRowEnumerator())
                lst.AddRange(r.Select(x=> Convert.ToSingle(x)).ToList());
            var arr = lst.ToArray();

            //
            var ndArray = new NDArray(arr);
            ndArray = ndArray.reshape(shape);
            return ndArray;
        }
    }
}
