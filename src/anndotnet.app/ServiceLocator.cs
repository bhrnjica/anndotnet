using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Anndotnet.App;


public class ServiceLocator
{
    private static ServiceLocator     _instance;
    private        IServiceCollection _services;
    private        IServiceProvider   _provider;

    private ServiceLocator()
    {
        _services = new ServiceCollection();
        // Register your services here if needed.
        _provider = _services.BuildServiceProvider();
    }

    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ServiceLocator();
            }
            return _instance;
        }
    }

    public void RegisterService<TService, TImplementation>() where TService : 
                                        class where TImplementation : 
                                        class, TService
    {
        _services.AddTransient<TService, TImplementation>();
        _provider = _services.BuildServiceProvider();
    }

    public TService GetService<TService>()
    {
        return _provider.GetRequiredService<TService>();
    }
}

