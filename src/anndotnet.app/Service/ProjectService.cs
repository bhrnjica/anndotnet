using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.Core.Mlconfig;
using Avalonia;
using Avalonia.Platform;
using ExCSS;
using Newtonsoft.Json;
using XPlot.Plotly;

namespace Anndotnet.App.Service
{
    internal class ProjectService : IProjectService
    {
        private static readonly string? Assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public ProjectModel LoadProject(string path)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, path);
            var projectFile = Directory.GetFiles(fullPath,"*.ann").FirstOrDefault();
            if (projectFile == null)
            {
                throw new Exception("Selected project cannot be found.");
            }
            var project = JsonConvert.DeserializeObject<ProjectModel>(File.ReadAllText(projectFile));
             
            return project;
            
        }

        public MlModel  LoadMlModel(string path)
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, path + ".mlconfig");

            if (!Path.Exists(fullPath))
            {
                throw new Exception("Selected model cannot be found.");
            }   
            var mlConfig= MlFactory.LoadfromFile(fullPath);
           

            MlModel mlModel = new MlModel();
            mlModel.Name= mlConfig.Name;
            mlModel.Description = "description";
            mlModel.Path = path;    
            return mlModel;
        }
    }
}
