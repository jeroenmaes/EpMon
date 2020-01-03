using CronScheduler.Extensions.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class EndpointJob : IScheduledJob
    {
        private readonly Model.Endpoint _endpoint;
        private readonly EndpointMonitor _endpointMonitor;
        public EndpointJob(Model.Endpoint endpoint, EndpointMonitor endpointMonitor)
        {
            _endpoint = endpoint;
            _endpointMonitor = endpointMonitor;

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
            _endpointMonitor.CheckHealth(_endpoint);

            await Task.CompletedTask;
        }
    }
}
