using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Anndotnet.App.ViewModel;

public partial class MlModelViewModel : BaseViewModel
{
    [ObservableProperty] private MlModel? _mlModel;
    public async Task OnLoadedAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnUnLoadedAsync()
    {
        await Task.CompletedTask;
        return;
    }
}
