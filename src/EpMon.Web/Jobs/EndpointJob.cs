using CronScheduler.Extensions.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class EndpointJob : IScheduledJob
    {
        private SchedulerOptions _options;

        public string Name { get; }

        private readonly Model.Endpoint _endpoint;
        
        private readonly IServiceProvider _provider;

        public EndpointJob(IServiceProvider provider, SchedulerOptions options, Model.Endpoint endpoint)
        {
            _options = options;
            _provider = provider;
            _endpoint = endpoint;

            Name = $"EndpointJob_{_endpoint.Id}";            
        }

        public string CronSchedule { get; set; }
        public string CronTimeZone { get; set; }
        public bool RunImmediately { get; set; }
        
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
