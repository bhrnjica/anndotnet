using NNetwork.Core.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;
using NNetwork.Core;
using NNetwork.Core.Common;
using NNetwork.Core.Network;
using NNetwork.Core.Network.Modules;
using Xunit;
using static NNetwork.Core.network.LSTMClassifier;

namespace anndotnet.unit
{

    public class LSTMUnitTests:BaseClass
    {
      
        float[][] mData = new float[][] {
                new float[] { 5.1f, 3.5f, 1.4f, 0.2f},
                new float[] { 4.9f, 3.0f, 1.4f, 0.2f},
                new float[] { 4.7f, 3.2f, 1.3f, 0.2f},
                new float[] { 4.6f, 3.1f, 1.5f, 0.2f},
                new float[] { 6.9f, 3.1f, 4.9f, 1.5f},
            };
        float[][] constants = new float[][]
        {
            //four constant
            //c1     c2   c3    c4
             new float[] {10f, 10f, 10f, 10f },
        };

        [Fact]
        public void lstm_test01()
        {
            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 4 }, DataType.Float, "input");
            var xValues = Value.CreateBatchOfSequences<float>(new int[] { 4 }, mData, device);

            //
            var lstm00 = RNN.RecurrenceLSTM(x, 3, 3, DataType.Float, device,false, Activation.TanH, true, true, 1);

            //           
            LSTMReccurentNN lstmNN = new LSTMReccurentNN(1, 1, device);
            //lstm implme reference 01
            var lstmCell = lstmNN.CreateLSTM(x, "output1");
            var lstm01 = CNTKLib.SequenceLast(lstmCell.h);

            //lstme implementation refe 02
            var lstm02 = LSTMSequenceClassifier.LSTMNet(x, 1, device, "output1");

            //
            var wParams00 = lstm00.Inputs.Where(p => p.Uid.Contains("Parameter")).ToList();
            var wParams01 = lstm00.Inputs.Where(p => p.Uid.Contains("Parameter")).ToList();
            var wParams02 = lstm00.Inputs.Where(p => p.Uid.Contains("Parameter")).ToList();

            //parameter count
            Assert.Equal(wParams00.Count,wParams01.Count);
            Assert.Equal(wParams00.Count, wParams02.Count);

            //structure of parameters test
            Assert.Equal(wParams00.Where(p => p.Name.Contains("b")).Count(), wParams01.Where(p => p.Name.Contains("b")).Count());
            Assert.Equal(wParams00.Where(p => p.Name.Contains("w")).Count(), wParams01.Where(p => p.Name.Contains("w")).Count());
            Assert.Equal(wParams00.Where(p => p.Name.Contains("u")).Count(), wParams01.Where(p => p.Name.Contains("u")).Count());
            Assert.Equal(wParams00.Where(p => p.Name.Contains("pe")).Count(), wParams01.Where(p => p.Name.Contains("pe")).Count());
            Assert.Equal(wParams00.Where(p => p.Name.Contains("st")).Count(), wParams01.Where(p => p.Name.Contains("st")).Count());


            //check structure of parameters with originaly developed lstm
            //chech for arguments
            Assert.True(lstm01.Arguments.Count == lstm02.Arguments.Count);
            for (int i = 0; i < lstm01.Arguments.Count; i++)
            {
                testVariable(lstm01.Arguments[i], lstm01.Arguments[i]);

            }

            ///
            Assert.True(lstm01.Inputs.Count == lstm02.Inputs.Count);
            for (int i = 0; i < lstm01.Inputs.Count; i++)
            {
                testVariable(lstm01.Inputs[i], lstm02.Inputs[i]);

            }

