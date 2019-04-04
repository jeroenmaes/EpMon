using EpMon.Data;
using FluentScheduler;
using System.Linq;

namespace EpMon.Monitor
{
    public class MonitorRegistry : Registry
    {
        private readonly HttpClientFactory _httpClientFactory;
        private readonly EpMonRepository _epMonRepository;

        public MonitorRegistry(HttpClientFactory httpClientFactory, EpMonRepository epMonRepository)
        {
            _epMonRepository = epMonRepository;
            _httpClientFactory = httpClientFactory;
            
            var endpoints = _epMonRepository.GetEndpoints();

            NonReentrantAsDefault();

            //Console.WriteLine($"Found '{endpoints.Count}' HTTP endpoints to monitor.");

            foreach (var endpoint in endpoints.Where(x => x.IsActive))
            {
                Schedule(() => new MonitorJob(endpoint, _httpClientFactory, _epMonRepository)).WithName($"EndpointId={endpoint.Id}")
                    .ToRunNow()
                    .AndEvery(endpoint.CheckInterval).Minutes();
            }

            //Schedule(() => MonitorJobs()).WithName("MonitorJobs").ToRunNow().AndEvery(1).Minutes();
            //Schedule(() => MonitorAlerts()).WithName("MonitorAlerts").ToRunNow().AndEvery(1).Minutes();

            //hourly cleanup task to remove old data
            Schedule(() => CleanData()).WithName("CleanData").ToRunNow().AndEvery(1).Hours();
        }

        //public MonitorRegistry()
        //{
        //    MonitorRegistry(new HttpClientFactory(), new EpMonRepository());
        //}

        public void MonitorJobs()
        {
            //Console.WriteLine("Active HTTP endpoint monitors: ");

            foreach (var monitor in JobManager.AllSchedules.ToList())
            {
                if (monitor.Name.StartsWith("MonitorEndpoint"))
                {
                    //Console.WriteLine(monitor.Name);

                    int endpointId = int.Parse(monitor.Name.Replace("MonitorEndpointId=", ""));

                    //TODO: Validate if monitor should still be active
                    //JobManager.RemoveJob();

                    //TODO: Register new monitors that are nog yet registered (new or previously disabled)
                    //JobManager.AddJob();
                }
            }
        }

        public void MonitorAlerts()
        {
            //TODO: Find unhealthy endpoints
            //TODO: Check if alerts are configured for the endpoint

            //TODO: Send alert
        }

        public void CleanData()
        {
            _epMonRepository.CleanStats(30);
        }
    }
}
