
namespace Anndotnet.App.Models;


public record AnndotnetModel
{
    public string Name { get; set; }
    public string Link { get; set; }

    public string? Icon       { get; set; }
    public bool    IsExpanded { get; set; }

    public List<NavigationItem> SubItems { get; set; }

}



