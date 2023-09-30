using Anndotnet.App.ViewModels;

namespace Anndotnet.App.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();

        return services;
    }
}
