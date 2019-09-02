using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using EpMon.Data;
using FluentScheduler;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EpMon.Monitor
{
    public class MonitorJobRegistry : Registry
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly EpMonRepository _epMonRepository;
        private readonly IServiceProvider _serviceProvider;

        public MonitorJobRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        
            _httpClientFactory = _serviceProvider.GetService<HttpClientFactory>();
            _epMonRepository = _serviceProvider.GetService<EpMonRepository>();
            
            NonReentrantAsDefault();
            
            Schedule(() => MonitorJobs()).WithName("MonitorJobs").ToRunNow().AndEvery(1).Minutes();
        }
        
        public void MonitorJobs()
        {
            //Check endpoints in database
            foreach (var endpoint in _epMonRepository.GetEndpoints().Where(x => x.IsActive))
            {
                if (!GetActiveEndpointMonitorJobs().Contains(endpoint.Id))
                {
                    var epMonRepository = _serviceProvider.GetService<EpMonRepository>();
                    //var logger = _serviceProvider.GetService<ILogger>();

                    JobManager.AddJob(() => new MonitorJob(endpoint, _httpClientFactory, epMonRepository), 
                        (s) => s.WithName($"MonitorEndpointId={endpoint.Id}")
                        .ToRunNow()
                        .AndEvery(endpoint.CheckInterval).Minutes());
                }
            }

            //Check registered jobs
            foreach (var endpointId in GetActiveEndpointMonitorJobs())
            {
                if (!IsActiveEndpointJob(endpointId))
                {
                    JobManager.RemoveJob("MonitorEndpointId=" + endpointId);
                }

                //TODO: validate when the details of a job are changed (type, interval,...)
            }
        }

        private IEnumerable<int> GetActiveEndpointMonitorJobs()
        {
            return JobManager.AllSchedules
                    .Where(x => x.Name.StartsWith("MonitorEndpoint"))
                    .Select(x => int.Parse(x.Name.Replace("MonitorEndpointId=", "")));
        }

        private bool IsActiveEndpointJob(int endpointId)
        {
            return _epMonRepository.GetEndpoints().Any(x => x.IsActive && x.Id == endpointId);
        }
    }
}
