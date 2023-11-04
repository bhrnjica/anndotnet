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
                       Link = "assets-projects/iris",
                       IsExpandedEx = true,
                       
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
    }
}
