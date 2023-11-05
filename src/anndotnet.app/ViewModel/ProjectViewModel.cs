using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Extensions;
using Anndotnet.Shared.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using Daany;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Anndotnet.App.ViewModel;

public partial class ProjectViewModel : BaseViewModel
{
    private readonly IProjectService _projectService;

    [ObservableProperty] private ProjectModel? _project;

    [ObservableProperty] private ObservableCollection<HeaderInfo>? _metadata =new ObservableCollection<HeaderInfo>();

    public ProjectViewModel(IProjectService projectService)
    {
        _projectService = projectService;
        
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
        var rawdata = _projectService.FromDataParser(projectParser: Project?.Parser);

        Metadata?.Clear();
        List<HeaderInfo>? list = Project?.Metadata;

        var frozen = new HeaderInfo()
         {
            Name = "Column Name: ",
            MlType = "ML Type: ",
            ValueColumnType = "Value Type: ",
            MissingValue = "Missing Handler: ",
            Data = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
            ColMissingValues = new List<string>{},
            ColTypes = new List<string> { },
            ColMlTypes = new List<string> { },
            IsNotFrozen = false
         };  

        Metadata?.Add(frozen);

        for (int i = 0; i < list?.Count; i++)
        {
            HeaderInfo? col = list[i];
            col.Data = rawdata[col.Name].Take(10).Select(x => x.ToString()).ToList();
            Metadata?.Add(col);
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