            ///
            Assert.True(lstm01.Outputs.Count == lstm02.Outputs.Count);
            for (int i = 0; i < lstm01.Outputs.Count; i++)
            {
                testVariable(lstm01.Outputs[i], lstm02.Outputs[i]);

            }
        }

        private void testVariable(Variable var1, Variable var2)
        {
            Assert.True(var1.DataType == var2.DataType);
            Assert.True(var1.DynamicAxes.Count() == var2.DynamicAxes.Count());
            Assert.True(var1.IsConstant == var2.IsConstant);
            Assert.True(var1.IsInput == var2.IsInput);
            Assert.True(var1.IsOutput == var2.IsOutput);
            Assert.True(var1.IsParameter == var2.IsParameter);
            Assert.True(var1.IsPlaceholder == var2.IsPlaceholder);
            Assert.True(var1.IsSparse == var2.IsSparse);
            Assert.True(var1.Kind == var2.Kind);
            Assert.True(var1.NeedsGradient == var2.NeedsGradient);
            Assert.True(var1.Shape.Dimensions.Count == var2.Shape.Dimensions.Count);
            for (int j = 0; j < var1.Shape.Dimensions.Count; j++)
                Assert.True(var1.Shape.Dimensions[j] == var2.Shape.Dimensions[j]);
            Assert.True(var1.Shape.HasInferredDimension == var2.Shape.HasInferredDimension);
            Assert.True(var1.Shape.HasUnboundDimension == var2.Shape.HasUnboundDimension);
            Assert.True(var1.Shape.TotalSize == var2.Shape.TotalSize);

            //Assert.True(var1.Name == var2.Name);

            
            //Assert.True(var1.Uid == var2.Uid);
        }
        float[][] xdata = new float[][] {
                new float[] { 1f, 3f},
                new float[] { 2f, 5f},
                new float[] { 3f, 7f},
                new float[] { 4f, 9f},
                new float[] { 5f, 11f},
            };

        [Fact]
        public void LSTM_Test_Params_Count()
        {

            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 2 }, DataType.Float, "input");
            Variable y = Variable.InputVariable(new int[] { 3 }, DataType.Float, "output");

            //Number of LSTM parameters
            var lstm1 = RNN.RecurrenceLSTM(x, 3, 3, DataType.Float, device, false, Activation.TanH, false, false, 1);

            var ft = lstm1.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
            var consts = lstm1.Inputs.Where(l => l.Uid.StartsWith("Constant")).ToList();
            var inp = lstm1.Inputs.Where(l => l.Uid.StartsWith("Input")).ToList();

            //bias params
            var bs = ft.Where(p => p.Name.Contains("b")).ToList();
            var totalBs = bs.Sum(v => v.Shape.TotalSize);
            Assert.Equal(12, totalBs);
            //weights
            var ws = ft.Where(p => p.Name.Contains("w")).ToList();
            var totalWs = ws.Sum(v => v.Shape.TotalSize);
            Assert.Equal(24, totalWs);
            //update
            var us = ft.Where(p => p.Name.Contains("u")).ToList();
            var totalUs = us.Sum(v => v.Shape.TotalSize);
            Assert.Equal(36, totalUs);
            
            //total number opf parameters
            var totalOnly = totalBs + totalWs + totalUs;
            Assert.Equal(72, totalOnly);
        }


        [Fact]
        public void LSTM_Test_Params_Count_with_peep_selfstabilize()
        {

            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 2 }, DataType.Float, "input");
            Variable y = Variable.InputVariable(new int[] { 3 }, DataType.Float, "output");

            #region lstm org implemented in cntk for reference
            //lstme implementation refe 02
            var lstmTest02 = LSTMSequenceClassifier.LSTMNet(x, 3, device, "output1");
            var ft2 = lstmTest02.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
            var totalSize = ft2.Sum(p=>p.Shape.TotalSize);
            //bias params
            var bs2 = ft2.Where(p => p.Name.Contains("_b")).ToList();
            var totalBs2 = bs2.Sum(v => v.Shape.TotalSize);

            //weights
            var ws2 = ft2.Where(p => p.Name.Contains("_w")).ToList();
            var totalWs2 = ws2.Sum(v => v.Shape.TotalSize);

            //update
            var us2 = ft2.Where(p => p.Name.Contains("_u")).ToList();
            var totalUs2 = us2.Sum(v => v.Shape.TotalSize);
        
            //peephole
            var ph2 = ft2.Where(p => p.Name.Contains("_peep")).ToList();
            var totalph2 = ph2.Sum(v => v.Shape.TotalSize);

            //stabilize
            var st2 = ft2.Where(p => p.Name.Contains("_stabilize")).ToList();
            var totalst2 = st2.Sum(v => v.Shape.TotalSize);
            #endregion

            #region anndotnet old implementation
            //           
            //LSTMReccurentNN lstmNN = new LSTMReccurentNN(3, 3, device);
            ////lstm implme reference 01
            //var lstmCell11 = lstmNN.CreateLSTM(x, "output1");
            //var lstmTest01 = CNTKLib.SequenceLast(lstmCell11.h);
            //var ft1 = lstmTest01.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
            //var consts1 = lstmTest01.Inputs.Where(l => l.Uid.StartsWith("Constant")).ToList();
            //var inp1 = lstmTest01.Inputs.Where(l => l.Uid.StartsWith("Input")).ToList();
            //var pparams1 = ft1.Sum(v => v.Shape.TotalSize);
            #endregion
            
            //Number of LSTM parameters
            var lstm1 = RNN.RecurrenceLSTM(x,3,3, DataType.Float,device, false, Activation.TanH,true,true,1);

            var ft = lstm1.Inputs.Where(l=>l.Uid.StartsWith("Parameter")).ToList();
            var consts = lstm1.Inputs.Where(l => l.Uid.StartsWith("Constant")).ToList();
            var inp = lstm1.Inputs.Where(l => l.Uid.StartsWith("Input")).ToList();

            //bias params
            var bs = ft.Where(p=>p.Name.Contains("b")).ToList();
            var totalBs = bs.Sum(v => v.Shape.TotalSize);
            Assert.Equal(12, totalBs);
            //weights
            var ws = ft.Where(p => p.Name.Contains("w")).ToList();
            var totalWs = ws.Sum(v => v.Shape.TotalSize);
            Assert.Equal(24, totalWs);
            //update
            var us = ft.Where(p => p.Name.Contains("u")).ToList();
            var totalUs = us.Sum(v => v.Shape.TotalSize);
            Assert.Equal(36, totalUs);
            //peephole
            var ph = ft.Where(p => p.Name.Contains("pe")).ToList();
            var totalPh = ph.Sum(v => v.Shape.TotalSize);
            Assert.Equal(9, totalPh);
            //stabilize
            var st = ft.Where(p => p.Name.Contains("st")).ToList();
            var totalst = st.Sum(v => v.Shape.TotalSize);
            Assert.Equal(6, totalst);

            var totalOnly = totalBs + totalWs + totalUs;
            var totalWithSTabilize = totalOnly + totalst;
            var totalWithPeep = totalOnly + totalPh;

            var totalP = totalOnly + totalst + totalPh;
            var totalParams = ft.Sum(v=>v.Shape.TotalSize);
            Assert.Equal(totalP,totalParams);
            //72- without peep and stab
            //75 - witout peep with stabil +3xm = 
            //81 - with peephole and without stabil
            //87 - with peep ans stab 3+9

        }

        [Fact]
        public void LSTM_Test_AGate()
        {

            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 2 }, DataType.Float, "input");
            Variable ht_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevOutput");
            Variable ct_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevCellState");
            // Variable ht = Variable.InputVariable(new int[] { 3 }, DataType.Float, "output");

            //data 01
            var x1Values = Value.CreateBatch<float>(new int[] { 2 }, new float[] { 1f, 2f }, device);
            var ct_1Values = Value.CreateBatch<float>(new int[] { 3 }, new float[] { 0f, 0f, 0f }, device);
            var ht_1Values = Value.CreateBatch<float>(new int[] { 3 }, new float[] { 0f, 0f, 0f }, device);

            //data 02
            var x2Values = Value.CreateBatch<float>(new NDShape(1, 2), new float[] { 3f, 4f }, device);


            //
            uint seed = 1;
            

            //evaluate 
            //Evaluate model after weights are setup
            var inV = new Dictionary<Variable, Value>();
            inV.Add(x, x1Values);
            inV.Add(ht_1, ht_1Values);
            inV.Add(ct_1, ct_1Values);

            //evaluate forgetgate
            var lstmCell = new LSTM();
            var fGate = (Function)lstmCell.AGate(x, ht_1, ct_1, DataType.Float, false, false, device, ref seed, "ForgetGate");

            //setup weights
            var ftparam = fGate.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
            var pa11 = new Parameter(ftparam[0]);
            pa11.SetValue(new NDArrayView(pa11.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
            var ws00 = new Parameter(ftparam[1]);
            //column based order
            (ws00).SetValue(new NDArrayView(ws00.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
            var us22 = new Parameter(ftparam[2]);
            (us22).SetValue(new NDArrayView(us22.Shape, new float[] { 0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));

            var outFt = new Dictionary<Variable, Value>();
            outFt.Add(fGate, null);
            fGate.Evaluate(inV, outFt, device);

            var resulft = outFt[fGate].GetDenseData<float>(fGate);
            Assert.Equal(0.5523079f, resulft[0][0]);//
            Assert.Equal(0.5695462f, resulft[0][1]);//
            Assert.Equal(0.5866176f, resulft[0][2]);//
        }

        [Fact]
    public void LSTM_Test_WeightsValues()
    {

        //define values, and variables
        Variable x = Variable.InputVariable(new int[] { 2 }, DataType.Float, "input");
        Variable y = Variable.InputVariable(new int[] { 3 }, DataType.Float, "output");

        //data 01
        var x1Values = Value.CreateBatch<float>(new NDShape(1, 2), new float[] { 1f, 2f }, device);
        var ct_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0f, 0f, 0f }, device);
        var ht_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0f, 0f, 0f }, device);

        var y1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0.0629f, 0.0878f, 0.1143f }, device);

        //data 02
        var x2Values = Value.CreateBatch<float>(new NDShape(1, 2), new float[] { 3f, 4f }, device);
        var y2Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0.1282f, 0.2066f, 0.2883f }, device);

        //Create LSTM Cell with predefined previous output and prev cell state
        Variable ht_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevOutput");
        Variable ct_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevCellState");
        var lstmCell = new LSTM(x, ht_1, ct_1, DataType.Float, Activation.TanH, false, false, 1, device);
            

        var ft = lstmCell.H.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
        var pCount = ft.Sum(p => p.Shape.TotalSize);
        var consts = lstmCell.H.Inputs.Where(l => l.Uid.StartsWith("Constant")).ToList();
        var inp = lstmCell.H.Inputs.Where(l => l.Uid.StartsWith("Input")).ToList();

        //bias params
        var bs = ft.Where(p => p.Name.Contains("b")).ToList();
        var pa = new Parameter(bs[0]);
        pa.SetValue(new NDArrayView(pa.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
        var pa1 = new Parameter(bs[1]);
        pa1.SetValue(new NDArrayView(pa1.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
        var pa2 = new Parameter(bs[2]);
        pa2.SetValue(new NDArrayView(pa2.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
        var pa3 = new Parameter(bs[3]);
        pa3.SetValue(new NDArrayView(pa3.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
            
        //set value to weights parameters
        var ws = ft.Where(p => p.Name.Contains("w")).ToList();
        var ws0 = new Parameter(ws[0]);
        var ws1 = new Parameter(ws[1]);
        var ws2 = new Parameter(ws[2]);
        var ws3 = new Parameter(ws[3]);
        (ws0).SetValue(new NDArrayView(ws0.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
        (ws1).SetValue(new NDArrayView(ws1.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
        (ws2).SetValue(new NDArrayView(ws2.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
        (ws3).SetValue(new NDArrayView(ws3.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
            
        //set value to update parameters
        var us = ft.Where(p => p.Name.Contains("u")).ToList();
        var us0 = new Parameter(us[0]);
        var us1 = new Parameter(us[1]);
        var us2 = new Parameter(us[2]);
        var us3 = new Parameter(us[3]);
        (us0).SetValue(new NDArrayView(us0.Shape, new float[] {  0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
        (us1).SetValue(new NDArrayView(us1.Shape, new float[] {  0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
        (us2).SetValue(new NDArrayView(us2.Shape, new float[] {  0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
        (us3).SetValue(new NDArrayView(us3.Shape, new float[] {  0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));

        //evaluate 
        //Evaluate model after weights are setup
        var inV = new Dictionary<Variable, Value>();
        inV.Add(x, x1Values);
        inV.Add(ht_1, ht_1Values);
        inV.Add(ct_1, ct_1Values);

        //evaluate output when previous values are zero
        var outV11 = new Dictionary<Variable, Value>();
        outV11.Add(lstmCell.H, null);
        lstmCell.H.Evaluate(inV, outV11, device);
            
        //test  result values
        var result = outV11[lstmCell.H].GetDenseData<float>(lstmCell.H);
        Assert.Equal(0.06286035f, result[0][0]);//
        Assert.Equal(0.0878196657f, result[0][1]);//
        Assert.Equal(0.114274316f, result[0][2]);//

        //evaluate cell state
        var outV = new Dictionary<Variable, Value>();
        outV.Add(lstmCell.C, null);
        lstmCell.C.Evaluate(inV, outV, device);

        var resultc = outV[lstmCell.C].GetDenseData<float>(lstmCell.C);
        Assert.Equal(0.114309236f, resultc[0][0]);//
        Assert.Equal(0.15543206f, resultc[0][1]);//
        Assert.Equal(0.197323829f, resultc[0][2]);//

        //evaluate second value, with previous values as previous state
        //setup previous state and output
        ct_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { resultc[0][0], resultc[0][1], resultc[0][2] }, device);
        ht_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { result[0][0], result[0][1], result[0][2] }, device);

        //Prepare for the evaluation
        inV = new Dictionary<Variable, Value>();
        inV.Add(x, x2Values);
        inV.Add(ht_1, ht_1Values);
        inV.Add(ct_1, ct_1Values);

        outV11 = new Dictionary<Variable, Value>();
        outV11.Add(lstmCell.H, null);
        lstmCell.H.Evaluate(inV, outV11, device);

        //test  result values
        result = outV11[lstmCell.H].GetDenseData<float>(lstmCell.H);
        Assert.Equal(0.128203362f, result[0][0]);//
        Assert.Equal(0.206633776f, result[0][1]);//
        Assert.Equal(0.288335562f, result[0][2]);//

        //evaluate cell state
        outV = new Dictionary<Variable, Value>();
        outV.Add(lstmCell.C, null);
        lstmCell.C.Evaluate(inV, outV, device);

        //evaluate cell state with previous value
        resultc = outV[lstmCell.C].GetDenseData<float>(lstmCell.C);
        Assert.Equal(0.227831185f, resultc[0][0]);//
        Assert.Equal(0.3523231f, resultc[0][1]);//
        Assert.Equal(0.4789199f, resultc[0][2]);//
    }

        [Fact]
        public void LSTM_Test_Output_CellState_Result()
        {

            //define values, and variables
            Variable x = Variable.InputVariable(new int[] { 2 }, DataType.Float, "input");
            Variable ht_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevOutput");
            Variable ct_1 = Variable.InputVariable(new int[] { 3 }, DataType.Float, "prevCellState");
           // Variable ht = Variable.InputVariable(new int[] { 3 }, DataType.Float, "output");

            //data 01
            var x1Values = Value.CreateBatch<float>(new NDShape(1, 2), new float[] { 1f, 2f }, device);
            var ct_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0f, 0f, 0f }, device);
            var ht_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { 0f, 0f, 0f }, device);

            
            //
            uint seed = 1;
            var lstmCell = new LSTM();
            var ct = lstmCell.CellState(x,ht_1, ct_1, DataType.Float, Activation.TanH, false, false,device,ref seed);
            var ht = lstmCell.CellOutput(x, ht_1, ct, DataType.Float, device, false, false, Activation.TanH, ref seed);


            var ft = ht.Inputs.Where(l => l.Uid.StartsWith("Parameter")).ToList();
            var pCount = ft.Sum(p => p.Shape.TotalSize);
            var consts = ht.Inputs.Where(l => l.Uid.StartsWith("Constant")).ToList();
            var inp = ht.Inputs.Where(l => l.Uid.StartsWith("Input")).ToList();

            //bias params
            var bs = ft.Where(p => p.Name.Contains("b")).ToList();
            var pa = new Parameter(bs[0]);
            pa.SetValue(new NDArrayView(pa.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
            var pa1 = new Parameter(bs[1]);
            pa1.SetValue(new NDArrayView(pa1.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
            var pa2 = new Parameter(bs[2]);
            pa2.SetValue(new NDArrayView(pa2.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));
            var pa3 = new Parameter(bs[3]);
            pa3.SetValue(new NDArrayView(pa3.Shape, new float[] { 0.16f, 0.17f, 0.18f }, device));

            //set value to weights parameters
            var ws = ft.Where(p => p.Name.Contains("w")).ToList();
            var ws0 = new Parameter(ws[0]);
            var ws1 = new Parameter(ws[1]);
            var ws2 = new Parameter(ws[2]);
            var ws3 = new Parameter(ws[3]);
            (ws0).SetValue(new NDArrayView(ws0.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
            (ws1).SetValue(new NDArrayView(ws1.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
            (ws2).SetValue(new NDArrayView(ws2.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));
            (ws3).SetValue(new NDArrayView(ws3.Shape, new float[] { 0.01f, 0.03f, 0.05f, 0.02f, 0.04f, 0.06f }, device));

            //set value to update parameters
            var us = ft.Where(p => p.Name.Contains("u")).ToList();
            var us0 = new Parameter(us[0]);
            var us1 = new Parameter(us[1]);
            var us2 = new Parameter(us[2]);
            var us3 = new Parameter(us[3]);
            (us0).SetValue(new NDArrayView(us0.Shape, new float[] { 0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
            (us1).SetValue(new NDArrayView(us1.Shape, new float[] { 0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
            (us2).SetValue(new NDArrayView(us2.Shape, new float[] { 0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));
            (us3).SetValue(new NDArrayView(us3.Shape, new float[] { 0.07f, 0.10f, 0.13f, 0.08f, 0.11f, 0.14f, 0.09f, 0.12f, 0.15f }, device));

            //evaluate 
            //Evaluate model after weights are setup
            var inV = new Dictionary<Variable, Value>();
            inV.Add(x, x1Values);
            inV.Add(ht_1, ht_1Values);
            inV.Add(ct_1, ct_1Values);

            //evaluate output when previous values are zero
            var outV11 = new Dictionary<Variable, Value>();
            outV11.Add(ht, null);
            ht.Evaluate(inV, outV11, device);

            //test  result values
            var result = outV11[ht].GetDenseData<float>(ht);
            Assert.Equal(0.06286035f, result[0][0]);//
            Assert.Equal(0.0878196657f, result[0][1]);//
            Assert.Equal(0.114274316f, result[0][2]);//

            //evaluate cell state
            var outV = new Dictionary<Variable, Value>();
            outV.Add(ct, null);
            ct.Evaluate(inV, outV, device);

            var resultc = outV[ct].GetDenseData<float>(ct);
            Assert.Equal(0.114309236f, resultc[0][0]);//
            Assert.Equal(0.15543206f, resultc[0][1]);//
            Assert.Equal(0.197323829f, resultc[0][2]);//

            //evaluate second value, with previous values as previous state
            //setup previous state and output
            //data 02
            var x2Values = Value.CreateBatch<float>(new NDShape(1, 2), new float[] { 3f, 4f }, device);
            ct_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { resultc[0][0], resultc[0][1], resultc[0][2] }, device);
            ht_1Values = Value.CreateBatch<float>(new NDShape(1, 3), new float[] { result[0][0], result[0][1], result[0][2] }, device);

            inV = new Dictionary<Variable, Value>();
            inV.Add(x, x2Values);
            inV.Add(ht_1, ht_1Values);
            inV.Add(ct_1, ct_1Values);

            //evaluate output when previous values are zero
            outV11 = new Dictionary<Variable, Value>();
            outV11.Add(ht, null);
            ht.Evaluate(inV, outV11, device);

            //test  result values
            result = outV11[ht].GetDenseData<float>(ht);
            Assert.Equal(0.128203362f, result[0][0]);//
            Assert.Equal(0.206633776f, result[0][1]);//
            Assert.Equal(0.288335562f, result[0][2]);//

            //evaluate cell state
            outV = new Dictionary<Variable, Value>();
            outV.Add(ct, null);
            ct.Evaluate(inV, outV, device);

            resultc = outV[ct].GetDenseData<float>(ct);
            Assert.Equal(0.227831185f, resultc[0][0]);//
            Assert.Equal(0.3523231f, resultc[0][1]);//
            Assert.Equal(0.4789199f, resultc[0][2]);//


        }
    }
}
