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
using GPdotNet.MathStuff;
using System.Threading.Tasks;
using System.Globalization;

namespace ANNdotNET.Core
{
    /// <summary>
    /// Implement network model evaluation for various purpose in the application
    /// </summary>
    public class MLEvaluator
    {
        #region Public Members

        /// <summary>
        /// Evaluate model defined in the mlconfig file 
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static async Task<EvaluationResult> EvaluateMLConfig(string mlconfigPath, DeviceDescriptor device, DataSetType dsType, EvaluationType evType)
        {
            try
            {
                //define eval result
                var er = new EvaluationResult();
                er.OutputClasses = new List<string>() {""};
                er.Actual = new List<float>();
                er.Predicted = new List<float>();
                er.Header = new List<string>();

                //Load ML configuration file
                var dicMParameters = MLFactory.LoadMLConfiguration(mlconfigPath);
                //add full path of model folder since model file doesn't contains any absolute path
                dicMParameters.Add("root", MLFactory.GetMLConfigFolder(mlconfigPath));

                // get model data paths
                var dicPath = MLFactory.GetMLConfigComponentPaths(dicMParameters["paths"]);
                //parse feature variables
                var projectValues = dicMParameters["training"].Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
                var modelName = MLFactory.GetParameterValue(projectValues, "TrainedModel");
                var nnModelPath = Path.Combine(dicMParameters["root"], modelName);
                //check if model exists
                if (!MLFactory.IsFileExist(nnModelPath))
                    return er;
                //
                var dataset = MLFactory.GetDataPath(dicMParameters, dsType);
                if (string.IsNullOrEmpty(dataset) || string.IsNullOrEmpty(dataset) || dataset == " ")
                {
                    if(dsType== DataSetType.Testing)
                        dataset = MLFactory.GetDataPath(dicMParameters, DataSetType.Validation);
                    if (string.IsNullOrEmpty(dataset) || string.IsNullOrEmpty(dataset) || dataset == " ")
                        return er;
                }
                   

                //get output classes in case the ml problem is classification
                var strCls = dicMParameters.ContainsKey("metadata") ? dicMParameters["metadata"] : "";
                var oc = MLFactory.GetOutputClasses(strCls);
                if (oc != null)
                    er.OutputClasses = oc;

                //MInibatch
                var mbTypestr = MLFactory.GetParameterValue(projectValues, "Type");
                MinibatchType mbType = (MinibatchType)Enum.Parse(typeof(MinibatchType), mbTypestr, true);
                var mbSizetr = MLFactory.GetParameterValue(projectValues, "BatchSize");

                var mf = MLFactory.CreateMLFactory(dicMParameters);


                //perform evaluation
                var evParams = new EvaluationParameters()
                {

                    MinibatchSize = uint.Parse(mbSizetr),
                    MBSource = new MinibatchSourceEx(mbType, mf.StreamConfigurations.ToArray(),mf.InputVariables, mf.OutputVariables, dataset, null, MinibatchSource.FullDataSweep, false,0),
                    Input = mf.InputVariables,
                    Ouptut = mf.OutputVariables,
                };

                //evaluate model
                if (evType == EvaluationType.FeaturesOnly)
                {
                    if (!dicMParameters.ContainsKey("metadata"))
                        throw new Exception("The result cannot be exported to Excel, since no metadata is stored in mlconfig file.");

                    var desc = MLFactory.ParseRawDataSet(dicMParameters["metadata"]);
                    er.Header = MLFactory.GenerateHeader(desc);
                    var fun = Function.Load(nnModelPath, device);
                    //
                    er.DataSet = await Task.Run(() => MLEvaluator.FeaturesAndLabels(fun, evParams, device));
                   
                    return er;
                }
                else if (evType == EvaluationType.Results)
                {
                    //define header
                    er.Header.Add(evParams.Ouptut.First().Name + "_actual");
                    er.Header.Add(evParams.Ouptut.First().Name + "_predicted");

                    var fun =  Function.Load(nnModelPath, device);
                    //
                    var result = await Task.Run(()=> MLEvaluator.EvaluateFunction(fun, evParams, device));
                    er.Actual = result.actual.ToList();
                    er.Predicted = result.predicted.ToList();

                    if(er.OutputClasses.Count<2 && evParams.Ouptut.First().Shape.Dimensions.Last()>1)
                    {
                        var result1 = await Task.Run(() => MLEvaluator.EvaluateFunctionEx(fun, evParams, device));
                        er.ActualEx = result1.actual;
                        er.PredictedEx = result1.predicted;
                    }
                    return er;
                }
                else if (evType == EvaluationType.ResultExtended)
                {
                    //define header
                    er.Header.Add(evParams.Ouptut.First().Name + "_actual");
                    er.Header.Add(evParams.Ouptut.First().Name + "_predicted");
                    er.Actual = new List<float>();
                    er.Predicted = new List<float>();
                    er.ActualEx = new List<List<float>>();
                    er.PredictedEx = new List<List<float>>();

                    //
                    var fun = Function.Load(nnModelPath, device);
                    var resultEx = await Task.Run(() => MLEvaluator.EvaluateFunctionEx(fun, evParams, device));
                    //var resultEx = EvaluateFunctionEx(nnModelPath, dataPath, evParams, device);
                    for (int i = 0; i < resultEx.actual.Count(); i++)
                    {
                        var res1 = MLValue.GetResult(resultEx.actual[i]);
                        er.Actual.Add(res1);
                        var res2 = MLValue.GetResult(resultEx.predicted[i]);
                        er.Predicted.Add(res2);
                    }
                    er.ActualEx = resultEx.actual;
                    er.PredictedEx = resultEx.predicted;

                    return er;
                }
                else
                    throw new Exception("Unknown evaluation type!");
            }
            catch (Exception)
            {

                throw;
            }
        }

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
                    //var mbData = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, device);
                    //var mdDataEx = MinibatchSourceEx.ToMinibatchValueData(mbData, evParam.Input.Union(evParam.Ouptut).ToList());
                    //var inMap = new Dictionary<Variable, Value>();
                    //
                    var isSweepEnd = false;
                    var iovars = evParam.Input.Union(evParam.Ouptut).ToList();
                    var mdDataEx = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, ref isSweepEnd, iovars, device);

