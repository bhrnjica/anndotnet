using Anndotnet.App.ViewModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Anndotnet.App;

public partial class ProjectView : UserControl
{
    public ProjectView()
    {
        InitializeComponent();
        
    }
    public ProjectView(ProjectViewModel viewModel)
    {
        this.DataContext = viewModel;
        InitializeComponent();
    }
    
}