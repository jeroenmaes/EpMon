using EpMon.Data;
using EpMon.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace EpMon.Monitor
{
    class MonitorJob
    {
        private readonly ILogger<MonitorJob> _logger;

        private readonly Endpoint _endpoint;

        private readonly HttpClientFactory _httpClientFactory;

        private readonly EpMonRepository _repo;

        public MonitorJob(Endpoint endpoint, HttpClientFactory httpClientFactory, EpMonRepository repo)
        {
            try
            {
                _httpClientFactory = httpClientFactory;
                _repo = repo;
                _endpoint = endpoint;

                var endpointStat = CheckHealth();
                
                if (endpoint.CheckType == CheckType.AvailabilityCheck)
                {
                    Validate(endpointStat);
                }
                else if (endpoint.CheckType == CheckType.ContentCheck)
                {
                    ValidateCustom(endpointStat);
                }

                //ConsoleLog(endpointStat);

                _repo.AddEndpointStat(endpointStat);

            }
            catch (Exception /*e*/)
            {
                //_logger.LogError($"Error while executing MonitorJob for endpoint {endpoint.Url}.", e);

                throw;
            }
        }

        private void ValidateCustom(EndpointStat endpointStat)
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

        private void ConsoleLog(EndpointStat result)
        {
            if (result.IsHealthy)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{result.TimeStamp} :: Healthy : {_endpoint.Url} : {result.ResponseTime} ms");
                Console.ResetColor();

                //_logger.LogInformation($"{result.Status} : {Endpoint.Url} : {result.ResponseTime} ms");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{result.TimeStamp} :: NotHealthy : {_endpoint.Url}");
                Console.ResetColor();

                //_logger.LogError($"{result.TimeStamp} :: NotHealthy {Endpoint.Url} : {result.Message}");
            }
        }

        private void Validate(EndpointStat endpointStat)
        {
            endpointStat.IsHealthy = endpointStat.Status == HttpStatusCode.OK;
        }

        private EndpointStat CheckHealth()
        {
            var result = new EndpointStat {EndpointId = _endpoint.Id};

            var monitor = new HttpMonitor(_endpoint.Id.ToString(), _httpClientFactory);
            var sw = Stopwatch.StartNew();
            var info = monitor.CheckHealth(_endpoint.Url);
            result.TimeStamp = DateTime.UtcNow;
            sw.Stop();

            result.ResponseTime = sw.ElapsedMilliseconds;
            result.Message = string.Empty;
            if (info.Details != null && info.Details.Any())
            {
                if(info.Details.ContainsKey("code"))
                    result.Status = (HttpStatusCode) Enum.Parse(typeof(HttpStatusCode), info.Details["code"]);

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

            return result;
        }
    }
}
