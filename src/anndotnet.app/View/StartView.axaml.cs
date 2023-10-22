using Anndotnet.App.ViewModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Anndotnet.App;

public partial class StartView : UserControl
{
    public StartView()
    {
        InitializeComponent();
        DataContext = this;
    }
    public StartView(StartViewModel viewModel)

    {
        DataContext = viewModel;
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);   
 
    }

}