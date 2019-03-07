using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data;
using EpMon.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using EpMon.Web.Core.Models;
using EpMon.Web.Models;
using Newtonsoft.Json;
using X.PagedList;

namespace EpMon.Web.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly EpMonAsyncRepository _repo;

        public HomeController()
        {
            _repo = new EpMonAsyncRepository();
        }
        
        public async Task<ActionResult> Index(string filter = "")
        {
            var endpoints = await _repo.GetEndpointsAsync(filter);
            var endpointsByTag = endpoints.GroupBy(x => x.Tags).ToDictionary(y => y.Key, y => y.ToList());
            var unHealthyEndpoints = endpoints.Count(x => x.Stats.FirstOrDefault().IsHealthy == false) > 0;

            Response.Headers.Add("Refresh", TimeSpan.FromMinutes(1).TotalSeconds.ToString());

            return View(new EndpointsOverview { EndpointsByTag = endpointsByTag, UnHealthyEndpoints = unHealthyEndpoints });
        }

        public async Task<ActionResult> EndpointStats(int? id, int? page, string start = "", string end = "")
        {
            id = id ?? 1;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var maxHours = 24;

            var stats = await _repo.GetStatsAsync(id.Value, maxHours);
            var pagedStats = await _repo.GetStatsAsync(id.Value, maxHours, pageNumber, pageSize);

            var endpoint = await _repo.GetEndpointAsync(id.Value);
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

            var uptime = 100;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
