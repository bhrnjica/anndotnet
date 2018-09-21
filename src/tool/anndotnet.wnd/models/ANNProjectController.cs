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
using DataProcessing.Core;
using MLDataPreparation.Dll;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms.Integration;

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
        public string ProjectInfo { get; internal set; }
        public new string IconUri { get => "Images/experiment.png"; }

        //initialize project controllers with project information
        internal void Initproject(string projectPath)
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
                Name = Project.GetParameterValue(dicData["project"],"Name");

                var prData = dicData["data"].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var dataPath = Project.GetParameterValue(prData, "RawData");
                var filePath = "";
                if (!string.IsNullOrEmpty(dataPath))
                    filePath = Path.Combine(Settings.ProjectFolder,Name ,dataPath);
                else
                    filePath = Name + "_rawdata.txt";

                //create dataset
                var ds = new ANNDataSet();
                
                ds.InitMetaColumn(prData.Where(x => x.StartsWith("Column")).OrderBy(x => x));
                var parser = Project.CreateDataParser(dicData["parser"]);

                //check if raw data file exists
                fi = new FileInfo(filePath);
                if (fi.Exists)
                {
                    //column separator is always ;
                    var result = ANNDataSet.prepareData(File.ReadAllLines(filePath), new char[] { ';'}, parser.FirstRowHeader);
                    ds.Data = result.data;
                    ds.IsPrecentige = Settings.PrecentigeSplit;
                    ds.TestRows = Settings.ValidationSetCount;
                    DataSet = ds;
                }

                //load existing models
                var models = Project.CreateMLConfigLists(dicData["project"]);
                foreach(var model in models)
                {
                    var m = new MLConfigController(activeModelChanged);
                    m.Settings = Settings;
                    //
                    m.Name = model;
                    //if (m.InitModel())// just add model but dont initialize it
                        Models.Add(m);
                    //else
                    //{
                    //    if (MessageBox.Show($"'{m.Name}' is corrupted and cannot be loaded into the project. " +
                    //        $"\nDo you still want it on the project list?", "ANNdotNET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    //    {
                    //        MLConfig.Add(m);
                    //    }
                    //}
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        internal void Save( )
        {
            //access Data Pane in order to update data
            var cntCtrl = anndotnet.wnd.App.Current.MainWindow;
            var ctrl = FindChild<WindowsFormsHost>(cntCtrl, "hostWF");
            var expCtrl = (DataPanel)ctrl.Child;
            if (expCtrl == null)
                return;
            DataSet = expCtrl.GetDataSet();
            setCategoryEncoding(DataSet);
            if (Settings == null)
            {
                var prjPath = promptToSaveFile();
                Settings = new ProjectSettings();
                //
                var fi = new FileInfo(prjPath);
                Settings.ProjectFolder = fi.Directory.FullName + "\\" + Name;
                Settings.ProjectFile = fi.Name;
            }
            //update setting info
            Settings.PrecentigeSplit = DataSet.IsPrecentige;
            //Settings.RandomizeData = DataSet.RandomizeData;
            Settings.ValidationSetCount = DataSet.TestRows;

            //load project information from file
            var prjPath1 = Path.Combine(Settings.ProjectFolder, Settings.ProjectFile);
            var dicData = Project.LoadProjectData(prjPath1);


            //save raw data file in to project folder
            var rawDataName = Project.GetParameterValue(dicData["data"], "RawData");
            if (string.IsNullOrEmpty(rawDataName))
                rawDataName = $"{Name}_rawdata.txt";//naming convention for the raw dataset
            //create file of raw data
            var dataFile = Path.Combine(Settings.ProjectFolder, Name, rawDataName);
            writeRawData(dataFile, DataSet.Data);

            //update project file with information about raw dataset
            generateProjectFile(prjPath1, rawDataName);

            //once the project information is updated, save models
            //foreach (var model in MLConfig)
            //{
            //    model.SaveModel();
            //}
        }

        /// <summary>
        /// Update project files with information about dataset and metadata
        /// </summary>
        /// <param name="projectPath">Full path of the project file</param>
        /// <param name="rawDataName">Raw data file path</param>
        private void generateProjectFile(string projectPath, string rawDataName)
        {
            var ps = Settings.PrecentigeSplit ? 1 : 0;
            //var rd = Settings.RandomizeData ? 1 : 0;
            var dataStr = $"data:|RawData:{rawDataName} " + toColumnToString(DataSet.MetaData);
            var models = string.Join(";", Models.Select(x => x.Name));
            var strProject = $"project:|Name:{Name}  |ValidationSetCount:{Settings.ValidationSetCount} |PrecentigeSplit:{ps} |MLConfigs:{models} |Info:{ProjectInfo}";
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
            if (dataSet.MetaData == null)
                return;

            foreach(var col in dataSet.MetaData)
            {

                if (col.Type == ColumnType.Category.ToString())
                    col.Encoding = CategoryEncoding.OneHot.ToString();

            }

            //extract the categories from 
        }

        private void writeRawData(string filePath, string[][] data)
        {
            if (data == null)
                return;
            var cnt = data.Select(x=>string.Join(";",x)).ToArray();
            File.WriteAllLines(filePath, cnt);
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

        internal void CreateMLConfig(MLConfigController model)
        {
            //save project with new created mlconfig
            Save();

            //create model name 
            string modelName = $"Model{Models.Count}";

            //Define ml data file paths
            var strModelFolder = Project.GetMLConfigFolder(Settings, modelName);
            var strModelDataFolder = Project.GetMLConfigDataFolder(Settings, modelName);
            var strPathTrain = Project.GetDefaultMLDatasetPath(Settings, modelName, true);
            var strPathValid = Project.GetDefaultMLDatasetPath(Settings, modelName, false);

            //check if model folder exists
            if (!Directory.Exists(strModelFolder))
               Directory.CreateDirectory(strModelFolder);
            //check if data folder exists
            if (!Directory.Exists(strModelDataFolder))
                Directory.CreateDirectory(strModelDataFolder);

            //get dataset based on options 
            var ds = DataSet.GetDataSet(DataSet.RandomizeData);
            //we want whole data set later the data will be split
            ds.TestRows = 0;
            //create experiment based created dataset
            var exp = new Experiment(ds);
            var data =  ExportData.PrepareDataSet(exp);

            //calculate validation and training rows
            int validCount = DataSet.IsPrecentige ? (int)(DataSet.TestRows * data.Count / 100.0) : DataSet.TestRows;
            //in case of empty validation data set skip file creation
            if(validCount==0)
                strPathValid = "";

            //create training ml ready dataset file
            int trainCount = data.Count - validCount;
            if(trainCount <= 0)
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

            //model name and settings 
            model.Name = modelName;
            model.Settings = Settings;

            //load project file in order to get settings information and data description needed for data transformation
            var strPPath = Path.Combine(Settings.ProjectFolder, Settings.ProjectFile);
            var proj = new Project();
            proj.Load(strPPath);
            
            //enumerate all column and setup column information needed for mlconfig creation
            foreach(var c in proj.Descriptor.Columns.Where(x=>x.Type== DataType.Category && x.Kind != DataKind.None))
            {
                var cc =  exp.GetColumns().Where(x => x.Name == c.Name && x.ColumnDataType == ColumnType.Category).FirstOrDefault();
                if (cc == null)
                    throw new Exception("Column not found!");

                c.Classes = cc.Statistics.Categories.ToArray();
            }

            //create mlconfig file
            Project.NewMLConfigFile(proj, modelName);

            //initialize model
            model.InitModel();

            //add newly created model in to Model collection of the project
            Models.Add(model);

            //save project with new created mlconfig
            Save();
        }
    }
}
