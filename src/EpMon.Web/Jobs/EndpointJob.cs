using CronScheduler.Extensions.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class EndpointJob : IScheduledJob
    {
        private readonly Model.Endpoint _endpoint;
        
        private readonly IServiceProvider _provider;

        public EndpointJob(IServiceProvider provider, Model.Endpoint endpoint)
        {
            _provider = provider;
            _endpoint = endpoint;

            ConfigureJob();
        }

        public string CronSchedule { get; set; }
        public string CronTimeZone { get; set; }
        public bool RunImmediately { get; set; }

        private void ConfigureJob()
        {
            CronSchedule = "* * * * *";
            RunImmediately = true;            
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                var endpointMonitor = scope.ServiceProvider.GetService<EndpointMonitor>();
                endpointMonitor.CheckHealth(_endpoint);
            }

            await Task.CompletedTask;

        }
    }
}
