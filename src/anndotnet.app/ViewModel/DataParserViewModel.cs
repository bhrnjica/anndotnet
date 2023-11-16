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

namespace Anndotnet.App.ViewModel;

public partial class DataParserViewModel : DialogBaseViewModel
{
    [ObservableProperty] private DataParser    _dataParser = new();
    [ObservableProperty] private string        _originData = string.Empty;
    [ObservableProperty] private TextDocument _dataText   = new ();

    [ObservableProperty] private bool       _isDecimalPoint = true;
    [ObservableProperty] private bool       _isDecimalComma = false;

    [ObservableProperty] private bool _isSemicolon = true;
    [ObservableProperty] private bool _isComma     = false;
    [ObservableProperty] private bool _isSpace     = false;
    [ObservableProperty] private bool _isTab       = false;
    [ObservableProperty] private bool _isOtherSeparator       = false;
    [ObservableProperty] private char _otherColumnSeparator;

    partial void OnIsDecimalCommaChanged(bool value)
    {
        DataParser.DecimalSeparator = value ? ',' : '.';
    }

    partial void OnIsDecimalPointChanged(bool value)
    {
        DataParser.DecimalSeparator = value ? '.' : ',';
    }

    partial void OnIsSemicolonChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ';';
        }
    }

    partial void OnIsCommaChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ',';
        }
    }

    partial void OnIsSpaceChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = ' ';
        }
    }

    partial void OnIsTabChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = '\t';
        }
    }

    partial void OnIsOtherSeparatorChanged(bool value)
    {
        if (value)
        {
            DataParser.ColumnSeparator = OtherColumnSeparator;
        }
    }       

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }

    private void ProcesData()
    {
        var originLines = OriginData.Split(Environment.NewLine);
        
        //


        //if (string.IsNullOrEmpty(OriginData))
        //    return;
        //var data = string.Join(Environment.NewLine, originLines.Take(1000));
        //if (string.IsNullOrEmpty(data))
        //    return;

        //if (checkBox2.Checked)
        //    data = data.Replace(";", "\t|\t");
        //if (checkBox3.Checked)
        //    data = data.Replace(",", "\t|\t");
        //if (checkBox4.Checked)
        //    data = data.Replace(" ", "\t|\t");
        //if (checkBox6.Checked)
        //    data = data.Replace("\t", "\t|\t");
        //if (checkBox5.Checked)
        //{
        //    if (!string.IsNullOrEmpty(textBox2.Text))
        //        data = data.Replace(textBox2.Text[0], '|');
        //}

        ////if header is present separate data with horizontal line
        //if (firstRowHeaderCheck.Checked)
        //{
        //    var index = data.IndexOf(Environment.NewLine);
        //    var index2 = data.IndexOf(Environment.NewLine, index + 1);
        //    int counter = 0;
        //    while (counter < index2 - index)
        //    {
        //        data = data.Insert(index, "-");
        //        counter++;
        //    }
        //    data = data.Insert(index, Environment.NewLine);
        //}


        //textBox3.Text = data;
    }

}
