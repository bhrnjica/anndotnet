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

public partial class NavigationViewModel : RecipientViewModelBase <RunExampleMessage >
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems = new();



    public NavigationViewModel()
    {
  
    }


    public override async Task Loaded()
    {
        IsActive= true;
        NavigationItems.CollectionChanged += NavigationItems_CollectionChanged;

        await Task.CompletedTask;
    }

    public override void Receive(RunExampleMessage message)
    {
        _ = OpenPrecalculatedModel(message.name);
    }

    private async Task OpenPrecalculatedModel(string messageName)
    {
        var iris = new NavigationItem()
                   {
                       Name = "Iris",
                       //Icon = "@Icons.Material.Filled.Attractions",
                       Link = "model-data",
                       IsExpanded = true,
                       SubItems = new List<NavigationItem>()
                                  {
                                      new NavigationItem()
                                      {
                                          Name = "FF3-6-3",
                                          //Icon = "@Icons.Material.Filled.AutoMode",
                                          Link = "model",
                                      },
                                      new NavigationItem()
                                      {
                                          Name = "Conv-6-3",
                                          // Icon = "@Icons.Material.Filled.AutoAwesomeMosaic",
                                          Link = "model",
                                      }
                                  }
                   };
        if (!NavigationItems.Any(x => x.Name == iris.Name) && NavigationItems.Count <= 5)
        {
            NavigationItems.Add(iris);
        }
        else if (NavigationItems.Count >= 5)
        {
           // Dialog.Show<DialogUsageExample_Dialog>("Custom Options Dialog", options);

        }

        await Task.CompletedTask;
    }

    private void NavigationItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(NavigationItems));
    }

   
}