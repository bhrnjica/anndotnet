
using System.ComponentModel;
namespace Anndotnet.App.Mvvm.Foundation;
public interface IViewModelBase : INotifyPropertyChanged
{
    Task OnInitializedAsync();
    Task Loaded();
}
