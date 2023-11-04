////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Anndotnet.Shared.Entities;

public record DataParser
{
    public string    RowSeparator       { get; set; } = "\r\n";
    public char      ColumnSeparator    { get; set; } = ';';
    public bool      HasHeader          { get; set; } = false;
    public int       SkipLines          { get; set; } = 0;
    public char[]?   MissingValueSymbol { get; set; }
    public char      DescriptionSymbol  { get; set; } = '!';
    public string?   DataPath        { get; set; }
    public string[]? Header             { get; set; }
    public string?   DateFormat         { get; set; }
}
public class HeaderInfo
{
    public int           Id              { get; set; }
    public string?        Name            { get; set; }
    public string? MlType          { get; set; }
    public string ValueColumnType { get; set; }

    public string? ValueFormat { get; set; }

    public ColMissingValue? MissingValue { get; set; }

    public DataTransformer? Transformer { get; set; }

    public List<string>? Data { get; set; }

}
public class DataTransformer
{
    public Daany.ColumnTransformer? DataNormalization   { get; set; }
    public string[]?                ClassValues         { get; set; }
    public float[]?                  NormalizationValues { get; set; }

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