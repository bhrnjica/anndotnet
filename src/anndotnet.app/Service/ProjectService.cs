using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;

namespace Anndotnet.App.Service
{
    internal class ProjectService : IProjectService
    {
        public void Run()
        {
            throw new NotImplementedException();
        }

        public NavigationItem LoadStartPage()
        {
            return new()
                   {
                       Name = "Home", 
                       Icon = "/assets/images/start.png", 
                       Link = "Home",
                       ItemType= ItemType.Start
                       
                   };
        }

        public NavigationItem LoadIrisProject()
        {
            return new()
                   {
                       Name = "Iris",
                       Link = "Iris",
                       IsExpandedEx = true,
                       
                       ItemType = ItemType.Project,

                       Icon = "/assets/images/experiment.png",

                       ModelItems = new()
                                    {
                                        new ()
                                        {
                                            Name="Model 1", 
                                            Link="Model 1",
                                            ItemType= ItemType.Model,
                                            Icon="/assets/images/model.png"
                                        }
                                    }
                   };
        }
    }
}
