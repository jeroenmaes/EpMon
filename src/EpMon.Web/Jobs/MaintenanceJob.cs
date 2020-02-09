using CronScheduler.Extensions.Scheduler;
using EpMon.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class MaintenanceJob : IScheduledJob
    {
        public string CronSchedule { get; set; }
        public string CronTimeZone { get; set; }
        public bool RunImmediately { get; set; }

        private readonly IServiceProvider _provider;

        public MaintenanceJob(IServiceProvider provider)
        {
            _provider = provider;
            ConfigureJob();
        }

        private void ConfigureJob()
        {
            CronSchedule = "1 * * * *";
            RunImmediately = true;            
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                var endpointStore = scope.ServiceProvider.GetService<EndpointStore>();
                endpointStore.RemoveEndpointStatsByDaysToKeep(30);
            }

            await Task.CompletedTask;
        }
    }
}
