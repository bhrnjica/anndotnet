

using Anndotnet.App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Anndotnet.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly NavigationViewModel _navViewModel;
    public MainViewModel(NavigationViewModel navViewModel)
    {
        _navViewModel = navViewModel;
    }
    //[ObservableProperty]
    //private ObservableCollection<NavigationItem> _navigationItems= new();

    public override async Task Loaded()
    {
        var home = new NavigationItem()
        {
            Name = "Start Page",
            //Icon = "@Icons.Material.Filled.Attractions",
            Link = "", 
            SubItems = new List<NavigationItem>(),
        };

        _navViewModel.NavigationItems.Add(home);

        await Task.CompletedTask;
    }
}
