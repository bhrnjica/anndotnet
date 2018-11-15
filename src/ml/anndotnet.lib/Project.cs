//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
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
using ANNdotNET.Core;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ANNdotNET.Lib.Ext;
using DataProcessing.Core;
using System.Threading.Tasks;

namespace ANNdotNET.Lib
{
    /// <summary>
    /// Main class of ANNdotNET tool holding information of the project. The ANNdotNET project is supposed to be
    /// starting point for developing ML solution. The project contains information about raw data set 
    /// which describes the problems. Project can contain more than one Machine Learning Configuration which can produce
    /// a neural Network model. 
    /// </summary>
    public class Project
    {
        #region Properties
        public string Name { get; private set; }

        public string InfoPath { get; private set; }

        public List<string> MLConfig { get; private set; }

        public DataDescriptor Descriptor { get; private set; }

        public ProjectSettings Settings { get; private set; }

        public Dictionary<string, string> LoadedMLConfig { get; private set; }

        public string LoadedModelName { get; private set; }
        #endregion 

        public void Load(string projPath)
        {
            try
            {
                var fi = new FileInfo(projPath);
                if (!fi.Exists)
                    throw new Exception("File not found!");

                //
                if (Settings == null)
                    Settings = new ProjectSettings();

                //load project tags
                var dicValues = MLFactory.LoadMLConfiguration(projPath);

                //parse project information
                var projectMetaData = dicValues["project"];
                parseProject(projectMetaData);
                //setup project folder where all files will be places
                Settings.ProjectFolder = fi.Directory.FullName;
                Settings.ProjectFile = fi.Name;

                //reset Descriptor
                Descriptor = new DataDescriptor();

                //parse data
                var dataValues = dicValues["parser"];
                parseParser(dataValues);

                //parse data
                var metaDataValues = dicValues["data"];
                parseMetaData(metaDataValues);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void SaveCurrentMLConfig(string mlconfigName)
        {
            if (mlconfigName != LoadedModelName)
            {
                throw new Exception("Only loaded mlconfig can be saved.");
            }

            saveMLConfig(LoadedMLConfig);
        }


        private bool saveMLConfig(Dictionary<string, string> loadedMLConfig)
        {
            try
            {
                var mlconfigPath = Path.Combine(Settings.ProjectFolder, LoadedModelName);
                return MLFactory.SaveMLConfiguration(mlconfigPath, LoadedMLConfig);
            }
            catch (Exception)
            {

                throw;
            }

        }


        public void OpenMLConfig(string mlconfigName)
        {
            var modelPath = Path.Combine(Settings.ProjectFolder, mlconfigName);
            LoadedMLConfig = MLFactory.LoadMLConfiguration(modelPath);
            LoadedModelName = mlconfigName;
        }


        public void RunModel(string mlconfigName, CancellationToken token, TrainingProgress trainingProgress, ProcessDevice pdevice)
        {
            Project.TrainModel(mlconfigName, token, trainingProgress, pdevice);
        }

        private string updateProjectTag(string row)
        {
            var strLine = "project:";
            var tags = row.Split(Descriptor.Parser.ColumnSeparator, StringSplitOptions.RemoveEmptyEntries);
            //project Name
            strLine += "|Name:" + Name;

            //ValidationSetCount
            strLine += " |ValidationSetCount:" + Settings.ValidationSetCount.ToString();

            //TestSetCount
            strLine += " |TestSetCount:" + Settings.TestSetCount.ToString();

            //PrecentigeSplit
            strLine += " |PrecentigeSplit:" + Settings.PrecentigeSplit.ToString();

            //mlconfig
            var mds = string.Join(";", MLConfig);
            strLine += " |MLConfigs:" + mds + " ";

            //info
            strLine += " |Info:" + InfoPath;

            return strLine;

        }

        private void parseMetaData(string metaDataValues)
        {
            //parse data
            var mdata = metaDataValues.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

            //file path for raw data
            var filePath = MLFactory.GetParameterValue(mdata, "RawData");
            Descriptor.DataPath = filePath;

            if (string.IsNullOrEmpty(Descriptor.DataPath))
            {
                Descriptor.Columns = null;
                return;
            }
            //parse columns
            Descriptor.Columns = Project.ParseRawDataSet(metaDataValues);
        }

        private void parseParser(string dataValues)
        {
            //create parser object
            DataParser dp = Project.CreateDataParser(dataValues);
            //assign to the property
            Descriptor.Parser = dp;
        }

        private void parseProject(string projectMetaData)
        {

            Name = GetProjectName(projectMetaData);

            Settings = CreateProjectSettings(projectMetaData);


            //parse models
            MLConfig = GetMLConfigs(projectMetaData);

            //project info
            //parse feature variables
            var projectValues = projectMetaData.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            var info = MLFactory.GetParameterValue(projectValues, "Info");
            InfoPath = info;
        }


        #region Methods exposed to ANNTool

        /// <summary>
        /// Deletes all files and folder withing the path
        /// </summary>
        /// <param name="mlconfigFolder"></param>
        public static void DeleteAllFiles(string mlconfigFolder)
        {
            try
            {
                MLFactory.DeleteAllFiles(mlconfigFolder);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Saves the mlconfig in CNTK format.
        /// </summary>
        /// <param name="filePathToCopy"></param>
        /// <param name="newFilePath"></param>
        /// <returns></returns>
        public static bool SaveCNTKModel(string filePathToCopy, string newFilePath)
        {
            try
            {
                File.Copy(newFilePath, filePathToCopy);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Parses the mlconfig file and check which dataset (train, Validation, test) is defined.
        /// </summary>
        /// <param name="mlConfigPath"></param>
        /// <returns></returns>
        public static (bool training, bool validation, bool test) GetDataSetAviability(string mlConfigPath)
        {
            try
            {
                return MLFactory.GetDataSetAviability(mlConfigPath);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        /// <summary>
        /// Returns full path of project info file
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string GetProjectInfoPath(ProjectSettings settings)
        {
            string fileName = Path.Combine(settings.ProjectFolder, "ProjectInfo.rtf");
            return fileName;
        }

        /// <summary>
        /// Evaluate mlconfig stored in mlconfigPath for input row stored in vector array
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="vector"></param>
        /// <param name="pdevice"></param>
        /// <returns></returns>
        public static object Predict(string mlconfigPath, float[] vector, ProcessDevice pdevice)
        {
            //device definition
            DeviceDescriptor device = MLFactory.GetDevice(pdevice);

            return MLEvaluator.TestModel(mlconfigPath, vector, device);
        }

        public static async Task<EvaluationResult> EvaluateMLConfig(string mlconfigPath, DataSetType dsType, EvaluationType evType, ProcessDevice pdevice)
        {
            //device definition
            DeviceDescriptor device = MLFactory.GetDevice(pdevice);
            return await MLEvaluator.EvaluateMLConfig(mlconfigPath, device, dsType, evType);
        }



        /// <summary>
        /// Main methods for model training
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="token"></param>
        /// <param name="trainingProgress"></param>
        /// <param name="pdevice"></param>
        /// <returns></returns>
        public static TrainResult TrainModel(string mlconfigPath, CancellationToken token, TrainingProgress trainingProgress, ProcessDevice pdevice)
        {
            try
            {
                //device definition
                DeviceDescriptor device = MLFactory.GetDevice(pdevice);

                //LOad ML configuration file
                var dicMParameters = MLFactory.LoadMLConfiguration(mlconfigPath);
                //add path of model folder
                dicMParameters.Add("root", Project.GetMLConfigFolder(mlconfigPath));

                //prepare NN data 
                var retVal = MLFactory.PrepareNNData(dicMParameters, CustomNNModels.CustomModelCallEntryPoint, device);

                //create trainer 
                var tr = new MLTrainer(retVal.f.StreamConfigurations, retVal.f.InputVariables, retVal.f.OutputVariables);

                //setup model checkpoint
                string modelCheckPoint = null;
                if (dicMParameters.ContainsKey("configid"))
                {
                    modelCheckPoint = MLFactory.GetModelCheckPointPath(mlconfigPath, dicMParameters["configid"].Trim(' '));
                }

                //setup model checkpoint
                string historyPath = null;
                if (dicMParameters.ContainsKey("configid"))
                {
                    historyPath = MLFactory.GetTrainingHistoryPath(mlconfigPath, dicMParameters["configid"].Trim(' '));
                }

                //create trainer 
                var trainer = tr.CreateTrainer(retVal.nnModel, retVal.lrData, retVal.trData, modelCheckPoint, historyPath);

                //perform training
                var result = tr.Train(trainer, retVal.nnModel, retVal.trData, retVal.mbs, device, token, trainingProgress, modelCheckPoint, historyPath);

                return result;

            }
            catch (Exception)
            {

                throw;
            }


        }

        public static string ReplaceBestModel(TrainingParameters trainingParameters, string mlconfigPath, string bestModelFile)
        {
            try
            {
                return MLFactory.ReplaceBestModel(trainingParameters, mlconfigPath, bestModelFile);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<VariableDescriptor> ParseRawDataSet(string metaDataValues)
        {
           return MLFactory.ParseRawDataSet(metaDataValues);
        }

        /// <summary>
        /// Creates a new ANNdotNET Model configuration file.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static bool NewMLConfigFile(Project project, string mlconfigName)
        {
            try
            {
                var strFeatures = $"features:";

                //first create features and labels based on data descriptor
                var numDim = project.Descriptor.Columns.Where(x => x.Kind == DataKind.Feature && x.Type == MLDataType.Numeric).Count();
                if (numDim > 0)
                    strFeatures += $"{ProjectSettings.m_NumFeaturesGroupName} {numDim} 0\t";
                //create category features
                foreach (var c in project.Descriptor.Columns.Where(x => x.Kind == DataKind.Feature && x.Type == MLDataType.Category))
                {
                    strFeatures += $"|{c.Name} {c.Classes.Length} 0\t";
                }
                //create label
                var strLabel = $"labels:";
                foreach (var c in project.Descriptor.Columns.Where(x => x.Kind == DataKind.Label && x.Type == MLDataType.Category))
                {
                    strLabel += $"|{c.Name} {c.Classes.Length} 0\t";
                }
                //
                foreach (var c in project.Descriptor.Columns.Where(x => x.Kind == DataKind.Label && x.Type == MLDataType.Numeric))
                {
                    strLabel += $"|{c.Name} {1} 0\t";
                }

                //only one Label is supported 
                var countLabel = project.Descriptor.Columns.Where(x => x.Kind == DataKind.Label && x.Type != MLDataType.None).Count();
                if (countLabel == 0)
                    throw new Exception("The mlconfig cannot be created, no label is defined!");
                if (countLabel > 1)
                    throw new Exception("The mlconfig cannot be created, more than one label is defined!");
                //create rest of the ML config file.
                //later the user will be able to setup model training and other params
                var strMlCOnfig = new List<string>();

                //first we have to generate unique model identified
                var modeId = $"configid:{Guid.NewGuid()}";
                strMlCOnfig.Add(modeId);

                //then generate metadat
                var strMetaData = $"metadata:{project.Descriptor.ToMetadataString(true)}";
                strMlCOnfig.Add(strMetaData);

                //add features and labels previously generated
                strMlCOnfig.Add(strFeatures);
                strMlCOnfig.Add(strLabel);

                //empty network 
                strMlCOnfig.Add("network:|Layer:Dense 1 0 0 None 0 0");
                strMlCOnfig.Add("learning:|Type:SGDLearner |LRate:0.01 |Momentum:1 |Loss:SquaredError |Eval:SquaredError");
                strMlCOnfig.Add("training:|Type:default |BatchSize:50 |Epochs:100 |Normalization:0 |RandomizeBatch:0 |SaveWhileTraining:1 |ProgressFrequency:100  |ContinueTraining:0 |TrainedModel: ");
                var strDicts = GetDefaultMLConfigPaths(project.Settings, mlconfigName);

                //add paths for ml config
                foreach (var d in strDicts)
                    strMlCOnfig.Add($"{d.Key}:{d.Value}");
                //
                var mlConfigPath = Project.GetMLConfigPath(project.Settings, mlconfigName);
                File.WriteAllLines(mlConfigPath, strMlCOnfig);
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }

        /// <summary>
        /// Returns full paths of the mlconfig components specified by its name in string format ready to be store in file
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMLConfigPaths(ProjectSettings settings, string mlconfigName)
        {
            try
            {
                var path = Project.GetMLConfigPath(settings, mlconfigName);
                var dic = MLFactory.LoadMLConfiguration(path);
                var p1 = Project.GetParameterValue(dic["paths"], "Training");
                var p2 = Project.GetParameterValue(dic["paths"], "Validation");
                var p3 = Project.GetParameterValue(dic["paths"], "Test");
                var p4 = Project.GetParameterValue(dic["paths"], "TempModels");
                var p5 = Project.GetParameterValue(dic["paths"], "Models");
                var p6 = Project.GetParameterValue(dic["paths"], "Result");
                var p7 = Project.GetParameterValue(dic["paths"], "Logs");

                Dictionary<string, string> strMlCOnfig = new Dictionary<string, string>();
                //
                var strPaths = $"|Training:{p1} " +
                               $"|Validation:{p2} " +
                               $"|Test:{p3} " +
                               $"|TempModels:{p4} |Models:{p5} " +
                               $"|Result:{p6} |Logs:{p7} ";

                //
                strMlCOnfig.Add("paths", strPaths);
                return strMlCOnfig;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Returns full path of specified parameter name
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static string GetMLConfigPath(ProjectSettings settings, string mlconfigName, string pathName)
        {
            try
            {
                var path = Project.GetMLConfigPath(settings, mlconfigName);
                var folder = Project.GetMLConfigFolder(path);
                var dic = MLFactory.LoadMLConfiguration(path);
                var file = Project.GetParameterValue(dic["paths"], pathName);
                var strPath = $"{folder}\\{file}";
                //
                return strPath;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        /// <summary>
        /// Returns full paths of the model components specified by its name
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDefaultMLConfigPaths(ProjectSettings settings, string mlconfigName)
        {
            try
            {
                Dictionary<string, string> strMlCOnfig = new Dictionary<string, string>();

                var validPath = settings.ValidationSetCount > 0 ? MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Validation) : "";
                var testPath = settings.TestSetCount > 0 ? MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Testing) : "";
                //
                var strPaths = $"|Training:{MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Training)} " +
                               $"|Validation:{validPath} " +
                               $"|Test:{testPath} " +
                               $"|TempModels:{MLFactory.m_MLTempModelFolder} |Models:{MLFactory.m_MLModelFolder} " +
                               $"|Result:{mlconfigName}_result.csv |Logs:{MLFactory.m_MLLogFolder} ";

                //
                strMlCOnfig.Add("paths", strPaths);
                return strMlCOnfig;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Returns dictionary containing ml configuration data 
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadMLConfig(string mlconfigPath)
        {
            if (File.Exists(mlconfigPath))
            {
                var mlconfigValues = MLFactory.LoadMLConfiguration(mlconfigPath);
                return mlconfigValues;
            }
            return null;
        }

        /// <summary>
        /// Create dictionary of data for empty project with specified name.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> EmptyProject(string projectName)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("project", $"|Name:{projectName} |ValidationSetCount:0 |TestSetCount:0 |PrecentigeSplit:0 |RandomizeData:0 |MLConfigs:  |Info:");
            dic.Add("data", $"|RawData: ");
            dic.Add("parser", $"parser:|RowSeparator:rn | ColumnSeparator:, ; |Header:0 |SkipLines:0");
            return dic;
        }

        /// <summary>
        /// Create new project and store it on disk
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static bool NewProjectFile(string projectName, string projectPath)
        {
            Dictionary<string, string> dic = EmptyProject(projectName);
            MLFactory.SaveMLConfiguration(projectPath, dic);
            var path = Path.GetDirectoryName(projectPath) + $"\\{Path.GetFileNameWithoutExtension(projectName)}";
            System.IO.Directory.CreateDirectory(path);
            return true;
        }

        /// <summary>
        /// return formated string from the network layer list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string NetworkParametersToString(List<NNLayer> network)
        {
            try
            {
                var strValue = "";
                foreach (var l in network)
                {
                    var stab = l.SelfStabilization == true ? 1 : 0;
                    var peep = l.Peephole == true ? 1 : 0;
                    strValue += $"|Layer:{l.Type} {l.HDimension} {l.CDimension} {l.Value} {l.Activation} {stab} {peep} ";
                }

                return strValue;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Updates changes to the project
        /// </summary>
        /// <param name="dicValues"></param>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static bool UpdateProject(Dictionary<string, string> dicValues, string projectPath)
        {
            string[] rows = File.ReadAllLines(projectPath);
            //update project tag
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].StartsWith("project:"))
                {
                    var projLine = rows[i];
                    rows[i] = dicValues["project"];
                }
                if (rows[i].StartsWith("data:"))
                {
                    var projLine = rows[i];
                    rows[i] = dicValues["data"];
                }
                if (rows[i].StartsWith("parser:"))
                {
                    var projLine = rows[i];
                    rows[i] = dicValues["parser"];
                }
            }

            File.WriteAllLines(projectPath, rows);
            return true;
        }

        /// <summary>
        /// Loads the Project into Dictionary list of components
        /// </summary>
        /// <param name="projPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadProjectData(string projPath)
        {
            //load project tags
            var dicValues = MLFactory.LoadMLConfiguration(projPath);
            return dicValues;
        }

        /// <summary>
        /// Returns the project name from the string data
        /// </summary>
        /// <param name="projectMetaData"></param>
        /// <returns></returns>
        public static string GetProjectName(string projectMetaData)
        {
            //parse feature variables
            var projectValues = projectMetaData.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

            //parse project name
            var name = MLFactory.GetParameterValue(projectValues, "Name");
            if (string.IsNullOrEmpty(name))
                throw new Exception("Project file is corrupted!");
            return name;
        }

        /// <summary>
        /// Parses string find the parameter and return parameter value 
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetParameterValue(string strData, string name)
        {
            //parse feature variables
            var projectValues = strData.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);
            return MLFactory.GetParameterValue(projectValues, name);
        }

        /// <summary>
        /// Parses the array of string find the parameter and return parameter value 
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetParameterValue(string[] strData, string name)
        {
            return MLFactory.GetParameterValue(strData, name);
        }

        /// <summary>
        /// Parses the string and create list of defined mlconfigs
        /// </summary>
        /// <param name="projectMetaData"></param>
        /// <returns></returns>
        public static List<string> GetMLConfigs(string projectMetaData)
        {
            //parse feature variables
            var dataValues = projectMetaData.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

            var mlconfigs = new List<string>();
            //parse mlconfigs
            var strData = MLFactory.GetParameterValue(dataValues, "MLConfigs");
            if (string.IsNullOrEmpty(strData))
                return mlconfigs;
            else
            {
                var mod = strData.Split(MLFactory.m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries);
                mlconfigs = mod.ToList();
            }

            return mlconfigs;
        }

        /// <summary>
        /// Parses the string data and create ProjectSettings  object
        /// </summary>
        /// <param name="projectMetaData"></param>
        /// <returns></returns>
        public static ProjectSettings CreateProjectSettings(string projectMetaData)
        {
            //parse feature variables
            var dataValues = projectMetaData.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

            var settings = new ProjectSettings();
            //validation set count
            var vsCount = MLFactory.GetParameterValue(dataValues, "ValidationSetCount");
            if (string.IsNullOrEmpty(vsCount))
                settings.ValidationSetCount = 0;
            else
                settings.ValidationSetCount = int.Parse(vsCount);
            //test set count
            var tsCount = MLFactory.GetParameterValue(dataValues, "TestSetCount");
            if (string.IsNullOrEmpty(tsCount))
                settings.TestSetCount = 0;
            else
                settings.TestSetCount = int.Parse(tsCount);

            //is percentage used when split data sets
            var isPrecentige = MLFactory.GetParameterValue(dataValues, "PrecentigeSplit");
            if (string.IsNullOrEmpty(isPrecentige))
                settings.PrecentigeSplit = false;
            else
                settings.PrecentigeSplit = isPrecentige == "1" ? true : false;
            return settings;
        }

        /// <summary>
        /// Parses the string data and creates the DataParser object
        /// </summary>
        /// <param name="dataValues"></param>
        /// <returns></returns>
        public static DataParser CreateDataParser(string dataValues)
        {
            // create parser object
            DataParser dp = new DataParser();

            //parse data
            var parser = dataValues.Split(MLFactory.m_cntkSpearator, StringSplitOptions.RemoveEmptyEntries);

            //row separator
            var row = MLFactory.GetParameterValue(parser, "RowSeparator");
            if (string.IsNullOrEmpty(row))
                dp.RowSeparator = new List<string>() { Environment.NewLine }.ToArray();
            else
            {
                var ll = new List<string>();
                var separators = row.Split(MLFactory.m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries);
                foreach (var r in separators)
                {
                    if (r == "rn")
                        ll.Add(Environment.NewLine);
                    else
                        ll.Add(r);
                }
                //save to property
                dp.RowSeparator = ll.ToArray();
            }

            //column separator
            var col = MLFactory.GetParameterValue(parser, "ColumnSeparator");
            if (string.IsNullOrEmpty(col))
                dp.ColumnSeparator = new List<char>() { ' ', '\t', ';', }.ToArray();
            else if (col == ";")
                dp.ColumnSeparator = new List<char>() { ';' }.ToArray();
            else
            {
                var separators = col.Split(MLFactory.m_ValueSpearator, StringSplitOptions.RemoveEmptyEntries);
                dp.ColumnSeparator = separators.Select(x => x[0]).ToArray();
            }

            //header
            var header = MLFactory.GetParameterValue(parser, "Header");
            if (string.IsNullOrEmpty(header))
                dp.FirstRowHeader = false;
            else
                dp.FirstRowHeader = header == "1" ? true : false;

            //skip first several rows. Usually we should skip some description of the data which is placed at the very beginning of the file
            var skipLines = MLFactory.GetParameterValue(parser, "SkipLines");
            if (string.IsNullOrEmpty(skipLines))
                dp.SkipFirstLines = 0;
            else
                dp.FirstRowHeader = header == "1" ? true : false;

            //assign to the property
            return dp;
        }

        /// <summary>
        /// Parses the string data and extract the instance of Training Parameters
        /// </summary>
        /// <param name="strTrainingData"></param>
        /// <returns></returns>
        public static TrainingParameters CreateTrainingParameters(string strTrainingData)
        {
            try
            {
                TrainingParameters trData = MLFactory.CreateTrainingParameters(strTrainingData);
                return trData;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Parses the string data and extract the Learning parameters instance
        /// </summary>
        /// <param name="strLearningData"></param>
        /// <returns></returns>
        public static LearningParameters CreateLearningParameters(string strLearningData)
        {
            try
            {
                LearningParameters trData = MLFactory.CreateLearningParameters(strLearningData);
                return trData;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Parses the string data and extract the List of NN Layers
        /// </summary>
        /// <param name="strNetwork"></param>
        /// <returns></returns>
        public static List<NNLayer> CreateNetworkParameters(string strNetwork)
        {
            try
            {
                var nnParams = MLFactory.CreateNetworkParameters(strNetwork);
                return nnParams;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// save data to configuration file, in case the file exist updates the file. 
        /// </summary>
        /// <param name="mlconfigPath"></param>
        /// <param name="configValues"></param>
        /// <returns></returns>
        public static bool SaveConfigFile(string mlconfigPath, Dictionary<string, string> configValues)
        {
            try
            {
                return MLFactory.SaveMLConfiguration(mlconfigPath, configValues);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Returns the full path of  mlconfig
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static string GetMLConfigPath(ProjectSettings settings, string mlconfigName)
        {
            return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile),
                mlconfigName +/* MLFactory.m_MLConfigSufix*/MLFactory.m_MLConfigFileExt);
        }

        public static string GetMLConfigId(string mlconfigPath)
        {
            try
            {
                return MLFactory.GetMLConfigId(mlconfigPath);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// Return full path of the mlconfig folder by specifying full mlconfig path
        /// </summary>
        /// <param name="mlconfigFullPath"></param>
        /// <returns></returns>
        public static string GetMLConfigFolder(string mlconfigFullPath)
        {
            return MLFactory.GetMLConfigFolder(mlconfigFullPath);

        }

        /// <summary>
        /// Returns the full path of the mlconfig folder, by specifying settings and mlconfig name
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static string GetMLConfigFolder(ProjectSettings settings, string mlconfigName)
        {
            return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile), mlconfigName);
        }

        /// <summary>
        /// Return full path od the folder where training and validation dataset files are stored
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <returns></returns>
        public static string GetMLConfigDataFolder(ProjectSettings settings, string mlconfigName)
        {
            return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile), mlconfigName, MLFactory.m_MLDataFolder);
        }

        /// <summary>
        /// Returns default full path of ml dataset
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="mlconfigName"></param>
        /// <param name="isTrain"></param>
        /// <returns></returns>
        public static string GetDefaultMLDatasetPath(ProjectSettings settings, string mlconfigName, DataSetType dsType)
        {

            if (dsType == DataSetType.Training)
                return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile), mlconfigName,
                    MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Training));
            else if (dsType == DataSetType.Validation)
                return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile), mlconfigName,
                    MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Validation));
            else if (dsType == DataSetType.Testing)
                return Path.Combine(settings.ProjectFolder, Path.GetFileNameWithoutExtension(settings.ProjectFile), mlconfigName,
                    MLFactory.GetDefaultMLConfigDatSetPath((int)DataSetType.Testing));
            else
                throw new Exception("Data set is not recognized.");
        }

        public static string GetTrainingHistoryPath(string mlconfigPath, string configid)
        {
            return MLFactory.GetTrainingHistoryPath(mlconfigPath, configid);
        }
        #endregion
    }
}
