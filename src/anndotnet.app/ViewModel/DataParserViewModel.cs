using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Anndotnet.Shared.Entities;
using AvaloniaEdit.Document;
using Avalonia.Controls;
using Anndotnet.App.Model;

namespace Anndotnet.App.ViewModel;

public partial class DataParserViewModel : DialogBaseViewModel
{
    [ObservableProperty] private DataParser    _dataParser = new();
    [ObservableProperty] private string        _originData = string.Empty;
    [ObservableProperty] private TextDocument _dataText   = new ();

    [ObservableProperty] private bool _isDecimalPoint ;
    [ObservableProperty] private bool _isDecimalComma ;
    [ObservableProperty] private bool _hasHeader      ;

    [ObservableProperty] private bool   _isSemicolon          ;
    [ObservableProperty] private bool   _isComma              ;
    [ObservableProperty] private bool   _isSpace              ;
    [ObservableProperty] private bool   _isTab                ;
    [ObservableProperty] private bool   _isOtherSeparator     ;
    [ObservableProperty] private string _otherColumnSeparator ;

    public DataParserViewModel()
    {
        _isDecimalPoint = _dataParser.DecimalSeparator == '.';
        _isDecimalComma = _dataParser.DecimalSeparator == ',';
        _hasHeader      = _dataParser.HasHeader;
        _isSemicolon          = _dataParser.ColumnSeparator == ';';
        _isComma              = _dataParser.ColumnSeparator == ',';
        _isSpace              = _dataParser.ColumnSeparator == ' ';
        _isTab                = _dataParser.ColumnSeparator == '\t';
        _isOtherSeparator     = false;
        _otherColumnSeparator = "|";
    }
    public async Task OnLoadedAsync()
    {
        ProcessData();
        await Task.CompletedTask;
    }

    public async Task OnUnLoadedAsync()
    {
        await Task.CompletedTask;
    }
    partial void OnIsDecimalCommaChanged(bool value)
    {
        DataParser.DecimalSeparator = value ? ',' : '.';
        ProcessData();
    }

    partial void OnIsDecimalPointChanged(bool value)
    {
        DataParser.DecimalSeparator = value ? '.' : ',';
        ProcessData();
    }

    partial void OnIsSemicolonChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ';';
            ProcessData();
        }
    }

    partial void OnIsCommaChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ',';
            ProcessData();
        }
    }

    partial void OnIsSpaceChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ' ';
            ProcessData();
        }
    }

    partial void OnIsTabChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = '\t';
            ProcessData();
        }
    }

    partial void OnIsOtherSeparatorChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = OtherColumnSeparator.First();
            ProcessData();
        }
    }

    partial void OnHasHeaderChanged(bool value)
    {
        DataParser.HasHeader = value;
        ProcessData();

    }

    partial void OnOtherColumnSeparatorChanged(string? oldValue, string? newValue)
    {
        if (string.IsNullOrEmpty(OtherColumnSeparator))
        {
            DataParser.ColumnSeparator =' ';
            return;
        }
        DataParser.ColumnSeparator = OtherColumnSeparator.First();
        ProcessData();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
        
        ProcessData();
        CreateHeader();

        DialogWnd.Close(true);
    }

    private void CreateHeader()
    {
        if (DataParser.Header == null)
        {
            var firstValidLine = OriginData.Split(Environment.NewLine).First(x => !x.StartsWith("!"));

            var colCount = firstValidLine.Split(DataParser.ColumnSeparator).Length;
            DataParser.Header = Enumerable.Range(0, colCount).Select(x=>$"Column_{x}").ToArray();
            
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Task.CompletedTask;
        DialogWnd.Close(false);

    }


    private void ProcessData()
    {
        var originLines = OriginData.Split(Environment.NewLine);

        if (string.IsNullOrEmpty(OriginData))
            return;

        var data = string.Join(Environment.NewLine, originLines.Take(1000));

        if (string.IsNullOrEmpty(data))
            return;

        
            
        data = data.Replace(DataParser.ColumnSeparator.ToString(), "\t|\t");

        if (DataParser.HasHeader)
        {
            var index = data.IndexOf(Environment.NewLine,  StringComparison.InvariantCulture);
            var index2 = data.IndexOf(Environment.NewLine, index + 1, StringComparison.InvariantCulture);
            int counter = 0;
            while (counter < index2 - index)
            {
                data = data.Insert(index, "-");
                counter++;
            }
            data = data.Insert(index, Environment.NewLine);
        }

        DataText.Text = data;
    }

}
