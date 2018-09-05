//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using ANNdotNet.Wnd.Dialogs;
using ANNdotNet.Wnd.Dll.Controllers;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ANNdotNet.Wnd.App
{
    public class AppController
    {
        public AppController()
        {
            Projects = new List<ProjectController>();
        }

        public Object ActiveView { get; set; }
        public List<ProjectController> Projects { get; set; }
        public Action<Exception> ReportException { get; set; }

        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        string m_StatusMessage = "No application message.";
        public string StatusMessage
        {
            get
            {
                return m_StatusMessage;
            }
            set
            {
                m_StatusMessage = value;
                // RaisePropertyChangedEvent("StatusMessage");
            }
        }

        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        string m_AppStatus = " Ready!";
        public string AppStatus
        {
            get
            {
                return m_AppStatus;
            }
            set
            {
                m_AppStatus = value;
                //("AppStatus");
            }
        }

        public object GetView(string guid)
        {
            if (guid == "startpage")
                return null;
            foreach (var p in Projects)
            {
                if (p.GetGuid().Equals(guid))
                    return p;
                else
                {
                    var m = p.Project.FindModel(guid);
                    if (m != null)
                        return m;
                }

            }

            return null;
        }

        
        public void NewProject(string tag, string name)
        {
            try
            {
                ProjectController p = new ProjectController(tag);
                p.InitiNewProject(name);
                Projects.Add(p);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ProjectController OpenProject(string filePath)
        {
            try
            {
                //var guid = Guid.NewGuid().ToString();
                //var pController = new ProjectController(guid);
                //var project = Project.Open(filePath);
                //foreach (var m in project.Models)
                //{
                //    var exp = m.ExpData;
                //    var g = Guid.NewGuid().ToString();
                //    var classes = exp.GetColumnsFromOutput()[0].Statistics.Categories;
                //    var label = exp.GetColumnsFromOutput()[0].Name;

                //    var mm = new ModelController(g, exp.GetOutputColumnType(), classes, label);
                //    mm.Model = m;
                //    mm.SetParent(pController);
                //    mm.InitPersistedModel();
                //    pController.Project.Models.Add(mm.Model);
                //    pController.Models.Add(mm);
                //}

                ////add project to app controller
                //pController.Project = project;
                //Projects.Add(pController);
                //return pController;
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveProject(string filePath, ProjectController pController, string currentModelGuid="")
        {
            try
            {
                ////save the trained model
                //if (m_Trainer != null)
                //{
                //    SaveFileDialog dlg = new SaveFileDialog();
                //    dlg.Filter = "CNTK Model File | *.model | All files(*.*) | *.* ";
                //    if (dlg.ShowDialog() == DialogResult.OK)
                //    {
                //        string inputDim = textBox6.Text;
                //        string outputDim = textBox5.Text;
                //        string embedDim = textBox7.Text;
                //        string hidDim = textBox8.Text;
                //        string cellDim = textBox9.Text;


                //        m_Trainer.SaveCheckpoint(dlg.FileName);
                //        File.WriteAllLines(dlg.FileName + ".dnn", new string[] { inputDim, outputDim, embedDim, hidDim, cellDim });
                //        var lns = File.ReadAllLines(dlg.FileName + ".dnn");
                //    }
                //}

                // return true;

                pController.PrepareForSave();
                //
                ANNProject project = pController.Project;
                JsonSerializerSettings sett = new JsonSerializerSettings
                { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
                sett.NullValueHandling = NullValueHandling.Ignore;

                //set experiment filename
                project.FilePath = filePath;

                //only for active project panel update data from GUI
                if (ActiveView is ProjectController)
                {
                    var p = ((ProjectController)ActiveView).Project;

                    //update changes from experimental model
                    //if (project.GetExperimentData != null && p.Guid == project.Guid)
                    //    project.DataSet = project.GetExperimentData();
                    //update changes from experimental model
                    if (project.GetDataSet != null && p.Guid == project.Guid)
                        project.DataSet = project.GetDataSet();
                }

                var currentProject = pController.Project;
                //
                for (int i = 0; i < project.Models.Count; i++)
                {
                    
                    var m = project.Models[i];
                    //update current active model only
                    if(currentModelGuid==m.Guid)
                        m.PrepareForSave(project.ActiveModelData);
                }
                //
                var str = JsonConvert.SerializeObject(project, sett);
                System.IO.File.WriteAllText(filePath, str);
                project.IsDirty = false;
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public bool CloseProject(ProjectController project)
        {
            //
            project.Close();
            return true;
        }

        public string IsOpen(string filePath)
        {
            
            return "";
        }

        //public bool IsModified(ProjectController exp, FunctionPanel funPanel1, ParametersPanel parPanel1, RunPanel runPanel1, ResultPanel resPanel1, TestPanel testPanel1)
        //{
        //    var models = exp.Models;
        //    if (exp.Project.IsDirty)
        //        return true;
        //    //
        //    foreach (var m in models)
        //    {
        //        //active model can contains unsaved data
        //        if (ActiveView is ModelController)
        //        {
        //            var mm = (ModelController)ActiveView;
        //            if (mm.Model.Guid == m.Model.Guid)
        //                m.getCurrentValues(funPanel1, parPanel1, runPanel1, resPanel1, testPanel1);
        //        }
        //        //check for modification
        //        if (m.IsModified())
        //            return true;
        //    }
        //    return false;
        //}

        internal void Run(ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>, List<List<float>>)> report, CancellationToken token)
        {
            try
            {
                ProjectController project=null; 
                var model = ActiveView as ANNModel;
                if (model != null)
                    project = (ProjectController)model.Parent.Controller;
                else
                    throw new Exception("Before Run, the model must be selected.");



                project.Run(model,setData,report,token);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// evaluate model
        /// </summary>
        /// <param name="model"></param>
        internal void EvaluateModel(ANNModel model)
        {
            var ret = model.EvaluateResults();

            if (ret != null)
            {

                //
                if (model.OutputDim==1)
                {
                    RModelEvaluation dlg = new RModelEvaluation();
                    dlg.Evaluate(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray(),
                            ret.ContainsKey("obs_test") ? ret["obs_test"].Select(x => (double)x).ToArray() : null,
                            ret.ContainsKey("prd_test") ? ret["prd_test"].Select(x => (double)x).ToArray() : null);

                    dlg.ShowDialog();

                }
                else if (model.OutputDim == 2)
                {
                    BModelEvaluation dlg = new BModelEvaluation();
                    var cl = ret["Classes"].Select(x => x.ToString()).ToArray();
                    dlg.loadClasses(cl);
                    dlg.loadData(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray(),
                        ret.ContainsKey("obs_test") ? ret["obs_test"].Select(x => (double)x).ToArray() : null,
                        ret.ContainsKey("prd_test") ? ret["prd_test"].Select(x => (double)x).ToArray() : null);

                    dlg.ShowDialog();

                }
                else
                {
                    MModelEvaluation dlg = new MModelEvaluation();
                    var cl = ret["Classes"].Select(x => x.ToString()).ToArray();
                    dlg.loadClasses(cl);
                    dlg.loadData(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray(),
                        ret.ContainsKey("obs_test") ? ret["obs_test"].Select(x => (double)x).ToArray() : null,
                        ret.ContainsKey("prd_test") ? ret["prd_test"].Select(x => (double)x).ToArray() : null);

                    dlg.ShowDialog();
                }
            }
            else
                MessageBox.Show("Evaluation process is not initialized.");

        }
    }
}
