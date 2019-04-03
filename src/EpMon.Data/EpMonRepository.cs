using EpMon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpMon.Data
{
    public class EpMonRepository
    {
        public IEnumerable<Endpoint> GetEndpoints()
        {
            using (var db = new EpMonContext())
            {
                return db.Endpoints.ToList();
            }
        }

        public void AddEndpointStat(EndpointStat stat)
        {
            using (var db = new EpMonContext())
            {
                db.EndpointStats.Add(new EndpointStat { EndpointId  = stat.EndpointId, IsHealthy = stat.IsHealthy, Message =  stat.Message, ResponseTime = stat.ResponseTime, TimeStamp = stat.TimeStamp, Status = stat.Status});

                db.SaveChanges();
            }
        }

        public void CleanStats(int daysToKeep)
        {
            using (var db = new EpMonContext())
            {
                var compareWith = DateTime.UtcNow.AddDays(-daysToKeep);
                var statsToRemove = db.EndpointStats.Where(x => (x.TimeStamp <= compareWith));
                db.EndpointStats.RemoveRange(statsToRemove);
                db.SaveChanges();
            }
        }
        
        public void CustomSeed()
        {
            using (var context = new EpMonContext())
            {
                //Only Seed when database is empty
                if (context.Endpoints.Any()) return;

                var testEndpoint = context.Endpoints.FirstOrDefault(b => b.Url == @"http:\\blog.jeroenmaes.eu");
                if (testEndpoint == null)
                {
                    context.Endpoints.Add(new Endpoint
                    {
                        CheckInterval = 5,
                        CheckType = CheckType.AvailabilityCheck,
                        Tags = "Personal",
                        Url = @"http:\\blog.jeroenmaes.eu",
                        Name = @"blog.jeroenmaes.eu",
                        IsActive = true
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
