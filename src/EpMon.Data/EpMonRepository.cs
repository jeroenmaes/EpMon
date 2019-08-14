using EpMon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpMon.Data
{
    public class EpMonRepository
    {
        private readonly EpMonContext _context;

        public EpMonRepository(EpMonContext context)
        {
            _context = context;
        }

        public IEnumerable<Endpoint> GetEndpoints()
        {
            return _context.Endpoints.AsNoTracking();
        }

        public void AddEndpointStat(EndpointStat stat)
        {
            _context.EndpointStats.Add(new EndpointStat { EndpointId = stat.EndpointId, IsHealthy = stat.IsHealthy, Message = stat.Message, ResponseTime = stat.ResponseTime, TimeStamp = stat.TimeStamp, Status = stat.Status });
            _context.SaveChanges();
        }

        public void CleanStats(int daysToKeep)
        {
            var compareWith = DateTime.UtcNow.AddDays(-daysToKeep);
            var statsToRemove = _context.EndpointStats.AsNoTracking().Where(x => (x.TimeStamp <= compareWith));
            _context.EndpointStats.RemoveRange(statsToRemove);
            _context.SaveChanges();
        }

        public void CustomSeed()
        {
            //Only Seed when database is empty
            if (_context.Endpoints.Any()) return;

            var testEndpoint = _context.Endpoints.AsNoTracking().FirstOrDefault(b => b.Url == @"http:\\blog.jeroenmaes.eu");
            if (testEndpoint == null)
            {
                _context.Endpoints.Add(new Endpoint
                {
                    CheckInterval = 5,
                    CheckType = CheckType.AvailabilityCheck,
                    Tags = "Personal",
                    Url = @"http:\\blog.jeroenmaes.eu",
                    Name = @"blog.jeroenmaes.eu",
                    IsActive = true,
                    IsCritical = true
                });
            }

            _context.SaveChanges();
        }
    }
}
