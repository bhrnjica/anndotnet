

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

using Anndotnet.App.Mvvm.Foundation;
using Anndotnet.App.Models;
using Anndotnet.App.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Anndotnet.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly NavigationViewModel _navViewModel;
    private readonly StartPageViewModel  _startPageViewModel;

    [ObservableProperty]
    private AnndotnetModel _currentModel;

    [ObservableProperty] 
    private ObservableCollection<AnndotnetModel> _availableModels;


    public MainViewModel(NavigationViewModel navViewModel, StartPageViewModel startPageViewModel)
    {
        _navViewModel = navViewModel;
        _startPageViewModel = startPageViewModel;
    }

    public override async Task Loaded()
    {
        
        await this.CreateStartPageCommand.ExecuteAsync(null);
        await Task.CompletedTask;
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

        _navViewModel.NavigationItems.Add(home);
        return Task.CompletedTask;
    }
}
