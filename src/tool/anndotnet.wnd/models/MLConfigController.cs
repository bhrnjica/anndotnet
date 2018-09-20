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

using ANNdotNET.Lib;
using GPdotNet.MathStuff;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZedGraph;
namespace anndotnet.wnd.Models
{

    /// <summary>
    /// Main VieModel for Modeling. Contains all information needed for GP running.
    /// </summary>
    public class MLConfigController: BaseModel
    {
        #region Fields and Constructors
        ProcessDevice m_device= ProcessDevice.Default;
        string m_Name;
        TrainingProgress m_TrainingProgress;
        /// <summary>
        /// Main Model Constructor 
        /// </summary>
        /// <param name="fun"></param>
        public MLConfigController(Action<BaseModel, bool> fun) : base(fun)
        {
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                if (value != m_Name)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(m_Name))
                        {
                            //change model file
                            var filePathold = Project.GetMLConfigPath(Settings, m_Name);
                            var filePathnew = Project.GetMLConfigPath(Settings, value);
                            System.IO.File.Move(filePathold, filePathnew);
                            //model folder path
                            var folderPathold = Project.GetMLConfigFolder(Settings, m_Name);
                            var folderPathnew = Project.GetMLConfigFolder(Settings, value);

                            //change model folder
                            System.IO.Directory.Move(folderPathold, folderPathnew);

                        }

