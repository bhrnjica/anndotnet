using Anndotnet.App.ViewModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anndotnet.App;

public partial class MlModelView : UserControl
{
    public MlModelView()
    {
        InitializeComponent();
    }

    public MlModelView(MlModelViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}