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
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Anndotnet.Shared.Entities;

public record DataParser
{
    public string    RowSeparator       { get; set; } = "\r\n";
    public char      ColumnSeparator    { get; set; } = ';';
    public bool      HasHeader          { get; set; } = false;
    public int       SkipLines          { get; set; } = 0;
    public char[]?   MissingValueSymbol { get; set; } = new char[]{' '};
    public char      DescriptionSymbol  { get; set; } = '!';
    public string?   FileName           { get; set; }
    public string[]? Header             { get; set; }
    public string?   DateFormat         { get; set; } = "yyyy/MM/dd";
    public char      DecimalSeparator   { get; set; } = '.';
}

public class SummaryInfo
{
    public int    Id { get; set; }
    public string? Name { get; set; }
    public string[]? Value { get; set; }


}
public class HeaderInfo 
{
    public int     Id              { get; set; }
    public string? Name            { get; set; }
    public string? MlType          { get; set; }
    public string? ValueColumnType { get; set; }

    public string? ValueFormat { get; set; }

    public string? MissingValue { get; set; }

    public DataTransformer? Transformer { get; set; } = new DataTransformer();

    [JsonIgnore]
    public List<string>? Data { get; set; }= new List<string>();
    [JsonIgnore]
    public string?        SummaryHeader { get; set; }
    [JsonIgnore]
    public List<string?>? SummaryData   { get; set; }
    [JsonIgnore]

    public bool          IsNotFrozen { get; set; } = true;
    [JsonIgnore]

    public List<string>? ColTypes         { get; set; } = Enum.GetNames(typeof(ColValueType)).ToList();
    [JsonIgnore]
    public List<string>? ColMlTypes       { get; set; } = Enum.GetNames(typeof(ColMlDataType)).ToList();
    [JsonIgnore]
    public List<string>? ColMissingValues { get; set; } = Enum.GetNames(typeof(ColMissingValue)).ToList();

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