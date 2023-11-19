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

            var dirPath = Path.GetDirectoryName(filePath.AbsolutePath);
            var projectFileName = Path.GetFileNameWithoutExtension(filePath.AbsolutePath);
            var mlFiles = Directory.GetFiles(dirPath, "*.mlconfig");

            var navItm = new NavigationItem();
            navItm.Name = projectFileName;
            navItm.Link = projectFileName;
            navItm.StartDir = dirPath;
            navItm.IsExpandedEx = true;
            navItm.ItemType = ItemType.Project;
            navItm.Icon = "/assets/images/experiment.png";
            navItm.ModelItems = new List<NavigationItem>();
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
