using System.Collections.ObjectModel;
using Anndotnet.App.Components;
using Anndotnet.App.Messages;
using Anndotnet.App.Models;
using Anndotnet.App.Mvvm.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Anndotnet.App.ViewModels;

public partial class NavigationViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems = new();



    public NavigationViewModel()
    {
  
    }


    public override async Task Loaded()
    {

        NavigationItems.CollectionChanged += NavigationItems_CollectionChanged;

        await Task.CompletedTask;
    }

   

    private void NavigationItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(NavigationItems));
    }

   
}