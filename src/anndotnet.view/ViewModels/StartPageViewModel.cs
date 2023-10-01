using Anndotnet.App.Messages;
using Anndotnet.App.Mvvm.Foundation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Xml.Linq;

namespace Anndotnet.App.ViewModels;

public enum Examples
{
    Slump,
    Iris,
    New,
    Open,
    Close
}


public partial class StartPageViewModel : RecipientViewModelBase <CreatePageMessage>
{
    private readonly NavigationViewModel _navViewModel;

    public StartPageViewModel(NavigationViewModel navViewModel)
    {
        _navViewModel = navViewModel;
    }

    public override async Task Loaded()
    {
       await Task.CompletedTask;
    }

    public override void Receive(CreatePageMessage message)
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    public virtual Task RunExample(Examples example)
    {

        switch (example)
        {
            case Examples.Slump:
                
                Messenger.Send(new RunExampleMessage("Slump"));

                break;
        }
       return Task.CompletedTask;
    }
}
