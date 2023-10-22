
using Anndotnet.Core.Entities;

namespace Anndotnet.App.Model;


public record AnndotnetModel
{
    public string? Name        { get; set; }
    public string? Description { get; set; }

    public MlConfig? MlConfig { get; set; }

    public ProjectItem? NavigationItem { get; set; }  
}



