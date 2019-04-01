using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace EpMon.Data
{
    public class EpMonAsyncRepository
    {
        public async Task<Endpoint> GetEndpointAsync(int endpointId)
        {
            using (var db = new EpMonContext())
            {
                return await db.Endpoints.FirstOrDefaultAsync(q => q.Id == endpointId);
            }
        }

        public async Task<IEnumerable<Endpoint>> GetEndpointsAsync(string tagFilter)
        {
            using (var db = new EpMonContext())
            {
                IEnumerable<Endpoint> endpoints;
                List<Endpoint> returnValues = new List<Endpoint>();

                if (tagFilter != "")
                {
                    endpoints = await db.Endpoints.Where(y => y.Tags.ToLower().Contains(tagFilter.ToLower())).ToListAsync();
                }
                else
                {
                    endpoints = await db.Endpoints.ToListAsync();
                }

                foreach (var endpoint in endpoints.Where(x => x.IsActive))
                {
                    endpoint.Stats = new List<EndpointStat>();
                    var stat = await GetLastStatAsync(endpoint.Id);
                    endpoint.Stats.Add(stat);

                    returnValues.Add(endpoint);
                }

                return returnValues;
            }
        }
        
        public async Task<IEnumerable<EndpointStat>> GetStatsAsync(int endpointId, int maxHours)
        {
            using (var db = new EpMonContext())
            {
                return await db.EndpointStats.Where(q => q.EndpointId == endpointId)
                    .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                    .OrderByDescending(x => x.TimeStamp).ToListAsync();
            }
        }

        public async Task<IEnumerable<EndpointStat>> GetStatsAsync(int endpointId, int maxHours, int pageNumber, int pageSize)
        {
            using (var db = new EpMonContext())
            {
                return await db.EndpointStats.Where(q => q.EndpointId == endpointId)
                                        .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                                        .OrderByDescending(x => x.TimeStamp)
                                        .ToPagedListAsync(pageNumber, pageSize);
            }
        }

        public async Task<EndpointStat> GetLastStatAsync(int endpointId)
        {
            using (var db = new EpMonContext())
            {
                var stat = db.EndpointStats.Where(q => q.Endpoint.Id == endpointId)
                                            .OrderByDescending(x => x.TimeStamp).FirstOrDefaultAsync();

                return await stat;
            }
        }
    }
}
