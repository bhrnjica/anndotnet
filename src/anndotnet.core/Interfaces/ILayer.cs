using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anndotnet.Core.Interfaces
{
    public interface ILayer
    {
        string Name { get; set; }
        LayerType Type { get; set; }
    }
}
