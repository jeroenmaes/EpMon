using CronScheduler.Extensions.Scheduler;
using EpMon.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EpMon.Web.Jobs
{
    public class MaintenanceJob : IScheduledJob
    {
        public string CronSchedule { get; set; }
        public string CronTimeZone { get; set; }
        public bool RunImmediately { get; set; }

        private readonly EndpointStore _endpointStore;

        public MaintenanceJob(EndpointStore endpointStore)
        {
            _endpointStore = endpointStore;
            ConfigureJob();
        }

        private void ConfigureJob()
        {
            CronSchedule = "1 * * * *";
            RunImmediately = true;            
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _endpointStore.RemoveEndpointStatsByDaysToKeep(30);

            await Task.CompletedTask;
        }
    }
}
