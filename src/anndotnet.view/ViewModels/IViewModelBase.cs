
using System.ComponentModel;
namespace Anndotnet.App.ViewModels;
public interface IViewModelBase:INotifyPropertyChanged
{
    Task OnInitializedAsync();
    Task Loaded();
}
