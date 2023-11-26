using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Extensions;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daany;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Anndotnet.App.ViewModel;

public partial class ProjectViewModel : BaseViewModel
{
    private readonly IProjectService _projectService;
    private readonly IDialogService _dlgService;
    private readonly IWindowService _wndService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty] private ProjectModel? _project;
    [ObservableProperty] private ObservableCollection<HeaderInfo>? _metadata = new ObservableCollection<HeaderInfo>();
    [ObservableProperty] private int? _selectedSummaryIndex;
    [ObservableProperty] private int? _selectedDataIndex;
    [ObservableProperty] private bool? _showParserDataDialog;

    public ProjectViewModel(IProjectService projectService,
                            IDialogService dlgService,
                            IWindowService wndService,
                            MainViewModel mainViewMode)
    {
        _projectService = projectService;
        _dlgService = dlgService;
        _wndService = wndService;
        _mainViewModel = mainViewMode;
    }

    public async Task OnLoadedAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnUnLoadedAsync()
    {
        await Task.CompletedTask;
    }

    private async Task<bool> SaveProjectAsync(ProjectModel? oldProject)
    {
        if (oldProject == null)
        {
            return false;
        }


        var itm = _mainViewModel.TreeNavigationItems?.Where(x => x.Id.Equals(oldProject?.Id)).FirstOrDefault() ?? throw new NullReferenceException("navigationItem");

        var proj = oldProject ?? throw new ArgumentNullException(nameof(oldProject));
        var itmVal = itm ?? throw new ArgumentNullException(nameof(oldProject));

        return await _projectService.SaveProjectAsync(oldProject, itmVal);
    }

    partial void OnProjectChanged(ProjectModel? oldValue, ProjectModel? newValue)
    {
        var app = Avalonia.Application.Current as Anndotnet.App.App;
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Wait);

            try
            {
                if (oldValue != null)
                {
                    SaveProjectAsync(oldValue!).ConfigureAwait(false);
                }

                if (newValue != null)
                {
                    LoadProject(newValue!);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                desktop.MainWindow.Cursor = null;
            }
        }

    }

    void LoadProject(ProjectModel? project)
    {
        if (project == null)
        {
            return;
        }

        var itm = _mainViewModel.TreeNavigationItems?.Where(x => x.Id.Equals(project.Id)).FirstOrDefault();

        if (itm == null)
        {
            throw new InvalidEnumArgumentException(nameof(itm));
        }

        Metadata?.Clear();

        if (project.Parser == null)
        {
            return;
        }


        var relativeDir = Path.GetDirectoryName(itm.Link) ?? throw new ArgumentNullException("Path.GetDirectoryName(itm.Link)");

        var currentDir = Path.Combine(itm!.StartDir, relativeDir);

        var rawData = _projectService.FromDataParser(projectParser: project.Parser, currentDir);

        if (project.Metadata == null || project.Metadata.Count == 0)
        {
            var labelColumn = rawData.Columns.Last();
            project.Metadata = rawData.MetadataFromDataFrame(labelColumn);
        }
        var list = project?.Metadata;

        //create description from rawData and add to summary
        var description = rawData.Describe(false, rawData.Columns.ToArray());
        var headerSummary = description.Index.Select(x => x.ToString()).ToList();
        var dataSummary = description.GetEnumerator().ToList();
        if (dataSummary == null)
        {
            throw new Exception("Data summary is null");
        }

        var frozen = new HeaderInfo()
        {
            Name = "Column Name: ",
            MlType = "ML Type: ",
            ValueColumnType = "Value Type: ",
            MissingValue = "Missing Handler: ",
            Data = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
            SummaryData = headerSummary!,
            SummaryHeader = "Summary",
            ColMissingValues = new List<string> { },
            ColTypes = new List<string> { },
            ColMlTypes = new List<string> { },
            IsNotFrozen = false
        };

        Metadata?.Add(frozen);


        for (int i = 0; i < list?.Count; i++)
        {
            HeaderInfo? col = list[i];
            string? colName = col.Name;
            if (string.IsNullOrEmpty(colName))
            {
                throw new Exception("Column name is empty");
            }

            col.Data = rawData[colName].Take(10).Select(x => x.ToString()).ToList()!;

            col.SummaryData = dataSummary.Select(x =>
            {
                var v = x.Values.ElementAt(i);
                return v == null! ? "n/a" : v.ToString();
            }).ToList();

            Metadata?.Add(col);
        }

    }

    [RelayCommand]
    private async Task LoadExperimentDataAsync()
    {

        //var file = await _dlgService.FileOpen("Load data file", "(.txt)");
        var file = await _dlgService.FileOpen("Load data file", "data file");

        if (file == null)
        {
            await Task.CompletedTask;
            return;
        }

        // Open writing stream from the file.
        await using var stream = await file.OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        // Write some content to the file.
        var strData = await streamReader.ReadToEndAsync();



        var app = Avalonia.Application.Current as Anndotnet.App.App;
        var dlgViewModel = app.Services.GetRequiredService<DataParserViewModel>();
        var dlgView = app.Services.GetRequiredService<DataParserView>();

        dlgViewModel.OriginData = strData;
        dlgViewModel.DataText.Text = strData;
        dlgView.DataContext = dlgViewModel;

        var result = await _wndService.ShowDialog<DataParserViewModel, DataParserView>(dlgViewModel, dlgView);

        if (result != null && result.Value)//load experimental data to the project
        {
            if (Project != null)//this should always be the true
            {
                var dataPath = Path.Combine(_mainViewModel.SelectedItem!.StartDir, Path.GetFileName(file.Path.LocalPath));
                var f1 = new FileInfo(dataPath);
                var f2 = new FileInfo(file.Path.LocalPath);
                if (!string.Equals(f1.FullName, f2.FullName, StringComparison.InvariantCulture))
                {
                    File.Copy(file.Path.LocalPath, dataPath, true);
                }

                Project.Parser = dlgViewModel.DataParser;
                Project.Parser.FileName = Path.GetFileName(file.Path.LocalPath);
                //reset metadata when the experimental data is loaded
                Project.Metadata = null;

                LoadProject(Project);
            }
        }
    }

    private string ResolveColType(ColType colType)
    {
        switch (colType)
        {
            case ColType.STR:
            case ColType.DT:
                return "String";
            case ColType.IN:
            case ColType.I2:
                return "Categorical";
            case ColType.I32:
            case ColType.I64:
                return "Integer";
            case ColType.F32:
            case ColType.DD:
                return "Float";
            default:
                return "String";
        }
    }
}


