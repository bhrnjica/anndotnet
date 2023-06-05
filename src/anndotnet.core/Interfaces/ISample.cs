using Anndotnet.Core;
using System.Collections.Generic;

namespace Anndotnet.Core.Interface
{
    public interface ISample
    {
        public List<ColumnInfo> Metadata { get; set; }
        public DataParser Parser { get; set; }
    }
}
