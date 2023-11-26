using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    private readonly IDialogService _dialogService;


    [ObservableProperty]
    private ObservableCollection<NavigationItem> _treeNavigationItems = new ();

    [ObservableProperty]
    private NavigationItem? _selectedItem;

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private Control? _content;

    public MainViewModel(IProjectService projectService, INavigationService navigationService, AppModel appModel, IDialogService dialogService)
    {
        _projectService = projectService;
        _navigationService = navigationService;
        _appModel = appModel;
        _dialogService = dialogService;

        WeakReferenceMessenger.Default.Register<InsertNavigationItemMessage>(this, (r, m) =>
        {
            OnInsertNavigationItem(m.Value);
        });

    }

    public async Task OnLoadedAsync()
    {
 
        await Task.CompletedTask;
    }

    private void OnInsertNavigationItem(NavigationItem itm)
    {
        var navItm = TreeNavigationItems.FirstOrDefault(x => string.Equals(x, itm));

        if (navItm == null)
        {
            TreeNavigationItems.Add(itm);
            navItm = TreeNavigationItems.Last();
        }

        SelectedItem = itm;
    }

    private void OnRemoveNavigationItem(NavigationItem itm)
    {
        var navItm = TreeNavigationItems.FirstOrDefault(x => string.Equals(x, itm));

        if (navItm != null)
        {
            SelectedItem = TreeNavigationItems.First();
            TreeNavigationItems.Remove(navItm);
        }
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
            //force the project to be saved on disk before starView is shown
            var projectViewModel = app.Services.GetRequiredService<ProjectViewModel>();
            projectViewModel.Project = null;

            //force the project to be saved on disk before starView is shown
            var mlConfigViewModel = app.Services.GetRequiredService<MlModelViewModel>();
            mlConfigViewModel.MlModel = null;

                        
            Content = app.Services.GetRequiredService<StartView>();
        }
        else if (value.ItemType == ItemType.Project)
        {
            var task = _projectService.LoadProjectAsync(value.Link!, value.StartDir);
            task.Wait();
            var projectModel = task.Result;
            projectModel.Id=value.Id;
           
            var projectView = app.Services.GetRequiredService<ProjectView>();
            var projectViewModel = app.Services.GetRequiredService<ProjectViewModel>();

            projectViewModel.Project = projectModel;
            Content = projectView;
        }
        else if (value.ItemType == ItemType.Model)
        {
            //force the project to be saved on disk before starView is shown
            var projectViewModel = app.Services.GetRequiredService<ProjectViewModel>();
            projectViewModel.Project = null;

            var mlModel = _projectService.LoadMlModel(value.Link!, value.StartDir);

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
        try
        {
            var folder = await _dialogService.OpenFolder("New Project Folder");

            if (folder != null)
            {
                var files = folder.GetItemsAsync();

                //check if folder is empty
                await foreach (var f in files)
                {
                    throw new Exception("Please select an empty folder.");
                }
                //create ann project file in the directory
                var file = await folder.CreateFileAsync("ann_new_project.ann");

               
                var itm = _navigationService.CreateNavigationItem(file!.Path);
                var newProject = new ProjectModel
                                 {
                                     Name = Path.GetFileNameWithoutExtension(file.Path.LocalPath)
                                 };

                await _projectService.SaveProjectAsync(newProject, itm);
                OnInsertNavigationItem(itm);
            }
        }
        catch (Exception e)
        {
            var box = MessageBoxManager
               .GetMessageBoxStandard(_appModel.AppFullName, e.Message, ButtonEnum.Ok);

            var result = await box.ShowAsync();
        }
       
        
    }
    [RelayCommand]
    private async Task OpenExistingProjectAsync()
    {
        var file = await _dialogService.FileOpen("Open Project file", "(.ann)");

        if (file == null)
        {
            await Task.CompletedTask;
            return;
        }

        if (file is null)
        {
            throw new Exception("File is null");
        }

        NavigationItem itm = _navigationService.CreateNavigationItem(file.Path);
        
        OnInsertNavigationItem(itm);
    }


    

    [RelayCommand]
    private async Task SaveCurrentProjectAsync()
    {
        var folder = await _dialogService.OpenFolder("Save current project to different location");

        if (folder != null)
        {
            var files = folder.GetItemsAsync();

            //check if folder is empty
            await foreach (var f in files)
            {
                throw new Exception("Please select an empty folder.");
            }
            //TO DO
            //1.) copy all content from the current folder to the new folder
            //2.) close current project 
            //3) open saved project


            ////create ann project file in the directory
            //var file = await folder.CreateFileAsync("ann_new_project.ann");


            //var itm = _navigationService.CreateNavigationItem(file!.Path);
            //var newProject = new ProjectModel
            //                 {
            //                     Name = Path.GetFileNameWithoutExtension(file.Path.LocalPath)
            //                 };

            //await _projectService.SaveProjectAsync(newProject, itm);
            //OnInsertNavigationItem(itm);
        }
    }

    [RelayCommand]
    private async Task CloseCurrentProjectAsync()
    {
        if (SelectedItem!.Id.Equals(new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)))
        {
            return;
        }

        if (SelectedItem!.ItemType == ItemType.Model)
        {
            var box = MessageBoxManager
               .GetMessageBoxStandard(_appModel.AppFullName, "The selected mlconfig file is part of the Project. Select project to close.", ButtonEnum.Ok);

            var result = await box.ShowAsync();
            return;
        }

        OnRemoveNavigationItem(SelectedItem);

    }
}

