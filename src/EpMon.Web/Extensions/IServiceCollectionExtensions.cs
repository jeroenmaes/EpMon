using EpMon.Web.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EpMon.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {      
        public static IServiceCollection AddScheduledJobs(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var endpointService = serviceProvider.GetService<EndpointService>();
                var endpoints = endpointService.GetAllActiveEndpoints();

                foreach (var endpoint in endpoints)
                {
                    services.AddScheduler(builder =>
                    {
                        builder.AddJob(provider => new EndpointJob(provider, new CronScheduler.Extensions.Scheduler.SchedulerOptions { CronSchedule = $"*/{endpoint.CheckInterval} * * * *", RunImmediately = true }, endpoint), options =>
                        {
                            options.CronSchedule = $"*/{endpoint.CheckInterval} * * * *";
                            options.RunImmediately = true;
                        },
                        jobName: $"{nameof(EndpointJob)}_{endpoint.Id}");

                        builder.UnobservedTaskExceptionHandler = (sender, exceptionEventArgs) => UnobservedJobHandlerHandler(exceptionEventArgs, services);
                    });
                }

                services.AddScheduler(builder =>
                {
                    builder.AddJob(provider => new MaintenanceJob(provider, new CronScheduler.Extensions.Scheduler.SchedulerOptions { CronSchedule = "1 * * * *", RunImmediately = true }), options =>
                    {
                        options.CronSchedule = "1 * * * *";
                        options.RunImmediately = true;
                    },
                    jobName: nameof(MaintenanceJob));

                    builder.UnobservedTaskExceptionHandler = (sender, exceptionEventArgs) => UnobservedJobHandlerHandler(exceptionEventArgs, services);
                });
            }
            catch (Exception e)
            {
                var logger = services.FirstOrDefault(service => service.ServiceType == typeof(ILogger));
                var loggerInstance = (ILogger)logger?.ImplementationInstance;
                loggerInstance?.LogCritical(e, "Exception in AddScheduledJobs");
            }

            return services;
        }

        private static void UnobservedJobHandlerHandler(UnobservedTaskExceptionEventArgs e, IServiceCollection services)
        {
            var logger = services.FirstOrDefault(service => service.ServiceType == typeof(ILogger));
            var loggerInstance = (ILogger)logger?.ImplementationInstance;
            loggerInstance?.LogCritical(e.Exception, "Unhandled job exception");
            e.SetObserved();
        }
    }
}
