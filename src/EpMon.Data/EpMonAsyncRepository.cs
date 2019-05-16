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
        internal EpMonContext _context;

        public EpMonAsyncRepository(EpMonContext context)
        {
            _context = context;
        }

        public async Task<Endpoint> GetEndpointAsync(int endpointId)
        {
            return await _context.Endpoints.FirstOrDefaultAsync(q => q.Id == endpointId);
        }

        public async Task<IEnumerable<Endpoint>> GetEndpointsAsync(string tagFilter)
        {
            IEnumerable<Endpoint> endpoints;
            List<Endpoint> returnValues = new List<Endpoint>();

            if (tagFilter != "")
            {
                endpoints = await _context.Endpoints.Where(y => y.Tags.ToLower().Contains(tagFilter.ToLower())).ToListAsync();
            }
            else
            {
                endpoints = await _context.Endpoints.ToListAsync();
            }

            foreach (var endpoint in endpoints.Where(x => x.IsActive))
            {                
                var stat = await GetLastStatAsync(endpoint.Id);
                if (stat != null)
                {
                    endpoint.Stats = new List<EndpointStat>();
                    endpoint.Stats.Add(stat);
                }
                
                returnValues.Add(endpoint);
            }

            return returnValues;
        }


        public async Task<IEnumerable<EndpointStat>> GetStatsAsync(int endpointId, int maxHours)
        {
            return await _context.EndpointStats.Where(q => q.EndpointId == endpointId)
                .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                .OrderByDescending(x => x.TimeStamp).ToListAsync();
        }

        public async Task<IEnumerable<EndpointStat>> GetStatsAsync(int endpointId, int maxHours, int pageNumber, int pageSize)
        {
            return await _context.EndpointStats.Where(q => q.EndpointId == endpointId)
                                    .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                                    .OrderByDescending(x => x.TimeStamp)
                                    .ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task<EndpointStat> GetLastStatAsync(int endpointId)
        {
            var stat = _context.EndpointStats.Where(q => q.Endpoint.Id == endpointId).OrderByDescending(x => x.TimeStamp).FirstOrDefaultAsync();

            return await stat;
        }
    }
}
