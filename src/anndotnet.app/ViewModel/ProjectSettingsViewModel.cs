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

public partial class ProjectSettingsViewModel : DialogBaseViewModel
{
   
    [ObservableProperty] private string _originData = string.Empty;
    [ObservableProperty] private string _dataText   = string.Empty;



    public ProjectSettingsViewModel()
    {
        
    }
    
}
