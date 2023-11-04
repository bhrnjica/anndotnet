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
       
        ProjectModel LoadProject(string         path);
        MlModel      LoadMlModel(string         path);
        DataFrame    FromDataParser(DataParser? projectParser);
    }
}
