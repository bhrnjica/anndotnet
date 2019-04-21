using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core;
using Xunit;
using NNetwork.Core.Metrics;
namespace anndotnet.unit
{

    public class WeightedSquaredErrorTest
    {
        static DeviceDescriptor device = DeviceDescriptor.CPUDevice;


        [Fact]
        private static void createWeightedSE()
        {
            NDShape shape = new int[] { 15 };
            var actual = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var predicted = new float[] { -5, -2, 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34, 37 };
            var weights = new NDArrayView(shape,new float[] {15 ,  14 , 13 , 12 , 11 , 10 , 9  , 8 ,  7 ,  6  , 5 ,  4  , 3  , 2 ,1}, device);
            var aValue = Value.CreateBatch(shape, actual, device, false);
            var pValue = Value.CreateBatch(shape, predicted, device, false);
            var pWeights = new Constant(weights); //Value.CreateBatch(shape, predicted, device, false);

            //define variables
            var vactual = Variable.InputVariable(shape, DataType.Float);
            var vpredicted = Variable.InputVariable(shape, DataType.Float);

            //calculate weighted squared error
            var loss = StatMetrics.WeightedSE(vpredicted, vactual, pWeights);

            //evaluate function
            var inMap = new Dictionary<Variable, Value>() { { vpredicted, pValue },{ vactual, aValue } };
            
            var outMap = new Dictionary<Variable, Value>() { { loss, null } };
            loss.Evaluate(inMap, outMap, device);

            //
            var result = outMap[loss].GetDenseData<float>(loss);

            Assert.Equal((double)result[0][0], (double)7680.0, 2);


        }

       


        
    }
}
