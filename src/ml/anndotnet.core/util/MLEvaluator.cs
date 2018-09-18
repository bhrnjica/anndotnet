//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using CNTK;
using NNetwork.Core.Common;
using NNetwork.Core.Metrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Implement model evaluation for various purpose in the application
    /// </summary>
    public class MLEvaluator
    {
        #region Public Members
        /// <summary>
        /// Evaluate the model against dataset sored in the dataset file, and extract the result
        /// </summary>
        /// <param name="mlF"> ml factory object contains members needed to evaluation process</param>
        /// <param name="mbs"> Minibatch source which provides helpers members needed for for evaluation</param>
        /// <param name="strDataSetPath"> file of dataset</param>
        /// <param name="modelPath"> models which will be evaluate</param>
        /// <param name="resultExportPath"> result file in which the result will be exported</param>
        /// <param name="device"> device for computation</param>
        public static (Dictionary<string, List<List<float>>> featuresDict, Dictionary<string, List<List<float>>> actualDict, Dictionary<string, List<List<float>>> predictedDict)
            EvaluateModel(MLFactory mlF, MinibatchType type, string strDataSetPath, string modelPath, bool includeFeatures, bool includePrediction, bool includeHotVector, DeviceDescriptor device)
        {
            try
            {
                //load model
                var model = Function.Load(modelPath, device);

                //get data for evaluation by calling GetFullBatch
                var minibatchData = MinibatchSourceEx.GetFullBatch(type, strDataSetPath, mlF.StreamConfigurations.ToArray(), device);

                //input map creation for model evaluation
                var inputMap = new Dictionary<Variable, Value>();
                foreach (var v in minibatchData)
                {
                    var vv = model.Arguments.Where(x => x.Name == v.Key.m_name).FirstOrDefault();
                    var streamInfo = v.Key;
                    if (vv != null)
                        inputMap.Add(vv, minibatchData[streamInfo].data);

                }
                //actual map Creation
                //input map creation for model evaluation
                var actualMap = new Dictionary<Variable, Value>();
                foreach (var v in minibatchData)
                {
                    var vv = model.Outputs.Where(x => x.Name == v.Key.m_name).FirstOrDefault();
                    var streamInfo = v.Key;
                    if (vv != null)
                        actualMap.Add(vv, minibatchData[streamInfo].data);

                }

                //predicted  map 
                var predictedDataMap = new Dictionary<Variable, Value>();

                //when exporting data to excel we not need prediction since the model is exported directly 
                // and excel will call evaluation as regular excel formula
                if (includePrediction)
                {
                    foreach (var outp in model.Outputs)
                    {
                        predictedDataMap.Add(outp, null);
                    }
                    //model evaluation
                    model.Evaluate(inputMap, predictedDataMap, device);
                }

                //features collection
                var featDic = new Dictionary<string, List<List<float>>>();

                //labels collection
                var actualDic = new Dictionary<string, List<List<float>>>();
                var predictDic = new Dictionary<string, List<List<float>>>();

                //extract features
                //features are extracted as they go in CNTK minibatch trainer
                //binary and categorical variables are presented as One-Hot Vectors
                if (includeFeatures)
                {
                    foreach (var arg in model.Arguments)
                    {
                        var features = new List<List<float>>();
                        //label stream
                        var featureStream = minibatchData.Keys.Where(x => x.m_name == arg.Name).First();

                        //features
                        var fv = MLValue.GetValues(arg, minibatchData[featureStream].data);
                        featDic.Add(arg.Name, fv);
                    }
                }

                //extract output values
                //Categorical and binary variables are encoded to its numerical representation of classes
                // so in case BInary one hot (0.23,0,77) =1, (0,77,0.23) =0,
                //categorical (0.1,0.3,0.6)=2
                foreach (var output in model.Outputs)
                {
                    var actual = new List<List<float>>();
                    var actual_hotvector = new List<List<float>>();
                    //predicted values
                    var aec = actualMap[output].GetDenseData<float>(output);
                    foreach (var row in aec)
                    {
                        actual.Add(new List<float>() { MLValue.GetResult(row) });

                    }
                    actualDic.Add($"{output.Name}_actual", actual);
                    //adda actual output as hot vector
                    //label stream
                    var featureStream = minibatchData.Keys.Where(x => x.m_name == output.Name).First();
                    if (featureStream != null && includeHotVector)
                    {
                        var av = MLValue.GetValues(output, minibatchData[featureStream].data);
                        actualDic.Add($"{output.Name}_actual_hotvector", av);
                    }

                    var predicted = new List<List<float>>();
                    if (includePrediction)
                    {
                        //predicted values
                        var vec = predictedDataMap[output].GetDenseData<float>(output);
                        foreach (var row in vec)
                        {
                            predicted.Add(new List<float>() { MLValue.GetResult(row) });
                        }
                        predictDic.Add($"{output.Name}_predicted", predicted);
                        //
                        if (featureStream != null && includeHotVector)
                        {
                            var fv = MLValue.GetValues(output, predictedDataMap[output]);
                            predictDic.Add($"{output.Name}_predicted_hotvector", fv);
                        }
                    }

                }

                return (featDic, actualDic, predictDic);
            }
            catch (Exception)
            {

                throw;
            }

            //export result
            // ExportResult(actual, predict, resultExportPath);
        }


        /// <summary>
        /// Test cntk model stored at 'modelPath' against vector of values
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="vector"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static object TestModel(string modelPath, float[] vector, DeviceDescriptor device)
        {
            try
            {
                //
                FileInfo fi = new FileInfo(modelPath);
                if (!fi.Exists)
                {
                    throw new Exception($"The '{fi.FullName}' does not exist. Make sure the model is places at this location.");
                }

                //load the model from disk
                var model = Function.Load(fi.FullName, device);

                //input map creation for model evaluation
                var inputMap = new Dictionary<Variable, Value>();
                int columnSpan = 0;
                foreach (var var in model.Arguments)
                {
                    var dim = var.Shape.Dimensions.Last();
                    Value values = Value.CreateBatch<float>(var.Shape, vector.Skip(columnSpan).Take(dim), device);
                    inputMap.Add(var, values);

                    columnSpan += dim;
                }

                //output map 
                var predictedDataMap = new Dictionary<Variable, Value>();
                foreach (var outp in model.Outputs)
                {
                    predictedDataMap.Add(outp, null);
                }

                //evaluate the model
                model.Evaluate(inputMap, predictedDataMap, device);

                //extract the result  as one hot vector
                var outputData = predictedDataMap[model.Output].GetDenseData<float>(model.Output);

                //extract result
                return MLValue.GetResult(outputData.First());
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Private Functions

        public static (IEnumerable<float> actual, IEnumerable<float> predicted) EvaluateFunction(Function fun, EvalParameters evParam, DeviceDescriptor device)
        {
            try
            {
                //declare return vars
                List<float> actualLst = new List<float>();
                List<float> predictedLst = new List<float>();

                while (true)
                {
                    Value actual = null;
                    Value predicted = null;
                    //get one minibatch of data for training
                    var mbData = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, device);
                    var arguments = MinibatchSourceEx.ToMinibatchValueData(mbData, evParam.Input.Union(evParam.Ouptut).ToList());
                    var inMap = new Dictionary<Variable, Value>();
                    //
                    for (int i = 0; i < arguments.Count; i++)
                    {
                        var d = arguments.ElementAt(i);
                        //skip output data 
                        if (!evParam.Ouptut.Any(x => x.Name.Equals(d.Key.Name)))
                            inMap.Add(d.Key, d.Value);
                    }

                    //map variables and data
                    var predictedDataMap = new Dictionary<Variable, Value>() { { fun, null } };

                    //evaluates model
                    fun.Evaluate(inMap, predictedDataMap, device);

                    //actual data if t is available
                    var actualStream = mbData.Keys.Where(x => x.m_name.Equals(fun.Output.Name)).FirstOrDefault();
                    var actualVar = arguments.Keys.Where(x => x.Name.Equals(fun.Output.Name)).FirstOrDefault();
                    if (actualStream != null)
                    {
                        actual = mbData[actualStream].data;
                        var act = actual.GetDenseData<float>(actualVar).Select(l => MLValue.GetResult(l));
                        actual.Erase();
                        actual.Dispose();
                        actualLst.AddRange(act);
                    }

                    //predicted data
                    predicted = predictedDataMap.Values.First();
                    var pred = predicted.GetDenseData<float>(fun).Select(l => MLValue.GetResult(l));
                    predicted.Erase();
                    predicted.Dispose();
                    predictedLst.AddRange(pred);

                    // check if sweepend reached
                    if (mbData.Any(x => x.Value.sweepEnd))
                        break;
                }

                return (actualLst, predictedLst);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Perform descriptive statistic calculation on set of data
        /// </summary>
        private static Func<double[], double[], double> createFunction(string functionName)
        {
            var fun = (EFunction)Enum.Parse(typeof(EFunction), functionName, true);
            switch (fun)
            {
                case EFunction.ClassificationError:
                    return GPdotNet.MathStuff.AdvancedStatistics.CE;
                case EFunction.SquaredError:
                    return GPdotNet.MathStuff.AdvancedStatistics.SE;
                case EFunction.RMSError:
                    return GPdotNet.MathStuff.AdvancedStatistics.RMSE;
                case EFunction.MSError:
                    return GPdotNet.MathStuff.AdvancedStatistics.MSE;
                case EFunction.ClassificationAccuracy:
                    return GPdotNet.MathStuff.AdvancedStatistics.CA;

                default:
                    throw new Exception($"The '{functionName}' function is not supported!");
            }
        }
        public static double CalculateMetrics(string functionName, IEnumerable<float> actual, IEnumerable<float> predicted, DeviceDescriptor device)
        {

            var fun = createFunction(functionName);
            var result = fun(actual.Select(x=>(double)x).ToArray(), predicted.Select(x => (double)x).ToArray());
            return result;
            //map variables and data
            //var actualVar = Variable.InputVariable(actual.Shape, DataType.Float, "actual");
            //var predictedVar = Variable.InputVariable(predicted.Shape, DataType.Float, "predicted");

            ////map vars and values
            //var inputDataMap = new Dictionary<Variable, Value>();
            //inputDataMap.Add(actualVar, actual);
            //inputDataMap.Add(predictedVar, predicted);

            ////evaluate 
            //var function = createFunction(fun, actualVar, predictedVar);  //statFun(actualVar, predictedVar);

            ////
            //var outputDataMap = new Dictionary<Variable, Value>() { { fun, null } };
            //fun.Evaluate(inputDataMap, outputDataMap, device);
            //var value = outputDataMap[fun].GetDenseData<float>(fun);

            //for testing the result of the above function . Can be removed once the test is completed
            //var xVal = actual.GetDenseData<float>(actualVar)[0].Select(x => (double)x).ToArray();
            //var yVal = predicted.GetDenseData<float>(predictedVar)[0].Select(x => (double)x).ToArray();
            //var se = AdvancedStatistics.R(xVal, yVal);
            //Debug.Assert(Math.Round(se, 5)==Math.Round(value[0][0],5));
            //end of test
            //return value[0][0];
        }
        #endregion
    }

    public class EvalParameters
    {
        //public string DataFilePath { get; set; }
        public uint MinibatchSize { get; set; }
        public MinibatchSourceEx MBSource { get; set; }
        public List<Variable> Input { get; set; }
        public List<Variable> Ouptut { get; set; }

      //  public StreamConfiguration[] StrmsConfig { get; set; }
    }
}
