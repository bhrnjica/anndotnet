using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anndotnet.Core.Util
{
    public static class LongEnumerable
    {
        public static IEnumerable<long> Range(long start, long count)
        {
            for (long i = 0; i < count; i++)
            {
                yield return start + i;
            }
        }
    }

}