                    //input
                    var vars = evParam.Input;
                    for (int i = 0; i < vars.Count() /*mdDataEx.Count*/; i++)
                    {
                        var vv = vars.ElementAt(i);
                        var d = mdDataEx.Where(x => x.Key.Name.Equals(vv.Name)).FirstOrDefault();
                        //
                        var fv = MLValue.GetValues(d.Key, d.Value);
                        if (featDic.ContainsKey(d.Key.Name))
                            featDic[d.Key.Name].AddRange(fv);
                        else
                            featDic.Add(d.Key.Name, fv);
                    }
                    //output 
                    var varso = evParam.Ouptut;
                    for (int i = 0; i < varso.Count() /*mdDataEx.Count*/; i++)
                    {
                        var vv = varso.ElementAt(i);
                        var d = mdDataEx.Where(x => x.Key.Name.Equals(vv.Name)).FirstOrDefault();
                        //
                        var fv = MLValue.GetValues(d.Key, d.Value);
                        if (vv.Shape.Dimensions.First() == 1)
                        {
                            var value = fv.Select(l => new List<float>() { l.First() }).ToList();
                            if (featDic.ContainsKey(d.Key.Name))
                                featDic[d.Key.Name].AddRange(value);
                            else
                                featDic.Add(d.Key.Name, value);
                        }
                        else
                        {
                            var value = fv.Select(l => new List<float>() { l.IndexOf(l.Max()) }).ToList();
                            if (featDic.ContainsKey(d.Key.Name))
                                featDic[d.Key.Name].AddRange(value);
                            else
                                featDic.Add(d.Key.Name, value);
                        }
                    }

                    // check if sweep end reached
                    if (/*mbData.Any(x => x.Value.sweepEnd)*/isSweepEnd)
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
                    //var mbData = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, device);
                    //var mbDataEx = MinibatchSourceEx.ToMinibatchValueData(mbData, evParam.Input.Union(evParam.Ouptut).ToList());
                    //var inMap = new Dictionary<Variable, Value>();

                    var isSweepEnd = false;
                    var iovars = evParam.Input.Union(evParam.Ouptut).ToList();
                    var mbDataEx = evParam.MBSource.GetNextMinibatch(evParam.MinibatchSize, ref isSweepEnd, iovars, device);
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
                    if (/*mbData.Any(x => x.Value.sweepEnd)*/isSweepEnd)
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
        /// Calculate smetrisc for 1D actual and predictes values
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="actual"></param>
        /// <param name="predicted"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static double CalculateMetrics(string functionName, IEnumerable<float> actual, IEnumerable<float> predicted, DeviceDescriptor device)
        {

            var fun = createFunction(functionName);
            var result = fun(actual.Select(x => (double)x).ToArray(), predicted.Select(x => (double)x).ToArray());
            return result;
        }

        /// <summary>
        /// Calculates matrics for time series with more than one timeStep ahead
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="actual"></param>
        /// <param name="predicted"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static double CalculateMetrics(string functionName, IEnumerable<IEnumerable<float>> actual, IEnumerable<IEnumerable<float>> predicted, DeviceDescriptor device)
        {

            var fun = createFunctionEx(functionName);
            var actualValues = actual.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray();
            var predictedValues = predicted.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray();
            var result = fun(actualValues, predictedValues);
            return result;
        }

