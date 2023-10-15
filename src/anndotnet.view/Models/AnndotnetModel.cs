
using Anndotnet.Core.Entities;

namespace Anndotnet.App.Models;


public record AnndotnetModel
{
    public string Name { get; set; }
    public string Description { get; set; }

    public MlConfig? MlConfig { get; set; }

    public NavigationItem NavigationItem { get; set; }  
}



