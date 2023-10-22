

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using Anndotnet.App.Mvvm.Foundation;
using Anndotnet.App.Models;
using Anndotnet.App.Messages;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Mlconfig;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components;
using static Anndotnet.App.Pages.FetchData;
using static System.Net.WebRequestMethods;

namespace Anndotnet.App.ViewModels;

public partial class MainViewModel : RecipientViewModelBase <RunExampleMessage >
{
    private readonly NavigationViewModel _navViewModel;
    private readonly StartPageViewModel  _startPageViewModel;
    private readonly NavigationManager   _navManager;
    private readonly HttpClient          _httpClient;

    [ObservableProperty]
    private AnndotnetModel _currentModel;

    [ObservableProperty] 
    private ObservableCollection<AnndotnetModel> _availableModels;

    public MainViewModel(NavigationViewModel navViewModel, StartPageViewModel startPageViewModel, NavigationManager navManager, HttpClient http, HttpClient httpClient)
    {
        _navViewModel = navViewModel;
        _startPageViewModel = startPageViewModel;
        _navManager = navManager;
        _httpClient = httpClient;
    }

    public override async Task Loaded()
    {
        IsActive = true;
        await LoadPreCalculatedModels();


        await this.CreateStartPageCommand.ExecuteAsync(null);
        _navManager.NavigateTo("start");  
    }

    public override void Receive(RunExampleMessage message)
    {
        var retVal = OpenPrecalculatedModel(message.name);
        retVal.Wait();
        CurrentModel = retVal.Result;
    }

    private async Task<AnndotnetModel> OpenPrecalculatedModel(string messageName)
    {
        var model = AvailableModels.FirstOrDefault(x => string.Equals(x.NavigationItem.Name, messageName, StringComparison.InvariantCultureIgnoreCase));

        if (!_navViewModel.NavigationItems.Any(x => string.Equals(x.Name, model.NavigationItem.Name)) && _navViewModel.NavigationItems.Count <= 5)
        {

            _navViewModel.NavigationItems.Add(model.NavigationItem);
        }
        else if (_navViewModel.NavigationItems.Count >= 5)
        {
            // Dialog.Show<DialogUsageExample_Dialog>("Custom Options Dialog", options);

        }

        await Task.CompletedTask;
        return model;
    }


    private async Task LoadPreCalculatedModels()
    {
        AvailableModels = new ObservableCollection<AnndotnetModel>();
        var jsonOptions = MlFactory.JsonSerializerOptions();

        var slump = new AnndotnetModel
                    {
                        Name = "Slump",
                        Description = "Concrete slump test",
                        MlConfig = await _httpClient.GetFromJsonAsync<MlConfig>("sample-data/slumptest/slumptest.mlconfig", jsonOptions),
                        NavigationItem = new NavigationItem()
                                         {
                                             Name = "Slump",
                                             Icon = "@Icons.Material.Filled.Attractions",
                                             Link = "model-data",
                                             IsExpanded = true,
                                             SubItems = new List<NavigationItem>()
                                                        {
                                                            new NavigationItem()
                                                            {
                                                                Name = "FF3-6-3",
                                                                Icon = "@Icons.Material.Filled.AutoMode",
                                                                Link = "model",
                                                            },
                                                        }
                                         }
                    };


        var iris = new AnndotnetModel
                   {
                       Name = "Iris",
                       Description = "Iris flower recognition",
                       MlConfig = await _httpClient.GetFromJsonAsync<MlConfig>("sample-data/iris/iris.mlconfig", jsonOptions),
                       NavigationItem = new NavigationItem()
                                        {
                                            Name = "Iris",
                                            Icon = "@Icons.Material.Filled.Attractions",
                                            Link = "model-data",
                                            IsExpanded = true,
                                            SubItems = new List<NavigationItem>()
                                                       {
                                                           new NavigationItem()
                                                           {
                                                               Name = "FF3-6-3",
                                                               Icon = "@Icons.Material.Filled.AutoMode",
                                                               Link = "model",
                                                           },
                                                           new NavigationItem()
                                                           {
                                                               Name = "FF3-6-3",
                                                               Icon = "@Icons.Material.Filled.AutoMode",
                                                               Link = "model",
                                                           },
                                                       }
                                        }
                   };

        var titanic = new AnndotnetModel
                      {
                          Name = "Titanic",
                          Description = "Titanic survival prediction",
                          MlConfig = await _httpClient.GetFromJsonAsync<MlConfig>("sample-data/titanic/titanic.mlconfig", jsonOptions),
                          NavigationItem = new NavigationItem()
                                           {
                                               Name = "Titanic",
                                               Icon = "@Icons.Material.Filled.Attractions",
                                               Link = "model-data",
                                               IsExpanded = true,
                                               SubItems = new List<NavigationItem>()
                                                          {
                                                              new NavigationItem()
                                                              {
                                                                  Name = "FF3-6-3",
                                                                  Icon = "@Icons.Material.Filled.AutoMode",
                                                                  Link = "model",
                                                              },
                                                          }
                                           }
                      };

        AvailableModels.Add(slump);
        AvailableModels.Add(iris);
        AvailableModels.Add(titanic);

    }

    [RelayCommand]
    protected virtual Task CreateStartPage()
    {
        var home = new NavigationItem()
                   {
                       Name = "Start Page",
                       Icon = "@Icons.Material.Filled.Home",
                       Link = "start",
                       SubItems = new List<NavigationItem>(),
                   };

        if (!_navViewModel.NavigationItems.Any(x =>
                string.Equals(x.Name, "Start Page", StringComparison.InvariantCultureIgnoreCase)))
        {
            _navViewModel.NavigationItems.Add(home);
        }
        return Task.CompletedTask;
    }
}