        public static ModelPerformance CalculatePerformance(EvaluationResult evalResult, string dataSetName = "Training set")
        {
            var mpt = new ModelPerformance();
            mpt.DatSetName = dataSetName;
            mpt.Classes = evalResult.OutputClasses.ToArray();

            //define collections for the results
            var actual = new double[evalResult.Actual.Count()];
            var predicted = new double[evalResult.Predicted.Count()];

            //extract result value             
            for (int i = 0; i < evalResult.Actual.Count(); i++)
                actual[i] = evalResult.Actual[i];

            for (int i = 0; i < evalResult.Predicted.Count(); i++)
                predicted[i] = evalResult.Predicted[i];

            //regression
            if (evalResult.OutputClasses.Count == 1)
            {
                //Training data set
                mpt.SE = (float)actual.SE(predicted);
                mpt.RMSE = (float)actual.RMSE(predicted);
                mpt.NSE = (float)actual.NSE(predicted);
                mpt.PB = (float)actual.PBIAS(predicted);
                mpt.CORR = (float)actual.R(predicted);
                mpt.DETC = (float)actual.R2(predicted);
            }
            if (evalResult.OutputClasses.Count < 2 && evalResult.ActualEx.First().Count > 1)
            {
                //Training data set
                var actualValues = evalResult.ActualEx.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray();
                var predictedValues = evalResult.PredictedEx.Select(x => x.Select(xx => (double)xx).ToArray()).ToArray();

                mpt.SE = (float)AdvancedStatistics.SE(actualValues, predictedValues);
                mpt.RMSE = (float)AdvancedStatistics.RMSE(actualValues, predictedValues);
                mpt.NSE = (float)AdvancedStatistics.NSE(actualValues, predictedValues);
                mpt.PB = (float)AdvancedStatistics.PBIAS(actualValues, predictedValues);
                mpt.CORR = (float)AdvancedStatistics.R(actualValues, predictedValues);
                mpt.DETC = (float)AdvancedStatistics.R2(actualValues, predictedValues);
            }
            else if (evalResult.OutputClasses.Count > 1)
            {
                var retVal = TransformResult(evalResult.ActualEx, evalResult.PredictedEx);
                retVal.Add("Classes", mpt.Classes.ToList<object>());
                mpt.PerformanceData = retVal;

                if (evalResult.OutputClasses.Count == 2)
                {
                   
                    //construct confusion matrix 
                    var o = retVal["obs_train"].Select(x => int.Parse(x.ToString())).ToArray();
                    var p = retVal["prd_train"].Select(x => ((double)x) < 0.5 ? 0 : 1).ToArray();
                    var cm = new ConfusionMatrix(o, p, 2);
                    //get result
                    mpt.ER = (float)ConfusionMatrix.Error(cm.Matrix);
                    //mpt.AUC = 

                    //confusion matrix for current threshold
                    mpt.Acc = (float)ConfusionMatrix.OAC(cm.Matrix);
                    mpt.ER = (float)ConfusionMatrix.Error(cm.Matrix);

                    mpt.Precision = (float)ConfusionMatrix.Precision(cm.Matrix, 1);
                    mpt.Recall = (float)ConfusionMatrix.Recall(cm.Matrix, 1);
                    mpt.F1Score = (float)ConfusionMatrix.Fscore(cm.Matrix, 1) ;

                    mpt.HSS = (float)ConfusionMatrix.HSS(cm.Matrix, o.Length) ;
                    mpt.PSS = (float)ConfusionMatrix.PSS(cm.Matrix, o.Length) ;

                    //lab
                    //mpt.Label = m_Classes[1];// "TRUE";
                    //txNegativeLable.Text = m_Classes[0];// "FALSE";

                    //false and true positive negative
                    mpt.FN = (float)cm.Matrix[1][0];//cm.FalseNegatives 
                    mpt.FP = (float)cm.Matrix[0][1];//cm.FalsePositives 
                    mpt.TP = (float)cm.Matrix[1][1];//cm.TruePositives 
                    mpt.TN = (float)cm.Matrix[0][0];//cm.TrueNegatives 
                }
                else 
                {
                    //construct confusion matrix 
                    var o = retVal["obs_train"].Select(x => int.Parse(x.ToString())).ToArray();
                    var p = retVal["prd_train"].Select(x => int.Parse(x.ToString())).ToArray();
                    var cm = new ConfusionMatrix(o, p, evalResult.OutputClasses.Count);

                    //confusion matrix for current threshold
                    mpt.OAcc = (float)ConfusionMatrix.OAC(cm.Matrix);
                    mpt.AAcc = (float)ConfusionMatrix.AAC(cm.Matrix);
                    mpt.MacPrec = (float)ConfusionMatrix.MacroPrecision(cm.Matrix);
                    mpt.MacRcall = (float)ConfusionMatrix.MacroRecall(cm.Matrix);
                    mpt.MicPrec = (float)ConfusionMatrix.MicroPrecision(cm.Matrix);
                    mpt.MicRcall = (float)ConfusionMatrix.MicroRecall(cm.Matrix);
                    //mpt.MacFscore = (float)ConfusionMatrix.MacroFscore(cm.Matrix);
                    //mpt.MicFscore = (float)ConfusionMatrix.MicroFscore(cm.Matrix);

                    mpt.HSS = (float)ConfusionMatrix.HSS(cm.Matrix, o.Length);
                    mpt.PSS = (float)ConfusionMatrix.PSS(cm.Matrix, o.Length);
                }
            }

            return mpt;
        }

