using EpMon.Data;
using EpMon.Web.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EpMon.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {      
        public static IServiceCollection AddScheduledJobs(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var endpointService = serviceProvider.GetService<EndpointService>();
            var endpoints = endpointService.GetAllActiveEndpoints();
            
            foreach (var endpoint in endpoints)
            {
                services.AddScheduler(builder =>
                {
                    builder.AddJob(serviceProvider => new EndpointJob(endpoint, serviceProvider.GetService<EndpointMonitor>()));
                    builder.UnobservedTaskExceptionHandler = (sender, exceptionEventArgs) => UnobservedJobHandlerHandler(sender, exceptionEventArgs, services);
                });
            }

            services.AddScheduler(builder =>
            {
                builder.AddJob(serviceProvider => new MaintenanceJob(serviceProvider.GetService<EndpointStore>()));
                builder.UnobservedTaskExceptionHandler = (sender, exceptionEventArgs) => UnobservedJobHandlerHandler(sender, exceptionEventArgs, services);
            });

            return services;
        }

        private static void UnobservedJobHandlerHandler(object sender, UnobservedTaskExceptionEventArgs e, IServiceCollection services)
        {
            var logger = services.FirstOrDefault(service => service.ServiceType == typeof(ILogger));
            var loggerInstance = (ILogger)logger?.ImplementationInstance;
            loggerInstance?.LogCritical(e.Exception, "Unhandled job exception");

            e.SetObserved();
        }
    }
}
