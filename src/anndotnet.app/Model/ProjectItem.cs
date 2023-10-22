
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Anndotnet.App.Model;

public class ProjectItem
{
    public string? Name { get; set; }
    public string? Link { get; set; }

    public string? Icon { get; set; }
    public bool IsExpandedEx { get; set; }

    public ItemType ItemType   { get; set; }
    public bool     IsSelected { get; set; }    

    public List<ProjectItem>? ModelItems { get; set; }

}

public enum ItemType
{
    Start,
    Project,
    Model,
    CustomPage,
}
