using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Avalonia;
using Avalonia.Platform;
using ExCSS;
using Newtonsoft.Json;
using XPlot.Plotly;

namespace Anndotnet.App.Service
{
    internal class NavigationService : INavigationService
    { 
        public NavigationItem StartPageItem()
        {
            return new()
                   {
                       Id = new Guid(1,2,3,4,5,6,7,8,9,10,11),
                       Name = "Home", 
                       Icon = "/assets/images/start.png", 
                       Link = "Home",
                       ItemType= ItemType.Start
                       
                   };
        }

        public NavigationItem IrisItem()
        {
            return new()
                   {
                       Name = "Iris",
                       Link = "assets-projects/iris/iris",
                       IsExpandedEx = true,
                       StartDir = Environment.CurrentDirectory,
                       ItemType = ItemType.Project,
                       Icon = "/assets/images/experiment.png",
                       ModelItems = new()
                                    {
                                        new ()
                                        {
                                            Name="iris", 
                                            Link="assets-projects/iris/iris",
                                            ItemType= ItemType.Model,
                                            Icon="/assets/images/model.png"
                                        },
                                        new ()
                                        {
                                            Name="iris01",
                                            Link="assets-projects/iris/iris01",
                                            ItemType= ItemType.Model,
                                            Icon="/assets/images/model.png"
                                        }
                                    }
                   };
        }

        public NavigationItem CreateNavigationItem(Uri filePath)
        {

            var dirPath = Path.GetDirectoryName(filePath.LocalPath);
            var projectFileName = Path.GetFileNameWithoutExtension(filePath.LocalPath);

            var mlFiles = Directory.GetFiles(dirPath, "*.mlconfig");

            var navItm = new NavigationItem
                         {
                             Name = projectFileName,
                             Link = projectFileName,
                             StartDir = dirPath,
                             IsExpandedEx = true,
                             ItemType = ItemType.Project,
                             Icon = "/assets/images/experiment.png",
                             ModelItems = new List<NavigationItem>()
                         };


            foreach (var mlName in mlFiles.Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                var nav = new NavigationItem()
                          {
                              Name = mlName,
                              Icon = "/assets/images/model.png",
                              Link = mlName,
                              ItemType = ItemType.Model,
                              StartDir = dirPath
                          };
                navItm.ModelItems.Add(nav);
            }

            return navItm;
        }

    }
}
