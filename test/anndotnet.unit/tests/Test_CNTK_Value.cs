using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core;
using Xunit;

namespace anndotnet.unit
{

    public class CNTK_Value_Object
    {
        static Value GetValue()
        {
            var a = new NDArrayView(DataType.Float, new int[] { 1, 2, 3 }, DeviceDescriptor.CPUDevice);
            return new Value(a);
        }

        static MinibatchData GetMinibatchData()
        {
            var mb = new MinibatchData();
            mb.data = GetValue();

            return mb;
        }
        //https://github.com/Microsoft/CNTK/issues/3439
        [Fact]
        public void ValueObject_test0()
        {
            
            DeviceDescriptor.TrySetDefaultDevice(DeviceDescriptor.CPUDevice);

            var value = GetMinibatchData().data;

            GC.Collect();
            Assert.True(value.IsValid);
           
            var ex = Assert.Throws<System.ApplicationException>(() =>
            {
                var lst = value.Shape.Dimensions.ToList();

            });

            var result = $"This Value object is invalid and can no longer be accessed." +
                    $"This usually happens when a temporary Value object returned by the CNTK library is not cloned and " +
                    $"accessed later after it has been erased by the library. The Value objects created inside and " +
                    $"returned by the library from APIs like Forward, Backward etc. are temporary and are only guaranteed " +
                    $"to be valid until the next Forward/Backward call. If you want to access the Values later, you must " +
                    $"explicitly clone them.";
                  
           //  Assert.True(result);

        }

        [Fact]
        public void ValueObject_test02()
        {

            DeviceDescriptor.TrySetDefaultDevice(DeviceDescriptor.CPUDevice);

            var value = GetMinibatchData().data.DeepClone();
            //            var value = GetValue();

            GC.Collect();
            Assert.True(value.IsValid);
            Assert.Equal(3, value.Shape.Dimensions.Last());
           
        }
    }
}
