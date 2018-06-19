//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ANNdotNet.Wnd 
{
    public static class Extensions
    {
        public static Image LoadImageFromName(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var pic = asm.GetManifestResourceStream(name);
            return Image.FromStream(pic);
        }

        public static Icon LoadIconFromName(string name)
        {
            
            Assembly asm = Assembly.GetExecutingAssembly();
            var names = asm.GetManifestResourceNames();
            var pic = asm.GetManifestResourceStream(name);
            if (pic == null)
                return null;
            else
                return new Icon(pic);
        }
    }
}
