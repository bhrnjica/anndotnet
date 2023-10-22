
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Anndotnet.App.Model;

public partial class MainAppModel:ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ProjectItem>? _navigationItems;

    [ObservableProperty]
    private ProjectItem? _selectedItem;
}
