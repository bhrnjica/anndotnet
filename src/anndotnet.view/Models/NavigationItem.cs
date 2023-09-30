
namespace Anndotnet.App.Models;

public class NavigationItem
{
    public string Name { get; set; }
    public string Link { get; set; }

    public string? Icon { get; set; }
    public bool IsExpanded { get; set; }

    public List<NavigationItem> SubItems { get; set; }

}
