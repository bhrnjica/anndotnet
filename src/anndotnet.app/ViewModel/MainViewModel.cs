using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Message;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;


namespace Anndotnet.App.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    private readonly AppModel        _appModel;
    private readonly IProjectService _projectService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<NavigationItem> _treeNavigationItems = new ();

    [ObservableProperty]
    private NavigationItem? _selectedItem;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private Control? _content;

    public MainViewModel(IProjectService projectService, INavigationService navigationService, AppModel appModel)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _appModel = appModel;

        WeakReferenceMessenger.Default.Register<InsertNavigationItemMessage>(this, (r, m) =>
        {
            OnInsertNavigationItem(m.Value);
        });

        LoadStartPageAsync().Wait();

    }

    public async Task OnLoadedAsync()
    {

        await Task.CompletedTask;
    }

    private void OnInsertNavigationItem(NavigationItem itm)
    {
        var navItm = TreeNavigationItems.FirstOrDefault(x => string.Equals(x.Name, itm.Name));

        if (navItm == null)
        {
            TreeNavigationItems.Add(itm);
            navItm = TreeNavigationItems.Last();
        }

        SelectedItem = itm;
    }

    partial void OnSelectedItemChanged(NavigationItem? value)
    {
        var app = Avalonia.Application.Current as Anndotnet.App.App;
        if (app == null)
        {
            throw new Exception("Application is null");
        }
        if (app.Services == null)
        {
            throw new Exception("Service provider is null");
        }

        if (value == null)
        {
            throw new Exception("'value' cannot be null");
        }

        if (value.ItemType == ItemType.Start)
        {
            Content = app.Services.GetRequiredService<StartView>();
        }
        else if (value.ItemType == ItemType.Project)
        {
            var projectModel = _projectService.LoadProject(value.Link!);
            
            var projectView = app.Services.GetRequiredService<ProjectView>();
            var projectViewModel = app.Services.GetRequiredService<ProjectViewModel>();

            projectViewModel.Project = projectModel;
            Content = projectView;
        }
        else if (value.ItemType == ItemType.Model)
        {
            var mlModel = _projectService.LoadMlModel(value.Link!);

            var mlModelViewModel = app.Services.GetRequiredService<MlModelViewModel>();
            var modelView = app.Services.GetRequiredService<MlModelView>();

            mlModelViewModel.MlModel = mlModel;
            Content = modelView;
        }
        else
        {
            //here comes more views
        }
    }

   

    [RelayCommand]
    private async Task LoadStartPageAsync()
    {
        var startPage = _navigationService.StartPageItem();
        OnInsertNavigationItem(startPage);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CreateNewProjectAsync()
    {
        var box = MessageBoxManager
           .GetMessageBoxStandard(_appModel.AppFullName, "Create new project", ButtonEnum.YesNo);

        var result = await box.ShowAsync();
    }
    [RelayCommand]
    private async Task OpenExistingProjectAsync()
    {
        var box = MessageBoxManager
           .GetMessageBoxStandard(_appModel.AppFullName, "Open Existing project", ButtonEnum.YesNo);

        var result = await box.ShowAsync();
    }

    [RelayCommand]
    private async Task SaveCurrentProjectAsync()
    {
        var box = MessageBoxManager
           .GetMessageBoxStandard(_appModel.AppFullName, "Save current project", ButtonEnum.YesNo);

        var result = await box.ShowAsync();
    }

    [RelayCommand]
    private async Task CloseCurrentProjectAsync()
    {
        var mp = new MessageBoxCustomParams()
                 {
                     Width = 450, Height = 200,
                     WindowStartupLocation = WindowStartupLocation.CenterScreen,
                     ContentTitle = _appModel.AppFullName,
                     ContentMessage = "Close current project",
                     ButtonDefinitions = new ButtonDefinition[1]{new ButtonDefinition(){Name = "Ok", IsDefault = true}},
                     Icon = Icon.Info
                 };


        var box = MessageBoxManager.GetMessageBoxCustom(mp);
          

        var result = await box.ShowAsync();

    }
}

