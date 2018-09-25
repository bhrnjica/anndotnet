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
            EvaluateModel(MLFactory mlF, MinibatchType type, string strDataSetPath, string modelPath, bool includeFeatures, bool includePrediction, DeviceDescriptor device)
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

                //features collection
                var featDic = new Dictionary<string, List<List<float>>>();

                //labels collection
                var actualDic = new Dictionary<string, List<List<float>>>();
                var predictDic = new Dictionary<string, List<List<float>>>();

                //*******extract features******
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

                //*****extract actual output values*****
                //Categorical and binary variables are encoded to its numerical representation of classes
                // so in case BInary one hot (0.23,0,77) =1, (0,77,0.23) =0,
                //categorical (0.1,0.3,0.6)=2
                foreach (var output in model.Outputs)
                {
                    var actual = new List<List<float>>();
                    var actual_hotvector = new List<List<float>>();

                    //
                    var featureStream = minibatchData.Keys.Where(x => x.m_name == output.Name).First();
                    if (featureStream != null)
                    {
                        var av = MLValue.GetValues(output, minibatchData[featureStream].data);
                        

                        //extract final result from hot-vector
                        foreach (var row in av)
                        {
                            actual.Add(new List<float>() { MLValue.GetResult(row) });

                        }
                        //order of adding items MUST NOT BE CHANGED
                        actualDic.Add($"{output.Name}", actual);
                        actualDic.Add($"{output.Name}_hotvector", av);
                    }

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

                //*****extract predicted values*****
                foreach (var output in model.Outputs)
                {
                    var predicted = new List<List<float>>();
                    if (includePrediction)
                    {
                        var fv = MLValue.GetValues(output, predictedDataMap[output]);
                        foreach (var row in fv)
                        {
                            predicted.Add(new List<float>() { MLValue.GetResult(row) });
                        }
                        //order of adding items MUST NOT BE CHANGED
                        predictDic.Add($"{output.Name}_predicted", predicted);
                        predictDic.Add($"{output.Name}_predicted_hotvector", fv);
                        
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
        /// Returns part of mldataset with features columns only
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="evParam"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Dictionary<string, List<List<float>>> Features(Function fun, EvalParameters evParam, DeviceDescriptor device)
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

        public static (IEnumerable<float> actual, IEnumerable<float> predicted) EvaluateFunction(Function fun, EvalParameters evParam, DeviceDescriptor device)
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

        public static (List<List<float>> actual, List<List<float>> predicted) EvaluateFunctionEx(Function fun, EvalParameters evParam, DeviceDescriptor device)
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
