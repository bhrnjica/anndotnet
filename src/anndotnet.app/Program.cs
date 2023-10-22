using Avalonia;
using System;
using Microsoft.Extensions.DependencyInjection;
using Anndotnet.App.Service;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using static Anndotnet.App.Extensions.ApplicationExtensions;
using Google.Protobuf.WellKnownTypes;

namespace Anndotnet.App;


internal class Program
{
    private readonly IServiceCollection _services= new ServiceCollection();
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
       
        var appBuilder = BuildAvaloniaApp();
        appBuilder.StartWithClassicDesktopLifetime(args);
        
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .WithInterFont()
                         .LogToTrace()
            ;
    }


}



