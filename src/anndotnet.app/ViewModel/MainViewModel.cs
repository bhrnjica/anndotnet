using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;


namespace Anndotnet.App.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    private readonly IProjectService _projectService;  

    [ObservableProperty]
    private ObservableCollection<ProjectItem> _treeNavigationItems = new ();

    [ObservableProperty]
    private ProjectItem _selectedItem;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private Control _content;

    public MainViewModel(IProjectService projectService)
    {
        _projectService = projectService;

    }

    public async Task OnLoadedAsync()
    {
       await LoadStartPageAsync();
       await LoadIrisProject();
       SelectedItem = TreeNavigationItems.Skip(1).FirstOrDefault();
    }

    partial void OnSelectedItemChanged(ProjectItem value)
    {
        var app = Avalonia.Application.Current as Anndotnet.App.App;

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
        TreeNavigationItems.Add(startPage);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task LoadIrisProject()
    {
        var irisProject = _projectService.LoadIrisProject();
        TreeNavigationItems.Add(irisProject);
    }


}

