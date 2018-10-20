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
using anndotnet.wnd.Mvvm;

namespace anndotnet.wnd.Models
{
    
    public class ExamplesModel : ObservableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

    }
}