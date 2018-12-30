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

    public class DataNormalizationTests
    {

        [Fact]
        public void gaussNormalization_test01()
        {
            DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();
            //create factory object
            MLFactory f = new MLFactory(); 
            //create config streams
            f.CreateIOVariables("feature 4 0", "flower 3 0", DataType.Float);
            var trData = MLFactory.CreateTrainingParameters("|Type: default |BatchSize: 130 |Epochs:5 |Normalization: 0 |SaveWhileTraining: 0 |RandomizeBatch: 0 |ProgressFrequency: 1");
        
            string trainingPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\iris_with_hot_vector.txt";
            string validationPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\iris_with_hot_vector_test.txt";
            //string trainingPath = "../../../../data/iris_with_hot_vector.txt";
            //string validationPath = "../../../../data/iris_with_hot_vector_test.txt";

            //string trainingNormalizedPathh = "../../../../data/iris_train_normalized.txt";
            string trainingNormalizedPathh = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\iris_train_normalized.txt";
            var strNormalizedLine = System.IO.File.ReadAllLines(trainingNormalizedPathh);

            string validationNormalizedPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\iris_valid_normalized.txt";
            //string validationNormalizedPath = "../../../../data/iris_valid_normalized.txt";
            var strValidNormalizedLine = System.IO.File.ReadAllLines(validationNormalizedPath);
            //
            List<Function> normalizedInputs = null;
            using (var mbs1 = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(),f.InputVariables, f.OutputVariables, trainingPath, validationPath, MinibatchSource.FullDataSweep, trData.RandomizeBatch,0))
            {
                normalizedInputs = mbs1.NormalizeInput(f.InputVariables, device);
            }

            //normalization test for train datatset
            using (var mbs1 = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(),f.InputVariables, f.OutputVariables, trainingPath, validationPath, MinibatchSource.FullDataSweep, trData.RandomizeBatch,0))
            { 
                var data = mbs1.GetNextMinibatch(130, device);

                //go through all functions and perform the calculation
                foreach( var fun in normalizedInputs)
                {
                    //
                    var input = new Dictionary<Variable, Value>() { { f.InputVariables.First(), data.First().Value.data } };

                    var output = new Dictionary<Variable, Value>() { { fun, null } };
                    //
                    fun.Evaluate(input, output, device);

                    var normalizedValues = output[fun].GetDenseData<float>(fun);
                    
                    for(int i=0; i<normalizedValues.Count; i++)
                    {
                        var currNorLine = strNormalizedLine[i].Split('\t').ToList();
                        for(int j=0; j<normalizedValues[0].Count(); j++)
                        {
                            var n1 = normalizedValues[i][j].ToString(CultureInfo.InvariantCulture).Substring(0,5);
                            var n2 = currNorLine[j].Substring(0,5);
                            Assert.Equal(n1, n2);
                        }
                    }
                }
            }

            using (var mbs1 = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(),f.InputVariables, f.OutputVariables,  trainingPath, validationPath, MinibatchSource.FullDataSweep, trData.RandomizeBatch,0))
            {
                var data = MinibatchSourceEx.GetFullBatch(mbs1.Type,mbs1.ValidationDataFile,mbs1.StreamConfigurations,device);

                //go through all functions and perform the calculation
                foreach (var fun in normalizedInputs)
                {

                    //
                    var input = new Dictionary<Variable, Value>() { { f.InputVariables.First(), data.First().Value.data } };

                    var output = new Dictionary<Variable, Value>() { { fun, null } };
                    //
                    fun.Evaluate(input, output, device);

                    var normalizedValues = output[fun].GetDenseData<float>(fun);

                    for (int i = 0; i < normalizedValues.Count; i++)
                    {
                        var currNorLine = strValidNormalizedLine[i].Split('\t').ToList();
                        for (int j = 0; j < normalizedValues[0].Count(); j++)
                        {
                            var n1 = normalizedValues[i][j].ToString(CultureInfo.InvariantCulture).Substring(0, 5);
                            var n2 = currNorLine[j].Substring(0, 5);
                            Assert.Equal(n1, n2);
                        }
                    }

                }


            }
        }

        [Fact]
        public void gaussNormalization_test02()
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

            DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();

            //define values, and variables
            var xValues = Value.CreateBatchOfSequences<float>(new int[] { 4 }, mData, device);

            var xConstants = Value.CreateBatchOfSequences<float>(new int[] { 4 }, constants, device);
            var inputVar = Variable.InputVariable(xValues.Shape, DataType.Float);

            //create simple function which multiple data by constant 
            var cnt = new Constant(xConstants.Data);
            var fn = CNTKLib.ElementTimes(cnt, inputVar);

            //evaluate function
            var inMap = new Dictionary<Variable, Value>() { { inputVar, xValues } };
            var inputVar2 = Variable.InputVariable(xValues.Shape, DataType.Float);
            var outMap = new Dictionary<Variable, Value>() { { fn, null} };
            fn.Evaluate(inMap, outMap, device);

            //
            var result = outMap[fn].GetDenseData<float>(fn);

            /*
            //Expected result
            //x1     x2   x3    x4
            51f, 35f, 14f, 2f,//row1
            49f, 30f, 14f, 2f,//row2
            47f, 32f, 13f, 2f,//row3
            46f, 31f, 15f, 2f,//row4
            69f, 31f, 49f, 15f,//row5

            //Actual result
            //x1     x2   x3    x4
            5.1f, 3.5f, 1.4f, 0.2f,//row1
            4.9f, 3.0f, 1.4f, 0.2f,//row2
            4.7f, 3.2f, 1.3f, 0.2f,//row3
            4.6f, 3.1f, 1.5f, 0.2f,//row4
            6.9f, 3.1f, 4.9f, 1.5f,//row5

            */


        }

        [Fact]
        public void NormalizationfeatureGroup_test03()
        {
            DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();
            //create factory object
            MLFactory f = new MLFactory();
            //create config streams
            f.CreateIOVariables("|Itemid 1 0 |Sales 4 0 |Color 1 0", "|Label 1 0", DataType.Float);
            var trData = MLFactory.CreateTrainingParameters("|Type: default |BatchSize: 130 |Epochs:5 |Normalization:Sales |SaveWhileTraining: 0 |RandomizeBatch: 0 |ProgressFrequency: 1");

            string trainingPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\cntk_dataset_for_normalization_test.txt";
            string trainingNormalizedPathh = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\cntk_dataset_for_normalization_test_result.txt";

            //string trainingPath = "../../../../data/cntk_dataset_for_normalization_test.txt";
            //string trainingNormalizedPathh = "../../../../data/cntk_dataset_for_normalization_test_result.txt";

            var strTrainData = System.IO.File.ReadAllLines(trainingNormalizedPathh);
            var normalizedResult = System.IO.File.ReadAllLines(trainingNormalizedPathh);
            var inputVars = MLFactory.NormalizeInputLayer(trData, f, trainingPath, trainingPath, device);

            //normalization test for train dataset
            using (var mbs1 = new MinibatchSourceEx(trData.Type, f.StreamConfigurations.ToArray(),f.InputVariables, f.OutputVariables, trainingPath, trainingPath, MinibatchSource.FullDataSweep, trData.RandomizeBatch,0))
            {
                var data = mbs1.GetNextMinibatch(10, device);

                //go through all functions and perform the calculation
                for (int i=0; i < inputVars.Count; i++)
                {
                    //
                    var fun = (Function)inputVars[i];
                    var strName = data.Keys.Where(x=>x.m_name.Equals(f.InputVariables[i].Name)).FirstOrDefault();
                    var input = new Dictionary<Variable, Value>() { { f.InputVariables[i], data[strName].data } };

                    var output = new Dictionary<Variable, Value>() { { fun, null } };
                    //
                    fun.Evaluate(input, output, device);
                    var inputValues = data[strName].data.GetDenseData<float>(fun).Select(x=>x[0]).ToList();
                    var normalizedValues = output[fun].GetDenseData<float>(fun).Select(x => x[0]).ToList();
                    int index = 0;
                    if (i < 2)
                        index = i;
                    else
                        index = i + 3;
                    var currNorLine = normalizedResult[index].Split(new char[] { '\t', ' ' }).ToList();

                    for (int j = 0; j < normalizedValues.Count; j++)
                    {
                       
                        var n1 = normalizedValues[j].ToString(CultureInfo.InvariantCulture);
                        var n2 = currNorLine[j];
                        if(n1.Length < 2)
                            Assert.Equal(n1, n2);
                        else
                            Assert.Equal(n1.Substring(0,5), n2.Substring(0,5));
                    }
                }
            }

        }   
    }
}
