//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////


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
using ANNdotNET.Core;
using ANNdotNET.Lib;
using GPdotNet.MathStuff;
using NNetwork.Core.Common;
using ZedGraph;
namespace anndotnet.wnd.Models
{

    /// <summary>
    /// Main VieModel for Modeling. Contains all information needed for GP running.
    /// </summary>
    public class MLConfigController : BaseModel
    {
        #region Fields and Constructors
        ProcessDevice m_device = ProcessDevice.Default;
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
                        bool isNameAllowedToChang = true;
                        //filenames on disk are not case sensitive 
                        if (!string.IsNullOrEmpty(m_Name) && !m_Name.Equals(value, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(m_Name))
                            {
                                //change model file
                                var filePathold = Project.GetMLConfigPath(Settings, m_Name);
                                var filePathnew = Project.GetMLConfigPath(Settings, value);

                                //model folder path
                                var folderPathold = Project.GetMLConfigFolder(Settings, m_Name);
                                var folderPathnew = Project.GetMLConfigFolder(Settings, value);

                                //change model folder

                                if (!FileInUse(filePathold))
                                {
                                    System.IO.Directory.Move(folderPathold, folderPathnew);
                                    System.IO.File.Move(filePathold, filePathnew);


                                }
                                else
                                {
                                    isNameAllowedToChang = false;
                                }


                            }
                        }
                        //change property
                        var temPane = m_Name;
                        if (isNameAllowedToChang)
                        {
                            m_Name = value;
                            RaisePropertyChangedEvent("Name");
                            //in case of renaming 
                            if (!string.IsNullOrEmpty(temPane) && !string.IsNullOrEmpty(value))
                                updateMLConfigNameInProject(temPane, value);
                        }



                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }
            }
        }
        bool m_IsTrainRunning;
        public bool IsTrainRunning
        {
            get
            {
                return m_IsTrainRunning;
            }
            set
            {
                if(value != m_IsTrainRunning)
                {
                    m_IsTrainRunning = value;
                    RaisePropertyChangedEvent("IsTrainRunning");
                    RaisePropertyChangedEvent("IsnotTrainRunning");
                }
            }
        }
        public bool IsnotTrainRunning
        {
            get
            {
                return !m_IsTrainRunning;
            }
        }
        public Tuple<bool, bool, bool> DataSetsDefined { get; set; }
        private bool FileInUse(string path)
        {
            try
            {
                //if file is not lock then below statement will successfully executed otherwise it's goes to catch.
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                }
                return false;
            }
            catch
            {
                return true;

            }
        }

        /// <summary>
        /// Updates ann project after the mlconfig name has changed.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        private void updateMLConfigNameInProject(string oldName, string newName)
        {
            //load project information from file
            var prjPath1 = Path.Combine(Settings.ProjectFolder, Settings.ProjectFile);
            var dicData = Project.LoadProjectData(prjPath1);           
            var strValue = dicData["project"];
            var projectName = Project.GetParameterValue(strValue,"Name");
            
            var projectType = Project.GetParameterValue(strValue, "Type");
            if (string.IsNullOrEmpty(projectType))
                projectType = "Default";

            //
            var vsc = Project.GetParameterValue(strValue, "ValidationSetCount");
            var tsc = Project.GetParameterValue(strValue, "TestSetCount");
            if (string.IsNullOrEmpty(tsc))
                tsc = "0";
            var ps = Project.GetParameterValue(strValue, "PrecentigeSplit");
            //
            var lstConfigs = Project.GetMLConfigs(strValue);
            for (int i=0; i< lstConfigs.Count; i++)
            {
                if (lstConfigs[i] == oldName)
                {
                    lstConfigs[i] = newName;
                    break;
                }
            }
            //update project
            var strMlconfigs = string.Join(";",lstConfigs);
            var strProject = $"|Name:{projectName} |Type:{projectType}  |ValidationSetCount:{vsc}  |TestSetCount:{vsc} |PrecentigeSplit:{ps} |MLConfigs:{strMlconfigs} |Info:ProjectInfo.rtf";
            dicData["project"] = strProject;
            //add keyword to dicvalues
            for (int i = 0; i < dicData.Count(); i++)
            {
                var itm = dicData.ElementAt(i);
                dicData[itm.Key] = $"{itm.Key}:{itm.Value}";
            }

            Project.UpdateProject(dicData, prjPath1);

        }

        public ProjectSettings Settings { get; set; }

        //input layer
        public ObservableCollection<IOLayer> InLayer { get; set; }

        public ObservableCollection<IOLayer> OutLayer { get; set; }

        public ObservableCollection<NNLayer> Network { get; set; }
        public int SelectedIndex { get; set; }

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

        public bool IsMetadataPresent { get; private set; }
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
        public bool IsRunning { get { return "Images/model.png" != IconUri; } set { RaisePropertyChangedEvent("IsRunning"); } }

        //dammy property for treeView templates
        public ObservableCollection<MLConfigController> Models { get; set; }

        public bool Deleted { get; private set; }
        public Action<int, double, double, double, double> UpdateTrainingtGraphs { get; internal set; }
        #endregion

        internal bool Init()
        {
            try
            {
                var mlConfigPath = Project.GetMLConfigPath(Settings, Name);
                var modValues = Project.LoadMLConfig(mlConfigPath);
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

                //which datasets is defined
                var tp = Project.GetDataSetAviability(mlConfigPath);
                DataSetsDefined = new Tuple<bool, bool, bool>(tp.training, tp.validation, tp.test);
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
        /// <summary>
        /// Check if Validation dataset defined. Loads the mlconifg file and search for validation path, then check if the file exist 
        /// </summary>
        /// <returns>tre if the file exists</returns>
        internal bool IsValidationSetDefined()
        {
            
            var pp = Project.GetMLConfigPath(Settings, Name, "Validation");
            if (string.IsNullOrEmpty(pp) || pp == " ")
                return false;
            else
            {
                var fi = new FileInfo(pp);
                if (fi.Exists)
                    return true;
                else
                    return false;
            }
            
        }

        internal void Save()
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

        internal void Delete()
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

        public async Task<ModelEvaluation> EvaluateModel()
        {
            try
            {
                //change application in run mode
                IconUri = "Images/runningmodel.png";
                RaisePropertyChangedEvent("IsRunning");

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
                   await Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                          new Action(
                     ()=>MainWindow.SetCursor(false)
                       ));

                    
                    return mEval;
                }
                
                
                //get model full path
                var modelMLPath = Project.GetMLConfigPath(Settings, Name);
                //check if file exists
                var fi = new FileInfo(modelMLPath);
                if (!fi.Exists)
                    return mEval;
                //evaluate model against training data 
                var task1 = await Task.Run(()=> Project.EvaluateMLConfig(modelMLPath, DataSetType.Training, EvaluationType.ResultExtended, ProcessDevice.Default));
                var resultTrain = task1;

                //evaluate model against validation data
                var task2 = await Task.Run(() => Project.EvaluateMLConfig(modelMLPath, DataSetType.Validation, EvaluationType.ResultExtended, ProcessDevice.Default));
                var resultValidation = task2;

                if (resultTrain.Actual == null && resultTrain.Actual.Count <= 0)
                    return mEval;

                ////prepare evaluation result              
                for (int i = 0; i < resultTrain.Actual.Count(); i++)
                mEval.TrainingValue.Add(new PointPair(i + 1, resultTrain.Actual[i]));

                for (int i = 0; i < resultTrain.Predicted.Count(); i++)
                    mEval.ModelValueTraining.Add(new PointPair(i + 1, resultTrain.Predicted[i]));

                ////no validation set defined
                if (resultValidation.Actual != null && resultValidation.Actual.Count >0)
                {
                    
                    for (int i = 0; i < resultValidation.Actual.Count(); i++)
                        mEval.ValidationValue.Add(new PointPair(i + 1, resultValidation.Actual[i]));

                    
                    for (int i = 0; i < resultValidation.Predicted.Count(); i++)
                        mEval.ModelValueValidation.Add(new PointPair(i + 1, resultValidation.Predicted[i]));
                }

                ////
                mEval.Classes = resultTrain.OutputClasses;
                mEval.ModelOutputDim = resultTrain.OutputClasses == null ? 1 : resultTrain.OutputClasses.Count;

                //training performance result
                mpt = MLEvaluator.CalculatePerformance(resultTrain, "Training set");
                //validation performance result
                mpv = MLEvaluator.CalculatePerformance(resultValidation, "Validation set");

                
                mEval.TrainPerformance = mpt;
                if (mEval.Classes != null)
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
            finally
            {
                //change application in normal mode
                IconUri = "Images/model.png";
                RaisePropertyChangedEvent("IsRunning");
            }
        }

        internal async void RunTraining(CancellationToken token)
        {
            try
            {
                IsTrainRunning = true;

                //check if the model parameters are valid for training
                isModelParametersValid();

                //first save model
                Save();

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
                    var mlconfigPath = Project.GetMLConfigPath(Settings, Name);

                    progressStartTraining(trainingProgress);
                    //
                    var res = await Task.Run<TrainResult>(() => Project.TrainModel(mlconfigPath, token, trainingProgress, m_device));

                    //save best trained model
                    TrainingParameters.LastBestModel = Project.ReplaceBestModel(TrainingParameters, mlconfigPath, res.BestModelFile);
                    //once the training process completes inform the GUI about it
                    var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                    //send note to GUI the training process is completed 
                    IconUri = "Images/model.png";
                    RaisePropertyChangedEvent("IsRunning");
                    appCnt.TrainingCompleated(res);

                    //save the mlconfig file after training process is over
                    Save();
                    IsTrainRunning = false;
                }
                else
                {
                    //once the training process completes inform the GUI about it
                    var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                    //send note to GUI the training process is completed 
                    IconUri = "Images/model.png";
                    RaisePropertyChangedEvent("IsRunning");
                    appCnt.TrainingCompleated(new TrainResult() { ProcessState= ProcessState.Compleated, Iteration= TrainingParameters.Epochs });
                    IsTrainRunning = false;
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
                IsTrainRunning = false;
            }
           
        }

        /// <summary>
        /// Generates GraphViz string from mlconfig object
        /// </summary>
        /// <returns></returns>
        internal string GenerateNetworkGraph()
        {
            var configPath = Project.GetMLConfigPath(Settings, Name);
            var retVal = Project.GenerateNetworkGraph(configPath);
            return retVal;
        }

        private void progressStartTraining(Action<ProgressData> trainingProgress)
        {
            ProgressData progress = new ProgressData();
            if(TrainingProgress.MBLossValue.Count>0)
            {
                progress.EpochCurrent = (int)TrainingProgress.MBLossValue.Last().X;
                progress.EpochTotal = 0;
                progress.EvaluationFunName = "";
                progress.MinibatchAverageEval = TrainingProgress.MBEvaluationValue.Last().Y;
                progress.MinibatchAverageLoss = TrainingProgress.MBLossValue.Last().Y;
                progress.TrainEval = TrainingProgress.TrainEvalValue.Last().Y;
                progress.ValidationEval = TrainingProgress.ValidationEvalValue.Last().Y;
            }
            else
            {
                progress.EpochCurrent = 0;
                progress.EpochTotal = 0;
                progress.EvaluationFunName = "";
                progress.MinibatchAverageEval = 0;
                progress.MinibatchAverageLoss = 0;
                progress.TrainEval = 0;
                progress.ValidationEval = 0;
            }
            
            if (trainingProgress!=null)
                trainingProgress(progress);
        }

        
        private void isModelParametersValid()
        {
            if (Network[0].Type == LayerType.Custom)
                return;
            //the last layer in the network should be output
            if (Network.Last().Param1.ToString() != OutLayer.First().Dimension)
                throw new Exception("The output dimension of the last layer in the network must be same as label dimension!");
            if(Network.Where(x=>x.Param1<=0 && (x.Type!= LayerType.Normalization && x.Type != LayerType.Drop)).Count() > 0)
                throw new Exception("Layer cannot be defined with zero output dimension.!");
            if (Network.Where(x => x.Param2 <= 0 && (x.Type== LayerType.LSTM || x.Name.StartsWith("CudaStacked"))).Count() > 0)
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
                            float lossMB = (float)double.Parse(row[1], CultureInfo.InvariantCulture);
                            float evalMB = (float)double.Parse(row[2], CultureInfo.InvariantCulture);
                            float trainEval = (float)double.Parse(row[3], CultureInfo.InvariantCulture);
                            float valiEval = (float)double.Parse(row[4], CultureInfo.InvariantCulture);
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

        private void trainingProgress(ProgressData progress)
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
                        else
                            TrainingProgress.TrainEvalValue.Add(new PointPair(progress.EpochCurrent, double.NaN));

                        if (!double.IsNaN(progress.ValidationEval) && !double.IsInfinity(progress.ValidationEval))
                            TrainingProgress.ValidationEvalValue.Add(new PointPair(progress.EpochCurrent, progress.ValidationEval));
                        else
                            TrainingProgress.ValidationEvalValue.Add(new PointPair(progress.EpochCurrent, double.NaN));


                        //update Graphs
                        if (UpdateTrainingtGraphs!=null)
                            UpdateTrainingtGraphs(progress.EpochCurrent, progress.MinibatchAverageLoss, progress.MinibatchAverageEval, progress.TrainEval, progress.ValidationEval);
                        //set status message
                        var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                        if(progress.EpochCurrent==0)
                            appCnt.StatusMessage = $"Training process is about to start. Please wait!";
                        else
                            appCnt.StatusMessage = $"Iteration:{progress.EpochCurrent} of {progress.EpochTotal} processed!";
                    }

                ));
            }
            catch (Exception ex)
            {
                var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                appCnt.ReportException(ex);
            }
        }

        #region Helper Methods for model saving/Loading

        private string saveNetworkParameters(ObservableCollection<NNLayer> network)
        {
            return Project.NetworkParametersToString(network.ToList());
        }

        private string saveLearningParameters(LearningParameters lp)
        {
            return lp.ToString();
        }

        private string saveTrainingParameters(TrainingParameters tp)
        {
            return tp.ToString();
        
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
            itm.Representation ="Shape";
            var inDim = fet[1].Split(MLFactory.m_ValueSpearator).Take(3).ToList();
            itm.Dimension =$"{string.Join(";",inDim)}";

            return itm;
        }

        private void initializeOutputLayer(ObservableCollection<IOLayer> outLayer, string strFeatures)
        {
            //
            var features = strFeatures.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; features.Length > i; i++)
            {
                var layer = ParseLayer(features[i]);
                //
                layer.Representation = "Shape";
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

        internal async Task<bool> ExportToExcel(string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model cannot be exported.");
                    return true;
                }
                if (TestData==null)
                {
                    MessageBox.Show("Export is not possible. No metadata is defined in the mlconfig file.");
                    return true;
                }
                //prepare for excel export
                //save cntk model in document folder
                var networkPath = filepath + ".model";
                //save cntk model in document folder
                await ExportToCNTK(networkPath);

                //Load ML configuration file
                var modelMLPath = Project.GetMLConfigPath(Settings, Name);

                EvaluationResult resultV = null;
                //EvaluationResult resultTe = null;
                var resultT = await Project.EvaluateMLConfig(modelMLPath, DataSetType.Training, EvaluationType.FeaturesOnly, ProcessDevice.Default);

                if (DataSetsDefined.Item2)
                    resultV = await Project.EvaluateMLConfig(modelMLPath, DataSetType.Validation, EvaluationType.FeaturesOnly, ProcessDevice.Default);

                //removed this since 1.3
                //if (DataSetsDefined.Item3)
                //    resultTe = await Project.EvaluateMLConfig(modelMLPath, DataSetType.Testing, EvaluationType.FeaturesOnly, ProcessDevice.Default);

                //prepare headers
                var header = resultT.Header;

                List<List<string>> trainData = prepareToPersist(resultT);
                List<List<string>> validData = prepareToPersist(resultV);
                //List<List<string>> testData = prepareToPersist(resultTe);

                ANNdotNET.Lib.Export.ExportToExcel.Export(trainData, validData, null, filepath, "ANNdotNETEval({0}:{1}, \"" + networkPath + "\")", false, resultT.OutputClasses);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<bool> ExportToCSV(string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model result cannot be exported.");
                    return false;
                }
                //Load ML configuration file
                var mlConfigPath = Project.GetMLConfigPath(Settings, Name);
                await MLExport.ExportToCSV(mlConfigPath, MLFactory.GetDevice(ProcessDevice.Default), filepath);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<bool> ExportToCNTK(string filepath)
        {

            try
            {
                if (string.IsNullOrEmpty(TrainingParameters.LastBestModel))
                {
                    MessageBox.Show("No trained model exist. The model cannot be exported.");
                    return false;
                }
                //save cntk model in document folder
                var bestModelFullPath = $"{Project.GetMLConfigFolder(Settings, Name)}\\{TrainingParameters.LastBestModel}";
                await Task.Run(()=> Project.SaveCNTKModel(filepath, bestModelFullPath));
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<bool> ExportToONNX(string filepath)
        {
            await Task.Delay(1);
            return true;
            //Not available in C#
            //if (string.IsNullOrEmpty(TrainingParameters.LastBestModel) || !File.Exists(TrainingParameters.LastBestModel))
            //{
            //    MessageBox.Show("No trained model exist. The mode cannot be exported.");
            //    return true;
            //}
            ////save cntk model in document folder
            //var modelPath = filepath + ".model";

            //Project.SaveCNTKModel(modelPath, TrainingParameters.LastBestModel);
        }

        /// <summary>
        /// Prepare evaluation result for persisting into Excel 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<List<string>> prepareToPersist(EvaluationResult result)
        {
            List<List<string>> strList = new List<List<string>>();
            if (result == null)
                return null;
            //first add header
            strList.Add(result.Header);
            if (result.DataSet == null)
                return null;
            var cc = result.DataSet.Values.First().Count();
            for (int i = 0; i < cc; i++)
            {
                var strLine = new List<string>();
                for (int j = 0; j < result.DataSet.Values.Count(); j++)
                {
                    var value = result.DataSet.Values.ElementAt(j)[i];
                    strLine.AddRange(value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }
                strList.Add(strLine);
            }

            return strList;
        }
        #endregion

    }
}
