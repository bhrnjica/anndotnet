using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace anndotnet.vnd.Extensions
{
    public static class Extensions
    {
        public static string GetPathInCurrentOS(this string path)
        {
            if (System.OperatingSystem.IsOSPlatform(OSPlatform.Windows.ToString()))
                return path.Replace("/", "\\");
            else
                return path.Replace("\\", "/");
        }
    }
}
