using System;
using System.Net;
using System.Threading;
using EpMon.Data;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EpMon.Monitor
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
                               
                JobManager.Initialize(new MonitorRegistry(_serviceProvider));

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

            collection.AddDbContext<EpMonContext>(ServiceLifetime.Transient);

            collection.AddSingleton<HttpClientFactory, HttpClientFactory>();
            collection.AddTransient<EpMonRepository, EpMonRepository>();

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
    }
}