                        //change property
                        m_Name = value;
                        RaisePropertyChangedEvent("Name");
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }
            }
        }

        public ProjectSettings Settings { get; set; }

        //input layer
        public ObservableCollection<IOLayer> InLayer { get; set; }

        public ObservableCollection<IOLayer> OutLayer { get; set; }

        public ObservableCollection<NNLayer> Network { get; set; }

        public List<VariableDescriptor> TestData { get; set; }
        public LearningParameters LearningParameters { get; set; }

        public TrainingParameters TrainingParameters { get; set; }

        public ModelEvaluation ModelEvaluation { get; set; }

        public TrainingHistory TrainingHistory { get; set; }

        public TrainingProgress TrainingProgress
        {
            get
            {
                return m_TrainingProgress;
            }
            set
            {
                if (m_TrainingProgress != value)
                {
                    m_TrainingProgress = value;
                    RaisePropertyChangedEvent("TrainingProgress");
                }
            }
        }

        //public new string IconUri { get => "Images/model.png"; }
        /// <summary>
        /// Icon for model representation in TreeVIew control
        /// </summary>
        string m_IconUri = "Images/model.png";
        public new string IconUri
        {
            get
            {
                return m_IconUri;
            }
            set
            {
                m_IconUri = value;
                RaisePropertyChangedEvent("IconUri");
            }
        }
        public bool IsRunning { get { return "Images/model.png" != IconUri; } }

        //dammy property for treeView templates
        public ObservableCollection<MLConfigController> Models { get; set; }

        public bool Deleted { get; private set; }
        public Action<int, double, double, double, double> UpdateTrainingtGraphs { get; internal set; }
        #endregion
        internal bool InitModel()
        {
            try
            {
                var modelPath = Project.GetMLConfigPath(Settings, Name);
                var modValues = Project.LoadMLConfig(modelPath);
                if (modValues == null)
                    throw new Exception("Model configuration is not found.");
                //initialize network model
                InLayer = new ObservableCollection<IOLayer>();
                initializeInputLayer(InLayer, modValues["features"]);
                //
                var lst = initializeNetwork(modValues["network"]);
                Network = new ObservableCollection<NNLayer>(lst);

                //initialize network model
                OutLayer = new ObservableCollection<IOLayer>();
                initializeOutputLayer(OutLayer, modValues["labels"]);

                //learning  parameters
                LearningParameters = initializeLearningParameters(modValues["learning"]);

                //taining  parameters
                TrainingParameters = initializeTrainingParameters(modValues["training"]);

                //initialize progress
                TrainingProgress = inittrainingProgress();

                //initialize evaluation
                var meta = modValues.ContainsKey("metadata") ? modValues["metadata"] : null;
                TestData = initializeTestData(meta);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
            

        }

        internal void SaveModel()
        {
            //configuration is empty 
            if (TrainingParameters == null)
                return;

            var mlCOnfig = new Dictionary<string,string>();
            var trainingValue = saveTrainingParameters(TrainingParameters);           
            mlCOnfig.Add("training", trainingValue);
            var learningValue = saveLearningParameters(LearningParameters);
            mlCOnfig.Add("learning", learningValue);
            var networkValue = saveNetworkParameters(Network);
            mlCOnfig.Add("network", networkValue);

            //save data paths
            var lstDict = Project.GetMLConfigPaths(Settings, Name);
            foreach (var d in lstDict)
                mlCOnfig.Add(d.Key,d.Value);

            //save to file
            var filePath = Project.GetMLConfigPath(Settings, Name);// Path.Combine(Settings.ProjectFolder,Path.GetFileNameWithoutExtension(Settings.ProjectFile), Name);
            Project.SaveConfigFile(filePath, mlCOnfig);
        }

        internal void DeleteModel()
        {
            try
            {
                Deleted = true;
                var modelPath = Project.GetMLConfigPath(Settings, Name);
                System.IO.File.Delete(modelPath);
                //then delete all content in the model folder
                var modelFolder = Project.GetMLConfigFolder(Settings, Name);
                Project.DeleteAllFiles(modelFolder);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ModelEvaluation EvaluateModel()
        {
            try
            {
                //init empty model evaluation
                var mEval = new ModelEvaluation()
                {
                    TrainingValue = new List<ZedGraph.PointPair>(),
                    ValidationValue = new List<ZedGraph.PointPair>(),
                    ModelValueTraining = new List<ZedGraph.PointPair>(),
                    ModelValueValidation = new List<ZedGraph.PointPair>(),
                    Classes = new List<string>(),
                    ModelOutputDim = 1

                };
                var mpt = new ModelPerformance();
                mpt.DatSetName = "Training set";
                var mpv = new ModelPerformance();
                mpv.DatSetName = "Validation set";


                //check if the trained model exists
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel) || string.IsNullOrEmpty(TrainingParameters.LastBestModel.Trim(' ')))
                {
                    return mEval;
                }
                
                //save model before evaluation since there is a data must be stored into model file.
                SaveModel();

                //get model full path
                var modelMLPath = Project.GetMLConfigPath(Settings, Name);

                //evaluate model against training data 
                var resultTrain = Project.EvaluateModel(modelMLPath, false, true, true, true, ProcessDevice.Default);

                //evaluate model against validation data
                var resultValidation = Project.EvaluateModel(modelMLPath, false, true, false,true, ProcessDevice.Default);

                //prepare evaluation result
                var actualT = resultTrain.actualDict.ElementAt(0).Value.Select(x => x.First());
                for (int i = 0; i < actualT.Count(); i++)
                    mEval.TrainingValue.Add(new PointPair(i + 1, actualT.ElementAt(i)));

                var predicT = resultTrain.predictedDict.ElementAt(0).Value.Select(x => x.First());
                for (int i = 0; i < predicT.Count(); i++)
                    mEval.ModelValueTraining.Add(new PointPair(i + 1, predicT.ElementAt(i)));


                var actualV = resultValidation.actualDict.ElementAt(0).Value.Select(x => x.First());
                for (int i = 0; i < actualV.Count(); i++)
                    mEval.ValidationValue.Add(new PointPair(i + 1, actualV.ElementAt(i)));

                var predicV = resultValidation.predictedDict.ElementAt(0).Value.Select(x => x.First());
                for (int i = 0; i < predicV.Count(); i++)
                    mEval.ModelValueValidation.Add(new PointPair(i + 1, predicV.ElementAt(i)));

                //
                mEval.Classes = resultValidation.outputClasses;
                mEval.ModelOutputDim = resultTrain.outputClasses == null ? 1 : resultTrain.outputClasses.Count;

                if(mEval.ModelOutputDim==1)
                {
                    //Training data set
                    var actTData = mEval.TrainingValue.Select(x => x.Y).ToArray();
                    var preTData = mEval.ModelValueTraining.Select(x => x.Y).ToArray();
                    mpt.SE = (float)actTData.SE(preTData);
                    mpt.RMSE = (float)actTData.RMSE(preTData);
                    mpt.NSE = (float)actTData.NSE(preTData);
                    mpt.PB = (float)actTData.PBIAS(preTData);
                    mpt.CORR = (float)actTData.R(preTData);
                    mpt.DETC = (float)actTData.R2(preTData);

                    //validation data set
                    var actVData = mEval.ValidationValue.Select(x => x.Y).ToArray();
                    var preVData = mEval.ModelValueValidation.Select(x => x.Y).ToArray();
                    mpv.SE = (float)actVData.SE(preVData);
                    mpv.RMSE = (float)actVData.RMSE(preVData);
                    mpv.NSE = (float)actVData.NSE(preVData);
                    mpv.PB = (float)actVData.PBIAS(preVData);
                    mpv.CORR = (float)actVData.R(preVData);
                    mpv.DETC = (float)actVData.R2(preVData);

                    
                }
                else if(mEval.ModelOutputDim > 1)
                {
                    var actualT1 = resultTrain.actualDict.ElementAt(1).Value;
                    var predictedT1 = resultTrain.predictedDict.ElementAt(1).Value;
                    var actualV1 = resultValidation.actualDict.ElementAt(1).Value;
                    var predictedV1 = resultValidation.predictedDict.ElementAt(1).Value;
                    var retVal = EvaluateResults(actualT1, predictedT1, null, null);
                    var retValV = EvaluateResults(actualV1, predictedV1, null, null);
                    retVal.Add("Classes", mEval.Classes.ToList<object>());
                    retValV.Add("Classes", mEval.Classes.ToList<object>());
                    
                    mpt.PerformanceData = retVal;
                    mpv.PerformanceData = retValV;
                }
                mEval.TrainPerformance = mpt;
                if(mEval.Classes!=null)
                    mEval.TrainPerformance.Classes = mEval.Classes.ToArray();
                mEval.ValidationPerformance = mpv;
                if (mEval.Classes != null)
                    mEval.ValidationPerformance.Classes = mEval.Classes.ToArray();
                ModelEvaluation = mEval;
                return mEval;

            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async void RunTraining(CancellationToken token)
        {
            try
            {
                //check if the model parameters are valid for training
                isModelParametersValid();

                //first save model
                SaveModel();

                //raise event since some controls depend on this property
                //change icon of treeView item to indicate this model is running
                IconUri = "Images/runningmodel.png";
                RaisePropertyChangedEvent("IsRunning");

                //create training progress var
                //reset previous and return new object
                if(TrainingParameters.ContinueTraining==false)
                {
                    TrainingProgress = new TrainingProgress()
                    {
                        Iteration = "0",
                        MBLossValue = new List<PointPair>(),
                        MBEvaluationValue = new List<PointPair>(),
                        TrainEvalValue = new List<PointPair>(),
                        ValidationEvalValue = new List<PointPair>(),
                        TrainingLoss = "0",
                    };
                    if (UpdateTrainingtGraphs != null)
                        UpdateTrainingtGraphs(0,0,0,0,0);
                }

                if (TrainingProgress.MBLossValue.Count==0 || TrainingParameters.Epochs > TrainingProgress.MBLossValue.Last().X)
                {
                    //LOad ML configuration file
                    var modelPath = Project.GetMLConfigPath(Settings, Name);
                    //
                    var res = await Task.Run<TrainResult>(() => Project.TrainModel(modelPath, token, trainingProgress, m_device));

                    //save best trained model
                    TrainingParameters.LastBestModel = res.BestModelFile;
                    //once the training process completes inform the GUI about it
                    var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                    //send note to GUI the training process is completed 
                    IconUri = "Images/model.png";
                    RaisePropertyChangedEvent("IsRunning");
                    appCnt.TrainingCompleated(res);

                }
                else
                {
                    //once the training process completes inform the GUI about it
                    var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                    //send note to GUI the training process is completed 
                    IconUri = "Images/model.png";
                    RaisePropertyChangedEvent("IsRunning");
                    appCnt.TrainingCompleated(new TrainResult() { ProcessState= ProcessState.Compleated, Iteration= TrainingParameters.Epochs });
                }

            }
            catch (Exception ex)
            {

                // throw;
                //once the training process completes inform the GUI about it
                var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                //send note to GUI the training process is completed 
                TrainResult res = new TrainResult();
                res.BestModelFile = null;
                res.Iteration = 0;
                res.ProcessState = ProcessState.Crashed;
                //save best trained model
                TrainingParameters.LastBestModel = res.BestModelFile;
                appCnt.TrainingCompleated(res);
                appCnt.ReportException(ex);
            }
           
        }

        private void isModelParametersValid()
        {
            if (Network.Count == 1 && Network[0].Type == LayerType.Custom)
                return;
            //the last layer in the network should be output
            if (Network.Last().HDimension != OutLayer.First().Dimension)
                throw new Exception("The output dimension of the last layer in the network must be same as label dimension!");
            if(Network.Where(x=>x.HDimension<=0 && (x.Type!= LayerType.Normalization && x.Type != LayerType.Drop)).Count() > 0)
                throw new Exception("Layer cannot be defined with zero output dimension.!");
            if (Network.Where(x => x.CDimension <= 0 && x.Type== LayerType.LSTM).Count() > 0)
                throw new Exception("Layer cannot be defined with zero output dimension.!");
        }

        private TrainingProgress inittrainingProgress()
        {
            try
            {
                var trProg = new TrainingProgress()
                {
                    Iteration = "0",
                    MBLossValue = new List<PointPair>(),
                    MBEvaluationValue = new List<PointPair>(),
                    TrainEvalValue = new List<PointPair>(),
                    ValidationEvalValue = new List<PointPair>(),
                    TrainingLoss = "0",
                };

                var configPath = Project.GetMLConfigPath(Settings, Name);
                var configId = Project.GetMLConfigId(configPath);
                if (configId == null)
                    return trProg;
                else
                {
                    var strhistoryPath = Project.GetTrainingHistoryPath(configPath, configId);
                    var fi = new FileInfo(strhistoryPath);
                    if (fi.Exists)
                    {
                        var cnt = File.ReadLines(strhistoryPath);
                        if (cnt == null || cnt.Count() == 0)
                            return trProg;

                        var header = cnt.ElementAt(0).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


                        var lossFunc = header[0];
                        var evalFun = header[1];

                        foreach (var line in cnt.Skip(1))
                        {
                            var row = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            int it = int.Parse(row[0]);
                            float lossMB = (float)double.Parse(row[1]);
                            float evalMB = (float)double.Parse(row[2]);
                            float trainEval = (float)double.Parse(row[3]);
                            float valiEval = (float)double.Parse(row[4]);
                            trProg.MBLossValue.Add(new PointPair(it, lossMB));
                            trProg.MBEvaluationValue.Add(new PointPair(it, evalMB));
                            trProg.TrainEvalValue.Add(new PointPair(it, trainEval));
                            trProg.ValidationEvalValue.Add(new PointPair(it, valiEval));
                        }
                        //
                        trProg.TrainingLoss = lossFunc;
                        trProg.Iteration = $"{trProg.MBLossValue.Last().X} of {trProg.MBLossValue.Last().X}";
                    }
                    
                    return trProg;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        void trainingProgress(ProgressData progress)
        {
            try
            {
                // 
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                new Action(

                    () =>
                    {
                        TrainingProgress.Iteration = $"{progress.EpochCurrent} of {progress.EpochTotal}";
                        
                        //TrainingProgress.TestEvaluation = $"{progress.ValidationEval.ToString("0.000")}";
                        //TrainingProgress.EvaluationFunction = $"'{progress.EvaluationFunName}' evaluation";


                        if (!double.IsNaN(progress.MinibatchAverageLoss) && !double.IsInfinity(progress.MinibatchAverageLoss))
                        {
                            TrainingProgress.MBLossValue.Add(new PointPair(progress.EpochCurrent, progress.MinibatchAverageLoss));
                            TrainingProgress.TrainingLoss = $"{progress.MinibatchAverageLoss.ToString("0.000")}";
                        }
                        else
                            TrainingProgress.TrainingLoss = $"Infinity";



                        if (!double.IsNaN(progress.MinibatchAverageEval) && !double.IsInfinity(progress.MinibatchAverageEval))
                            TrainingProgress.MBEvaluationValue.Add(new PointPair(progress.EpochCurrent, progress.MinibatchAverageEval));


                        if (!double.IsNaN(progress.TrainEval) && !double.IsInfinity(progress.TrainEval))
                            TrainingProgress.TrainEvalValue.Add(new PointPair(progress.EpochCurrent, progress.TrainEval));

                        if (!double.IsNaN(progress.ValidationEval) && !double.IsInfinity(progress.ValidationEval))
                            TrainingProgress.ValidationEvalValue.Add(new PointPair(progress.EpochCurrent, progress.ValidationEval));


                        //update Graphs
                        if(UpdateTrainingtGraphs!=null)
                            UpdateTrainingtGraphs(progress.EpochCurrent, progress.MinibatchAverageLoss, progress.MinibatchAverageEval, progress.TrainEval, progress.ValidationEval);
                        //set status message
                        var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                        appCnt.StatusMessage = $"Iteration:{progress.EpochCurrent} of {progress.EpochTotal} processed!";
                    }

                ));
            }
            catch (Exception ex)
            {
                var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                appCnt.ReportException(ex);
            }
            

            ////
            //Console.WriteLine($"Epoch={progress.EpochCurrent} of {progress.EpochTotal};\t Evaluation of {progress.EvaluationFunName}=" +
            //    $"(TrainMB = {progress.MinibatchAverageEval},TrainFull = {progress.TrainEval}, Valid = {progress.ValidationEval})");
        }

        #region Helper Methods for model saving/Loading

        private string saveNetworkParameters(ObservableCollection<NNLayer> network)
        {
            var strValue = "";
            foreach(var l in network)
            {
                var stab = l.SelfStabilization == true ? 1 : 0;
                var peep = l.Peephole == true ? 1 : 0;
                strValue += $"|Layer:{l.Type} {l.HDimension} {l.CDimension} {l.Value} {l.Activation} {stab} {peep} ";
            }

            return strValue;
        }

        private string saveLearningParameters(LearningParameters lp)
        {
            var strValue = $"|Type:{lp.LearnerType} |LRate:{lp.LearningRate.ToString(CultureInfo.InvariantCulture)} " +
                $"|Momentum:{lp.Momentum.ToString(CultureInfo.InvariantCulture)} |Loss:{lp.LossFunction}" +
                $"|Eval:{lp.EvaluationFunction}" + $"|L1:{lp.L1Regularizer}" + $"|L2:{lp.L2Regularizer}";

            return strValue;
        }

        private string saveTrainingParameters(TrainingParameters tp)
        {
            var norm = tp.Normalization;
            if (tp.Normalization == null)
                norm = new string[] { "0" };
            //
            var ct = tp.ContinueTraining ? 1 : 0;
            var smwt = tp.SaveModelWhileTraining ? 1 : 0;
            var rb = tp.RandomizeBatch ? 1 : 0;
            var ftse = tp.FullTrainingSetEval ? 1 : 0;
            var strValue = $"|Type:{tp.Type} |BatchSize:{tp.BatchSize} |Epochs:{tp.Epochs} " +
                $"|Normalization:{string.Join(";", norm)} |RandomizeBatch:{rb}" +
                $" |SaveWhileTraining:{smwt} |FullTrainingSetEval:{ftse} |ProgressFrequency:{tp.ProgressFrequency}" +
                $" |ContinueTraining:{ct} |TrainedModel:{tp.LastBestModel} ";

            return strValue;
        }

        /// <summary>
        /// Once the model is created input layer should be initialized
        /// </summary>
        /// <param name="inLayer"></param>
        private void initializeInputLayer(ObservableCollection<IOLayer> inLayer, string strFeatures)
        {
            //
            var features = strFeatures.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i=0; features.Length > i; i++)
            {
                var layer = ParseLayer(features[i]);
                layer.Id = i + 1;
                inLayer.Add(layer);
            }
        }

        /// <summary>
        /// Parses string in order to initialize IOLayer object
        /// </summary>
        /// <param name="strValue">string to parse</param>
        /// <returns>IOLayer</returns>
        private IOLayer ParseLayer(string strValue)
        {
            var fet = strValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var itm = new IOLayer();
            itm.Name = fet[0];
            //
            itm.Representation = /* itm.Name == "NumFeatures" ?*/ "Dimension:";// : "One-Hot-Vector:";
            itm.Dimension = int.Parse(fet[1]);

            return itm;
        }

        private void initializeOutputLayer(ObservableCollection<IOLayer> outLayer, string strFeatures)
        {
            //
            var features = strFeatures.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; features.Length > i; i++)
            {
                var layer = ParseLayer(features[i]);
                if (layer.Dimension < 2)
                    layer.Representation = "Dimension";
                layer.Id = i + 1;
                outLayer.Add(layer);
            }
        }

        private List<NNLayer> initializeNetwork(string strNetwork)
        {
            var nnL =  Project.CreateNetworkParameters(strNetwork);
            return nnL;
        }

        private LearningParameters initializeLearningParameters(string strLearning)
        {
            var lp = Project.CreateLearningParameters(strLearning);
            return lp;
        }

        private TrainingParameters initializeTrainingParameters(string strTraining)
        {
            var tp = Project.CreateTrainingParameters(strTraining);
            return tp;
            
        }

        private List<VariableDescriptor> initializeTestData(string strMetadata)
        {
            if (strMetadata == null)
                return null;
            //parse metadat and define TestData form in which the user will enter data for testing model
            var cols = Project.ParseRawDataSet(strMetadata);
            return cols; //
        }
        #endregion

        #region Export Methods
        internal void ExportToExcel(string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model cannot be exported.");
                    return;
                }
                
                //prepare for excel export
                //save cntk model in document folder
                var networkPath = filepath + ".model";
                //save cntk model in document folder
                ExportToCNTK(networkPath);

                //Load ML configuration file
                var modelMLPath = Project.GetMLConfigPath(Settings, Name);
                //
                var resultTrain = Project.EvaluateModel(modelMLPath, true, false, true, false, ProcessDevice.Default);
                var resultValidation = Project.EvaluateModel(modelMLPath,true, false, false, false, ProcessDevice.Default);
                //transform data
                var trainData = TransformData(resultTrain, false);
                var validData = TransformData(resultTrain, false);
                
                ANNdotNET.Lib.Export.ExportToExcel.Export(trainData, validData, filepath, "ANNdotNETEval({0}:{1}, \"" + networkPath + "\")", false);


            }
            catch (Exception)
            {

                throw;
            }
        }
        internal void ExportToCSV(string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model result cannot be exported.");
                    return;
                }

                //Load ML configuration file
                var modelPath = Project.GetMLConfigPath(Settings, Name);
                //
                var result = Project.EvaluateModel(modelPath, true, true, false, false, ProcessDevice.Default);
                var strLines = TransformData(result, true);

                //store content to file
                File.WriteAllLines(filepath, strLines.Select(x=>string.Join(";",x)));
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal void ExportToCNTK(string filepath)
        {

            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model cannot be exported.");
                    return;
                }
                //save cntk model in document folder
                var bestModelFullPath = $"{Project.GetMLConfigFolder(Settings, Name)}\\{TrainingParameters.LastBestModel}";
                Project.SaveCNTKModel(filepath, bestModelFullPath);
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal void ExportToONNX(string filepath)
        {
            return;
            if (string.IsNullOrEmpty(TrainingParameters.LastBestModel) || !File.Exists(TrainingParameters.LastBestModel))
            {
                MessageBox.Show("No trained model exist. The mode cannot be exported.");
                return;
            }
            //save cntk model in document folder
            var modelPath = filepath + ".model";

            Project.SaveCNTKModel(modelPath, TrainingParameters.LastBestModel);
        }
        /// <summary>
        /// Prepares the evaluated data to export into Excel and CSV files
        /// </summary>
        /// <param name="result">Evaluation results dictionary sets</param>
        /// <param name="includePrediction">Excluded or included prediction column in the data</param>
        /// <returns></returns>
        private static List<List<string>> TransformData(
            (Dictionary<string, List<List<float>>> featuresDict, 
            Dictionary<string, List<List<float>>> actual, 
            Dictionary<string, List<List<float>>> predicted, List<string> ouptutClasses) 
            result, bool includePrediction)
        {
            var strData = new List<List<string>>();
            List<string> header = new List<string>();

            //create header
            foreach (var dic in result.featuresDict)
            {
                var fGroup = dic.Value.First();
                for (int i = 0; i < fGroup.Count; i++)
                {
                    header.Add($"{dic.Key}{i + 1}");
                }
            }
            //add actual to header
            foreach (var dic in result.actual)
            {
                var aGroup = dic.Value.First();
                for (int i = 0; i < aGroup.Count; i++)
                {
                    if(aGroup.Count==1)
                        header.Add($"{dic.Key}");
                    else
                        header.Add($"{dic.Key}{i + 1}");
                }
            }
            //add predicted names to header
            if(includePrediction)
            {
                foreach (var dic in result.predicted)
                {
                    var pGroup = dic.Value.First();
                    for (int i = 0; i < pGroup.Count; i++)
                    {
                        if (pGroup.Count == 1)
                            header.Add($"{dic.Key}");
                        else
                            header.Add($"{dic.Key}{i + 1}");
                    }
                }
            }
            
            //make a header
            strData.Add(header);

            //add data
            var rowCount = result.featuresDict.ElementAt(0).Value.Count();
            for (int i = 0; i < rowCount; i++)
            {
                //prepare row
                var dataRow = new List<string>();

                //get feature row
                var featureCount = result.featuresDict.Count;
                for (int j = 0; j < featureCount; j++)
                {
                    var v = result.featuresDict.ElementAt(j).Value[i];
                    dataRow.AddRange(v.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }
                //get actual row
                var actualCount = result.actual.Count;
                for (int j = 0; j < actualCount; j++)
                {
                    var v = result.actual.ElementAt(j).Value[i];
                    dataRow.AddRange(v.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }

                //get predicted row
                if(includePrediction)
                {
                    var predictedCount = result.predicted.Count;
                    for (int j = 0; j < predictedCount; j++)
                    {
                        var v = result.predicted.ElementAt(j).Value[i];
                        dataRow.AddRange(v.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                }
             
                //add full row to collection
                strData.Add(dataRow);
            }

            return strData;
        }

        public Dictionary<string, List<object>> EvaluateResults(List<List<float>> ActualT, List<List<float>> PredictedT, 
            List<List<float>> ActualV, List<List<float>> PredictedV)
        {
            //var ActualT = new List<List<float>>();
            //var PredictedT = new List<List<float>>();
            //var ActualV = new List<List<float>>();
            //var PredictedV = new List<List<float>>();

            var dic = new Dictionary<string, List<object>>();
            //get output for training data set
            List<double> actualT = new List<double>();
            List<double> predictedT = new List<double>();
            List<double> actualV = new List<double>();
            List<double> predictedV = new List<double>();
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

            //
            if (ActualV != null && ActualV.Count > 0)
            {
                

                //category output
                for (int i = 0; i < ActualV.Count; i++)
                {
                    float act = 0;
                    float pred = 0;
                    //category output
                    if (ActualV[i].Count > 2)
                    {
                        act = ActualV[i].IndexOf(ActualV[i].Max());
                        pred = PredictedV[i].IndexOf(PredictedV[i].Max());
                    }
                    else if (ActualV[i].Count == 2)
                    {
                        act = ActualV[i].IndexOf(ActualT[i].Max());
                        pred = (1.0f - PredictedV[i][0]);
                    }
                    else
                    {
                        act = ActualV[i].First();
                        pred = PredictedV[i].First();
                    }


                    actualV.Add(act);
                    predictedV.Add(pred);
                }
            }
                //
                //if (evalM.ModelOutputDim > 1)
                //    dic.Add("Classes", evalM.Classes.ToList<object>());


                if(actualT!=null)
                {
                    //add data sets
                    dic.Add("obs_train", actualT.Select(x => (object)x).ToList<object>());
                    dic.Add("prd_train", predictedT.Select(x => (object)x).ToList<object>());
                }

                //add test dataset
                if (actualV != null)
                {
                    dic.Add("obs_test", actualV.Select(x => (object)x).ToList<object>());
                    dic.Add("prd_test", predictedV.Select(x => (object)x).ToList<object>());
                }

                return dic;

        }
        #endregion

    }
}
