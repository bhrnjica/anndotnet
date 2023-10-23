using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

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



