

using Anndotnet.App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Anndotnet.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems= new();

    public override async Task Loaded()
    {
        var home = new NavigationItem()
        {
            Name = "Start Page",
            //Icon = "@Icons.Material.Filled.Attractions",
            Link = "",            
        };

        NavigationItems.Add(home);

        await Task.CompletedTask;
    }
}
