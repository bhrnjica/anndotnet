using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Mlconfig;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Controls.Shapes;
using Daany;
using ExCSS;
using XPlot.Plotly;
using Path = System.IO.Path;

namespace Anndotnet.App.Service
{
    internal class ProjectService : IProjectService
    {
        private static readonly string? Assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public async Task<ProjectModel> LoadProjectAsync(string relativePath,string currentDir = "" )
        {
            var options = MlFactory.JsonSerializerOptions();

            var startDir = string.IsNullOrEmpty(currentDir) ? Environment.CurrentDirectory : currentDir;
            string fullPath = System.IO.Path.Combine(startDir, relativePath+".ann");

            //var projectFile = Directory.GetFiles(fullPath,"*.ann").FirstOrDefault();

            if (string.IsNullOrEmpty(fullPath) || !Path.Exists(fullPath))
            {
                throw new Exception("Selected project cannot be found.");
            }

            //
            await using FileStream fs = File.OpenRead(fullPath);
            var project = await System.Text.Json.JsonSerializer.DeserializeAsync<ProjectModel>(fs, options).ConfigureAwait(false);
            if (project != null)
            {
                return project;
            }
            else
            {
                throw new NullReferenceException(nameof(project));
            }
        }

        //selected item is not correct
        public async Task<bool> SaveProjectAsync(ProjectModel project, NavigationItem item = null)
        {
            string fullPath = System.IO.Path.Combine(item.StartDir, item.Link + ".ann");

            var options = MlFactory.JsonSerializerOptions();
            //
            await using FileStream fs = File.Open(fullPath, FileMode.Create);
            await System.Text.Json.JsonSerializer.SerializeAsync<ProjectModel>(fs, project, options: options);

            return true;
        }

        public MlModel LoadMlModel(string path, string currentDir = "")
        {
            var startDir = string.IsNullOrEmpty(currentDir) ? Environment.CurrentDirectory : currentDir;
            string fullPath = System.IO.Path.Combine(startDir, path + ".mlconfig");

            if (!System.IO.Path.Exists(fullPath))
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

        public DataFrame FromDataParser(Anndotnet.Shared.Entities.DataParser? parser, string currentDir = "")
        {
            string[] colNames = null;
            if (!parser.HasHeader && parser.Header != null )
            {
                colNames = parser.Header;
            }
            else if (!parser.HasHeader && parser.Header == null)
            {
                throw new NullReferenceException(nameof(parser.Header));
            }
            var startDir = string.IsNullOrEmpty(currentDir) ? Environment.CurrentDirectory : currentDir;
            string fullPath = System.IO.Path.Combine(startDir, parser?.FileName!);

            return DataFrame.FromCsv(filePath: fullPath, sep: parser.ColumnSeparator!, names: colNames,
                dformat: parser.DateFormat!, missingValues: parser.MissingValueSymbol!, colTypes: null!, skipLines: parser.SkipLines);
        }
    }
}
