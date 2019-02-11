using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Logging;
using EpMon.Data;
using EpMon.Data.Entities;

namespace EpMon.Monitor
{
    class MonitorJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<MonitorJob>();
        
        private Endpoint Endpoint { get; }
        public MonitorJob(Endpoint endpoint)
        {
            try
            {
                Endpoint = endpoint;
                var endpointStat = CheckHealth();
                
                if (endpoint.CheckType == CheckType.AvailabilityCheck)
                {
                    Validate(endpointStat);
                }
                else if (endpoint.CheckType == CheckType.ContentCheck)
                {
                    ValidateCustom(endpointStat);
                }

                ConsoleLog(endpointStat);

                var repo = new EpMonRepository();
                repo.AddStat(endpointStat);

            }
            catch (Exception e)
            {
                Logger.Error($"Error while executing MonitorJob for endpoint {endpoint.Url}.", e);

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
                Console.WriteLine($"{result.TimeStamp} :: Healthy : {Endpoint.Url} : {result.ResponseTime} ms");
                Console.ResetColor();

                Logger.Info($"{result.Status} : {Endpoint.Url} : {result.ResponseTime} ms");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{result.TimeStamp} :: NotHealthy : {Endpoint.Url}");
                Console.ResetColor();

                Logger.Error($"{result.TimeStamp} :: NotHealthy {Endpoint.Url} : {result.Message}");
            }
        }

        private void Validate(EndpointStat endpointStat)
        {
            endpointStat.IsHealthy = endpointStat.Status == HttpStatusCode.OK;
        }

        private EndpointStat CheckHealth()
        {
            var result = new EndpointStat {EndpointId = Endpoint.Id};

            var monitor = new HttpMonitor();
            var sw = Stopwatch.StartNew();
            var info = monitor.CheckHealth(Endpoint.Url);
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
            }

            return result;
        }
    }
}
