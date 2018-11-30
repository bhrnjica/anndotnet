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

    public class NDArrayViewAndValueTests
    {
        static DeviceDescriptor device = DeviceDescriptor.CPUDevice;
        [Fact]
        private static void createValueObject()
        {
            //helpers
            NDShape shape = new int[] { 4 };
            var array = new float[] { 10f, 11f, 12f, 13f };
            var array1 = new float[] { 3f, 20f, 0f, 78f };
            var nd = new NDArrayView(shape, array, device);
            var nd1 = new NDArrayView(shape, array1, device);


            //create value by using list of NDArrays
            var val = Value.Create(shape, new List<NDArrayView>() { nd, nd1 }, device, false);
            //1. create variable which must has the same shape as underlying data 
            var v1 = Variable.InputVariable(shape, DataType.Float);

            //get data from Value object
            var dd = val.GetDenseData<float>(v1);
            Assert.Equal(10f, dd[0][0], 1);
            Assert.Equal(11f, dd[0][1], 1);
            Assert.Equal(12f, dd[0][2], 1);
            Assert.Equal(13f, dd[0][3], 1);
            Assert.Equal(3f, dd[1][0], 1);
            Assert.Equal(20f, dd[1][1], 1);
            Assert.Equal(0f, dd[1][2], 1);
            Assert.Equal(78f, dd[1][3], 1);
            Console.WriteLine("create Value by list of NDArrayView");
            print(dd,v1.Shape);


            //2. create batch using array list
            val = Value.CreateBatch<float>(shape, array, device);
            dd = val.GetDenseData<float>(v1);
            Assert.Equal(10f, dd[0][0], 1);
            Assert.Equal(11f, dd[0][1], 1);
            Assert.Equal(12f, dd[0][2], 1);
            Assert.Equal(13f, dd[0][3], 1);
            Console.WriteLine("Value.CreateBatch by array");
            print(dd,v1.Shape);

            //3. create sequence
            //get list of data
            var seqList = new List<float>() { 1, 2, 3, 4, 5, 6, 10, 20, 30, 40, 50, 60 };
            //
            var shape1 = new NDShape(1, 4);//sequenceLength=12, so any shape multiple by 12 can apply(1,2,3,4,6,12)
            var vs1 = Variable.InputVariable(shape1, DataType.Float);
            //The number of elements in sequence data must be a multiple of the size of the sample shape in the variable
            val = Value.CreateSequence<float>(shape1, seqList, device);

            //get data from the sequence
            dd = val.GetDenseData<float>(vs1);
            Assert.Equal(1f, dd[0][0], 1);
            Assert.Equal(10f, dd[0][6], 1);
            Assert.Equal(60f, dd[0][11], 1);
            Console.WriteLine("Value.CreateBatchOfSequences<float>(shape, 2dArray, device)");
            print(dd, vs1.Shape);



            //4. create batch of sequence
            //create 2d list
            var a2dArray = new List<List<float>>() {{ new List<float>() { 1, 2, 3, 4, 5, 6 } },
                                                    { new List<float>() { 7, 8, 9 } },
                                                    { new List<float>() { 10, 11 } } };
            //shape can be any umber which is multiplication to number of element in each sequence.
            //so the number 1 can pass for any number of sequences 
            var shyape1 = new NDShape(1, 1);//only 1 can pass, since we have three sequences with 2,3, and 6 element which is common divisor is 1
            v1 = Variable.InputVariable(shyape1, DataType.Float);


            //batch of sequence 
            val = Value.CreateBatchOfSequences<float>(shyape1, a2dArray, device);


            //
            dd = val.GetDenseData<float>(v1);
            Assert.Equal(1f, dd[0][0], 1);
            Assert.Equal(9f, dd[1][2], 1);
            Assert.Equal(11f, dd[2][1], 1);
            Console.WriteLine("Value.CreateBatchOfSequences<float>(shyape, 2dArray, device)");
            //print as single sequence
            print(dd, v1.Shape);
            //result
            //1
            //2
            //3
            //4
            //5
            //6
            //7
            //8
            //9
            //10
            //11
            //12

            //print as set of sequence of 4 and less element
            //each sequence is divided in 4 element and printed
            print(dd, new int[] { 4, 1 });
            //result
            //1,2,3,4
            //5,6
            //7,8,9,10
            //11,12
        }

        [Fact]
        void createNDArrayViewObjects()
        {
            NDShape shape = new int[] { 4 };
            var v1 = Variable.InputVariable(shape, DataType.Float);
            //create NDArray by setting the values manually
            var dna1 = new NDArrayView(DataType.Float, shape, device);
            dna1.SetValue(1);
            dna1.SetValue(2);
            dna1.SetValue(3);
            dna1.SetValue(4);
            dna1.SetValue(5);

            //create NDArray 
            var value = new Value(dna1);
            var dd = value.GetDenseData<float>(v1);
            Assert.Equal(5f, dd[0][0], 1);
            Assert.Equal(5f, dd[0][1], 1);
            Assert.Equal(5f, dd[0][2], 1);
            Assert.Equal(5f, dd[0][3], 1);
            Console.WriteLine("NDArrayView SetValue");
            print(dd, v1.Shape);



            //2
            var array = new float[] { 10f, 11f, 12f, 13f };
            var dna2 = new NDArrayView(shape, array, device);
            value = new Value(dna2);
            dd = value.GetDenseData<float>(v1);
            Assert.Equal(10f, dd[0][0], 1);
            Assert.Equal(11f, dd[0][1], 1);
            Assert.Equal(12f, dd[0][2], 1);
            Assert.Equal(13f, dd[0][3], 1);
            Console.WriteLine("NDArrayView add array of value");
            print(dd, v1.Shape);

            //3
            var dna3 = new NDArrayView(shape, array, 5/* must be greater or equal then array length*/, device);
            value = new Value(dna3);
            dd = value.GetDenseData<float>(v1);
            Assert.Equal(10f, dd[0][0], 1);
            Assert.Equal(11f, dd[0][1], 1);
            Assert.Equal(12f, dd[0][2], 1);
            Assert.Equal(13f, dd[0][3], 1);
            Console.WriteLine("NDArrayView add array and number of elements");
            print(dd, v1.Shape);

            //4
            var axes = new AxisVector() { { Axis.DefaultBatchAxis() }, { Axis.DefaultDynamicAxis() } };
            NDShape shape2 = new int[] { 2, 2 };
            var v2 = Variable.InputVariable(shape2, DataType.Float, "", axes);
            var dna4 = new NDArrayView(shape, array, device);
            var dna5 = dna4.AsShape(shape2);
            value = new Value(dna5);
            Assert.Equal(10f, dd[0][0], 1);
            Assert.Equal(11f, dd[0][1], 1);
            Assert.Equal(12f, dd[0][2], 1);
            Assert.Equal(13f, dd[0][3], 1);
            dd = value.GetDenseData<float>(v2);
            print(dd, v2.Shape);
        }

        private static void print(IList<IList<float>> array, NDShape shape)
        {
            foreach (var row in array)
            {
                var n = shape.Dimensions.First();
                for (int i = 0; i < row.Count; i += n)
                {
                    var cells = row.Skip(i).Take(n).ToList();
                    Console.Write(string.Join(",", cells));
                    Console.WriteLine();
                }
            }
        }

        [Fact]
        private static void calculateCircleArea()
        {
            //Define constants
            var pi = Constant.Scalar<float>(3.14f, device);
            var r = Constant.Scalar<float>(2f, device);

            //circle area
            var areaExpr = CNTKLib.ElementTimes(CNTKLib.ElementTimes(r, r), pi);

            //Evaluate expression in 3 steps
            //1. prepare input values as dictionary <variable, value>
            var inputMap = new Dictionary<Variable, Value>();
            //inputMap.Add(r, new Value(r.Value()));
            //inputMap.Add(pi, new Value(pi.Value()));

            //2. prepare output value as dictionary <variable, null>
            var outMap = new Dictionary<Variable, Value>();
            outMap.Add(areaExpr, null);

            //3. call evaluate and prepare result
            areaExpr.Evaluate(inputMap, outMap, device);
            var result = outMap.First().Value.GetDenseData<float>(areaExpr)[0][0];

            //print the result
            Assert.Equal(12.56f, result,2);
            //Console.WriteLine($"Circe area for r=2, is A={result}");

        }

        private static void printCNTKVersion()
        {
            var ver = System.Reflection.Assembly.GetAssembly(typeof(CNTK.Trainer)).FullName;
            Assert.Contains(ver,$"Cntk.Core.Managed-");
            Assert.Contains(ver, $"Version=");
            Assert.Contains(ver, $"Culture=neutral, PublicKeyToken=a82c1f3f67b62253");
        }

    }
}
