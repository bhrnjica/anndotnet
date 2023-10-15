using System.ComponentModel;
using Anndotnet.App.Messages;
using Anndotnet.App.Mvvm.Foundation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Xml.Linq;
using Anndotnet.App.Models;

namespace Anndotnet.App.ViewModels;




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
    public virtual void RunExample(Examples example)
    {

        switch (example)
        {
            case Examples.Slump:
                
                Messenger.Send(new RunExampleMessage(Examples.Slump.ToString()));

                break;
            case Examples.Titanic:

                Messenger.Send(new RunExampleMessage(Examples.Titanic.ToString()));

                break;
            case Examples.Iris:

                Messenger.Send(new RunExampleMessage(Examples.Iris.ToString()));

                break;
            case Examples.Mnist:

                Messenger.Send(new RunExampleMessage(Examples.Mnist.ToString()));

                break;
            case Examples.New:

                Messenger.Send(new RunExampleMessage(Examples.New.ToString()));

                break;
            case Examples.Open:

                Messenger.Send(new RunExampleMessage(Examples.Open.ToString()));

                break;
        }
       return;
    }
}
