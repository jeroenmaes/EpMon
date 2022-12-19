using EpMon.Data.Entities;
using EpMon.Infrastructure;
using EpMon.Model;
using EpMon.Monitor;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using EpMon.Publisher;

namespace EpMon
{
    public class EndpointMonitor
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EndpointService _service;
        private readonly ITokenService _tokenService;
        private readonly PrometheusPublisher _prometheusPublisher;
        private readonly ApplicationInsightsPublisher _aiPublisher;

        private Model.Endpoint _endpoint;

        public EndpointMonitor(IHttpClientFactory httpClientFactory, EndpointService service, ILogger<EndpointMonitor> logger, ITokenService tokenService, PrometheusPublisher prometheusPublisher, ApplicationInsightsPublisher aiPublisher)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _service = service;
            _logger = logger;
            _prometheusPublisher = prometheusPublisher;
            _aiPublisher = aiPublisher;
        }

        public void CheckHealth(Model.Endpoint endpoint)
        {

            try
            {
                _endpoint = endpoint;

                var healthReport = InternalCheckHealth();
                
                ConsoleLog(healthReport);

                _service.SaveHealthReport(_endpoint.Id, healthReport);

                _prometheusPublisher.PublishHealthReport(_endpoint, healthReport);
                _aiPublisher.PublishHealthReport(_endpoint, healthReport);
            }
            catch (Exception e)
            { 
                _logger.LogError(e,$"Error while executing MonitorJob for endpoint {endpoint.Url} :: {e.Message}");
            }
        }

        private void ValidateCustom(HealthReport endpointStat)
        {
            if (endpointStat.Status == HttpStatusCode.OK)
            {
                var isMatch = ContentCheck(endpointStat.Message, "ShouldNotContain", ",\"isHealthy\":false,");
                endpointStat.IsHealthy = isMatch;
            }
            else
            {
                endpointStat.IsHealthy = false;
            }

        }

        private bool ContentCheck(string message, string checkType, string value)
        {
            if (checkType == "ShouldContain")
            {
                if (message.ToLower().Contains(value.ToLower()))
                {
                    return true;
                }
            }
            if (checkType == "ShouldNotContain")
            {
                if (!message.ToLower().Contains(value.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private void ConsoleLog(HealthReport result)
        {
            if (result.IsHealthy)
            {               
                _logger.LogInformation($"{result.Status} : {_endpoint.Url} : {result.ResponseTime} ms");
            }
            else
            {
                _logger.LogError($"{result.TimeStamp} :: NotHealthy {_endpoint.Url} : {result.Message}");
            }
        }

        private void Validate(HealthReport endpointStat)
        {
            endpointStat.IsHealthy = endpointStat.Status == HttpStatusCode.OK;
        }

        private HealthReport InternalCheckHealth()
        {
            var result = new HealthReport();

            //calculate httpMonitor timeout based on the check interval (1 minute for instance) substracted with 5 seconds
            var timeout = TimeSpan.FromMinutes(_endpoint.CheckInterval).Subtract(TimeSpan.FromSeconds(5)).TotalSeconds;

            var monitor = new HttpMonitor(_endpoint.Id.ToString(), _httpClientFactory, _tokenService, timeout, _logger);
            var sw = Stopwatch.StartNew();
            var info = monitor.CheckHealth(_endpoint.Url);
            result.TimeStamp = DateTime.UtcNow;
            sw.Stop();

            result.ResponseTime = sw.ElapsedMilliseconds;
            result.Message = string.Empty;
            if (info.Details != null && info.Details.Any())
            {
                if (info.Details.ContainsKey("code"))
                    result.Status = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), info.Details["code"]);

                if (info.Details.ContainsKey("content"))
                    result.Message = info.Details["content"];

                if (info.Details.ContainsKey("contentType"))
                {
                    if (info.Details["contentType"].Contains("html"))
                    {
                        result.Message = "Unexpected html content";
                    }
                }
            }

            if (_endpoint.CheckType == (int)CheckType.AvailabilityCheck)
            {
                Validate(result);
            }
            else if (_endpoint.CheckType == (int)CheckType.ContentCheck)
            {
                ValidateCustom(result);
            }

            return result;
        }
    }
}
