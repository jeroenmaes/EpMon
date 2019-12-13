using System;
using System.Net;
using System.Threading;
using EpMon.Data;
using EpMon;
using EpMon.Infrastructure;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EpMon.ConsoleHost
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 1000;
            ThreadPool.SetMinThreads(100, 100);

            try
            {
                RegisterServices();
                MigrateDatabase(_serviceProvider);

                JobManager.UseUtcTime();
                JobManager.Initialize(new MonitorOrchestrator(_serviceProvider));
                JobManager.Initialize(new CleanupOrchestrator(_serviceProvider));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();

            DisposeServices();

        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddDbContext<EpMonContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);

            collection.AddLogging(configure => configure.AddLog4Net());
            
            collection.AddSingleton<HttpClientFactory, HttpClientFactory>();
            collection.AddTransient<ITokenService, CachedTokenService>();
            collection.AddTransient<EndpointMonitor, EndpointMonitor>();
            collection.AddTransient<EndpointStore, EndpointStore>();
            collection.AddTransient<EndpointService, EndpointService>();
            
            _serviceProvider = collection.BuildServiceProvider();
        }
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            using var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<EpMonContext>();
            context.Database.Migrate();
        }
    }
}
