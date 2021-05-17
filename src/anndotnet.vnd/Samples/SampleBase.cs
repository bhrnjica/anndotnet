using Anndotnet.Core;
using Anndotnet.Vnd;
using Anndotnet.Vnd.Layers;
using Daany;
using Daany.Ext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Extensions;
using NumSharp;

namespace Anndotnet.Vnd.Samples
{
    public class SampleBase
    {
        public List<ColumnInfo> Metadata { get; set; }
        public DataParser Parser { get; set; }
    }
}
