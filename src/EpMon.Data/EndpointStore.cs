using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace EpMon.Data
{
    public class EndpointStore
    {
        private readonly EpMonContext _context;

        public EndpointStore(EpMonContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Endpoint>> GetAllEndpointsAsync()
        {
            return await _context.Endpoints.AsNoTracking().ToListAsync();
        }


        public async Task StoreEndpointAsync(Endpoint endpoint)
        {
            _context.Endpoints.Add(endpoint);
            await _context.SaveChangesAsync();
        }

        public async Task<Endpoint> GetByEndpointIdAsync(int endpointId)
        {
            return await _context.Endpoints.AsNoTracking().FirstOrDefaultAsync(q => q.Id == endpointId);
        }

        public async Task<IEnumerable<string>> GetAllEndpointTagsAsync()
        {
            return await _context.Endpoints.AsNoTracking().Select(x => x.Tags).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<Endpoint>> GetAllEndpointsAsync2(string tagFilter)
        {
            IEnumerable<Endpoint> endpoints;
            List<Endpoint> returnValues = new List<Endpoint>();

            if (tagFilter != "")
            {
                endpoints = await _context.Endpoints.AsNoTracking().Where(y => y.Tags.ToLower().Equals(tagFilter.ToLower())).ToListAsync();
            }
            else
            {
                endpoints = await _context.Endpoints.AsNoTracking().ToListAsync();
            }


            foreach (var endpoint in endpoints.Where(x => x.IsActive))
            {
                var stat = await GetLastEndpointStatAsync(endpoint.Id);
                if (stat != null)
                {
                    endpoint.Stats = new List<EndpointStat>();
                    endpoint.Stats.Add(stat);
                }

                returnValues.Add(endpoint);
            }

            return returnValues;
        }
        
        public async Task DeleteEndpointById(int endpointId)
        {
            _context.Endpoints.Remove(await GetByEndpointIdAsync(endpointId));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEndpoint(Endpoint endpoint)
        {
            var endpointEntity = _context.Endpoints.SingleOrDefault(x => x.Id == endpoint.Id);

            endpointEntity.Name = endpoint.Name;
            endpointEntity.Url = endpoint.Url;
            endpointEntity.CheckInterval = endpoint.CheckInterval;
            endpointEntity.CheckType = endpoint.CheckType;
            endpointEntity.IsActive = endpoint.IsActive;
            endpointEntity.IsCritical = endpoint.IsCritical;
            endpointEntity.Tags = endpointEntity.Tags;

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<EndpointStat>> GetEndpointStatsAsync(int endpointId, int maxHours)
        {
            return await _context.EndpointStats.AsNoTracking().Where(q => q.EndpointId == endpointId)
                .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                .OrderByDescending(x => x.TimeStamp).ToListAsync();
        }

        public async Task<IEnumerable<EndpointStat>> GetEndpointStatsAsync(int endpointId, int maxHours, int pageNumber, int pageSize)
        {
            return await _context.EndpointStats.AsNoTracking().Where(q => q.EndpointId == endpointId)
                .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-maxHours))
                .OrderByDescending(x => x.TimeStamp)
                .ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task<EndpointStat> GetLastEndpointStatAsync(int endpointId)
        {
            var stat = _context.EndpointStats.AsNoTracking().Where(q => q.Endpoint.Id == endpointId).OrderByDescending(x => x.TimeStamp).Take(1).FirstOrDefaultAsync();
            return await stat;
        }

        public async Task AddEndpointStat(EndpointStat stat)
        {
            _context.EndpointStats.Add(new EndpointStat { EndpointId = stat.EndpointId, IsHealthy = stat.IsHealthy, Message = stat.Message, ResponseTime = stat.ResponseTime, TimeStamp = stat.TimeStamp, Status = stat.Status });
            await _context.SaveChangesAsync();
        }

        public async Task RemoveEndpointStatsByDaysToKeep(int daysToKeep)
        {
            var compareWith = DateTime.UtcNow.AddDays(-daysToKeep);
            var statsToRemove = _context.EndpointStats.AsNoTracking().Where(x => (x.TimeStamp <= compareWith));
            _context.EndpointStats.RemoveRange(statsToRemove);
            await _context.SaveChangesAsync();
        }
    }
}
