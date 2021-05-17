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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Daany.Ext;

namespace Anndotnet.Core
{
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
}
