﻿using CronScheduler.Extensions.Scheduler;
using EpMon.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EpMon.Web.Jobs
{
    public class MaintenanceJob : IScheduledJob
    {
        private SchedulerOptions _options;
        public string Name { get; } = nameof(MaintenanceJob);

        public string CronSchedule { get; set; }
        public string CronTimeZone { get; set; }
        public bool RunImmediately { get; set; }

        private readonly IServiceProvider _provider;

        public MaintenanceJob(IServiceProvider provider, SchedulerOptions options)
        {
            _options = options;
            _provider = provider;
        }
              

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                var endpointStore = scope.ServiceProvider.GetService<EndpointStore>();
                var config = scope.ServiceProvider.GetService<IConfiguration>();
                var daysToKeepSetting = config.GetSection("EpMon:DataRetentionInDays").Value;

                var daysToKeep = 1;
                if (!string.IsNullOrEmpty(daysToKeepSetting))
                {
                    daysToKeep = int.Parse(daysToKeepSetting);
                }
                
                await endpointStore.RemoveEndpointStatsByDaysToKeep(daysToKeep);
            }

            await Task.CompletedTask;
        }
    }
}
