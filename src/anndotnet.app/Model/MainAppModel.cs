﻿
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Anndotnet.App.Model;

public partial class MainAppModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<NavigationItem>? _navigationItems;

    [ObservableProperty]
    private NavigationItem? _selectedItem;
}
