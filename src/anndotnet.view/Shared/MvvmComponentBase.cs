
using Anndotnet.App.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Anndotnet.App.Shared;

public abstract class MvvmComponentBase<TViewModel> : ComponentBase
    where TViewModel : IViewModelBase
{
    [Inject]
    [NotNull]
#pragma warning disable CS8618
    protected TViewModel ViewModel { get; set;}
#pragma warning restore CS8618

    protected override void OnInitialized()
    {
        //Cause changes to the ViewModel to make Blazor re render
        ViewModel.PropertyChanged += (_, _) => StateHasChanged();
        base.OnInitialized();
    }

    protected override Task OnInitializedAsync()
    {
        return ViewModel.OnInitializedAsync();
    }
}
