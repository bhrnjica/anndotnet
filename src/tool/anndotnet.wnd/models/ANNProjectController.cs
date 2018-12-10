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
using anndotnet.wnd.Pages;
using ANNdotNET.Core;
using ANNdotNET.Lib;
using DataProcessing.Core;
using DataProcessing.Wnd;
using DocumentFormat.OpenXml.Presentation;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
namespace anndotnet.wnd.Models
{
    

    public class ANNProjectController : BaseModel
    {

        public ANNProjectController(Action<BaseModel, bool> fun)
            : base(fun)
        {
            Models = new ObservableCollection<MLConfigController>();
        }
        string m_Name;

        public Project Project { get; internal set; }
        public ProjectSettings Settings { get; internal set; }
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
                        if (m_Name != null)
                        {
                            var filePathold = Settings.ProjectFolder + $"\\{m_Name}";
                            var filePathnew = Settings.ProjectFolder + $"\\{value}"; 
                            System.IO.Directory.Move(filePathold, filePathnew);
                            System.IO.File.Move(filePathold+".ann", filePathnew+ ".ann");
                            Settings.ProjectFile = filePathnew + ".ann";
                            //load existing models
                            foreach (var model in Models)
                            {
                                model.Settings = Settings;
                            }
                        }

                        m_Name = value;
                        RaisePropertyChangedEvent("Name");
                    }
                    catch (Exception ex)
                    {
                        
                        var cntCtrl = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                        if(cntCtrl!=null)
                        {
                            cntCtrl.ReportException(ex);        
                        }
                       
                    }


                }
                
            }
        }

        private ObservableCollection<MLConfigController> m_Models;
 
        public ObservableCollection<MLConfigController> Models
        {
            get
            {
                return m_Models;
            }
            set
            {
                m_Models = value;
            }
        }
 
        public ANNDataSet DataSet { get; internal set; }

        public new string IconUri { get => "Images/experiment.png"; }

        //initialize project controllers with project information
        internal async Task<bool> Initproject(string projectPath, ProjectType pType= ProjectType.Default)
        {
            try
            {

                var fi = new FileInfo(projectPath);
                if (!fi.Exists)
                    throw new Exception("Project File not found!");

                //load project information from file
                var dicData = Project.LoadProjectData(projectPath);

                //load project info
                Settings = Project.CreateProjectSettings(dicData["project"]);
                Settings.ProjectFolder = fi.Directory.FullName;
                Settings.ProjectFile = fi.Name;

                //
                Name = Project.GetParameterValue(dicData["project"], "Name");

                

                var prData = dicData["data"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var dataPath = Project.GetParameterValue(prData, "RawData");
                var filePath = "";
                if (!string.IsNullOrEmpty(dataPath))
                    filePath = Path.Combine(Settings.ProjectFolder, Name, dataPath);
                else
                    filePath = Path.Combine(Settings.ProjectFolder, Name,Name + "_rawdata.txt"); 

                //load rawdataset
                var parser = Project.CreateDataParser(dicData["parser"]);
                var result = await initRawData(filePath, parser);

                //create dataset
                var ds = new ANNDataSet();
                ds.InitMetaColumn(prData.Where(x => x.StartsWith("Column")).OrderBy(x => x));             
                ds.Data = result.data;
                ds.IsPrecentige = Settings.PercentigeSplit;
                ds.RowsToValidation = Settings.ValidationSetCount;
                ds.RowsToTest = Settings.TestSetCount;
                DataSet = ds;

                //load existing mlconfigs
                var models = Project.GetMLConfigs(dicData["project"]);
                foreach (var model in models)
                {
                    var m = new MLConfigController(activeModelChanged);
                    m.Settings = Settings;
                    //
                    m.Name = model;
                    Models.Add(m);
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private async Task<(List<string> header, List<List<string>> data)> initRawData(string filePath, DataParser parser)
        {
            

            //check if raw data file exists
            var fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                //
                var result = await Task.Run<(List<string> header, List<List<string>> data)>(() => ANNDataSet.prepareDataFromFile(filePath, parser.ColumnSeparator, parser.FirstRowHeader, loadProjectProgress));

                return result;
            }
            else
                return (null,null);
        }

        void loadProjectProgress(int current , int total)
        {
            Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Background,
               new Action(

                   () =>
                   {
                       var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                       appCnt.OpenProjectProgressAction(current, total);
                   }

               ));
           
        }

       /// The method only saves the project file, no related mlconfigs are saved.
       /// </summary>
        internal async Task<bool> Save( )
        {
            try
            {
                //get dataset from the data panel
                DataSet = getDataSet(Settings.ProjectType);

                //save rich 
                RichTextBox rtfCtrl = getRichCtrl();
                //get rich text to save content
                if (rtfCtrl != null)
                {
                    saveRich(rtfCtrl);

                }


                //
                if (Settings == null)
                {
                    var prjPath = promptToSaveFile();
                    Settings = new ProjectSettings();
                    //
                    var fi = new FileInfo(prjPath);
                    Settings.ProjectFolder = fi.Directory.FullName + "\\" + Name;
                    Settings.ProjectFile = fi.Name;
                }
                //
                if(DataSet!=null)
                {
                    //update setting info
                    Settings.PercentigeSplit = DataSet.IsPrecentige;
                    //Settings.RandomizeData = DataSet.RandomizeData;
                    Settings.ValidationSetCount = DataSet.RowsToValidation;
                    Settings.TestSetCount = DataSet.RowsToTest;
                }
                

                //load project information from file
                var prjPath1 = Path.Combine(Settings.ProjectFolder, Settings.ProjectFile);
                var dicData = Project.LoadProjectData(prjPath1);


                //save raw data file in to project folder
                var rawDataName = Project.GetParameterValue(dicData["data"], "RawData");
                if (string.IsNullOrEmpty(rawDataName))
                    rawDataName = $"{Name}_rawdata.txt";//naming convention for the raw dataset

                //create file of raw data
                var dataFile = Path.Combine(Settings.ProjectFolder, Name, rawDataName);
                if (DataSet != null)
                    await Task<bool>.Run(() => writeRawData(dataFile, DataSet.Data)); 

                //update project file with information about raw dataset
                generateProjectFile(prjPath1, rawDataName);
                MainWindow.SetCursor(false);

                return true;
            }
            catch (Exception)
            {
                MainWindow.SetCursor(false);
                throw;
            }
            
        }

       
        internal async Task<bool> CreateMLConfig(MLConfigController mlconfig)
        {
            //save project with new created mlconfig
            await Save();

            //create model name 
            string modelName = $"MLConfig{Models.Count}";

            //Define ml data file paths
            var strModelFolder = Project.GetMLConfigFolder(Settings, modelName);
            var strModelDataFolder = Project.GetMLConfigDataFolder(Settings, modelName);
            

            //check if model folder exists
            if (!Directory.Exists(strModelFolder))
                Directory.CreateDirectory(strModelFolder);
            //check if data folder exists
            if (!Directory.Exists(strModelDataFolder))
                Directory.CreateDirectory(strModelDataFolder);

            //model name and settings 
            mlconfig.Name = modelName;
            mlconfig.Settings = Settings;

            //load project file in order to get settings information and data description needed for data transformation
            var strPPath = Path.Combine(Settings.ProjectFolder, Settings.ProjectFile);
            var proj = new Project();
            proj.Load(strPPath);

            //ToDo: optimizes code for huge dataset
            if (Settings.ProjectType== ProjectType.Default)
            {
                createDefaultDataSets(DataSet, modelName, proj.Descriptor);
            }
            else if (Settings.ProjectType == ProjectType.ImageClassification)
            {
                createImgClassificationDataSets(DataSet, modelName, proj.Descriptor);
            }
            else
            {
                ;
            }


            //create mlconfig file
            Project.NewMLConfigFile(proj, modelName);

            //initialize model
            mlconfig.Init();

            //add newly created model in to Model collection of the project
            Models.Add(mlconfig);

            //save project with new created mlconfig
            return await Save();
        }

        /// <summary>
        /// Parses the information from the Raw Data and created mlready data sets for image classification project
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="modelName"></param>
        /// <param name="descriptor"></param>
        private void createImgClassificationDataSets(ANNDataSet dataSet, string modelName, DataDescriptor descriptor)
        {
            //define daata paths
            var strPathTrain = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Training);
            var strPathValid = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Validation);
            var strPathTest = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Testing);

            //
            var data = DataSet.Data;
           
            //retrieve image information form files
            var trainingData = new List<string>();
            var validationData = new List<string>();
            var testData = new List<string>();
            int dataCount=0;
            //
            int cahannel= int.Parse(data[0][3]);
            int height = int.Parse(data[0][4]);
            int width = int.Parse(data[0][5]);

            //first count how many images we are handling
            for (int i=0; i < data.Count; i++)
            {
                //ith label
                var label = data[i][0];
                var folder = data[i][1];
                var query = data[i][2];
                string[] images = null;

                if(string.IsNullOrEmpty(query)|| query==" ")
                    images = Directory.GetFiles(folder);
                else
                    images = Directory.GetFiles(folder, query, SearchOption.TopDirectoryOnly);
                //
                if (images==null || images.Length == 0)
                    throw new Exception($"No images have been found for '{label}' label. Folder path must not be empty.");

                dataCount += images.Length;
            }

            //create data sets
            var classesList = new List<string>();
            for (int i = 0; i < data.Count; i++)
            {
                //i-th label
                var label = data[i][0];
                //add class into list 
                classesList.Add(label);
                string lblIndex = i.ToString();
                //
                var folder = data[i][1];
                var query = data[i][2];
                string[] images = null;

                if (string.IsNullOrEmpty(query) || query == " ")
                    images = Directory.GetFiles(folder);
                else
                    images = Directory.GetFiles(folder, query, SearchOption.TopDirectoryOnly);
                //
                if (images == null || images.Length == 0)
                    throw new Exception($"No images have been found for '{label}' label. Folder path must not be empty.");

                //construct the rows of map files
                var strValue = images.Select(x => $"{x}\t{lblIndex}");
               
                //calculate amount of rows to put into train, valid and test data set
                var validCount = strValue.Count()*dataSet.RowsToValidation / 100.0f;
                var testCount = strValue.Count() * dataSet.RowsToTest / 100.0f;
                var trainCount = strValue.Count() - (int)validCount - (int)testCount;
                
                //add to train set
                trainingData.AddRange(strValue.Take(trainCount));

                //add to valid set
                if(validCount > 0)
                    validationData.AddRange(strValue.Skip(trainCount).Take((int)validCount));
                //add to test set
                if(testCount>0)
                    testData.AddRange(strValue.Skip((int)trainCount+ (int)validCount).Take((int)testCount));
            }

            //define metadata needed for MLConfiguration creation
            //Label columm classes definition
            descriptor.Columns.Last().Classes = classesList.ToArray();
            //image shape definition
            descriptor.Columns.First().Shape=$"{cahannel};{height};{width}";


            //check if the training count number valid
            if (trainingData.Count() <= 0)
            {
                throw new Exception("Train dataset is empty. Split data set on correct parts.");
            }
            File.WriteAllLines(strPathTrain, trainingData);

            //in case of empty validation data set skip file creation
            if (validationData.Count() > 0)
            {
                File.WriteAllLines(strPathValid, validationData);
            }
            //in case of empty validation data set skip file creation
            if (testData.Count() > 0)
            {
                File.WriteAllLines(strPathTest, testData);
            }

        }

        private string createHotVector(int index, int count)
        {
            var hotVector = "";
            for(int i=0; i< count; i++)
            {
                if (i == index)
                    hotVector += "1 ";
                else
                    hotVector += "0 ";
            }

            return hotVector;
        }

        /// <summary>
        /// Parses the information from the Raw Data and created mlready data sets for default project
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="modelName"></param>
        /// <param name="dataDesc"></param>
        private void createDefaultDataSets(ANNDataSet dataSet, string modelName, DataDescriptor dataDesc)
        {
            var strPathTrain = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Training);
            var strPathValid = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Validation);
            var strPathTest = Project.GetDefaultMLDatasetPath(Settings, modelName, DataSetType.Testing);

            //get dataset based on options 
            var ds = dataSet.GetDataSet(dataSet.RandomizeData);
            //we want whole data set later the data will be split
            ds.RowsToValidation = 0;
            ds.RowsToTest = 0;
            //create experiment based created dataset
            var exp = new DataFrame(ds);
            var data = ExportData.PrepareDataSet(exp);

            //calculate validation and training rows
            int validCount = dataSet.IsPrecentige ? (int)(dataSet.RowsToValidation * data.Count / 100.0) : dataSet.RowsToValidation;
            //in case of empty validation data set skip file creation
            if (validCount == 0)
                strPathValid = "";

            //calculate testing rows
            int testCount = dataSet.IsPrecentige ? (int)(dataSet.RowsToTest * data.Count / 100.0) : dataSet.RowsToValidation;
            //in case of empty validation data set skip file creation
            if (testCount == 0)
                strPathTest = "";

            //create training ml ready dataset file
            int trainCount = data.Count - validCount - testCount;
            //check if the training count number valid
            if (trainCount <= 0)
            {
                throw new Exception("Train dataset is empty. Split data set on correct parts.");
            }
            File.WriteAllLines(strPathTrain, data.Take(trainCount).ToList());

            //in case of empty validation data set skip file creation
            if (validCount > 0)
            {
                var d = data.Skip(trainCount).Take(validCount).ToList();
                File.WriteAllLines(strPathValid, d);
            }
            //in case of empty validation data set skip file creation
            if (testCount > 0)
            {
                var d = data.Skip(trainCount + validCount).Take(testCount).ToList();
                File.WriteAllLines(strPathTest, d);
            }


            //enumerate all column and setup column information needed for mlconfig creation
            foreach (var c in dataDesc.Columns.Where(x => x.Type == MLDataType.Category && x.Kind != DataKind.None))
            {
                var cc = exp.GetColumns().Where(x => x.Name == c.Name && x.ColumnDataType == ColumnType.Category).FirstOrDefault();
                if (cc == null)
                    throw new Exception("Column not found!");

                c.Classes = cc.Statistics.Categories.ToArray();
            }
        }

        private RichTextBox getRichCtrl()
        {
            var cntCtrl = anndotnet.wnd.App.Current.MainWindow as MainWindow;
            var tab = FindVisualChild<TabControl>(cntCtrl.content);
            if (tab == null)
                return null;
            var tbItm = tab.Items[tab.Items.Count-1] as TabItem;
            if (tbItm == null)
                return null;

            var ctrl = tbItm.Content as RichTextBox;
            return ctrl;
        }

        private DataPanelWPF getDataPanel()
        {
            var cntCtrl = anndotnet.wnd.App.Current.MainWindow as MainWindow;
            var tab = FindVisualChild<TabControl>(cntCtrl.content);
            if (tab == null)
                return null;
            var tbItm = tab.Items[0] as TabItem;
            if (tbItm == null)
                return null;

            var ctrl = FindVisualChild<DataPanelWPF>(tbItm.Content as Grid);

            //var expCtrl = (DataPanel)ctrl.Child;
            return ctrl;
        }

        private ANNDataSet getDataSet(ProjectType projectType)
        {
            var ds = new ANNDataSet();
            var cntCtrl = anndotnet.wnd.App.Current.MainWindow as MainWindow;
            var tab = FindVisualChild<TabControl>(cntCtrl.content);
            if (tab == null)
                return null;

            //
            if (projectType== ProjectType.Default)
            {
                var tbItm = tab.Items[0] as TabItem;
                if (tbItm == null)
                    return null;

                var expCtrl = FindVisualChild<DataPanelWPF>(tbItm.Content as Grid);
                if (expCtrl == null)
                    return null;
                ds = expCtrl.GetDataSet();              
            }
            else
            {
                var tbItm = tab.Items[1] as TabItem;
                if (tbItm == null)
                    return null;

                var ctrl = FindVisualChild<ImageClassificator>(tbItm.Content as Grid);
               
               //
                if (ctrl == null)
                    return null;
                //
                ds = ctrl.GetDataSet();
            }

            setCategoryEncoding(ds);
            return ds;
        }


        /// <summary>
        /// load comtent from info project file and load into rich control
        /// </summary>
        /// <param name="richCtrl"></param>
        /// <returns></returns>
        public string LoadRichText(RichTextBox richCtrl)
        {
            string fileName = Project.GetProjectInfoPath(Settings);

            var fi = new FileInfo(fileName);
            if(fi.Exists)
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    TextRange range = new TextRange(richCtrl.Document.ContentStart, richCtrl.Document.ContentEnd);
                    if (fileStream.Length > 0)
                        range.Load(fileStream, DataFormats.Rtf);

                    
                }
                    
            }
            return fileName;
        }

        /// <summary>
        /// Save rich content from 
        /// </summary>
        /// <param name="richCtrl"></param>
        /// <returns></returns>
        protected void saveRich(RichTextBox richCtrl)
        {
            string fileName = ANNdotNET.Lib.Project.GetProjectInfoPath(Settings);
            var fi = new FileInfo(fileName);
            if(!fi.Exists)
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create));

            }
            using (FileStream fileStream = new FileStream(fileName, FileMode.Truncate))
            {
                TextRange range = new TextRange(richCtrl.Document.ContentStart, richCtrl.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
                
            return;
        }

        /// <summary>
        /// Update project files with information about dataset and metadata
        /// </summary>
        /// <param name="projectPath">Full path of the project file</param>
        /// <param name="rawDataName">Raw data file path</param>
        private void generateProjectFile(string projectPath, string rawDataName)
        {
            var ps = Settings.PercentigeSplit ? 1 : 0;
            var strMeta = DataSet == null ? "" : toColumnToString(DataSet.MetaData);
            //var rd = Settings.RandomizeData ? 1 : 0;
            var dataStr = $"data:|RawData:{rawDataName} " + strMeta;
            var models = string.Join(";", Models.Select(x => x.Name));
            var strProject = $"project:|Name:{Name} |Type:{Settings.ProjectType}  |ValidationSetCount:{Settings.ValidationSetCount}  |TestSetCount:{Settings.TestSetCount} |PrecentigeSplit:{ps} |MLConfigs:{models} |Info:ProjectInfo.rtf";
            var strParser = "parser:|RowSeparator:rn |ColumnSeparator: ; |Header:0 |SkipLines:0";
            
            //construct diction 
            Dictionary<string, string> dicValues = new Dictionary<string, string>();
            dicValues.Add("data", dataStr);
            dicValues.Add("project", strProject);
            dicValues.Add("parser", strParser);
            Project.UpdateProject(dicValues, projectPath);
        }

        private void setCategoryEncoding(ANNDataSet dataSet)
        {
            if (dataSet==null || dataSet.MetaData == null)
                return;

            foreach(var col in dataSet.MetaData)
            {

                if (col.Type == ColumnType.Category.ToString())
                    col.Encoding = CategoryEncoding.OneHot.ToString();

            }

            //extract the categories from 
        }

        private bool writeRawData(string filePath, List<List<string>> data)
        {
            if (data == null)
                return false;
            var cnt = data.Select(x=>string.Join(";",x)).ToArray();
            File.WriteAllLines(filePath, cnt);
            return true;
        }

        private string toColumnToString(MetaColumn[] metaData)
        {
            if (metaData == null)
                return "";
            var strColumns = "";
            int intCounter = 1;
            foreach (var c in metaData)
            {
                var strCounter = "";
                if (intCounter < 10)
                    strCounter = $"0{intCounter}";
                else
                    strCounter = $"{intCounter}";
                //
                strColumns += $"|Column{strCounter}:{c.Name};{c.Type};{c.Param};{c.MissingValue};{c.Scale} ";
                intCounter++;
            }

            return strColumns;
        }

      
    }
}
