using Anndotnet.App.ViewModels;

namespace Anndotnet.App.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<NavigationViewModel>();
        services.AddSingleton<StartPageViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<ModelDataViewModel>();

        return services;
    }

    public static IServiceCollection AddModels(this IServiceCollection services)
    {
       //create model here

        return services;
    }
}
