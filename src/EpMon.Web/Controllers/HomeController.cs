using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EpMon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EndpointStore _store;
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger, EndpointStore store)
        {
            _logger = logger;
            _store = store;
        }
        
        public async Task<ActionResult> Index()
        {
            var tags = await _store.GetAllEndpointTagsAsync();
           
            return View(new EndpointTags { Tags = tags.ToList() });
        }
        
        [HttpGet]
        public async Task<ActionResult> EndpointsPartial(string tagName = "")
        {
            var endpoints = await _store.GetAllEndpointsAsync(tagName);
                       
            return PartialView("EndpointOverview", new EndpointsOverview { TagName = tagName, Endpoints = endpoints.ToList() });
        }

        public async Task<ActionResult> EndpointStats(int? id, int? page, string start = "", string end = "")
        {
            id = id ?? 1;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var maxHours = 24;

            var stats = await _store.GetEndpointStatsAsync(id.Value, maxHours);
            var pagedStats = await _store.GetEndpointStatsAsync(id.Value, maxHours, pageNumber, pageSize);

            var endpoint = await _store.GetByEndpointIdAsync(id.Value);
            var lastStat = stats.FirstOrDefault();

            var responseTimes = new List<long[]>();
            var uptimes = new List<long[]>();

            foreach (var stat in stats)
            {
                var unixTimestamp = ((DateTimeOffset)stat.TimeStamp).ToUnixTimeMilliseconds();

                responseTimes.Add(new[] { unixTimestamp, stat.ResponseTime });

                uptimes.Add(stat.IsHealthy
                    ? new[] { unixTimestamp, 1 }
                    : new[] { unixTimestamp, 0 });
            }

            var responseTimeData = JsonConvert.SerializeObject(responseTimes);
            var uptimeData = JsonConvert.SerializeObject(uptimes);

            var uptime = Math.Round(((double)stats.Count(x => x.IsHealthy == true) / (double) stats.Count()) * 100.00, 2);
            var responseTime = 0.0;

            if (stats.Any())
                responseTime = Math.Round(stats.Average(x => x.ResponseTime), 2);

            return View(new EndpointDetails
            {
                Stats = pagedStats,
                Endpoint = endpoint,
                ResponseTimeData = responseTimeData,
                UptimeData = uptimeData,
                ResponseTime = responseTime,
                Uptime = uptime,
                LastStat = lastStat
            });
        }
    }
}
