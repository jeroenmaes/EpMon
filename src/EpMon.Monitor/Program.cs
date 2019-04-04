using System;
using EpMon.Data;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace EpMon.Monitor
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            try
            {
                //RegisterServices();
                
                JobManager.Initialize(new MonitorRegistry(new HttpClientFactory(), new EpMonRepository()));

                //DisposeServices();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();

            //collection.AddTransient<EpMonAsyncRepository, EpMonAsyncRepository>();
            //collection.AddTransient<EpMonRepository, EpMonRepository>();

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
