using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Data.Entities;
using EpMon.Model;
using Microsoft.Extensions.Configuration;
using Prometheus;
using Endpoint = EpMon.Model.Endpoint;


namespace EpMon
{
    public class EndpointService
    {
        private readonly EndpointStore _store;
        
        public EndpointService(EndpointStore store)
        {
            _store = store;
        }

        public IEnumerable<Model.Endpoint> GetAllActiveEndpoints()
        {
            return _store.GetAllEndpoints().Where(x => x.IsActive).Select(x => new Model.Endpoint { Name = x.Name, Id = x.Id, Url = x.Url, IsActive = x.IsActive, CheckInterval = x.CheckInterval, CheckType = (int)x.CheckType, Tags = x.Tags, IsCritical = x.IsCritical });
        }

        public void SaveHealthReport(int endpointId, HealthReport report)
        {
            _store.AddEndpointStat(new Data.Entities.EndpointStat { EndpointId = endpointId, IsHealthy = report.IsHealthy, Message = report.Message, ResponseTime = report.ResponseTime, TimeStamp = report.TimeStamp, Status = report.Status });
        }

        public void PublishHealthReport(Model.Endpoint endpoint, HealthReport report)
        {
            var gauge1 = Metrics.CreateGauge($"epmon_healthcheck", "Shows health check status (0 = Unhealthy, 1 = Healthy)", new GaugeConfiguration
            {
                SuppressInitialValue = true,
                LabelNames = GetLabelNames()
            });
            gauge1.WithLabels(GetLabelValues(endpoint, report)).Set(Convert.ToDouble(report.IsHealthy));

            var gauge2 = Metrics.CreateGauge($"epmon_healthcheck_duration", "Shows duration of the health check execution in miliseconds", new GaugeConfiguration
            {
                SuppressInitialValue = true,
                LabelNames = GetLabelNames()
            });
            gauge2.WithLabels(GetLabelValues(endpoint, report)).Set(report.ResponseTime);
        }

        private static string[] GetLabelValues(Endpoint endpoint, HealthReport report)
        {
            return new[] { endpoint.Name, endpoint.Url, endpoint.Tags, endpoint.CheckType.ToString(), report.Status.ToString() };
        }

        private static string[] GetLabelNames()
        {
            return new[] { "name", "url", "tag", "type", "status" };
        }
    }
}
