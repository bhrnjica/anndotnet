using System.Collections.ObjectModel;
using System.ComponentModel;
using Anndotnet.App.Messages;
using Anndotnet.App.Mvvm.Foundation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Xml.Linq;
using Anndotnet.App.Models;
using Anndotnet.Core.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using Anndotnet.App.Extensions;

namespace Anndotnet.App.ViewModels;


public partial class ModelDataViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ModelDataMetaData> _metaData;

    [ObservableProperty]
    private AnndotnetModel _annModel;

    public ModelDataViewModel(MainViewModel mainViewModel)
    {
        AnnModel = mainViewModel.CurrentModel;
        MetaData = CreateMetaData(AnnModel.MlConfig.Metadata);
        //MetaData = LoadRandomMetaData();
    }

    private ObservableCollection<ModelDataMetaData> CreateMetaData(List<ColumnInfo> mlConfigMetadata)
    {
        var metaData = new ObservableCollection<ModelDataMetaData>();
        foreach (var col in mlConfigMetadata)
        {
            metaData.Add(new ModelDataMetaData
                         {
                             Id = col.Id,
                             Title = col.Name,
                             Description = col.Name,
                             MlDataType = col.MLType.Map(),
                             ValueType = col.ValueColumnType.Map(),
                             MissingValue = col.MissingValue.Map(),
                         });
        }   

        return metaData;
    }


    public override async Task Loaded()
    {
       //var model = _mainViewModel.CurrentModel;
       await Task.CompletedTask;
    }

    
    public void OnColumnTypeChanged(ModelDataMetaData col, ColValueType colType)
    {
        switch (colType)
        {
            case ColValueType.Numeric:
                col.ValueType = ColValueType.Numeric;
                break;
            case ColValueType.Categoric:
                col.ValueType = ColValueType.Categoric;
                break;
            case ColValueType.None:
                col.ValueType = ColValueType.None;
                break;
        }   
    }
    public void OnMlDataTypeChanged(ModelDataMetaData col, ColMlDataType mlDataType)
    {
        switch (mlDataType)
        {
            case ColMlDataType.Feature:
                col.MlDataType = ColMlDataType.Feature;
                break;
            case ColMlDataType.Label:
                col.MlDataType = ColMlDataType.Label;
                break;
            case ColMlDataType.Ignore:
                col.MlDataType = ColMlDataType.Ignore;
                break;
        }   
    }
    public void OnMissingValueChanged(ModelDataMetaData col, ColMissingValue missingValue)
    {
        switch (missingValue)
        {
            case ColMissingValue.None:
                col.MissingValue = ColMissingValue.None;
                break;
            case ColMissingValue.Random:
                col.MissingValue = ColMissingValue.Random;
                break;
            case ColMissingValue.Average:
                col.MissingValue = ColMissingValue.Average;
                break;
        }   
    }
    private static ObservableCollection<ModelDataMetaData> LoadRandomMetaData()
    {
        return new ObservableCollection<ModelDataMetaData>()
               {
                   new ModelDataMetaData
                   {
                       Id = 1,
                       Title = "Column 1",
                       Description = "Column 1 description",
                       MlDataType = ColMlDataType.Feature,
                       ValueType = ColValueType.Numeric,
                       MissingValue = ColMissingValue.None,

                   },
                   new ModelDataMetaData
                   {
                       Id = 2,
                       Title = "Column 2",
                       Description = "Column 2 description",
                       MlDataType = ColMlDataType.Feature,
                       ValueType = ColValueType.Categoric,
                       MissingValue =ColMissingValue.Random,

                   },
                   new ModelDataMetaData
                   {
                       Id = 3,
                       Title = "Column 3",
                       Description = "Column 3 description",
                       MlDataType = ColMlDataType.Feature,
                       ValueType = ColValueType.Numeric,
                       MissingValue = ColMissingValue.Average,

                   },
                   new ModelDataMetaData
                   {
                       Id = 4,
                       Title = "Column 4",
                       Description = "Column 4 description",
                       MlDataType = ColMlDataType.Ignore,
                       ValueType = ColValueType.Numeric,
                       MissingValue = ColMissingValue.None

                   },
                   new ModelDataMetaData
                   {
                       Id = 5,
                       Title = "Column 5",
                       Description = "Column 5 description",
                       MlDataType = ColMlDataType.Label,
                       ValueType = ColValueType.Numeric,
                       MissingValue = ColMissingValue.Random,

                   }

               };
    }

}
