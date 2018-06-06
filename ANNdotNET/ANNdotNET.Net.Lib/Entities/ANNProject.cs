using DataProcessing.MLBasicTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Controllers;
using DataProcessing.MLData;

namespace ANNdotNET.Net.Lib.Entities
{
    /// <summary>
    /// Implements project class for handling one or more models
    /// </summary>
    public class ANNProject
    {
        public Func<string> GetExperimentData { get; set; }
        public Func<MLDataSet> GetDataSet { get; set; }

        public ANNProject(string guid)
        {
            this.Guid = guid;
            Models = new List<ANNModel>();
        }
        //man data loaded in experimental panel
        public MLDataSet DataSet { get; set; }
        //project Controller 
        public BaseController Controller { get; set; }
        //list of models in the project
        public List<ANNModel> Models { get; set; }
        //guid of the project
        public string Guid { get; internal set; }
        //Project name
        public string Name { get; set; }

        public string FilePath { get; set; }

        public string ProjectInfo { get; set; }
        public bool IsDirty { get; set; }
        ActiveModelData m_activeModelData;
        public ActiveModelData ActiveModelData {
            get
            {
                return m_activeModelData;
            }
            set
            {
                m_activeModelData = new ActiveModelData(value);
            }
        }

        /// <summary>
        /// Method called which the project is initialized
        /// </summary>
        /// <param name="name"></param>
        public void InitiNewProject(string name)
        {
            Name = name;
        }
        //Find model with specified guid
        public ANNModel FindModel(string guid)
        {
            foreach (var m in Models)
            {
                if (m.Guid == guid)
                    return m;
            }
            return null;
        }
        //Is model running
        public bool IsRunning()
        {
            foreach (var m in Models)
            {
                if (m.IsRunnig)
                    return true;

            }
            return false;
        }

        /// <summary>
        /// Start training process for the specified model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setData"></param>
        /// <param name="report"></param>
        /// <param name="token"></param>
        public void Run(ANNModel model, ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>, List<List<float>>)> report, CancellationToken token)
        {
            try
            {
                model.Run(setData,report,token);
            }
            catch (Exception)
            {

                throw;
            }
        }

        //create new model
        public void CreateModel(string guid, string nName, string fileName, int inputDim, int outDim, uint trainRowCount, uint testRowCount, List<string> classes, string label, bool randomizeData)
        {
            var m = new ANNModel();
            m.Classes = classes;
            m.Label = label;
            m.Guid = guid;
            m.Name = nName;
            m.DataFileName = fileName;
            m.Parent = this;
            m.InputDim = inputDim;
            m.OutputDim = outDim;
            m.TrainCount = trainRowCount;
            m.TestCount = testRowCount;
            m.Randomize = randomizeData;
            Models.Add(m);
        }

        /// <summary>
        /// Close model
        /// </summary>
        public void CloseModels()
        {
            foreach(var m in Models)
            {
                m.Close();
            }
            Models.Clear();
        }

        public static ANNProject Open(string filePath)
        {
            try
            {
                //JsonSerializerSettings sett = new JsonSerializerSettings();
                //sett.NullValueHandling = NullValueHandling.Ignore;

                //var strJson = System.IO.File.ReadAllText(filePath);
                //var obj = JsonConvert.DeserializeObject(strJson, sett);
                //var d = ((dynamic)obj)["DataSet"] as Newtonsoft.Json.Linq.JObject;
                //var mods = ((dynamic)obj)["Models"] as Newtonsoft.Json.Linq.JArray;
                //var name = ((dynamic)obj)["Name"] as Newtonsoft.Json.Linq.JValue;
                //var projectInfo = ((dynamic)obj)["ProjectInfo"] as Newtonsoft.Json.Linq.JValue;


                ////create NET Project object from Json
                //var guid = System.Guid.NewGuid().ToString();
                //var project = new ANNProject(guid);
                //project.Name = (string)name;

                //project.DataSet = d.ToObject<MLDataSet>();
                //project.FilePath = filePath;
                //project.ProjectInfo = (string)projectInfo;

                ////
                ////de-serialize models
                //ANNModel[] mod = mods.ToObject<ANNModel[]>();
                //foreach (var m in mod)
                //{
                //    var g = System.Guid.NewGuid().ToString();
                //    var exp = new Experiment(m.);

                //    var classes = exp.GetColumnsFromOutput()[0].Statistics.Categories;
                //    var label = exp.GetColumnsFromOutput()[0].Name;

                //    var mm = new ANNModel(g);
                //    mm.fac = m.Factory;
                //    mm.Name = m.Name;
                //    mm.DataSet = m.DataSet;
                //    mm.ExpData = exp;
                //    //mm.SetParent(pController);
                //    mm.InitPersistedModel();
                //    //project.Models.Add(mm);
                //    project.Models.Add(mm);
                //}

                //return project;
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}