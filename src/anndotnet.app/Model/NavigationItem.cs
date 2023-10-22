
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Anndotnet.App.Model;

public class NavigationItem : IEquatable<NavigationItem>
{
    public string? Name { get; set; }
    public string? Link { get; set; }

    public string? Icon { get; set; }
    public bool IsExpandedEx { get; set; }

    public ItemType ItemType   { get; set; }
    public bool     IsSelected { get; set; }    

    public List<NavigationItem>? ModelItems { get; set; }

    public bool Equals(NavigationItem? other)
    {
        if (string.Equals(Name, other.Name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum ItemType
{
    Start,
    Project,
    Model,
    CustomPage,
}
