using EpMon.Model;
using Prometheus;
using System;

namespace EpMon.Publisher
{
    public class PrometheusPublisher
    {
        public void PublishHealthReport(Model.Endpoint endpoint, HealthReport report)
        {
            if (!endpoint.PublishStats)
            {
                return;
            }

            var gauge1 = Metrics.CreateGauge($"epmon_healthcheck", "Shows health check status (0 = Unhealthy, 1 = Healthy)", new GaugeConfiguration
            {
                SuppressInitialValue = true,
                LabelNames = GetLabelNames()
            });
            gauge1.WithLabels(GetLabelValues(endpoint)).Set(Convert.ToDouble(report.IsHealthy));

            var gauge2 = Metrics.CreateGauge($"epmon_healthcheck_duration", "Shows duration of the health check execution in miliseconds", new GaugeConfiguration
            {
                SuppressInitialValue = true,
                LabelNames = GetLabelNames()
            });
            gauge2.WithLabels(GetLabelValues(endpoint)).Set(report.ResponseTime);
        }

        private static string[] GetLabelValues(Endpoint endpoint)
        {
            return new[] { endpoint.Name, endpoint.Url, endpoint.Tags, endpoint.CheckType.ToString() };
        }

        private static string[] GetLabelNames()
        {
            return new[] { "name", "url", "tag", "type" };
        }
    }
}
