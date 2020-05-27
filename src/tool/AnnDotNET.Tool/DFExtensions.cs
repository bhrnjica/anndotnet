using NumSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace AnnDotNET.Tool
{
    public static class DFExtensions
    {
       public static (NDArray X, NDArray Y)  PrepareData(this DataFrame df, string[] features, string label)
        {
            //var train_X = np.array(3.3f, 4.4f, 5.5f, 6.71f, 6.93f, 4.168f, 9.779f, 6.182f, 7.59f, 2.167f, 7.042f, 10.791f, 5.313f, 7.997f, 5.654f, 9.27f, 3.1f);
            //var train_Y = np.array(1.7f, 2.76f, 2.09f, 3.19f, 1.694f, 1.573f, 3.366f, 2.596f, 2.53f, 1.221f, 2.827f, 3.465f, 1.65f, 2.904f, 2.42f, 2.94f, 1.3f);
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
