using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EpMon.Data;
using EpMon.Data.Entities;
using EpMon.Web.Models;
using Newtonsoft.Json;
using X.PagedList;

namespace EpMon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EpMonRepository _repo;
        public HomeController()
        {
            _repo = new EpMonRepository();
        }

        public ActionResult Index(string filter = "")
        {
            Dictionary<string, List<Endpoint>> endpointsByTag;
            IEnumerable<Endpoint> endpoints;
            filter = filter.ToLower();

            endpoints = filter != "" ? _repo.GetEndpoints().Where(y => y.Tags.ToLower().Contains(filter)).ToList() : _repo.GetEndpoints().ToList();

            endpointsByTag = endpoints.GroupBy(x => x.Tags).ToDictionary(y => y.Key, y => y.ToList());

            var unHealthy = endpoints.Count(x => x.Stats.FirstOrDefault().IsHealthy == false) > 0;
            
            Response.AddHeader("Refresh", TimeSpan.FromMinutes(2).TotalSeconds.ToString());

            return View(new EndpointsOverview { EndpointsByTag = endpointsByTag, UnHealthyEndpoints = unHealthy });
        }

        public ActionResult EndpointStats(int? id, int? page, string start = "", string end = "")
        {
            // if start and end are empty, show last 24 hours
            // else filter...

            id = id ?? 1;
            var pageNumber = page ?? 1;
            var pageSize = 15;

            var stats = _repo.GetStats(id.Value).Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).OrderByDescending(x => x.TimeStamp).ToPagedList(pageNumber, pageSize);
            var endpoint = _repo.GetEndpoint(stats[0].EndpointId);
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

            if(stats.Any())
                responseTime = Math.Round(stats.Average(x => x.ResponseTime), 2);

            return View(new EndpointDetails
            {
                Stats = stats,
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