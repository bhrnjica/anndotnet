using System;
using System.Collections.Generic;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core.Common;
using Xunit;

namespace anndotnet.unit
{
    public class NetworkConfiguration_Tests
    {
      //TO DO: unit Tests for emnedding and normalization layer 
        [Fact]
        public void networkConfiguration_test09()
        {

            MLFactory f = new MLFactory();

            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.Dense, Param1=5, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=1, FParam= Activation.Softmax, Id=2, Name="out1" },
            };
            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 1 }, DataType.Float, "out1");

            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();
            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(2, w.Count);//2 = 1 in hidden and 1 in out layer

            // total weights 1x5 + 4*5 = 25
            Assert.Equal(25, w.Sum(p => p.Shape.TotalSize));

            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(2, b.Count);//2 = 1 in hidden and 1 in out layer

            //1x5 + 1x1 = 6
            Assert.Equal(6, b.Sum(p => p.Shape.TotalSize));


            //last parameter is related to network output 
            var outputLayer = nnModel.Outputs.Where(p => p.Name.Equals(labelVar.Name)).ToList();
            Assert.Single(outputLayer);
            //dimension is 1 
            Assert.Equal(1, outputLayer.Sum(p => p.Shape.TotalSize));


            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Empty(constants);//9 constants are from peep
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(1, nnModel.Output.Shape.Dimensions[0]);

        }

        [Fact]
        public void networkConfiguration_test08()
        {
            MLFactory f = new MLFactory();
            //LSTM Network  in(4)-LSTM(5,5)-out(3), with peepholes and stabilization
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.LSTM, Param1=5, Param2=5, FParam= Activation.TanH, BParam2=true, BParam1=true, Id=1, Name="LSTM Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.None, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();

            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(5, w.Count);//4 in lstm and  1 in hidden layer

            // total weights 3x5 + 4*5x4
            Assert.Equal(95, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(5, b.Count);//4 in lstm and  1 in output
            //4*1*5 in lstm and  3 in output layer
            Assert.Equal(23, b.Sum(p => p.Shape.TotalSize));

            //4*5*4
            var u = nnparams.Where(p => p.Name.Equals("u")).ToList();
            Assert.Equal(4, u.Count);//4 in lstm 
            //4*5*5 in lstm 
            Assert.Equal(100, u.Sum(p => p.Shape.TotalSize));

            //peephole only in LSTM. 
            var peep = nnparams.Where(p => p.Name.Equals("pe")).ToList();
            //Peep connection in 3 gates ft, it and ot
            Assert.Equal(3, peep.Count);
            //3*5
            Assert.Equal(15, peep.Sum(p => p.Shape.TotalSize));

            //stabilization on all gates: ft, it and ot. when using peepholes 3 extra.
            var stab = nnparams.Where(p => p.Name.Equals("st")).ToList();
            //for peephole lstm count is 3+3
            Assert.Equal(6, stab.Count);
            //6x1
            Assert.Equal(6, stab.Sum(p => p.Shape.TotalSize));
            //constant: 6x3 +1x3
            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Equal(21, constants.Count);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }
        [Fact]
        public void networkConfiguration_test07()
        {
            MLFactory f = new MLFactory();
            //LSTM Network  in(4)-LSTM(5,5)-out(3), with stabilization
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.LSTM, Param1=5, Param2=5, FParam= Activation.TanH, BParam2=false, BParam1=true, Id=1, Name="LSTM Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.None, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();

            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(5, w.Count);//4 in lstm and  1 in hidden layer

            // total weights 3x5 + 4*5x4
            Assert.Equal(95, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(5, b.Count);//4 in lstm and  1 in output
            //4*1*5 in lstm and  3 in output layer
            Assert.Equal(23, b.Sum(p => p.Shape.TotalSize));

            //4*5*4
            var u = nnparams.Where(p => p.Name.Equals("u")).ToList();
            Assert.Equal(4, u.Count);//4 in lstm 
            //4*5*5 in lstm 
            Assert.Equal(100, u.Sum(p => p.Shape.TotalSize));

            //peephole only in LSTM. 
            var peep = nnparams.Where(p => p.Name.Equals("pe")).ToList();
            //Peep connection in 3 gates ft, it and ot
            Assert.Empty(peep);
            //3*5
            Assert.Equal(0, peep.Sum(p => p.Shape.TotalSize));

            //stabilization on all gates: ft, it and ot. when using peepholes 3 extra.
            var stab = nnparams.Where(p => p.Name.Equals("st")).ToList();
            //for non peephole lstm count is 3
            Assert.Equal(3,stab.Count);
            //1x3
            Assert.Equal(3, stab.Sum(p => p.Shape.TotalSize));

            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Equal(12, constants.Count);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }

        [Fact]
        public void networkConfiguration_test06()
        {
            MLFactory f = new MLFactory();
            //LSTM Network  in(4)-LSTM(5,5)-out(3) without peepholes and stabilization
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.LSTM, Param1=5, Param2=5, FParam= Activation.TanH, Id=1, Name="LSTM Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.None, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();

            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(5, w.Count);//4 in lstm and  1 in hidden layer

            // total weights 3x5 + 4*5x4
            Assert.Equal(95, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(5, b.Count);//4 in lstm and  1 in output
            //4*1*5 in lstm and  3 in output layer
            Assert.Equal(23, b.Sum(p => p.Shape.TotalSize));

            //4*5*4
            var u = nnparams.Where(p => p.Name.Equals("u")).ToList();
            Assert.Equal(4, u.Count);//4 in lstm 
            //4*5*5 in lstm 
            Assert.Equal(100, u.Sum(p => p.Shape.TotalSize));

            //peephole only in LSTM. 
            var peep = nnparams.Where(p => p.Name.Equals("peep")).ToList();
            //Peep connection in 3 gates ft, it and ot
            Assert.Empty(peep);
            //3*5
            Assert.Equal(0, peep.Sum(p => p.Shape.TotalSize));

            //stabilization on all gates: ft, it and ot. when using peepholes 3 extra.
            var stab = nnparams.Where(p => p.Name.Equals("stabilize")).ToList();
            //for non peephole lstm count is 3
            Assert.Empty(stab);
            //outDim*6
            Assert.Equal(0, stab.Sum(p => p.Shape.TotalSize));

            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Equal(3, constants.Count);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }

        [Fact]
        public void networkConfiguration_test05()
        {
            MLFactory f = new MLFactory();
            //Deep Neural Network in(4) - 5-10-15-out(3)
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.Dense, Param1=5, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=10, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=15, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.None, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);


            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();
            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(4, w.Count);//3. One for each hidden layer 
            // total weights 4x5 + 5x10 + 10x15 + 15x3 = 265
            Assert.Equal(265, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(4, b.Count);//4 (3 for hidden and 1 for output layer)
            // 5x1 + 4x1 + 15x1 + 10x1 +
            Assert.Equal(33, b.Sum(p => p.Shape.TotalSize));

           
            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Empty(constants);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }

        [Fact]
        public void networkConfiguration_test04()
        {
            MLFactory f = new MLFactory();
            //FeedForward in(4)-5-out(3)
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.Dense, Param1=5, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.None, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();
            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(2, w.Count);//1 for one hidden layer 
            // total weights 4x5 + 3x5
            Assert.Equal(35, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(2, b.Count);//2 (1 for hidden and 1 for output layer)
            //5x1 + 3x1
            Assert.Equal(8, b.Sum(p => p.Shape.TotalSize));

            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Empty(constants);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }

        [Fact]
        public void networkConfiguration_test02()
        {
            MLFactory f = new MLFactory();
            //FeedForward in(4)-5-out(3)
            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.Dense, Param1=5, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=3, FParam= Activation.Softmax, Id=2, Name="out1" },
            };

            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 3 }, DataType.Float, "out1");


            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();
            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(2, w.Count);//1 for one hidden layer 
            // total weights 4x5 + 3x5
            Assert.Equal(35, w.Sum(p => p.Shape.TotalSize));
            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(2, b.Count);//2 (1 for hidden and 1 for output layer)
            //5x1 + 3x1
            Assert.Equal(8, b.Sum(p => p.Shape.TotalSize));

            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Empty(constants);
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(3, nnModel.Output.Shape.Dimensions[0]);
        }

        [Fact]
        public void networkConfiguration_test01()
        {

            MLFactory f = new MLFactory();

            List<NNLayer> layers = new List<NNLayer>()
            {
                new NNLayer(){ Type= LayerType.Dense, Param1=5, FParam= Activation.TanH, Id=1, Name="Dense Layer" },
                new NNLayer(){ Type= LayerType.Dense, Param1=1, FParam= Activation.None, Id=2, Name="out1" },
            };
            //create input and output variable
            var device = DeviceDescriptor.UseDefaultDevice();
            Variable featureVar = Variable.InputVariable(new int[] { 4 }, DataType.Float, "in1");
            Variable labelVar = Variable.InputVariable(new int[] { 1 }, DataType.Float, "out1");

            var nnModel = MLFactory.CreateNetwrok(layers, new List<Variable>() { { featureVar } }, labelVar, device);

            //Structure of the network  parameters
            var nnparams = nnModel.Inputs.Where(p => p.Uid.StartsWith("Parameter")).ToList();
            //weights
            var w = nnparams.Where(p => p.Name.Equals("w")).ToList();
            Assert.Equal(2, w.Count);//2 = 1 in hidden and 1 in out layer

            // total weights 1x5 + 4*5 = 25
            Assert.Equal(25, w.Sum(p => p.Shape.TotalSize));

            //total biases
            var b = nnparams.Where(p => p.Name.Equals("b")).ToList();
            Assert.Equal(2, b.Count);//2 = 1 in hidden and 1 in out layer

            //1x5 + 1x1 = 6
            Assert.Equal(6, b.Sum(p => p.Shape.TotalSize));


            //last parameter is related to network output 
            var outputLayer = nnModel.Outputs.Where(p => p.Name.Equals(labelVar.Name)).ToList();
            Assert.Single(outputLayer);
            //dimension is 1 
            Assert.Equal(1, outputLayer.Sum(p => p.Shape.TotalSize));


            var constants = nnModel.Inputs.Where(p => p.Uid.StartsWith("Constant")).ToList();
            Assert.Empty(constants);//9 constants are from peep
            var variables = nnModel.Inputs.Where(p => p.Name.StartsWith("in1")).ToList();
            var outVars = nnModel.Outputs.ToList();

            //check first and last variable
            Assert.Equal("in1", nnModel.Arguments[0].Name);
            Assert.Equal("out1", nnModel.Outputs[0].Name);
            Assert.Equal(1, nnModel.Output.Shape.Dimensions[0]);

        }
    }
}
