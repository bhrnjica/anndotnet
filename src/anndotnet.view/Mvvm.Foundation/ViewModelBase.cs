
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Anndotnet.App.Mvvm.Foundation;

public abstract partial class ViewModelBase : ObservableObject, IViewModelBase
{

    public virtual async Task OnInitializedAsync()
    {
        await Loaded().ConfigureAwait(false);
    }

    protected virtual void NotifyStateChanged() => OnPropertyChanged((string?)null);

    [RelayCommand]
    public virtual async Task Loaded()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
