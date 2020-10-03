using NumSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Daany;
using Microsoft.ML;
using Daany.Ext;

namespace AnnDotNET.Common
{
    public static class DFExtensions
    {
       public static (NDArray X, NDArray Y)  PrepareData(this DataFrame df, string[] features, string label)
        {
             var featureDf = prepareDf(df[features]);
            //
            var lDf = df.Create((label, null));
            var labelDf = prepareDf(lDf);
            //iterate through rows
            var x = featureDf.ToNDArray();
            var y = labelDf.ToNDArray();
            
            return (x, y);
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


        private static DataFrame prepareDf(DataFrame dfF)
        {
            
            var cols = dfF.Columns;
            var mlCntx = new MLContext();

            //check id all columns have valid type
            if (dfF.ColTypes.Any(x => x == ColType.DT))
                throw new Exception("DataTime column cannot be directly prepare to ML. Consider to transform it to another type.");

            //string and categorical column should be transformed in to OneHot Encoding
            var finalColumns = new List<String>();
            var finalDf = dfF[dfF.Columns.ToArray()];
            for (int j = 0; j < dfF.ColCount(); j++)
            {
                if (dfF.ColTypes[j] == ColType.STR || dfF.ColTypes[j] == ColType.IN)
                {
                    var hot = dfF.EncodeColumn(mlCntx, cols[j], encodedOnly: true);
                    finalDf = finalDf.Append(hot, verticaly: false);
                    //add to column list
                    foreach (var c in hot.Columns)
                    {
                        finalColumns.Add(c);
                    }

                }
                else
                    finalColumns.Add(cols[j]);
            }

            return finalDf[finalColumns.ToArray()];
        }
    }
}
