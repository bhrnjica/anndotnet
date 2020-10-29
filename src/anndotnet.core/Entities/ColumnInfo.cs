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
   
    public class ColumnInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MLColumnType MLType { get; set; }
        public Daany.ColType ValueColumnType { get; set; }
        public string ValueFormat { get; set; }
        public Daany.Aggregation MissingValue { get; set; }
        public CategoryEncoding Encoding { get; set; }
        public string[] ClassValues { get; set; }

    }
}
