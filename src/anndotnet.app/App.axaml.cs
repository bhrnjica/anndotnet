using Avalonia;
using Avalonia.Metadata;
using Anndotnet.App.Service;
using Avalonia.Markup.Xaml;
using Anndotnet.App.ViewModel;
using Avalonia.Controls.ApplicationLifetimes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using Anndotnet.App.Extensions;

namespace Anndotnet.App;

public partial class App : Application
{
    public IServiceCollection? ServiceCollection { get; }      = new ServiceCollection();
    public IServiceProvider?   ServiceProvider   { get; set; }

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
            ServiceProvider = ConfigureServices();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            // Set the MainWindow
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
        
    }

    private IServiceProvider? ConfigureServices()
    {
        var serviceCollection = ServiceCollection;

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
