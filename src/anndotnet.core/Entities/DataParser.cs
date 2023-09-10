////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

namespace Anndotnet.Core.Entities;

public class DataParser
{
    public string RowSeparator { get; set; } = "\r\n";
    public char ColumnSeparator { get; set; } = ';';
    public bool HasHeader { get; set; } = false;
    public int SkipLines { get; set; } = 0;
    public char[] MissingValueSymbol { get; set; }
    public char DescriptionSymbol { get; set; } = '!';
    public string RawDataName { get; set; }
    public string[] Header { get; set; }
    public string DateFormat { get; set; }
        
}