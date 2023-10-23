using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Message;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;


namespace Anndotnet.App.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    private readonly IProjectService _projectService;  

    [ObservableProperty]
    private ObservableCollection<NavigationItem> _treeNavigationItems = new ();

    [ObservableProperty]
    private NavigationItem? _selectedItem;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private Control? _content;

    public MainViewModel(IProjectService projectService)
    {
        _projectService = projectService;
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
        if (app.ServiceProvider == null)
        {
            throw new Exception("Service provider is null");
        }

        if (value == null)
        {
            throw new Exception("'value' cannot be null");
        }
        if (value.ItemType == ItemType.Start)
        {
            Content = app.ServiceProvider.GetRequiredService<StartView>();
        }
        else if (value.ItemType == ItemType.Project)
        {
            Content = app.ServiceProvider.GetRequiredService<ProjectView>();
        }
        else if (value.ItemType == ItemType.Model)
        {
            Content = app.ServiceProvider.GetRequiredService<MlModelView>();
        }
        else
        {
            //here comes more views
        }
    }
   
    [RelayCommand]
    private async Task LoadStartPageAsync()
    {

        var startPage = _projectService.LoadStartPage();
        OnInsertNavigationItem(startPage);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task LoadIrisProject()
    {
        var irisProject = _projectService.LoadIrisProject();
        TreeNavigationItems.Add(irisProject);
        await Task.CompletedTask;
    }


}

