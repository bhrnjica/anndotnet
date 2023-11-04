﻿using Anndotnet.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anndotnet.App.Service
{
    public interface IProjectService
    {
       
        ProjectModel LoadProject(string path);
        MlModel LoadMlModel(string path);
    }
}
