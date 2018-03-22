using ANNdotNet.Wnd.Dialogs;
using ANNdotNet.Wnd.Dll;
using ANNdotNet.Wnd.Dll.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Entities;

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

        public object OpenProject(string filePath)
        {
            try
            {

                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveProject(string filePath, object pController)
        {
            try
            {
                
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

        internal void Run(ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>)> report, CancellationToken token)
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
