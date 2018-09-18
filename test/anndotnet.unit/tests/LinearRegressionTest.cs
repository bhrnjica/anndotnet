using CNTK;
using NNetwork.Core.network;
using NNetwork.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace anndotnet.unit
{

    public class LinearRegressionTest: BaseClass
    {
       
        [Fact]
        public void LinearRegression_Test()
        {
            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 1 }, DataType.Float, "input");
            Variable y = Variable.InputVariable(new int[] { 1 }, DataType.Float, "output");

            //define training data set from table above
            var xValues = Value.CreateBatch<float>(new NDShape(1, 1), new float[] { 1f, 2f, 3f, 4f, 5f }, device);
            var yValues = Value.CreateBatch<float>(new NDShape(1, 1), new float[] { 3f, 5f, 7f, 9f, 11f }, device);

            //create linear regression model, LRM
            NetworkFoundation net = new NetworkFoundation();
            var model = (Function)net.Layer(x, 1, DataType.Float, device);

            //LRM contains only two parameters b0 and b1
            var ps = model.Inputs.Where(z => z.IsParameter).ToList();
            var totalParameters = ps.Sum(c => c.Shape.TotalSize);

            //test parameters count
            Assert.Equal(2, totalParameters);

            //create trainer
            var trainer = createTrainer(model, y, 0.05);

            //training
            for (int i = 1; i <= 100; i++)
            {
                var d = new Dictionary<Variable, Value>();
                d.Add(x, xValues);
                d.Add(y, yValues);

                //
                trainer.TrainMinibatch(d, true, device);
                //
                var loss = trainer.PreviousMinibatchLossAverage();
                var eval = trainer.PreviousMinibatchLossAverage();
                //
                Console.WriteLine($"Loss={loss}, Eval={eval}");

                //print weights
                var b0_name = ps[0].Name;
                var b0 = new Value(ps[0].GetValue()).GetDenseData<float>(ps[0]);
                var b1_name = ps[1].Name;
                var b1 = new Value(ps[1].GetValue()).GetDenseData<float>(ps[1]);

                Console.WriteLine($"b0={b0[0][0]}, b1={b1[0][0]}");

            }


        }

        [Fact]
        public void LinearMultipleRegression_Test()
        {
            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 3 }, DataType.Float, "input");
            Variable y = Variable.InputVariable(new int[] { 1 }, DataType.Float, "output");

            //data
            var xData = new float[] { 1f,2f,3f,/*first row*/
                                      2f,4f,6f,/*second row*/
                                      3f,6f,9f,/*third row*/
                                      4f,8f,11f,/*fourth row*/
                                      5f,10f,14f/*fifth row*/ };
            var xValues = Value.CreateBatch<float>(new NDShape(1, 3), xData, device);
            var yValues = Value.CreateBatch<float>(new NDShape(1, 1), new float[] { 21f, 41f, 61f, 77f, 98f }, device);

            //create linear regression model, LRM
            NetworkFoundation net = new NetworkFoundation();
            var model = (Function)net.Layer(x, 1/**/, DataType.Float, device);

            //LRM contains only two parameters b0 and b1
            var ps = model.Inputs.Where(z => z.IsParameter).ToList();
            var totalParameters = ps.Sum(c => c.Shape.TotalSize);

            //test parameters count
            Assert.Equal(4, totalParameters);

            //create trainer
            var trainer = createTrainer(model, y, 0.005);

            //training
            for (int i = 1; i <= 200; i++)
            {
                var d = new Dictionary<Variable, Value>();
                d.Add(x, xValues);
                d.Add(y, yValues);

                //
                trainer.TrainMinibatch(d, true, device);
                //
                var loss = trainer.PreviousMinibatchLossAverage();
                var eval = trainer.PreviousMinibatchLossAverage();
                //
                Console.WriteLine($"Loss={loss}, Eval={eval}");

                //print weights
                var b0_name = ps[0].Name;
                var b0 = new Value(ps[0].GetValue()).GetDenseData<float>(ps[0]);
                var b1_name = ps[1].Name;
                var b1 = new Value(ps[1].GetValue()).GetDenseData<float>(ps[1]);
                if(i%10==0)
                Console.WriteLine($"b0={b0[0][0]}, b1={b1[0][0]}, b2={b1[0][1]}, b3={b1[0][2]}");

            }
        }

        [Fact]
        public void LinearRegression_regression_parameters_Test01()
        {
            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 1 }, DataType.Float, "input");
            Variable y = Variable.InputVariable(new int[] { 1 }, DataType.Float, "output");

            //data
            var xValues = Value.CreateBatch<float>(new NDShape(1, 1), new float[] { 1f, 2f, 3f, 4f, 5f }, device);
            var yValues = Value.CreateBatch<float>(new NDShape(1, 1), new float[] { 3f, 5f, 7f, 9f, 11f }, device);

            //create linear regression model, LRM
            NetworkFoundation net = new NetworkFoundation();
            var model = (Function)net.Layer(x, 1, DataType.Float, device);

            //test model by manually assign weights parameters value
            var ps = model.Inputs.Where(z => z.IsParameter).ToList();
            var totalParameters = ps.Sum(c => c.Shape.TotalSize);

            //set bias parameter to 1
            ps[0].GetValue().SetValue(1);

            //set weight parameter to 2
            ps[1].GetValue().SetValue(2);


            //print weights
            var b0_name = ps[0].Name;
            var b0 = new Value(ps[0].GetValue()).GetDenseData<float>(ps[0]);
            var b1_name = ps[1].Name;
            var b1 = new Value(ps[1].GetValue()).GetDenseData<float>(ps[1]);

            //check regression parameters
            Assert.Equal(1f, b0[0][0]);
            Assert.Equal(2f, b1[0][0]);
            Console.WriteLine($"b0={b0[0][0]}, b1={b1[0][0]}");

            //Evaluate model after weights are setup
            var inV = new Dictionary<Variable, Value>();
            inV.Add(x, xValues);
            var outV = new Dictionary<Variable, Value>();
            outV.Add(model, null);

            //evaluate
            model.Evaluate(inV, outV, device);

            //collect result
            var result = outV[model].GetDenseData<float>(model);

            //check results
            Assert.Equal(3f, result[0][0]);
            Assert.Equal(5f, result[1][0]);
            Assert.Equal(7f, result[2][0]);
            Assert.Equal(9f, result[3][0]);
            Assert.Equal(11f, result[4][0]);


        }

    }
}
