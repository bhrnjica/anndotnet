using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.App.Message;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using XPlot.Plotly;


namespace Anndotnet.App.ViewModel;

public partial class StartViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    
    
    public StartViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

    }

    public void SendNavigationItem( NavigationItem item)
    {
        WeakReferenceMessenger.Default.Send(new InsertNavigationItemMessage(item));
    }

    public async Task OnLoadedAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnUnLoadedAsync()
    {
        await Task.CompletedTask;
        return;
    }

    [RelayCommand]
    void NewProject()
    {

    }

    [RelayCommand]
    void OpenProject()
    {

    }

    [RelayCommand]
    void OpenSlumpTest()
    {
        
    }
    [RelayCommand]
    void OpenBikeSharing()
    {
        var navItm = new NavigationItem();
        
        // Send a message
        var message = new InsertNavigationItemMessage(navItm);

        SendNavigationItem(navItm);
    }
    [RelayCommand]
    void OpenBreastCancer()
    {

    }

    [RelayCommand]
    void OpenIris()
    {
      var navItm = _navigationService.IrisItem();

      // Send a message
      var message = new InsertNavigationItemMessage(navItm);
      
      SendNavigationItem(navItm);
       
    }

    [RelayCommand]
    void OpenTitanic()
    {

    }

    [RelayCommand]
    void OpenMushroom()
    {

    }

    [RelayCommand]
    void OpenGlass()
    {

    }

    [RelayCommand]
    void OpenAirQuality()
    {

    }

    [RelayCommand]
    void OpenMNIST()
    {

    }

    [RelayCommand]
    void OpenCatDog()
    {

    }


    [RelayCommand]
    void OpenGitHubLink()
    {
        var url = "https://github.com/bhrnjica/anndotnet";
        OpenUrl(url);
    }
    [RelayCommand]
    void OpenPortalLink()
    {
        var url = @"https://hrnjica.net/bhrnjica/anndotnet";
        OpenUrl(url);
    }
    [RelayCommand]
    void OpenGitHubIssueLink()
    {
        var url = "https://github.com/bhrnjica/anndotnet/issues";
        OpenUrl(url);
    }

    private void OpenUrl(string url)
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo
                                         {
                                             FileName = url,
                                             UseShellExecute = true
                                         });
    }
}

internal class OnNavigationItemMessageReceived
{
}

