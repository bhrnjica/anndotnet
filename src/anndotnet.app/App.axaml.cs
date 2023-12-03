using Avalonia;
using Avalonia.Metadata;
using Anndotnet.App.Service;
using Avalonia.Markup.Xaml;
using Anndotnet.App.ViewModel;
using Avalonia.Controls.ApplicationLifetimes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Anndotnet.App.Extensions;

namespace Anndotnet.App;

public partial class App : Application
{
    public IServiceProvider?   Services   { get; private set; }

    public App()
    {
        
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Services = ConfigureServices();

            if (Services == null)
            {
                throw new Exception("Service provider is null");
            }
            // Get the main window
            var mainWindow = Services.GetRequiredService<MainWindow>();

            // Set the MainWindow
            desktop.MainWindow = mainWindow;
        }


        base.OnFrameworkInitializationCompleted();

        var mvm = Services?.GetRequiredService<MainViewModel>() ?? throw new NullReferenceException(nameof(MainViewModel));

        //send command to load Start Page
        mvm.LoadStartPageCommand.ExecuteAsync(null).Wait();

    }

    private IServiceProvider? ConfigureServices()
    {
        var serviceCollection = new ServiceCollection(); ;

        //Add services to the service collection
        serviceCollection?.AddServices();

        //Add Models to the service collection
        serviceCollection?.AddModels();

        // Add viewModels to the service collection
        serviceCollection?.AddViewModels();

        //Add Views to the service collection
        serviceCollection?.AddViews();

        // Add main window nto the service collection
        serviceCollection?.AddSingleton<MainWindow>();

        return serviceCollection?.BuildServiceProvider();
    }
}
