﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Extensions;
using Anndotnet.Shared.Entities;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daany;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Anndotnet.App.ViewModel;

public partial class ProjectViewModel : BaseViewModel
{
    private readonly             IProjectService _projectService;
    private readonly             IDialogService  _dlgService;
    private readonly             IWindowService  _wndService;
    [ObservableProperty] private ProjectModel?   _project;
    [ObservableProperty] private ObservableCollection<HeaderInfo>? _metadata = new ObservableCollection<HeaderInfo>();
    [ObservableProperty] private int? _selectedSummaryIndex;
    [ObservableProperty] private int? _selectedDataIndex;
    [ObservableProperty] private bool? _showParserDataDialog;

    public ProjectViewModel(IProjectService projectService, 
                            IDialogService dlgService, 
                            IWindowService wndService)
    {
        _projectService = projectService;
        _dlgService = dlgService;
        _wndService = wndService;
    }

    public async Task OnLoadedAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnUnLoadedAsync()
    {
        await Task.CompletedTask;
        return;
    }

    partial void OnProjectChanged(ProjectModel? oldValue, ProjectModel? newValue)
    {
        LoadData();
    }

    void LoadData()
    {
        //unzip the file
        var rawData = _projectService.FromDataParser(projectParser: Project?.Parser);

        Metadata?.Clear();
        var list = Project?.Metadata;

        //create description from rawData and add to summary
        var description = rawData.Describe(false, rawData.Columns.ToArray());
        var headerSummary = description.Index.Select(x => x.ToString()).ToList();
        var dataSummary = description.GetEnumerator();
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
            SummaryHeader= "Summary",
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
    private async Task LoadProjectDataAsync()
    {
        
        //var file = await _dlgService.FileOpen("Load data file", "(.txt)");
        var file = await _dlgService.FileOpen("Load data file", "data file");

        if (file == null)
        {
            await Task.CompletedTask;
        }

        if (file is null)
        {
            throw new Exception("File is null");    
        }

        // Open writing stream from the file.
        await using var stream = await file.OpenReadAsync();
        using var streamReader = new StreamReader(stream);
        // Write some content to the file.
        var strData = await streamReader.ReadToEndAsync();

        // var project = _projectService.ParseData(file.);

        var app = Avalonia.Application.Current as Anndotnet.App.App;
        var dlgViewModel = app.Services.GetRequiredService<DataParserViewModel>();
        var dlgView = app.Services.GetRequiredService<DataParserView>();

        dlgViewModel.OriginData = strData;
        dlgViewModel.DataText.Text = strData;
        dlgView.DataContext = dlgViewModel;
        var result = await _wndService.ShowDialog(dlgView,dlgViewModel);
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


