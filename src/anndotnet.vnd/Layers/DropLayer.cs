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
using Anndotnet.Core;
using Anndotnet.Core.Interfaces;

namespace Anndotnet.Vnd.Layers
{
    public class DropLayer : ILayer
    {
        public int DropPercentage { get; set; }
        public string Name { get; set; }
        public LayerType Type { get; set; }
        public DropLayer() => Type = LayerType.Drop;
    }   
}