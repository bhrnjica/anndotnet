using System.Collections.ObjectModel;
using Anndotnet.App.Messages;
using Anndotnet.App.Models;
using Anndotnet.App.Mvvm.Foundation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;

namespace Anndotnet.App.ViewModels;

public partial class NavigationViewModel : RecipientViewModelBase <RunExampleMessage >
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems = new();

    public override async Task Loaded()
    {
        NavigationItems.CollectionChanged += NavigationItems_CollectionChanged;

        await Task.CompletedTask;
    }

    public override void Receive(RunExampleMessage message)
    {
        
    }

    private void NavigationItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(NavigationItems));
    }

   
}