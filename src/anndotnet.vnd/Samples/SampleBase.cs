using Anndotnet.Core;
using System.Collections.Generic;

namespace Anndotnet.Vnd.Samples
{
    public class SampleBase
    {
        public List<ColumnInfo> Metadata { get; set; }
        public DataParser Parser { get; set; }
    }
}
