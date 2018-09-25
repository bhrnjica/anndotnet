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
        /// Returns part of mldataset with features labels columns this is needed in case Excel export is performed.
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="evParam"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Dictionary<string, List<List<float>>> FeaturesAndLabels(Function fun, EvaluationParameters evParam, DeviceDescriptor device)
        {
            try
            {
                //declare return vars
                var featDic = new Dictionary<string, List<List<float>>>();

                while (true)
                {
                    //get one minibatch of data for training
                    var mbData = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, device);
                    var mdDataEx = MinibatchSourceEx.ToMinibatchValueData(mbData, evParam.Input.Union(evParam.Ouptut).ToList());
                    var inMap = new Dictionary<Variable, Value>();
                    //
                    for (int i = 0; i < mdDataEx.Count; i++)
                    {
                        var d = mdDataEx.ElementAt(i);

                        if(!evParam.Ouptut.First().Name.Equals(d.Key.Name))
                        {
                            //
                            var fv = MLValue.GetValues(d.Key, d.Value);
                            if (featDic.ContainsKey(d.Key.Name))
                                featDic[d.Key.Name].AddRange(fv);
                            else
                                featDic.Add(d.Key.Name, fv);
                        }
                        else
                        {
                            //
                            var fv = MLValue.GetValues(d.Key, d.Value);
                            var value = fv.Select(l=> new List <float>() { l.IndexOf(l.Max()) }).ToList();
                            if (featDic.ContainsKey(d.Key.Name))
                                featDic[d.Key.Name].AddRange(value);
                            else
                                featDic.Add(d.Key.Name, value);
                        }
                    }

                    // check if sweep end reached
                    if (mbData.Any(x => x.Value.sweepEnd))
                        break;
                }

                return featDic;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static (IEnumerable<float> actual, IEnumerable<float> predicted) EvaluateFunction(Function fun, EvaluationParameters evParam, DeviceDescriptor device)
        {
            try
            {
                //declare return vars
                List<float> actualLst = new List<float>();
                List<float> predictedLst = new List<float>();

                var result = EvaluateFunctionEx(fun, evParam, device);
                for (int i = 0; i < result.actual.Count(); i++)
                {
                    var res1 = MLValue.GetResult(result.actual[i]);
                    actualLst.Add(res1);
                    var res2 = MLValue.GetResult(result.predicted[i]);
                    predictedLst.Add(res2);
                }
                return (actualLst, predictedLst);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static (List<List<float>> actual, List<List<float>> predicted) EvaluateFunctionEx(Function fun, EvaluationParameters evParam, DeviceDescriptor device)
        {
            try
            {
                //declare return vars
                List<List<float>> actualLst = new List<List<float>>();
                List<List<float>> predictedLst = new List<List<float>>();

                while (true)
                {
                    Value predicted = null;
                    //get one minibatch of data for training
                    var mbData = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, device);
                    var mbDataEx = MinibatchSourceEx.ToMinibatchValueData(mbData, evParam.Input.Union(evParam.Ouptut).ToList());
                    var inMap = new Dictionary<Variable, Value>();
                    //
                    var vars = fun.Arguments.Union(fun.Outputs);
                    for (int i = 0; i < vars.Count()/* mbDataEx.Count*/; i++)
                    {
                        var d = mbDataEx.ElementAt(i);
                        var v = vars.Where(x=>x.Name.Equals(d.Key.Name)).First();
                        //skip output data 
                        if (!evParam.Ouptut.Any(x => x.Name.Equals(v.Name)))
                            inMap.Add(v, d.Value);
                    }

                    //actual data if t is available
                    var actualVar = mbDataEx.Keys.Where(x => x.Name.Equals(evParam.Ouptut.First().Name)).FirstOrDefault();
                    var act = mbDataEx[actualVar].GetDenseData<float>(actualVar).Select(l => l.ToList());
                    actualLst.AddRange(act);

                    //predicted data
                    //map variables and data
                    var predictedDataMap = new Dictionary<Variable, Value>() { { fun, null } };

                    //evaluates model
                    fun.Evaluate(inMap, predictedDataMap, device);
                    predicted = predictedDataMap.Values.First();
                    var pred = predicted.GetDenseData<float>(fun).Select(l => l.ToList());
                    predicted.Erase();
                    predicted.Dispose();
                    predictedLst.AddRange(pred);

                    // check if sweep end reached
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

        public static double CalculateMetrics(string functionName, IEnumerable<float> actual, IEnumerable<float> predicted, DeviceDescriptor device)
        {

            var fun = createFunction(functionName);
            var result = fun(actual.Select(x => (double)x).ToArray(), predicted.Select(x => (double)x).ToArray());
            return result;
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
       
        #endregion
    }
}
