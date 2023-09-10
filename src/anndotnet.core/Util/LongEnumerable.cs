////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

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

        public static Type GetdotnetType(this Tensor tensor)
        {
            switch (tensor.dtype)
            {
                case ScalarType.Byte:
                  //  break;
                case ScalarType.Int8:
                  //  break;
                case ScalarType.Int16:
                  //  break;
                case ScalarType.Int32:
                    return typeof(int);
                case ScalarType.Int64:
                    return typeof(long);
                case ScalarType.Float16:
                    
                case ScalarType.Float32:
                    return typeof(float);
                case ScalarType.Float64:
                    return typeof(double);
                case ScalarType.ComplexFloat32:
                    break;
                case ScalarType.ComplexFloat64:
                    break;
                case ScalarType.Bool:
                    break;
                case ScalarType.BFloat16:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new ArgumentOutOfRangeException();

        }
    }

}
