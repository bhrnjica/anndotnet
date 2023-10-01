using Anndotnet.App.ViewModels;

namespace Anndotnet.App.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<ViewModels.NavigationViewModel>();
        services.AddSingleton<StartPageViewModel>();
        services.AddSingleton<MainViewModel>(); 

        return services;
    }
}