        public static Dictionary<string, List<object>> TransformResult(List<List<float>> ActualT, List<List<float>> PredictedT)
        {

            var dic = new Dictionary<string, List<object>>();
            //get output for training data set
            List<double> actualT = new List<double>();
            List<double> predictedT = new List<double>();
            //
            if (ActualT != null && ActualT.Count > 0)
            {
                //category output
                for (int i = 0; i < ActualT.Count; i++)
                {
                    float act = 0;
                    float pred = 0;
                    //category output
                    if (ActualT[i].Count > 2)
                    {
                        act = ActualT[i].IndexOf(ActualT[i].Max());
                        pred = PredictedT[i].IndexOf(PredictedT[i].Max());
                    }
                    else if (ActualT[i].Count == 2)
                    {
                        act = ActualT[i].IndexOf(ActualT[i].Max());
                        pred = PredictedT[i][1];
                    }
                    else
                    {
                        act = ActualT[i].First();
                        pred = PredictedT[i].First();
                    }


                    actualT.Add(act);
                    predictedT.Add(pred);
                }
            }

            
            //add train data
            if (actualT != null)
            {
                //add data sets
                dic.Add("obs_train", actualT.Select(x => (object)x).ToList<object>());
                dic.Add("prd_train", predictedT.Select(x => (object)x).ToList<object>());
            }

            return dic;

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
                    var dim = var.Shape.Dimensions.First();
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
        /// <summary>
        /// Test cntk model stored at 'modelPath' against array of image paths
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="vector"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static List<int> TestModel(string modelPath, string[] imagePaths, DeviceDescriptor device)
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
                //get input feature
                var features = model.Arguments.ToList();
                var labels = model.Outputs.ToList();
                var stremsConfig = MLFactory.CreateStreamConfiguration(features, labels);
                var mapFile = "testMapFile";
                File.WriteAllLines(mapFile, imagePaths.Select(x=>$"{x}\t0"));

                var testMB = new MinibatchSourceEx(MinibatchType.Image, stremsConfig.ToArray(),features,labels,mapFile,null,30,false, 0);

                //
                var vars = features.Union(labels).ToList();
                var retVal = new List<int>();
                var mbSize = imagePaths.Count();
                if (mbSize > 30)
                    mbSize = 30;
                while (true)
                {
                    bool isSweepEnd = false;
                    var inputMap = testMB.GetNextMinibatch((uint)mbSize,ref isSweepEnd, vars, device);
                    //prepare data for trainer
                    //var inputMap = new Dictionary<Variable, Value>();
                    //inputMap.Add(features.First(), nextMB.Where(x => x.Key.m_name.Equals(features.First().Name)).Select(x => x.Value.data).FirstOrDefault());
                    

                    var outputMap = new Dictionary<Variable, Value>();
                    outputMap.Add(labels.First(), null);
                    //evaluate model
                    model.Evaluate(inputMap, outputMap, device);
                    var result = outputMap[labels.First()].GetDenseData<float>(labels.First());

                    //extract result
                    foreach(var r in result)
                    {
                        var l= MLValue.GetResult(r);
                        retVal.Add((int)l);
                    }

                    if (/*nextMB.Any(x => x.Value.sweepEnd)*/ isSweepEnd)
                        break;
                }


                return retVal;
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

        private static Func<double[][], double[][], double> createFunctionEx(string functionName)
        {
            var fun = (EFunction)Enum.Parse(typeof(EFunction), functionName, true);
            switch (fun)
            {
                //case EFunction.ClassificationError:
                //    return GPdotNet.MathStuff.AdvancedStatistics.CE;
                case EFunction.SquaredError:
                    return GPdotNet.MathStuff.AdvancedStatistics.SE;
                case EFunction.RMSError:
                    return GPdotNet.MathStuff.AdvancedStatistics.RMSE;
                case EFunction.MSError:
                    return GPdotNet.MathStuff.AdvancedStatistics.MSE;
                ////case EFunction.ClassificationAccuracy:
                ////    return GPdotNet.MathStuff.AdvancedStatistics.CA;

                default:
                    throw new Exception($"The '{functionName}' function is not supported!");
            }
        }

        #endregion
    }
}
