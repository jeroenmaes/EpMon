using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpMon.Data
{
    public class EndpointStore
    {
        private readonly EpMonContext _context;

        public EndpointStore(EpMonContext context)
        {
            _context = context;
        }

        public IEnumerable<Endpoint> GetAllEndpoints()
        {
            return _context.Endpoints.AsNoTracking();
        }

        public void AddEndpointStat(EndpointStat stat)
        {
            using var context = new EpMonContext();
            context.EndpointStats.Add(new EndpointStat
            {
                EndpointId = stat.EndpointId,
                IsHealthy = stat.IsHealthy,
                Message = stat.Message,
                ResponseTime = stat.ResponseTime,
                TimeStamp = stat.TimeStamp,
                Status = stat.Status
            });
            context.SaveChanges();
        }

        public async Task RemoveEndpointStatsByDaysToKeep(int daysToKeep)
        {
            await using var context = new EpMonContext();

            var minDate = DateTime.UtcNow.AddDays(-daysToKeep);
            
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM EndpointStats WHERE TimeStamp <= '{minDate.ToString("yyyy-MM-dd HH:mm:ss")}'");

            await context.SaveChangesAsync();
        }

        public async Task StoreEndpointAsync(Endpoint endpoint)
        {
            await _context.Endpoints.AddAsync(endpoint);
            await _context.SaveChangesAsync();
        }

        public async Task<Endpoint> GetByEndpointIdAsync(int endpointId)
        {
            return await _context.Endpoints.AsNoTracking()
                .Include(x => x.Stats)
                .Where(x => x.Id == endpointId)
                .Select(y => new Endpoint
                {
                    Id = y.Id,
                    CheckType = y.CheckType,
                    PublishStats = y.PublishStats,
                    CheckInterval = y.CheckInterval,
                    IsCritical = y.IsCritical,
                    IsActive = y.IsActive,
                    Name = y.Name,
                    Tags = y.Tags,
                    Url = y.Url,
                    Stats = y.Stats.Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetAllEndpointTagsAsync()
        {
            return await _context.Endpoints.AsNoTracking().Select(x => x.Tags).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<Endpoint>> GetAllEndpointsAsync(string tagFilter)
        {
            IEnumerable<Endpoint> endpoints;
            
            if (tagFilter != "")
            {
                endpoints = await _context.Endpoints.AsNoTracking().Include(x => x.Stats)
                    .Where(y => y.Tags.ToLower().Equals(tagFilter.ToLower()) && y.IsActive)
                    .Select(y => new Endpoint
                    {
                        Id = y.Id,
                        CheckType = y.CheckType,
                        PublishStats = y.PublishStats,
                        CheckInterval = y.CheckInterval,
                        IsCritical = y.IsCritical,
                        IsActive = y.IsActive,
                        Name = y.Name,
                        Tags = y.Tags,
                        Url = y.Url,
                        Stats = y.Stats.Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).ToList()
                    })
                    .ToListAsync();
            }
            else
            {
                endpoints = await _context.Endpoints.AsNoTracking().Include(x => x.Stats)
                    .Where(y => y.IsActive)
                    .Select(y => new Endpoint
                    {
                        Id = y.Id,
                        CheckType = y.CheckType,
                        PublishStats = y.PublishStats,
                        CheckInterval = y.CheckInterval,
                        IsCritical = y.IsCritical,
                        IsActive = y.IsActive,
                        Name = y.Name,
                        Tags = y.Tags,
                        Url = y.Url,
                        Stats = y.Stats.Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-24)).ToList()
                    })
                    .ToListAsync();
            }
            
            return endpoints;
        }

        public async Task DeleteEndpointById(int endpointId)
        {
            _context.Endpoints.Remove(await GetByEndpointIdAsync(endpointId));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEndpointAsync(Endpoint endpoint)
        {
            var endpointEntity = _context.Endpoints.SingleOrDefault(x => x.Id == endpoint.Id);

            endpointEntity.Name = endpoint.Name;
            endpointEntity.Url = endpoint.Url;
            endpointEntity.CheckInterval = endpoint.CheckInterval;
            endpointEntity.CheckType = endpoint.CheckType;
            endpointEntity.IsActive = endpoint.IsActive;
            endpointEntity.IsCritical = endpoint.IsCritical;
            endpointEntity.Tags = endpoint.Tags;
            endpointEntity.PublishStats = endpoint.PublishStats;

            await _context.SaveChangesAsync();
        }
    }
}
