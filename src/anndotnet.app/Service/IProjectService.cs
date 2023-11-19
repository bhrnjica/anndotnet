using Anndotnet.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Shared.Entities;
using Daany;

namespace Anndotnet.App.Service
{
    public interface IProjectService
    {
       
        Task<ProjectModel> LoadProjectAsync(string       path,          string currentDir = "");
        Task<bool>         SaveProjectAsync(ProjectModel project,  NavigationItem item = null);
        MlModel            LoadMlModel(string            path,          string currentDir = "");
        DataFrame          FromDataParser(DataParser?    projectParser, string currentDir = "");
    }
}
