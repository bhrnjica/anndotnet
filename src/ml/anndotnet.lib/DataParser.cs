//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////

namespace ANNdotNET.Lib
{
    /// <summary>
    /// Class implements parser for the raw  file
    /// </summary>
    public class DataParser
    {
        public string[] RowSeparator { get; set; }
        public char[] ColumnSeparator { get; set; }
        public bool FirstRowHeader { get; set; }
        public int SkipFirstLines { get; set; }

    }
}
