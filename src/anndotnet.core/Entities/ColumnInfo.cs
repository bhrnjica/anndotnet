///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//            Copyright 2017-2021 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                           //
//             For feedback:https://github.com/bhrnjica/anndotnet/issues     //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

namespace AnnDotNet.Core.Entities;

public class ColumnInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MLColumnType MLType { get; set; }
    public Daany.ColType ValueColumnType { get; set; }

    public string ValueFormat { get; set; }

    //Missing Value Handler
    public Daany.Aggregation MissingValue { get; set; }

    //Information how the column can be transformed
    public DataTransformer Transformer { get; set; }

    public ColumnInfo()
    {
        Transformer = new DataTransformer();
    }
}

public class DataTransformer
{
    public Daany.ColumnTransformer DataNormalization { get; set; }
    public string[] ClassValues { get; set; }
    public float[] NormalizationValues { get; set; }

    public DataTransformer()
    {

    }
    public DataTransformer(Daany.ColumnTransformer colTransformer, string[] classValues, float[] normalizationValues)
    {
        DataNormalization = colTransformer;
        ClassValues = classValues;
        NormalizationValues = normalizationValues;
    }

}