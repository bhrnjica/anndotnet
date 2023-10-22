using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Anndotnet.App.Service;
using Anndotnet.App.ViewModel;

namespace Anndotnet.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddViews(this IServiceCollection services)
        {
            services.AddSingleton<StartView>();
            services.AddSingleton<ProjectView>();
            services.AddSingleton<MlModelView>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IProjectService, ProjectService>();
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<StartViewModel>();
            services.AddSingleton<ProjectViewModel>();
            services.AddSingleton<ProjectViewModel>();
            services.AddSingleton<MainViewModel>();
        }

        public static void AddModels(this IServiceCollection services)
        {
            //app.Services.AddTransient<IService, Service>();
        }
    }
}
