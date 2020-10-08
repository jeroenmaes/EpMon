using EpMon.Data;
using EpMon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace EpMon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EndpointStore _store;
        
        public HomeController(EndpointStore store)
        {
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
                       
            return PartialView("EndpointOverview", new EndpointsOverviewViewModel() { TagName = tagName, Endpoints = endpoints.ToDto() });
        }

        public async Task<ActionResult> EndpointStats(int? id, int? page, string start = "", string end = "")
        {
            id ??= 1;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            
            var endpoint = await _store.GetByEndpointIdAsync(id.Value);
            var pagedStats = await endpoint.Stats.Select(x => x.ToDto())
                .OrderByDescending( x=> x.TimeStamp)
                .ToPagedListAsync(pageNumber, pageSize);

            var lastStat = endpoint.Stats.LastOrDefault();
            var responseTimes = new List<long[]>();
            var uptimes = new List<long[]>();

            foreach (var stat in endpoint.Stats)
            {
                var unixTimestamp = ((DateTimeOffset)stat.TimeStamp).ToUnixTimeMilliseconds();

                responseTimes.Add(new[] { unixTimestamp, stat.ResponseTime });

                uptimes.Add(stat.IsHealthy
                    ? new[] { unixTimestamp, 1 }
                    : new[] { unixTimestamp, 0 });
            }

            var responseTimeData = JsonConvert.SerializeObject(responseTimes);
            var uptimeData = JsonConvert.SerializeObject(uptimes);

            return View(new EndpointDetailsViewModel()
            {
                Stats = pagedStats,
                Endpoint = endpoint.ToDto(),
                ResponseTimeData = responseTimeData,
                UptimeData = uptimeData,
                LastStat = lastStat.ToDto()
            });
        }
    }
}
