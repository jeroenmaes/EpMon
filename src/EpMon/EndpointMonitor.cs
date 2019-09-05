using EpMon.Data;
using EpMon.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace EpMon
{
    public class EndpointMonitor
    {
        private readonly ILogger _logger;
        private readonly HttpClientFactory _httpClientFactory;
        private readonly EndpointService _service;

        private Model.Endpoint _endpoint;

        public EndpointMonitor(HttpClientFactory httpClientFactory, EndpointService service, ILogger<EndpointMonitor> logger)
        {
            _httpClientFactory = httpClientFactory;
            _service = service;
            _logger = logger;
        }

        public void CheckHealth(Model.Endpoint endpoint)
        {

            try
            {
                this._endpoint = endpoint;

                var healthReport = InternalCheckHealth();
                
                ConsoleLog(healthReport);

                _service.SaveHealthReport(_endpoint.Id, healthReport);
            }
            catch (Exception e)
            { 
                _logger.LogError($"Error while executing MonitorJob for endpoint {endpoint.Url}.", e);

                throw;
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
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{result.TimeStamp} :: Healthy : {_endpoint.Url} : {result.ResponseTime} ms");
                Console.ResetColor();

                _logger.LogInformation($"{result.Status} : {_endpoint.Url} : {result.ResponseTime} ms");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{result.TimeStamp} :: NotHealthy : {_endpoint.Url}");
                Console.ResetColor();

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

            var monitor = new HttpMonitor(_endpoint.Id.ToString(), _httpClientFactory);
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
                        result.Message = string.Empty;
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
