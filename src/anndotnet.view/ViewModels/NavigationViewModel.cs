

using Anndotnet.App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Anndotnet.App.ViewModels;

public partial class NavigationViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems= new();

    public override async Task Loaded()
    {
        NavigationItems.CollectionChanged += NavigationItems_CollectionChanged;
        var home = new NavigationItem()
        {
            Name = "Start Page",
            //Icon = "@Icons.Material.Filled.Attractions",
            Link = "", 
            SubItems = new List<NavigationItem>(),
        };

        NavigationItems.Add(home);
        
        await Task.CompletedTask;
    }

    private void NavigationItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
       OnPropertyChanged(nameof(NavigationItems));
    }
}
