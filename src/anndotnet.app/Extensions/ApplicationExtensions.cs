using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Anndotnet.App.Model;
using Anndotnet.App.Service;
using Anndotnet.App.ViewModel;
using Avalonia.Controls;

namespace Anndotnet.App.Extensions
{
    public static class ApplicationExtensions
    {
        public static (TView view, TViewModel viewModel) GetRequiredViewModelView<TView, TViewModel>(this IServiceProvider svcProvider) where TView : UserControl where TViewModel : BaseViewModel
        {
            var view = svcProvider.GetRequiredService<TView>();
            var viewModel = svcProvider.GetRequiredService<TViewModel>();
            return (view, viewModel);
        }
        public static void AddViews(this IServiceCollection services)
        {
            services.AddTransient<DataParserView>();
            services.AddSingleton<StartView>();
            services.AddSingleton<ProjectView>();
            services.AddSingleton<MlModelView>();
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IWindowService, WindowService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IProjectService, ProjectService>();
        }

        public static void AddViewModels(this IServiceCollection services)
        {
            
            services.AddTransient<DataParserViewModel>();
            services.AddSingleton<DialogBaseViewModel>();
            services.AddSingleton<MlModelViewModel>();
            services.AddSingleton<StartViewModel>();
            services.AddSingleton<ProjectViewModel>();
            services.AddSingleton<MainViewModel>();
        }

        public static void AddModels(this IServiceCollection services)
        {
            services.AddSingleton<AppModel>();
        }
    }
}
