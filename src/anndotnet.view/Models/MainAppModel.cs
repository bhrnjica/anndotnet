
using CommunityToolkit.Mvvm.ComponentModel;

namespace Anndotnet.App.Models;

public partial class MainAppModel:ObservableObject
{
    [ObservableProperty]
    private List<NavigationItem>? _navigationItems;

    [ObservableProperty]
    private NavigationItem? _selectedItem;
}
