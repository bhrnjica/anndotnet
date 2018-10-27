//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
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
using System.ComponentModel;

namespace ANNdotNET.Lib
{
    

    //Scaling of the numerical column
    public enum DataNormalization
    {
        [Description("None")]
        None,
        [Description("MinMax")]
        MinMax,
        [Description("Gauss")]
        Gauss,
        [Description("Custom")]
        Custom,
    }

}
