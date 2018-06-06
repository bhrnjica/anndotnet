using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Controllers;
using ANNdotNET.Net.Lib.Entities;

namespace ANNdotNet.Wnd.Dll.Controllers
{
    public class ProjectController : BaseController

    {
        public ANNProject Project { get; set; }
       

        public ProjectController(string gui)
        {
            Project = new ANNProject(gui);
            Project.Controller = this;
        }

    

        public void InitiNewProject(string name)
        {
            Project.InitiNewProject(name);
        }

        public string GetGuid()
        {
            return Project.Guid;
        }

       

        public void CreateModel(string guid, string mName, string fileName, int inputDim, int outDim, uint trainRowCount, uint testRowCount, List<string> classes, string label, bool randomizeData)
        {
            Project.CreateModel(guid, mName, fileName, inputDim, outDim, trainRowCount, testRowCount, classes, label, randomizeData);
           
        }

       

        public void Run(ANNModel model, ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>, List<List<float>>)> report, CancellationToken token)
        {
            try
            {
                Project.Run(model, setData, report, token);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Close()
        {
            Project.CloseModels();
            
        }

        public void PrepareForSave()
        {
           // throw new NotImplementedException();
        }
    }
}
