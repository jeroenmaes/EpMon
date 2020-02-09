using EpMon.Model;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace EpMon.Publisher
{
    public class ApplicationInsightsPublisher
    {
        private static TelemetryClient _client;
        private static readonly object SyncRoot = new object();
        private readonly string _instrumentationKey;

        public ApplicationInsightsPublisher(string instrumentationKey)
        {
            _instrumentationKey = instrumentationKey;
        }

        public void PublishHealthReport(Endpoint endpoint, HealthReport healthReport)
        {
            if (!endpoint.PublishStats || string.IsNullOrEmpty(_instrumentationKey))
            {
                return;
            }

            var client = GetOrCreateTelemetryClient();

            client.TrackEvent("epmon_healthcheck",
                properties: new Dictionary<string, string>
                {
                    { "epmon_healthcheck_url", endpoint.Url },
                    { "epmon_healthcheck_tag", endpoint.Tags },
                    { "epmon_healthcheck_host", Environment.MachineName }
                },
                metrics: new Dictionary<string, double>
                {
                    { "epmon_healthcheck_result", healthReport.IsHealthy ? 1 : 0  },
                    { "epmon_healthcheck_duration", healthReport.ResponseTime }
                });
        }

        private TelemetryClient GetOrCreateTelemetryClient()
        {
            if (_client == null)
            {
                lock (SyncRoot)
                {
                    if (_client == null)
                    {
                        var tc = new TelemetryConfiguration(_instrumentationKey);
                        _client = new TelemetryClient(tc);
                    }
                }
            }
            return _client;
        }
    }
}
