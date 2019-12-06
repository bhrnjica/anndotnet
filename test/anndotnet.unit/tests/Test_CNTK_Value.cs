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

            Action testCode = () => { var lst = value.Shape.Dimensions.ToString(); };
            var ex = Record.Exception(testCode);
            
           Assert.True(ex.Message.StartsWith("This Value object is invalid and can no longer be accessed."));

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
