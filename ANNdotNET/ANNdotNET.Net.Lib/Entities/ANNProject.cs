using DataProcessing.MLBasicTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Controllers;

namespace ANNdotNET.Net.Lib.Entities
{
    /// <summary>
    /// Implements project class for handling one or more models
    /// </summary>
    public class ANNProject
    {
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
        //Paroject name
        public string Name { get; set; }

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
        /// Start training process for the speciefied model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setData"></param>
        /// <param name="report"></param>
        /// <param name="token"></param>
        public void Run(ANNModel model, ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>)> report, CancellationToken token)
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

        //creae new model
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
    }
}