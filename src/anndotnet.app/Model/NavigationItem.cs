
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Anndotnet.App.Model;

public class NavigationItem : IEquatable<NavigationItem>
{
    public Guid Id { get; set; }    
    public string? Name { get; set; }
    public string? Link { get; set; }

    public string? Icon { get; set; }
    public bool IsExpandedEx { get; set; }

    public ItemType ItemType   { get; set; }
    public bool     IsSelected { get; set; }    
    public string?  StartDir   { get; set; }

    public List<NavigationItem>? ModelItems { get; set; }


    public NavigationItem()
    {
        Id= Guid.NewGuid();
    }
    public bool Equals(NavigationItem? other)
    {
        if(other == null)
        {
            return false;
        } 
        
        if (string.Equals(Id, other.Id))
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
