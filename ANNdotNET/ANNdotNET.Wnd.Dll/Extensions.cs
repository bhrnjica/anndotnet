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
